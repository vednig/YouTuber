using System;
using Shouldly;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using YouTuber.Helper;
using YouTuber.Service;

namespace YouTuber.Test
{
    public class YouTuberMock
    {
        private static readonly Uri Sample = AppSetting.SampleId.CreateUri();

        [Test]
        public void YouTuberStressCalls()
        {
            var service = Substitute.For<IYouTuberService>();
            IEnumerable<Uri> dummyUrls = new List<Uri> { Sample, Sample };

            var counter = 0;
            service.When(e => e.Execute(Arg.Any<List<Uri>>()))
                .Do(e => counter++);

            for (int i = 0; i < 10000; i++)
            {
                service.Execute(dummyUrls);
            }

            counter.ShouldBe(10000);
        }

        [Test]
        public void YouTuberExceptionThrownTest()
        {
            var service = Substitute.For<IYouTuberService>();

            service
                .When(x => x.Execute(Arg.Any<Uri>()))
                .Do(x => throw new Exception("This is an exception message"));

            var uri = new Uri("http://local.domain.com");
            Action action = () => service.Execute(uri);
            action.ShouldThrow<Exception>().Message.ShouldStartWith("This is an exception message");
        }

        [Test]
        public void FileToListMock()
        {
            var service = Substitute.For<YouTuberService>("test");
            service.FileToList(Arg.Any<string>()).Returns(new List<Uri> { Sample, Sample });

            service.FileToList("").ShouldContain(e => e.AbsoluteUri.Contains(Sample.AbsoluteUri));
        }

        [Test]
        public void FileToListFileMock()
        {
            var service = Substitute.For<YouTuberService>("test");
            service.When(x => x.FileToList("youtubelist.txt")).DoNotCallBase();
            service.FileToList("youtubelist.txt")
                .Returns(new List<Uri>() { Sample, Sample, Sample }
                );

            var fileToList = service.FileToList("youtubelist.txt");
            var all = fileToList.ToList().All(e => e == Sample);
            all.ShouldBeTrue();
        }
    }

}
