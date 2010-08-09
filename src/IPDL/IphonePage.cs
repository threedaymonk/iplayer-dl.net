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

    public string Title {
      get {
        var query = from programme in document.SelectNodes("//*[@id='programme']")
                    let h2 = programme.SelectSingleNode("h2")
                    let p  = programme.SelectSingleNode("p")
                    where h2 != null && p != null
                    select h2.InnerText.Trim() + ": " + p.InnerText.Trim();
        return query.SingleOrDefault();
      }
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

    public string Kind {
      get {
        var url = EmbeddedMediaUrl;
        if      (Regex.IsMatch(url, @"\.mp3"))
          return "radio";
        else if (Regex.IsMatch(url, @"\.mp4"))
          return "tv";
        else
          return null;
      }
    }
  }
}
