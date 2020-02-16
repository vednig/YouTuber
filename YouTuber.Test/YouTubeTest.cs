using System;
using Shouldly;
using System.IO;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using NSubstitute;
using YouTuber.Client;
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
            result.Title.ShouldStartWith("I am basketball man, street performer");
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
        public void YouTuberWrongUriTest()
        {
            var service = Substitute.For<YouTuberService>();
            var uri = new Uri(Sample.AbsoluteUri.Substring(4));
            //Service.Execute(uri);
            service.Execute(uri);

            //service.

            var xz = service.ReceivedCalls();
            service.Returns(e => e[0]);

            //result.Title.ShouldStartWith("McDonalds FUNNY AD - YouTube");
            //result.GetBytes().ShouldNotBeEmpty();
            Console.WriteLine(xz.GetEnumerator());
        }

        [Test]
        public void PreventDuplicatesTest()
        {
            var uri = new List<Uri>
            {
                new Uri("http://local.domain.com"),
                new Uri("http://local.domain.com"),
                new Uri("http://local.domain.com"),
                new Uri("http://local.domain.com"),
                new Uri("http://local.domain.com")
            };

            var result = uri.Select(e => Service.PreventDuplicates(e)).ToList();

            result.Count.ShouldBe(uri.Count);
            result.FirstOrDefault().ShouldBeFalse();
            result.Skip(1).All(e => e).ShouldBeTrue();
        }

        [Test]
        public void DownloadFromYouTubePreventDuplicatesTest()
        {
            var uri = new List<Uri>
            {
                new Uri("http://local.domain.com"),
                new Uri("http://local.domain.com"),
                new Uri("http://local.domain.com"),
                new Uri("http://local.domain.com"),
                new Uri("http://local.domain.com")
            };

            var result = Service.Execute(uri);

            var enumerable = result as string[] ?? result.ToArray();
            enumerable.Length.ShouldBe(1);
            enumerable.FirstOrDefault().ShouldBe("Start downloading of: http://local.domain.com/ is fail with error message: One or more errors occurred. (URL is not a valid YouTube URL!)");
        }

    }
}