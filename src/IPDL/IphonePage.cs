using System.Text.RegularExpressions;
using System.IO;

namespace IPDL {
  class IphonePage {
    private static string urlPrefix = "http://www.bbc.co.uk/mobile/iplayer/episode/";
    private string document;

    public static string Url(string pid) {
      return IphonePage.urlPrefix + pid;
    }

    public IphonePage(string source) {
      Init(source);
    }

    public IphonePage(Stream source) {
      Init((new StreamReader(source)).ReadToEnd());
    }

    private void Init(string source) {
      this.document = source;
    }

    public string EmbeddedMediaUrl {
      get {
        var regex = new Regex(@"<embed\s[^>]*href=['""]?([^'""\s]+)",
                              RegexOptions.IgnoreCase |
                              RegexOptions.Singleline |
                              RegexOptions.CultureInvariant);
        var match = regex.Match(document);
        if (match.Success)
          return match.Groups[1].Value;
        else
          return null;
      }
    }

    public bool IsAvailable {
      get { return (EmbeddedMediaUrl != null); }
    }

    public string FileExtension {
      get {
        var url = EmbeddedMediaUrl;
        if      (Regex.IsMatch(url, @"\.mp3"))
          return ".mp3";
        else if (Regex.IsMatch(url, @"\.mp4"))
          return ".mp4";
        else
          return "";
      }
    }
  }
}
