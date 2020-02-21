using System;
using VideoLibrary;
using System.Threading.Tasks;

namespace YouTuber
{
    public class VideoDetails
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Done { get; set; }
        public Uri Link { get; set; }
        public StateType StateType { get; set; }
        public string Error { get; set; }
        public Exception Ex { get; set; }
        public Task<YouTubeVideo> Video { get; set; }
    }
    
    public enum StateType
    {
        Success,
        Error,
        Exception
    }
}
