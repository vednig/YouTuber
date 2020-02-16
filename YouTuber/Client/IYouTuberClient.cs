using System;
using System.Threading.Tasks;
using VideoLibrary;

namespace YouTuber.Client
{
    public interface IYouTuberClient
    {
        Task<YouTubeVideo> GetVideoAsync(Uri uri);
    }
}
