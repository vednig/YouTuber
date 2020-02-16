using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YouTuber.Helper;
using YouTuber.Service;

namespace YouTuber.Cl
{
    public class Program
    {
        private static readonly YouTuberService Service = new YouTuberService();

        //Sample download
        private static readonly IEnumerable<Uri> SampleVideos = new List<Uri>
        {
           new Uri("https://www.youtube.com/watch?v=y9ajRIgTJNA"),
           new Uri("https://www.youtube.com/watch?v=dE40z4fODQA")
        };

        public static void Main(string[] args)
        {
            var input = args.Length == 0 ? "" : args[0];

            if (input.StartsWith("-"))
            {
                switch (input)
                {
                    case "-d":
                    case "--demo":
                        CreateSampleList();
                        Service.Execute(SampleVideos);
                        break;
                    case "-v":
                    case "--version":
                        Console.WriteLine(Helpers.GetVersion());
                        break;
                    case "-h":
                    case "--help":
                        Help();
                        break;
                    default:
                        Help();
                        break;
                }
            }
            else
            {
                if (input.EndsWith(".txt") && args.Length == 1)
                {
                    var uris = Service.FileToList(input).ToArray();
                    if (uris.Any())
                    {
                        Service.Execute(uris);
                    }
                    else
                    {
                        Console.WriteLine("Your list is empty");
                    }
                }
                else
                {
                    try
                    {
                        Service.Execute(GetList(input));
                    }
                    catch
                    {
                        Console.WriteLine("Some thing went wrong");
                    }
                }
            }
        }

        private static void CreateSampleList()
        {
            File.WriteAllLines("download.txt", SampleVideos.Select(e => e.ToString()));
        }

        private static IEnumerable<Uri> GetList(string input)
        {
            return input.Split(';').Select(e => new Uri(e));
        }

        private static void Help()
        {
            Console.WriteLine("[-h | --help] Get help");
            Console.WriteLine("[-v | --version] Get version");
            Console.WriteLine("[-d | --demo] Create a download.txt file with a list of youtube samples and starts downloading the samples");
            Console.WriteLine("youtube.exe followed by url or video id, use ';' as separator for multiple urls/Ids");
            Console.WriteLine();
            Console.WriteLine("example1: youtuber.exe 'https://www.youtube.com/watch?v=y9ajRIgTJNA'");
            Console.WriteLine("example2: youtuber.exe 'y9ajRIgTJNA;https://www.youtube.com/watch?v=NcumhqTDPpE'");
            Console.WriteLine("example3: youtuber.exe 'y9ajRIgTJNA;pYlYt9iuJdc;NcumhqTDPpE'");
            Console.WriteLine("example4: youtuber.exe 'https://www.youtube.com/watch?v=y9ajRIgTJNA;https://www.youtube.com/watch?v=NcumhqTDPpE'");
            Console.WriteLine("example5: youtuber.exe download.txt");
            Console.WriteLine("[download.txt] Create your own list that contains valid youtube links");
            Console.WriteLine("By using this App, you agree to be bound by the terms and conditions of this Agreement");
        }
    }
}