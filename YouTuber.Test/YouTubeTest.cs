using System;
using Shouldly;
using System.IO;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using NSubstitute;
using YouTuber.Client;
using YouTuber.Helper;
using YouTuber.Service;

namespace YouTuber.Test
{
    [TestFixture]
    public class YouTubeTest
    {
        private static readonly string RunningPath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string FromFile = Path.GetFullPath(Path.Combine(RunningPath, @"..\..\..\Resources\YoutubeList.txt"));
        private static readonly YouTuberService Service = new YouTuberService();
        private static readonly YouTuberClient Client = new YouTuberClient();
        private static readonly Uri Sample = AppSetting.SampleId.CreateUri();
        private static readonly IList<Uri> Uris = Service.FileToList(FromFile).ToList();
        private static readonly VideoDetails Details = new VideoDetails
        {
            Link = Sample,
            StateType = StateType.Success
        };

        [Test]
        public void YouTuberClientTest()
        {
            var video = Client.GetVideoAsync(Sample);
            var result = video.Result;
            result.Title.ShouldStartWith("Full HD Test Screen - YouTube");
            result.GetBytes().ShouldNotBeEmpty();
        }

        [Test]
        public void FileToListTest()
        {
            Uris.Count.ShouldBe(3);
            Uris.Contains(Sample).ShouldBeTrue();
            Uris.ShouldAllBe(e => e.AbsoluteUri.StartsWith("https://www.youtube.com/watch?v="));
        }

        [Test]
        public void PreventDuplicatesTest()
        {
            var uris = new List<Uri> { Sample, Sample, Sample };
            var list = uris.Select(uri => Service.PreventDuplicates(new VideoDetails { Link = uri })).ToList();
            list.Count.ShouldBe(uris.Count);
            list.FirstOrDefault()?.StateType.ShouldBe(StateType.Success);
            list.Skip(1).All(e => e.StateType == StateType.Error).ShouldBeTrue();
            list.Skip(1).All(e => e.Error.StartsWith("Duplicated")).ShouldBeTrue();
        }

        [Test]
        public void YouTuberWrongUriTest()
        {
            var service = Substitute.For<YouTuberService>();
            var uri = new Uri(Sample.AbsoluteUri.Substring(4));
            var details = service.Execute(uri);
            details.StateType.ShouldBe(StateType.Exception);
            details.Error.ShouldBe($"DownloadFromYouTube exception: One or more errors occurred. (URL is not a valid YouTube URL!)");
        }

        [Test]
        public void YouTuberDownloadWrongIdTest()
        {
            var unavailableVideo = "FScfGUfrQaM".CreateUri();
            var details = Service.Execute(unavailableVideo);
            details.StateType.ShouldBe(StateType.Exception);
            details.Error.ShouldBe("DownloadFromYouTube exception: One or more errors occurred. (Sequence contains no elements)");
            details.Video.ShouldBeNull();
        }

        [Test]
        public void SaveFileTest()
        {
            Service.DownloadFromYouTube(Details);
            Service.Save(Details);

            var path = $"./{AppSetting.BaseFolder}/{Details.Video.Result.FullName}";

            var file = new FileInfo(path);

            file.Exists.ShouldBeTrue();
            file.Name.ShouldStartWith("Full HD Test Screen");

            file.Delete();
        }

        [Test]
        public void DoneTest()
        {
            Details.Done.ShouldBe(DateTime.MinValue);
            Service.Done(Details);
            var x = DateTime.Now.Subtract(Details.Done);
            x.TotalMilliseconds.ShouldBeLessThan(0.1);
            Details.Done.ShouldBeGreaterThan(Details.Created);
        }
    }

}
