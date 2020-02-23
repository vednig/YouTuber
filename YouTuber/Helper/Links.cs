using System;
using System.ComponentModel;

namespace YouTuber.Helper
{
    public static class Links
    {
        private const string BaseUrl = "https://www.youtube.com";
        private const string ShortUrl = "https://youtu.be";
        private const string ImageUrl = "https://img.youtube.com";

        /// <summary>
        /// Create standard youtube uri
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Uri CreateUri(this string id)
        {
            return new Uri($"{BaseUrl}/watch?v={id}");
        }

        /// <summary>
        /// Create shorten version of youtube uri
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Uri CreateShortenUri(this string id)
        {
            return new Uri($"{ShortUrl}/{id}");
        }

        /// <summary>
        /// Get video details as json from youtube
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Uri GetVideoDetailsUri(this string id)
        {
            return new Uri($"{BaseUrl}/oembed?url=http%3A//www.youtube.com/watch?v%3D{id}&format=json");
        }

        /// <summary>
        /// Check if id contains exact 11 characters
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsIdLengthValid(this string id)
        {
            return id.Length == 11;
        }

        //public static bool 
        //test online as well with json


        public static Uri GetVideoThumbnailUri(this string id, ImageSize imageSize)
        {
            var url = $"{ImageUrl}/vi/{id}/";

            switch (imageSize)
            {
                case ImageSize.NormalQuality:
                    url = $"{url}default.jpg";
                    break;
                case ImageSize.MediumQuality:
                    url = $"{url}mqdefault.jpg";
                    break;
                case ImageSize.HighQuality:
                    url = $"{url}hqdefault.jpg";
                    break;
                case ImageSize.StandardDefinition:
                    url = $"{url}sddefault.jpg";
                    break;
                case ImageSize.MaximumResolution:
                    url = $"{url}maxresdefault.jpg";
                    break;
                case ImageSize.Background:
                    url = $"{url}0.jpg";
                    break;
                case ImageSize.Start:
                    url = $"{url}1.jpg";
                    break;
                case ImageSize.Middle:
                    url = $"{url}2.jpg";
                    break;
                case ImageSize.End:
                    url = $"{url}3.jpg";
                    break;
                default:
                    url = $"{url}default.jpg";
                    break;
            }

            return new Uri(url);
        }
    }

    public enum ImageSize
    {
        [Description("0-480x360")]
        Background,
        [Description("1-120x90")]
        Start,
        [Description("2-120x90")]
        Middle,
        [Description("3-120x90")]
        End,
        [Description("default-120x90")]
        NormalQuality,
        [Description("mq-320x180")]
        MediumQuality,
        [Description("hq-480x360")]
        HighQuality,
        [Description("sd-640x480")]
        StandardDefinition,
        [Description("hd-1920x1080")]
        MaximumResolution,
    }

}
