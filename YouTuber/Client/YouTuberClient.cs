using System;
using VideoLibrary;
using System.Threading.Tasks;

namespace YouTuber.Client
{
    public class YouTuberClient : IYouTuberClient
    {
        public virtual Task<YouTubeVideo> GetVideoAsync(Uri uri)
        {
            var youtube = YouTube.Default;
            return youtube.GetVideoAsync(uri.AbsoluteUri);
        }
    }

}
