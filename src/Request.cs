using System.Net;
using System.Text.RegularExpressions;

namespace EMP {
  class AbstractRequest {
    private string userAgent;
    private string url;
    private CookieContainer cookies;

    public AbstractRequest(
      string url,
      CookieContainer cookies,
      string userAgent)
    {
      this.url       = url;
      this.cookies   = cookies;
      this.userAgent = userAgent;
    }

    protected HttpWebRequest Request() {
      HttpWebRequest r  = (HttpWebRequest)WebRequest.Create(url);
      r.UserAgent       = userAgent;
      r.CookieContainer = cookies;
      return r;
    }
  }

  class IphoneRequest : AbstractRequest {
    private static string UserAgent =
      "Mozilla/5.0 (iPhone; U; CPU iPhone OS 3_1_2 like Mac OS X; en-us) "+
      "AppleWebKit/528.18 (KHTML, like Gecko) Version/4.0 Mobile/7D11 Safari/528.16";

    public IphoneRequest(string url, CookieContainer cookies)
      : base(url, cookies, IphoneRequest.UserAgent) {}

    public WebResponse GetResponse() {
      return Request().GetResponse();
    }
  }

  class CoreMediaRequest : AbstractRequest {
    private static string UserAgent =
      "Apple iPhone v1.1.4 CoreMedia v1.0.0.4A102";

    private int contentLength = -1;

    public CoreMediaRequest(string url, CookieContainer cookies)
      : base(url, cookies, CoreMediaRequest.UserAgent) {}

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
      var response = request.GetResponse();
      this.contentLength = int.Parse(Regex.Match(response.Headers["Content-Range"], @"\d+$").Value);
      response.Close();
    }

    public WebResponse GetResponseFromOffset(int offset) {
      var contentLength = ContentLength;
      var request = Request();
      request.AddRange(offset, contentLength);
      return request.GetResponse();
    }
  }
}
