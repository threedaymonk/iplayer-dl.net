using System;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace IPDL {
  public enum DownloadStatus { Complete, Incomplete, AlreadyExists, Unavailable, Error };

  public class Downloader {
    public delegate void AtStartHandler(string filename);
    public delegate void ProgressHandler(int bytesFetched, int bytesTotal);
    public delegate void AtEndHandler(DownloadStatus status, string message);

    private CookieContainer cookies;

    public Downloader() {
      this.cookies = new CookieContainer();
    }

    // Virtual solely so that we can subclass for testing, because mocks don't work with delegates.
    public virtual void Download(string pid, AtStartHandler atStart, ProgressHandler progress, AtEndHandler atEnd) {
      var page = GetIphonePage(pid);

      if (!page.IsAvailable) {
        atEnd(DownloadStatus.Unavailable, pid);
        return;
      }

      var finalPath = FilenameSafe(GetTitle(pid)) + page.FileExtension;
      var tempPath  = finalPath + ".partial";

      if (File.Exists(finalPath)){
        atEnd(DownloadStatus.AlreadyExists, finalPath);
        return;
      }

      atStart(finalPath);

      var request       = new CoreMediaRequest(page.EmbeddedMediaUrl, cookies);
      var contentLength = request.ContentLength;
      int totalReceived = 0;

      using (var localStream = new FileStream(tempPath, FileMode.Append, FileAccess.Write, FileShare.Read)) {
        totalReceived = (int)localStream.Position;
        request.GetResponseStreamFromOffset(totalReceived, remoteStream => {
          ReadFromStream(remoteStream, (buffer, bytesRead) => {
            localStream.Write(buffer, 0, bytesRead);
            totalReceived += bytesRead;
            progress(totalReceived, contentLength);
          });
        });
      }

      if (totalReceived >= contentLength) {
        File.Move(tempPath, finalPath);
        atEnd(DownloadStatus.Complete, finalPath);
      } else {
        atEnd(DownloadStatus.Incomplete, tempPath);
      }
    }

    private Playlist GetPlaylist(string pid) {
      Playlist playlist = null;
      (new GeneralRequest(Playlist.Url(pid))).GetResponseStream(stream => {
        playlist = new Playlist(stream);
      });
      return playlist;
    }

    private string GetTitle(string pid) {
      try {
        return GetPlaylist(pid).Title;
      } catch {
        return pid;
      }
    }

    private IphonePage GetIphonePage(string pid) {
      IphonePage page = null;
      new IphoneRequest(IphonePage.Url(pid), cookies).GetResponseStream(stream => {
        page = new IphonePage(stream);
      });
      return page;
    }

    private string FilenameSafe(string text) {
      var result = text;
      foreach (var c in Path.GetInvalidFileNameChars()) {
        result = result.Replace(c.ToString(), "");
      }
      return result;
    }

    private static int BufferSize = 0x10000;
    private void ReadFromStream(Stream stream, Action<byte[], int> handler) {
      byte[] buffer = new byte[Downloader.BufferSize];
      int bytesRead;

      do {
        bytesRead = stream.Read(buffer, 0, buffer.Length);
        handler(buffer, bytesRead);
      } while (bytesRead > 0);
    }
  }
}
