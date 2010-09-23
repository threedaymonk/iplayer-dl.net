using NUnit.Framework;
using FakeItEasy;
using System;
using System.IO;
using System.Text.RegularExpressions;
using IPDL;

class DownloaderWithFixedStatus : Downloader {
  private DownloadStatus status;
  private string message;

  public DownloaderWithFixedStatus(DownloadStatus status, string message) {
    this.status = status;
    this.message = message;
  }

  public override void Download(string pid, AtStartHandler atStart, ProgressHandler progress, AtEndHandler atEnd) {
    atStart("example.mp4");
    progress(1, 2);
    atEnd(this.status, this.message);
  }
}

[TestFixture] public class CliTest {
  private TextWriter originalOutput;
  private MemoryStream outputStream;

  [SetUp] public void SetUp() {
    originalOutput = Console.Out;
    outputStream   = new MemoryStream();
    Console.SetOut(new StreamWriter(outputStream));
  }

  [TearDown] public void TearDown() {
    Console.SetOut(originalOutput);
  }

  private string GetConsoleOutput() {
    Console.Out.Flush();
    outputStream.Seek(0, SeekOrigin.Begin);
    return (new StreamReader(outputStream)).ReadToEnd();
  }

  [Test] public void ShouldShowVersion() {
    var downloader = A.Fake<Downloader>();
    var cli = new Cli(downloader);
    cli.Run(new string[] {"--version"});
    string output = GetConsoleOutput();
    StringAssert.StartsWith("test version", output);
  }

  [Test] public void ShouldShowHelp() {
    var downloader = A.Fake<Downloader>();
    var cli = new Cli(downloader);
    cli.Run(new string[] {"--help"});
    StringAssert.StartsWith("iplayer-dl (.NET version)", GetConsoleOutput());
  }

  [Test] public void ShouldHaveSucceededIfVersionWasRequested() {
    var downloader = A.Fake<Downloader>();
    var cli = new Cli(downloader);
    cli.Run(new string[] {"--version"});
    Assert.IsTrue(cli.Succeeded);
  }

  [Test] public void ShouldHaveSucceededIfHelpWasRequested() {
    var downloader = A.Fake<Downloader>();
    var cli = new Cli(downloader);
    cli.Run(new string[] {"--help"});
    Assert.IsTrue(cli.Succeeded);
  }

  [Test] public void ShouldHaveSucceededIfDownloadCompleted() {
    var downloader = new DownloaderWithFixedStatus(DownloadStatus.Complete, "blah.mp4");
    var cli = new Cli(downloader);
    cli.Run(new string[] {"b0000000"});
    Assert.IsTrue(cli.Succeeded);
  }

  [Test] public void ShouldHaveSucceededIfDownloadWasSkipped() {
    var downloader = new DownloaderWithFixedStatus(DownloadStatus.AlreadyExists, "blah.mp4");
    var cli = new Cli(downloader);
    cli.Run(new string[] {"b0000000"});
    Assert.IsTrue(cli.Succeeded);
  }

  [Test] public void ShouldNotHaveSucceededIfDownloadWasIncomplete() {
    var downloader = new DownloaderWithFixedStatus(DownloadStatus.Incomplete, "blah.mp4.partial");
    var cli = new Cli(downloader);
    cli.Run(new string[] {"b0000000"});
    Assert.IsFalse(cli.Succeeded);
  }

  [Test] public void ShouldNotHaveSucceededIfDownloadWasUnavailable() {
    var downloader = new DownloaderWithFixedStatus(DownloadStatus.Unavailable, "b0000000");
    var cli = new Cli(downloader);
    cli.Run(new string[] {"b0000000"});
    Assert.IsFalse(cli.Succeeded);
  }

  [Test] public void ShouldNotHaveSucceededIfPidWasBogus() {
    var downloader = A.Fake<Downloader>();
    var cli = new Cli(downloader);
    cli.Run(new string[] {"xxx"});
    Assert.IsFalse(cli.Succeeded);
  }

  [Test] public void ShouldNotPrintToConsoleIfQuietFlagIsSet() {
    var downloader = new DownloaderWithFixedStatus(DownloadStatus.Complete, "blah.mp4");
    var cli = new Cli(downloader);
    cli.Run(new string[] {"--quiet", "b0000000"});
    Assert.AreEqual("", GetConsoleOutput());
  }
}
