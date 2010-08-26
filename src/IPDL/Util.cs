using System;
using System.Text.RegularExpressions;

namespace IPDL {
  static class Util {
    private static string[] SISuffixes = { "", "K", "M", "G", "T", "P", "E", "Z", "Y" };

    public static string SIFormat(int number, string units) {
      if (number < 1000)
        return String.Format("{0} {1}", number, units);
      int k = (int)Math.Floor(Math.Log10(number) / 3);
      return String.Format("{0:0.00} {1}{2}", number / Math.Pow(1000, k), Util.SISuffixes[k], units);
    }

    public static string ExtractPid(string url) {
      var regex = new Regex(@"(?:/iplayer/episode/|/programmes/|\b)(b[a-z0-9]{7})\b");
      var match = regex.Match(url);
      if (match.Success)
        return match.Groups[1].Value;
      else
        return null;
    }
  }
}
