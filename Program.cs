using System.Text.RegularExpressions;
using System.Net;

namespace TWOch
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("args needed");
                return;
            }

            string source = args[0];
            string path = args[1];

            string domen = GetDomenFromUrl(source);

            FileInfo fileInfo;
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            dirInfo.Create();
            List<string> urls = new List<string>();
            string[] fileNameInCurrentDir = dirInfo
                .EnumerateFiles()
                .Select(x => x.Name)
                .ToArray();

            string str = "";
            double totalVolume = 0;

            using(WebClient wc = new WebClient())
            {
                str = wc.DownloadString(source);
                Console.WriteLine(str.Length + " characters downloaded");
            }
            
            MediaLinkCollection mlk = new MediaLinkCollection(str, domen);

            if(mlk.MediaLinks.Count == 0)
            {
                Console.WriteLine("no media founded");
                return;
            }
            System.Console.WriteLine(new string('-', 80));
            foreach(var mediaLink in mlk.MediaLinks)
            {
                if(fileNameInCurrentDir.Contains(mediaLink.Name))
                    continue;

                fileInfo = new FileInfo(path + "/" + mediaLink.Name);

                if(!urls.Contains(mediaLink.Url))
                {
                    urls.Add(mediaLink.Url);

                    using(WebClient wc = new WebClient())
                    {
                        Console.Write(domen + "/.../" + mediaLink.Name);
                        FileStream fs = fileInfo.OpenWrite();
                        byte[] data = wc.DownloadData(mediaLink.Url);
                        long bytesLen = data.Length;
                        fs.Write(data);
                        fs.Close();
                        double volume = Math.Round((double)bytesLen / (double)(1024 * 1024), 1);
                        totalVolume += volume;
                        Console.Write($" => done (~{volume} mb)\n");
                    }
                }
            }
            System.Console.WriteLine(new string('-', 80));
            Console.WriteLine("Downloaded: " + urls.Count() + " media units, total volume: "
                + Math.Round(totalVolume, 1) + " mb.");
            System.Console.WriteLine("Current count of different file extensions:");
            int totalExtCount = 0;
            foreach(var e in GetExtList(dirInfo))
            {
                totalExtCount += e.Value;
                Console.WriteLine(e.Key + $"|{e.Value} files".PadLeft(10));
            }
            System.Console.WriteLine("Total count : " + totalExtCount + " files");
        }
        
        public static Dictionary<string, int> GetExtList(DirectoryInfo dirInfo)
        {
            string[] extList = dirInfo
                .EnumerateFiles()
                .Select(str => str.Name)
                .ToArray();
            Regex reg = new Regex(@"\.\w+");
            Dictionary<string, int> result = new Dictionary<string, int>();
            foreach(var item in extList)
            {
                string ext = reg.Match(item).Value;
                if(!result.ContainsKey(ext))
                {
                    result[ext] = 1;
                }
                else
                {
                    result[ext] += 1;
                }
            }
            return result;
        }

        public static string GetDomenFromUrl(string url)
        {
            string str = (new Regex(@"\w+://(\w|\.)+/")).Match(url).Value;
            return string.Join("", str.SkipLast(1));
        }
    }
}
