using System;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace IPDL {
  class Downloader {
    public enum Status { Complete, Incomplete, AlreadyExists, Unavailable, Error };

    public delegate void AtStartHandler(string filename);
    public delegate void ProgressHandler(int bytesFetched, int bytesTotal);
    public delegate void AtEndHandler(Status status, string message);

    private CookieContainer cookies;
    private string pid;

    public Downloader(string pid) {
      this.pid     = pid;
      this.cookies = new CookieContainer();
    }

    public void Download(AtStartHandler atStart, ProgressHandler progress, AtEndHandler atEnd) {
      var page = GetIphonePage();

      if (!page.IsAvailable) {
        atEnd(Status.Unavailable, pid);
        return;
      }

      var finalPath = FilenameSafe(GetTitle()) + page.FileExtension;
      var tempPath  = finalPath + ".partial";

      if (File.Exists(finalPath)){
        atEnd(Status.AlreadyExists, finalPath);
        return;
      }

      atStart(finalPath);

      var request       = new CoreMediaRequest(page.EmbeddedMediaUrl, cookies);
      var contentLength = request.ContentLength;
      int totalReceived = 0;

      OpenFileForWriting(tempPath, localStream => {
        totalReceived = (int)localStream.Position;
        request.GetResponseStreamFromOffset(totalReceived, remoteStream => {
          ReadFromStream(remoteStream, (buffer, bytesRead) => {
            localStream.Write(buffer, 0, bytesRead);
            totalReceived += bytesRead;
            progress(totalReceived, contentLength);
          });
        });
      });

      if (totalReceived >= contentLength) {
        File.Move(tempPath, finalPath);
        atEnd(Status.Complete, finalPath);
      } else {
        atEnd(Status.Incomplete, tempPath);
      }
    }

    private Playlist GetPlaylist() {
      Playlist playlist = null;
      (new GeneralRequest(Playlist.Url(pid))).GetResponseStream(stream => {
        playlist = new Playlist(stream);
      });
      return playlist;
    }

    private string GetTitle() {
      try {
        return GetPlaylist().Title;
      } catch {
        return pid;
      }
    }

    private IphonePage GetIphonePage() {
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

    private static int BufferSize = 100000;
    private void ReadFromStream(Stream stream, Action<byte[], int> handler) {
      byte[] buffer = new byte[Downloader.BufferSize];
      int bytesRead;

      do {
        bytesRead = stream.Read(buffer, 0, buffer.Length);
        handler(buffer, bytesRead);
      } while (bytesRead > 0);
    }

    private void OpenFileForWriting(string path, Action<Stream> handler) {
      Stream stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read);
      handler(stream);
      stream.Close();
    }
  }
}
