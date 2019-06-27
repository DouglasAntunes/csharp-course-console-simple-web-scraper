using SimpleWebScraper.Builders;
using SimpleWebScraper.Data;
using SimpleWebScraper.Workers;
using System;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("SimpleWebScraper.Test.Unit")]
namespace SimpleWebScraper
{
    class Program
    {
        private const string Method = "search";

        static void Main(string[] args)
        {
            try
            {
                Console.Write("Please enter which city you would like to scrape information from: ");
                var craigsListCity = Console.ReadLine() ?? string.Empty;
                Console.Write("Please enter the CraigsList category that you would like to scrape: ");
                var craigsListCategoryName = Console.ReadLine() ?? string.Empty;

                using (WebClient client = new WebClient())
                {
                    string content = client.DownloadString($"https://{craigsListCity.Replace(" ", string.Empty)}.craigslist.org/{Method}/{craigsListCategoryName}");
                    ScrapeCriteria scrapeCriteria = new ScrapeCriteriaBuilder()
                        .WithData(content)
                        .WithRegex(@"<a href=\""(.*?)\"" data-id=\""(.*?)\"" class=\""result-title hdrlnk\"">(.*?)</a>")
                        .WithRegexOption(RegexOptions.ExplicitCapture)
                        .WithPart(new ScrapeCriteriaPartBuilder()
                            .WithRegex(@">(.*?)</a>")
                            .WithRegexOption(RegexOptions.Singleline)
                            .Build()
                        )
                        .WithPart(new ScrapeCriteriaPartBuilder()
                            .WithRegex(@"href=\""(.*?)\""")
                            .WithRegexOption(RegexOptions.Singleline)
                            .Build()
                        )
                    .Build();

                    Scraper scraper = new Scraper();
                    var scrapedElements = scraper.Scrape(scrapeCriteria);

                    if (scrapedElements.Any())
                    {
                        foreach (var scrapedElement in scrapedElements)
                        {
                            Console.WriteLine(scrapedElement);
                        }
                    }
                    else
                    {
                        Console.WriteLine("There where no matches for the specifies scrape criteria.");
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
