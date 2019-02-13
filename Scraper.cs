using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CsvHelper;
using HtmlAgilityPack;

namespace AmazonScraper
{
    public class Scraper
    {
        private ObservableCollection<EntryModel> _entries = new ObservableCollection<EntryModel>();

        public ObservableCollection<EntryModel> Entries
        {
            get { return _entries; }
            set { _entries = value; }
        }


        public void ScrapeData(string page)
        {
            var web = new HtmlWeb();
            string html;
            using (var wc = new GZipWebClient())
                html = wc.DownloadString(page);

            var htmldocObject = new HtmlDocument();
            htmldocObject.LoadHtml(html);

            //var doc = web.Load(page);

            var Articles = htmldocObject.DocumentNode.SelectNodes("//*[@class = 's-result-item  celwidget  ']");

            foreach (var articles in Articles)
            {
                var header = HttpUtility.HtmlDecode(articles.SelectSingleNode(".//div[@class = 'a-row a-spacing-mini']").InnerText);
                var description = HttpUtility.HtmlDecode(articles
                    .SelectSingleNode(".//div[@class = 'a-row a-spacing-none']")
                    .InnerText);

                Debug.Print($"Title: {header} \n Description {description}");
                _entries.Add(new EntryModel {Title = header, Description = description});
            }

        }

        public void Export()
        {
            using (TextWriter tw = File.CreateText("SampleData.csv"))
            {
                using (var cw = new CsvWriter(tw))
                {
                    foreach (var entry in Entries)
                    {
                        cw.WriteRecord(entry);
                        cw.NextRecord();
                    }
                }
            }
        }
    }
}
