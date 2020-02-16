using System;
using System.Collections.Generic;

namespace YouTuber.Service
{
    public interface IYouTuberService
    {
        string Execute(Uri uri);
        IEnumerable<string> Execute(IEnumerable<Uri> uris);
    }

}
