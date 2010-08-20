using System;

namespace IPDL {
  class Util {
    private static string[] SISuffixes = { "", "K", "M", "G", "T", "P", "E", "Z", "Y" };

    public static string SIFormat(int number, string units) {
      if (number < 1000)
        return String.Format("{0} {1}", number, units);
      int k = (int)Math.Floor(Math.Log10(number) / 3);
      return String.Format("{0:0.00} {1}{2}", number / Math.Pow(1000, k), Util.SISuffixes[k], units);
    }
  }
}
