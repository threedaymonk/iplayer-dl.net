using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace IPDL {
  class Downloader {
    public enum Status { Complete, Incomplete, AlreadyExists, Unavailable, Error };

    private static int BufferSize = 100000;

    private CookieContainer cookies;
    private string pid;

    public delegate void AtStartHandler(string filename);
    public delegate void ProgressHandler(int bytesFetched, int bytesTotal);
    public delegate void AtEndHandler(Status status, string message);

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

      var request         = new CoreMediaRequest(page.EmbeddedMediaUrl, cookies);
      var contentLength   = request.ContentLength;
      Stream localStream  = new FileStream(tempPath, FileMode.Append, FileAccess.Write, FileShare.Read);
      byte[] buffer       = new byte[Downloader.BufferSize];
      int bytesRead;
      int totalReceived   = (int)localStream.Position; // TODO use longs
      var response        = request.GetResponseFromOffset(totalReceived);
      Stream remoteStream = response.GetResponseStream();

      do {
        bytesRead = remoteStream.Read(buffer, 0, buffer.Length);
        localStream.Write(buffer, 0, bytesRead);
        totalReceived += bytesRead;
        progress(totalReceived, contentLength);
      } while (bytesRead > 0);

      response.Close();
      localStream.Close();

      if (totalReceived >= contentLength) {
        File.Move(tempPath, finalPath);
        atEnd(Status.Complete, finalPath);
      } else {
        atEnd(Status.Incomplete, tempPath);
      }
    }

    private Playlist GetPlaylist() {
      var response = ((HttpWebRequest)WebRequest.Create(Playlist.Url(pid))).GetResponse();
      var playlist = new Playlist(response.GetResponseStream());
      response.Close();
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
      var response = new IphoneRequest(IphonePage.Url(pid), cookies).GetResponse();
      var page = new IphonePage(response.GetResponseStream());
      response.Close();
      return page;
    }

    private string FilenameSafe(string text) {
      var result = text;
      foreach (var c in Path.GetInvalidFileNameChars()) {
        result = result.Replace(c.ToString(), "");
      }
      return result;
    }
  }
}
