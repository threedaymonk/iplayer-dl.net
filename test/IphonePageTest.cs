using NUnit.Framework;
using System.IO;
using IPDL;

public class IphonePageTest {
  public FileStream ReadFile(string name) {
    return new FileStream("test/data/episode/" + name + ".html",
                          FileMode.Open,
                          FileAccess.Read);
  }
}

[TestFixture] public class IphonePageUrlTest {
  [Test] public void ShouldGenerateUrlForPid() {
    Assert.AreEqual("http://www.bbc.co.uk/mobile/iplayer/episode/b00t4vjz", IphonePage.Url("b00t4vjz"));
  }
}

[TestFixture] public class TVPageTest : IphonePageTest {
  private IphonePage page;

  [SetUp] public void SetUp() {
    this.page = new IphonePage(ReadFile("b00td8g6"));
  }

  [Test] public void ShouldExtractEmbeddedMediaUrl() {
    Assert.AreEqual("http://download.iplayer.bbc.co.uk/iplayer_streaming_http_mp4/5500147145443163744.mp4?token=iVXXxZp7S9ghZFBoBk1zMqZkty%2FxVaSS5auvKTc39ly9Uya9t4k%3D%0A", page.EmbeddedMediaUrl);
  }

  [Test] public void ShouldBeMP4() {
    Assert.AreEqual(".mp4", page.FileExtension);
  }

  [Test] public void ShouldBeAvailable() {
    Assert.IsTrue(page.IsAvailable);
  }
}

[TestFixture] public class RadioPageTest : IphonePageTest {
  private IphonePage page;

  [SetUp] public void SetUp() {
    this.page = new IphonePage(ReadFile("b00t4vjz"));
  }

  [Test] public void ShouldExtractEmbeddedMediaUrl() {
    Assert.AreEqual("http://download.iplayer.bbc.co.uk/iplayer_streaming_http_mp4/httpdl_iphone/direct/radio4/secure_auth//RBN2_radio_4_fm_-_friday_1415_b00t4twn_2010_07_30_14_24_19.mp3?token=iVXXxZp7S9gva1BoBioETKQK8XmkCP755cf4clstnQ7hVzLo4ucl9aq6oL1P8gzhOd00HPOhSEr6%0A9s2V%2FBc%2F5oZ9Q2l0cFWhpMov3b4%3D%0A", page.EmbeddedMediaUrl);
  }

  [Test] public void ShouldBeMP3() {
    Assert.AreEqual(".mp3", page.FileExtension);
  }

  [Test] public void ShouldBeAvailable() {
    Assert.IsTrue(page.IsAvailable);
  }
}

[TestFixture] public class ExpiredPageTest : IphonePageTest {
  private IphonePage page;

  [SetUp] public void SetUp() {
    this.page = new IphonePage(ReadFile("expired"));
  }

  [Test] public void ShouldNotBeAvailable() {
    Assert.IsFalse(page.IsAvailable);
  }
}
