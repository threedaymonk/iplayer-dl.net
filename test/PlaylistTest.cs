using NUnit.Framework;
using System.IO;
using System.Linq;
using IPDL;

public class PlaylistTest {
  public FileStream ReadFile(string name) {
    return new FileStream("test/data/playlist/" + name + ".xml",
                          FileMode.Open,
                          FileAccess.Read);
  }
}

[TestFixture] public class PlaylistUrlTest {
  [Test] public void ShouldGenerateUrlForPid() {
    Assert.AreEqual("http://www.bbc.co.uk/iplayer/playlist/b00t4vjz", Playlist.Url("b00t4vjz"));
  }
}

[TestFixture] public class RadioPlaylistTest : PlaylistTest {
  private Playlist playlist;

  [SetUp] public void SetUp() {
    this.playlist = new Playlist(ReadFile("b00t4vjz"));
  }

  [Test] public void ShouldHaveTitleWithPidSuffix() {
    Assert.AreEqual("Afternoon Play: Depth Charge (b00t4vjz)", playlist.Title);
  }

  [Test] public void ShouldHaveOnePlaylistItem() {
    Assert.AreEqual(1, playlist.Items.Count());
  }

  [Test] public void ShouldBeRadio() {
    Assert.AreEqual("radio", playlist.Items.First().Kind);
  }

  [Test] public void ShouldHaveIdentifier() {
    Assert.AreEqual("b00t4twn", playlist.Items.First().Identifier);
  }

  [Test] public void ShouldHaveGroup() {
    Assert.AreEqual("b00t4vjz", playlist.Items.First().Group);
  }

  [Test] public void ShouldHaveAlternate() {
    Assert.AreEqual("default", playlist.Items.First().Alternate);
  }
}

[TestFixture] public class TVPlaylistTest : PlaylistTest {
  private Playlist playlist;

  [SetUp] public void SetUp() {
    this.playlist = new Playlist(ReadFile("b00td8g6"));
  }

  [Test] public void ShouldHaveTitleWithPidSuffix() {
    Assert.AreEqual("BBC Proms: 2010: Sondheim's 80th Birthday Celebration (b00td8g6)", playlist.Title);
  }

  [Test] public void ShouldHaveOnePlaylistItem() {
    Assert.AreEqual(1, playlist.Items.Count());
  }

  [Test] public void ShouldBeTV() {
    Assert.AreEqual("tv", playlist.Items.First().Kind);
  }

  [Test] public void ShouldHaveIdentifier() {
    Assert.AreEqual("b00td81r", playlist.Items.First().Identifier);
  }

  [Test] public void ShouldHaveGroup() {
    Assert.AreEqual("b00td8g6", playlist.Items.First().Group);
  }

  [Test] public void ShouldHaveAlternate() {
    Assert.AreEqual("default", playlist.Items.First().Alternate);
  }
}

[TestFixture] public class DatestampedPlaylistTest : PlaylistTest {
  private Playlist playlist;

  [SetUp] public void SetUp() {
    this.playlist = new Playlist(ReadFile("b00tt6b2"));
  }

  [Test] public void ShouldRearrangeDateIntoYMDOrder() {
    Assert.AreEqual("In Our Time: 2010-09-23 (b00tt6b2)", playlist.Title);
  }
}
