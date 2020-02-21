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
        private static readonly Uri Sample = new Uri("https://www.youtube.com/watch?v=y9ajRIgTJNA");

        [Test]
        public void YouTuberClientTest()
        {
            var video = Client.GetVideoAsync(Sample);
            var result = video.Result;
            result.Title.ShouldStartWith("McDonalds FUNNY AD - YouTube");
            result.GetBytes().ShouldNotBeEmpty();
        }

        [Test]
        public void FileToListTest()
        {
            IList<Uri> list = Service.FileToList(FromFile).ToList();

            list.Count.ShouldBe(2);
            list.ShouldContain(Sample);
            list.ShouldAllBe(e => e.AbsoluteUri.StartsWith("https://www.youtube.com/watch?v="));
        }

        [Test]
        public void ResourceFileTest()
        {
            var list = Service.FileToList(FromFile).ToList();
            var contains = list.Contains(Sample);
            contains.ShouldBeTrue();
        }

        [Test]
        public void PreventDuplicatesTest()
        {
            var uris = new List<Uri> { Sample, Sample, Sample };
            var result = uris.Select(uri => Service.PreventDuplicates(new VideoDetails { Link = uri })).ToList();
            result.Count.ShouldBe(uris.Count);
            result.FirstOrDefault()?.StateType.ShouldBe(StateType.Success);
            result.Skip(1).All(e => e.StateType == StateType.Error).ShouldBeTrue();
        }

        [Test]
        public void YouTuberWrongUriTest()
        {
            var service = Substitute.For<YouTuberService>();
            var uri = new Uri(Sample.AbsoluteUri.Substring(4));
            var result = service.Execute(uri);
            result.StateType.ShouldBe(StateType.Exception);
            result.Error.ShouldBe($"DownloadFromYouTube exception: One or more errors occurred. (URL is not a valid YouTube URL!)");
        }

        [Test]
        public void YouTuberDownloadWrongIdTest()
        {
            var unavailableVideo = new Uri("https://www.youtube.com/watch?v=FScfGUfrQaM");
            var video = Service.Execute(unavailableVideo);
            video.StateType.ShouldBe(StateType.Exception);
            video.Error.ShouldBe("DownloadFromYouTube exception: One or more errors occurred. (Sequence contains no elements)");
            video.Video.ShouldBeNull();
        }

        // mock first to be true and the rest fails??
        [Test]
        public void DownloadFromYouTubePreventDuplicatesTest()
        {
            var badUri = new Uri("http://local.domain.com");
            var uri = new List<Uri> { badUri, badUri, badUri, badUri };
            var result = Service.Execute(uri);
            var enumerable = result.ToList();
            enumerable.Count.ShouldBe(0);
        }

        //PreventDuplicates(details);
        //DownloadFromYouTube(details);
        //Save(details);
        //Done(details);
    }
}