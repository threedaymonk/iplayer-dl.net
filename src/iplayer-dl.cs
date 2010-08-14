using System;
using System.IO;
using IPDL;

public class App {
  public static void Main(string[] args) {
    foreach (var arg in args) {
      Download(arg);
    }
  }

  private static string FormatBytes(int b) {
    string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
    double scaled = (double)b;
    int i;
    for (i = 0; i < suffixes.Length; i++) {
      scaled = b / Math.Pow(1000.0, i);
      if (scaled < 1000) break;
    }
    return String.Format("{0:0.00} {1}", scaled, suffixes[i]);
  }

  private static void Begun(string filename) {
    Console.WriteLine("Downloading: {0}", filename);
  }

  private static void Progress(string filename, int bytesDownloaded, int total) {
    Console.CursorLeft = 0;
    Console.Write("{1:0.0}% complete; {0} left",
                  FormatBytes((total - bytesDownloaded)),
                  (bytesDownloaded * 100.0) / total);
  }

  private static void Finished(string filename, Downloader.Status status) {
    Console.WriteLine();
    switch (status) {
      case Downloader.Status.Complete:
        Console.WriteLine("Downloaded {0}", filename);
        break;
      case Downloader.Status.Incomplete:
        Console.WriteLine("Incomplete download");
        break;
      case Downloader.Status.AlreadyExists:
        Console.WriteLine("File exists");
        break;
    }
  }

  private static void Download(string pid) {
    var downloader = new Downloader(pid);
    downloader.Download(Begun, Progress, Completed);
  }
}
