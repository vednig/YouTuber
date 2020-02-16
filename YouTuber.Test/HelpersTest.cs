using NUnit.Framework;
using Shouldly;
using YouTuber.Helper;

namespace YouTuber.Test
{
    [TestFixture]
    public class HelpersTest
    {
        [Test]
        public void GetVersionTest()
        {
            Helpers.GetVersion().ShouldBe("2.0.0.0");
        }
    }
}
