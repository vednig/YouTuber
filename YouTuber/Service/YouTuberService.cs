using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoLibrary;
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

        //, bool extractMp3 = false

        public string Execute(Uri uri)
        {
            if (PreventDuplicates(uri))
            {
                return string.Empty;
            }
            var sb = new StringBuilder();
            var download = DownloadFromYouTube(uri);
            sb.Append(download);
            //var save = Save(uri)
            return sb.ToString();
        }

        /// <summary>
        /// Take only the first uri if duplicates found
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public bool PreventDuplicates(Uri uri)
        {
            lock (_set)
            {
                if (_set.Contains(uri))
                {
                    return true;
                }
                _set.Add(uri);
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public string DownloadFromYouTube(Uri uri)
        {
            string status;

            var video = _client.GetVideoAsync(uri);

            try
            {
                var getResultUri = video.Result.Uri;
                status = $"{getResultUri} Downloaded";
            }
            catch (Exception e)
            {
                status = $"{uri} got error message: {e.Message}";
            }
            
            return status;
        }

        public string Save(Task<YouTubeVideo> video)
        {
            string status;
            var path = $"./{BaseFolder}/{video.Result.FullName}";

            try
            {
                File.WriteAllBytes(path, video.Result.GetBytes());
                status = $"{path} Saved";
            }
            catch (Exception e)
            {
                status = $"{path} got error message: {e.Message}";
            }

            return status;
        }

        public IEnumerable<string> Execute(IEnumerable<Uri> uris)
        {
            var options = new ParallelOptions();
            var results = new List<string>();
            var maxProc = Environment.ProcessorCount;
            options.MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling(maxProc * 1.75));
            Parallel.ForEach(uris, options, e =>
            {
                var status = Execute(e);
                if (!string.IsNullOrWhiteSpace(status))
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
