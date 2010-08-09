using System;
using System.IO;
using EMP;
using System.Text.RegularExpressions;

public class App {
  public static void Main(string[] args) {
    foreach (var arg in args) {
      Download(arg);
    }
  }

  private static void Download(string pid) {
    var downloader = new Downloader(pid);

    Downloader.ProgressDelegate progress = delegate(IphonePage page, int i, int total) {
      Console.CursorLeft = 0;
      Console.Write("{0}/{1} ({2}%)", i, total, Math.Ceiling((i * 100.0) / total));
    };

    Downloader.CompletionDelegate completion = delegate(IphonePage page, string tempFilename, bool didComplete) {
      Console.WriteLine();
      if (didComplete) {
        string filename = Regex.Replace(page.Title, @"[^a-zA-Z0-9]+", "-");
        switch (page.Kind) {
          case "tv":
            filename += ".mp4";
            break;
          case "radio":
            filename += ".mp3";
            break;
        }
        File.Move(tempFilename, filename);
        Console.WriteLine("Downloaded {0}", filename);
      } else {
        Console.WriteLine("Incomplete download");
      }
    };

    downloader.Download(progress, completion);
  }
}
