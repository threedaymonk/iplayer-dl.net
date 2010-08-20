using HtmlAgilityPack;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace IPDL {
  class IphonePage {
    private static string urlPrefix = "http://www.bbc.co.uk/mobile/iplayer/episode/";
    private HtmlNode document;

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
      var h = new HtmlDocument();
      h.LoadHtml(source);
      this.document = h.DocumentNode;
    }

    public string EmbeddedMediaUrl {
      get {
        var node = document.SelectSingleNode("//embed[@href]");
        if (node == null)
          return null;
        else
          return node.Attributes["href"].Value;
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
