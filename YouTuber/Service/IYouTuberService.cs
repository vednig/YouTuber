using System;
using System.Collections.Generic;

namespace YouTuber.Service
{
    public interface IYouTuberService
    {
        VideoDetails Execute(Uri uri);
        IEnumerable<VideoDetails> Execute(IEnumerable<Uri> uris);
    }

}
