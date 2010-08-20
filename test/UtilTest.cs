using NUnit.Framework;
using IPDL;

[TestFixture] public class UtilSIFormatTest {
  [Test] public void ShouldLeaveBytesUnchanged() {
    Assert.AreEqual("123 B", Util.SIFormat(123, "B"));
  }

  [Test] public void ShouldFormatKilobytesInBase10() {
    Assert.AreEqual("1.23 KB", Util.SIFormat(1234, "B"));
  }

  [Test] public void ShouldFormatMegabytesInBase10() {
    Assert.AreEqual("1.23 MB", Util.SIFormat(1234567, "B"));
  }

  [Test] public void ShouldFormatGigabytesInBase10() {
    Assert.AreEqual("1.23 GB", Util.SIFormat(1234567890, "B"));
  }

  [Test] public void ShouldRound0Point5Up() {
    Assert.AreEqual("1.24 GB", Util.SIFormat(1235000000, "B"));
  }
}

[TestFixture] public class UtilPidExtractionTest {
  [Test] public void ShouldExtractPidFromDesktopUrl() {
    Assert.AreEqual("b00tj7rp", Util.ExtractPid("http://www.bbc.co.uk/iplayer/episode/b00tj7rp/Digging_for_Britain_The_Romans/"));
  }

  [Test] public void ShouldExtractPidFromProgrammesUrl() {
    Assert.AreEqual("b00tc8b1", Util.ExtractPid("http://www.bbc.co.uk/programmes/b00tc8b1"));
  }

  [Test] public void ShouldExtractPidFromIphoneUrl() {
    Assert.AreEqual("b007jn0h", Util.ExtractPid("http://www.bbc.co.uk/mobile/iplayer/episode/b007jn0h"));
  }

  [Test] public void ShouldRecognisePidOnItsOwn() {
    Assert.AreEqual("b007jn0h", Util.ExtractPid("b007jn0h"));
  }

  [Test] public void ShouldExtractPidFromUnfamiliarUrl() {
    Assert.AreEqual("b007jn0h", Util.ExtractPid("foo/bar/abcdefgh/b007jn0h/hoge/hogehoge"));
  }

  [Test] public void ShouldReturnNullWhenNoPidIsFound() {
    Assert.IsNull(Util.ExtractPid("meh"));
  }
}
