using System;
using System.Data;
using VideoLibrary;

namespace YouTuber.Model
{
    public class YouTuberInfo
    {
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Done { get; set; }
        public Uri Link { get; set; }
        public State State { get; set; }
        public YouTubeVideo YouTubeVideo { get; set; }
    }

    public class State
    {
        public StateType StateType { get; set; }
        public string Error { get; set; }
    }

    public enum StateType
    {
        Success,
        Error,
        Failed,
        Exception
    }
}
