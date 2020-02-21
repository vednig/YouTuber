using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YouTuber.Client;
using YouTuber.Helper;

namespace YouTuber.Service
{
    public class YouTuberService : IYouTuberService
    {
        private const string BaseFolder = "downloads";
        private readonly HashSet<Uri> _set = new HashSet<Uri>();
        private readonly IYouTuberClient _client;

        public YouTuberService()
        {
            _client = new YouTuberClient();
            CreateFolder(BaseFolder);
        }

        //bool extractMp3 = false

        public VideoDetails Execute(Uri uri)
        {
            var details = new VideoDetails
            {
                Link = uri,
                StateType = StateType.Success
            };

            PreventDuplicates(details);
            DownloadFromYouTube(details);
            Save(details);
            Done(details);

            return details;
        }

        /// <summary>
        /// Take only the first details if duplicates found
        /// </summary>
        /// <returns></returns>
        public VideoDetails PreventDuplicates(VideoDetails details)
        {
            lock (_set)
            {
                if (_set.Contains(details.Link))
                {
                    details.StateType = StateType.Error;
                    details.Error = $"Duplicated {details.Link}";
                    return details;
                }
                _set.Add(details.Link);
            }

            return details;
        }

        /// <summary>
        /// Time stamp when done successfully
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        public VideoDetails Done(VideoDetails details)
        {
            if (details.StateType != StateType.Success)
            {
                return details;
            }

            details.Done = DateTime.Now;
            return details;
        }

        /// <summary>
        /// Download content from Youtube
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        public VideoDetails DownloadFromYouTube(VideoDetails details)
        {
            if (details.StateType != StateType.Success)
            {
                return details;
            }

            var video = _client.GetVideoAsync(details.Link);

            try
            {
                var s = video.Result.Uri;
                if (string.IsNullOrWhiteSpace(s))
                {
                    details.StateType = StateType.Error;
                    details.Error = $"{Helpers.GetCurrentMethod()} error";
                }
                else
                {
                    details.Video = video;
                }
            }
            catch (Exception ex)
            {
                details.Ex = ex;
                details.StateType = StateType.Exception;
                details.Error = $"{Helpers.GetCurrentMethod()} exception: {ex.Message}";
            }

            return details;
        }

        /// <summary>
        /// Save video to disk
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        public VideoDetails Save(VideoDetails details)
        {
            if (details.StateType != StateType.Success)
            {
                return details;
            }

            var path = $"./{BaseFolder}/{details.Video.Result.FullName}";

            try
            {
                File.WriteAllBytes(path, details.Video.Result.GetBytes());
            }
            catch (Exception ex)
            {
                details.Ex = ex;
                details.StateType = StateType.Exception;
                details.Error = $"Save exception: {ex.Message}";
            }

            return details;
        }

        public IEnumerable<VideoDetails> Execute(IEnumerable<Uri> uris)
        {
            var options = new ParallelOptions();
            var results = new List<VideoDetails>();
            var maxProc = Environment.ProcessorCount;
            options.MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling(maxProc * 1.75));
            Parallel.ForEach(uris, options, e =>
            {
                var status = Execute(e);
                if (status.StateType == StateType.Success)
                {
                    results.Add(status);
                }
            });
            return results;
        }

        private static Uri ValidateYoutubeLink(string id)
        {
            if (id.IsValidId())
                return id.GetUnifiedUri();

            throw new Exception("Some thing is wrong with your video id");
        }

        public virtual IEnumerable<Uri> FileToList(string file)
        {
            IEnumerable<Uri> results;
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                using (var sr = new StreamReader(fs))
                {
                    results = sr.ReadToEnd()
                        .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(e => new Uri(e));
                }
            }
            return results;
        }

        private static void CreateFolder(string folder)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), folder);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private static string CleanFilename(string rawFilename)
        {
            return rawFilename
                .Replace(" - YouTube", "")
                .Replace(".webm", "")
                .Replace(".mp3", "")
                .Replace(".mp4", "");
        }

        private static void TryToDelete(string file)
        {
            try
            {
                File.Delete(file);
            }
            catch (IOException ex)
            {
            }
        }
    }

}
