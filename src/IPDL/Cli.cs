using System;
using System.IO;
using Mono.Options;
using System.Reflection;

namespace IPDL {
  class Cli {
    private Downloader downloader;
    private bool succeeded, quiet;

    public Cli(Downloader downloader) {
      this.downloader   = downloader;
      this.succeeded    = true;
      this.quiet        = false;
    }

    public Cli() : this(new Downloader()) {}

    public void Run(string[] args) {
      bool showHelp    = false;
      bool showVersion = false;
      var opts = new OptionSet(){
        {"d=|download-path=", v => Directory.SetCurrentDirectory(v)},
        {"v|version",         v => showVersion = true},
        {"q|quiet",           v => quiet = true},
        {"h|help",            v => showHelp = true}
      };
      var identifiers = opts.Parse(args);
      if (showVersion) {
        ShowVersion();
        return;
      }
      if (showHelp || identifiers.Count == 0) {
        ShowHelp();
        return;
      }
      foreach (var identifier in identifiers) {
        Download(identifier);
      }
    }

    public bool Succeeded {
      get { return this.succeeded; }
    }

    private void ShowVersion() {
      var details = Assembly.GetName();
      Console.WriteLine("{0} version {1}", details.Name, details.Version);
    }

    private void ShowHelp() {
      Console.WriteLine((new StreamReader(Assembly.GetManifestResourceStream("help.txt"))).ReadToEnd());
    }

    private Assembly Assembly {
      get { return Assembly.GetExecutingAssembly(); }
    }

    private void Download(string identifier) {
      var pid = Util.ExtractPid(identifier);
      if (pid == null) {
        if (!this.quiet) {
          Console.WriteLine("ERROR: {0} is not recognised as a programme ID", identifier);
        }
        this.succeeded = false;
        return;
      }
      downloader.Download(pid, DownloadStart, DownloadProgress, DownloadEnd);
    }

    private void DownloadStart(string filename) {
      if (this.quiet) return;

      Console.WriteLine("Downloading: {0}", filename);
    }

    private string lastOutput = "";
    private void DownloadProgress(int bytesDownloaded, int total) {
      if (this.quiet) return;

      string output = String.Format("{1:0.0}% of {0}",
                                    Util.SIFormat(total, "B"),
                                    (bytesDownloaded * 100.0) / total);
      if (output != lastOutput) {
#if !TEST_BUILD
        Console.CursorLeft = 0;
#endif
        Console.Write(output);
        lastOutput = output;
      }
    }

    private void DownloadEnd(DownloadStatus status, string message) {
      string output = "";
      switch (status) {
        case DownloadStatus.Complete:
          output = String.Format("SUCCESS: Downloaded to {0}", message);
          break;
        case DownloadStatus.Incomplete:
          output = String.Format("FAILED: Incomplete download saved as {0}", message);
          this.succeeded = false;
          break;
        case DownloadStatus.AlreadyExists:
          output = String.Format("SKIP: File exists: {0}", message);
          break;
        case DownloadStatus.Unavailable:
          output = String.Format("ERROR: {0} is currently unavailable", message);
          this.succeeded = false;
          break;
      }
      if (!this.quiet) {
        Console.WriteLine();
        Console.WriteLine(output);
      }
    }
  }
}
