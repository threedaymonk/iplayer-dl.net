using NUnit.Framework;
using FakeItEasy;
using System;
using System.IO;
using System.Text.RegularExpressions;
using IPDL;

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
    cli.Run(new string[] {"-v"});
    string output = GetConsoleOutput();
    StringAssert.StartsWith("test version", output);
  }

  [Test] public void ShouldShowHelp() {
    var downloader = A.Fake<Downloader>();
    var cli = new Cli(downloader);
    cli.Run(new string[] {"-h"});
    StringAssert.StartsWith("iplayer-dl (.NET version)", GetConsoleOutput());
  }
}
