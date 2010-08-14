using System;
using System.IO;
using IPDL;

public class App {
  public static void Main(string[] args) {
    foreach (var arg in args) {
      (new App()).Download(arg);
    }
  }

  private void Download(string pid) {
    var downloader = new Downloader(pid);
    downloader.Download(DownloadStart, DownloadProgress, DownloadEnd);
  }

  private string FormatBytes(int b) {
    string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
    double scaled = (double)b;
    int i;
    for (i = 0; i < suffixes.Length; i++) {
      scaled = b / Math.Pow(1000.0, i);
      if (scaled < 1000) break;
    }
    return String.Format("{0:0.00} {1}", scaled, suffixes[i]);
  }

  private void DownloadStart(string filename) {
    Console.WriteLine("Downloading: {0}", filename);
  }

  private void DownloadProgress(int bytesDownloaded, int total) {
    Console.CursorLeft = 0;
    Console.Write("{1:0.0}% complete; {0} left",
                  FormatBytes((total - bytesDownloaded)),
                  (bytesDownloaded * 100.0) / total);
  }

  private void DownloadEnd(Downloader.Status status, string message) {
    Console.WriteLine();
    switch (status) {
      case Downloader.Status.Complete:
        Console.WriteLine("SUCCESS: Downloaded to {0}", message);
        break;
      case Downloader.Status.Incomplete:
        Console.WriteLine("FAILED: Incomplete download saved as {0}", message);
        break;
      case Downloader.Status.AlreadyExists:
        Console.WriteLine("SKIP: File exists: {0}", message);
        break;
      case Downloader.Status.Unavailable:
        Console.WriteLine("ERROR: {0} is currently unavailable", message);
        break;
    }
  }
}
