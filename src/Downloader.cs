using System.Net;
using System.IO;

namespace EMP {
  class Downloader {
    private static int BufferSize = 100000;

    private CookieContainer cookies;
    private string pid;

    public delegate void ProgressDelegate(IphonePage page, int i, int total);
    public delegate void CompletionDelegate(IphonePage page, string filename, bool didComplete);

    public Downloader(string pid) {
      this.pid     = pid;
      this.cookies = new CookieContainer();
    }

    private IphonePage GetIphonePage() {
      var response = new IphoneRequest(IphonePage.Url(pid), cookies).GetResponse();
      var page = new IphonePage(response.GetResponseStream());
      response.Close();
      return page;
    }

    public void Download(ProgressDelegate progress, CompletionDelegate completion) {
      var page          = GetIphonePage();

      var request       = new CoreMediaRequest(page.EmbeddedMediaUrl, cookies);
      var contentLength = request.ContentLength;
      var filename      = pid + ".partial";


      Stream localStream = new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.Read);

      byte[] buffer = new byte[Downloader.BufferSize];
      int bytesRead;
      int totalReceived = (int)localStream.Position; // TODO use longs

      var response        = request.GetResponseFromOffset(totalReceived);
      Stream remoteStream = response.GetResponseStream();

      do {
        bytesRead = remoteStream.Read(buffer, 0, buffer.Length);
        localStream.Write(buffer, 0, bytesRead);
        totalReceived += bytesRead;
        progress(page, totalReceived, contentLength);
      } while (bytesRead > 0);

      response.Close();
      localStream.Close();

      completion(page, filename, totalReceived >= contentLength);
    }
  }
}
