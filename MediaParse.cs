using System;
using System.Text.RegularExpressions;

namespace TWOch
{
    public class MediaLink
    {
        public readonly string Url;
        public string Ext { get; set; }
        public string Name { get; set; }

        
        public MediaLink(string url)
        {
            if(url.Length == 0)
                throw new ArgumentException($"url string(\"{url}\"): invalid format or empty");
            Url = url;
            Name = GetNameFromUrl(url);
            Ext = GetExtFromUrl(url);
        }
        public MediaLink(string url, string name)
        {
            Url = url;
            if(url.Length == 0)
                throw new ArgumentException($"url string(\"{url}\"): invalid format or empty");
            if(name.Length == 0)
                Name = GetNameFromUrl(url);
            else Name = name;
            Ext = GetExtFromUrl(url);
        }
        public MediaLink(string url, string name, string ext)
        {
            Url = url;
            if(url.Length == 0)
                throw new ArgumentException($"url string(\"{url}\"): invalid format or empty");
            if(name.Length == 0)
                Name = GetNameFromUrl(url);
            else Name = name;
            if(ext.Length == 0)
                Ext = GetExtFromUrl(url);
            else Ext = ext;
        }

        private string GetNameFromUrl(string url)
        {
            Regex reg = new Regex(@"\w+\.\w+$");
            return reg.Match(url).Value;
        }

        private string GetExtFromUrl(string name)
        {
            Regex reg = new Regex(@"\.\w+$");
            return reg.Match(name).Value;
        }

        public static MediaLink ToMediaLink(string url)
        {
            return new MediaLink(url);
        }
    }
    //
    
    public class MediaLinkCollection
    {
        public List<MediaLink> MediaLinks;

        public MediaLinkCollection(string sourceText, string domen)
        {
            InitMedialinksList(sourceText, domen);
        }

        private void InitMedialinksList(string sourceText, string domen)
        {
            Regex reg = new Regex(@"data-src=\S*" + "\"");
            var matches = reg.Matches(sourceText);
            MediaLinks = new List<MediaLink>();
            List<string> tempCont = matches.Select(x => x.Value).Distinct().ToList();
            tempCont.ForEach(
                                str => MediaLinks.Add(MediaLink
                                                    .ToMediaLink(
                                                        domen
                                                        + str.Replace("data-src=","")
                                                             .Replace("\"","")))
                            );
        }
    } 
}