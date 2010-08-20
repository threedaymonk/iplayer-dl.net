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


