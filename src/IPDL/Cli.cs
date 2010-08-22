using System;
using System.IO;
using Mono.Options;
using System.Reflection;

namespace IPDL {
  public class Cli {
    public void Run(string[] args) {
      var opts = new OptionSet(){
        {"d=|download-path=", v => Directory.SetCurrentDirectory(v)},
        {"v|version",         v => ShowVersion()},
        {"h|help",            v => ShowHelp()}
      };
      var identifiers = opts.Parse(args);
      if (identifiers.Count == 0)
        ShowHelp();
      foreach (var identifier in identifiers) {
        Download(identifier);
      }
    }

    private void ShowVersion() {
      var details = Assembly.GetName();
      Console.WriteLine("{0} version {1}", details.Name, details.Version);
      Environment.Exit(0);
    }

    private void ShowHelp() {
      Console.WriteLine((new StreamReader(Assembly.GetManifestResourceStream("help.txt"))).ReadToEnd());
      Environment.Exit(0);
    }

    private Assembly Assembly {
      get { return Assembly.GetExecutingAssembly(); }
    }

    private void Download(string identifier) {
      var pid = Util.ExtractPid(identifier);
      if (pid == null) {
        Console.WriteLine("ERROR: {0} is not recognised as a programme ID", identifier);
        return;
      }
      var downloader = new Downloader(pid);
      downloader.Download(DownloadStart, DownloadProgress, DownloadEnd);
    }

    private void DownloadStart(string filename) {
      Console.WriteLine("Downloading: {0}", filename);
    }

    private void DownloadProgress(int bytesDownloaded, int total) {
      Console.CursorLeft = 0;
      Console.Write("{1:0.0}% complete; {0} left        ",
                    Util.SIFormat(total - bytesDownloaded, "B"),
                    (bytesDownloaded * 100.0) / total);
      Console.CursorLeft -= 8;
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
}
