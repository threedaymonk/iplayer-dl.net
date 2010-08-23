using System;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace IPDL {
  abstract class AbstractRequest {
    private string userAgent;
    private string url;
    private CookieContainer cookieContainer;

    public delegate void ResponseHandler(WebResponse response);
    public delegate void ResponseStreamHandler(Stream stream);

    public AbstractRequest(string url, CookieContainer cookieContainer, string userAgent) {
      this.url             = url;
      this.cookieContainer = cookieContainer;
      this.userAgent       = userAgent;
    }

    protected HttpWebRequest Request() {
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
      request.Accept          = "*/*";
      request.UserAgent       = userAgent;
      request.CookieContainer = cookieContainer;
#if DEBUG
      Console.WriteLine("Request\n{0}\n\n{1}\n----", url, request.Headers);
#endif
      return request;
    }

    protected void WithResponse(HttpWebRequest request, ResponseHandler handler) {
      using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
        if (cookieContainer != null)
          cookieContainer.Add(response.Cookies);
#if DEBUG
        Console.WriteLine("Response\n{0}\n----", response.Headers);
#endif
        handler(response);
      }
    }

    protected void WithResponseStream(HttpWebRequest request, ResponseStreamHandler handler) {
      WithResponse(request, response => {
        using (var stream = response.GetResponseStream()) {
          handler(stream);
        }
      });
    }
  }

  class GeneralRequest : AbstractRequest {
    public GeneralRequest(string url)
      : base(url, null, null) {}

    public void GetResponseStream(ResponseStreamHandler streamHandler) {
      WithResponseStream(Request(), streamHandler);
    }
  }

  class IphoneRequest : AbstractRequest {
    private static string UserAgent =
      "Mozilla/5.0 (iPhone; U; CPU iPhone OS 3_1_2 like Mac OS X; en-us) "+
      "AppleWebKit/528.18 (KHTML, like Gecko) Version/4.0 Mobile/7D11 Safari/528.16";

    public IphoneRequest(string url, CookieContainer cookieContainer)
      : base(url, cookieContainer, IphoneRequest.UserAgent) {}

    public void GetResponseStream(ResponseStreamHandler streamHandler) {
      WithResponseStream(Request(), streamHandler);
    }
  }

  class CoreMediaRequest : AbstractRequest {
    private static string UserAgent =
      "Apple iPhone v1.1.4 CoreMedia v1.0.0.4A102";
    private static int MaxSegmentSize = 0x400000;

    private int contentLength = -1;

    public CoreMediaRequest(string url, CookieContainer cookieContainer)
      : base(url, cookieContainer, CoreMediaRequest.UserAgent) {}

    public int ContentLength {
      get {
        MakeInitialRangeRequestIfNecessary();
        return contentLength;
      }
    }

    private void MakeInitialRangeRequestIfNecessary() {
      if (contentLength >= 0)
        return;
      var request = Request();
      request.AddRange(0, 1);
      WithResponse(request, response => {
        this.contentLength = int.Parse(Regex.Match(response.Headers["Content-Range"], @"\d+$").Value);
      });
    }

    public void GetResponseStreamFromOffset(int offset, ResponseStreamHandler streamHandler) {
      var contentLength = ContentLength;
      for (int firstByte = offset; firstByte < contentLength; firstByte += CoreMediaRequest.MaxSegmentSize) {
        int lastByte = Math.Min(firstByte + CoreMediaRequest.MaxSegmentSize, contentLength) - 1;
        var request = Request();
        request.AddRange(firstByte, lastByte);
        WithResponseStream(request, streamHandler);
      }
    }
  }
}
