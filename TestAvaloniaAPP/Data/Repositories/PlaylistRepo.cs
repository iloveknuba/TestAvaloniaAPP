using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestAvaloniaAPP.Data.Interfaces;
using System.IO;
using System.Drawing;

namespace TestAvaloniaAPP.Data.Repositories
{
    public class PlaylistRepo : IPlaylist
    {
       
        string playlistXPath = @"//*[@id=""atf""]/music-detail-header";
        string songsXPath = @"//*[@id=""Web.TemplatesInterface.v1_0.Touch.DetailTemplateInterface.DetailTemplate_1""]/music-container/div/div/div[2]/div/div/music-image-row[1]";
       public  Playlist GetPlaylist(string url)
        {
         

            HtmlWeb web = new HtmlWeb();

            var htmlDoc = web.Load(url);

            
            var nodePlaylist = htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/div/div[3]/div/music-app/div/div/div/div/div[2]/music-detail-header//div/header/div[2]/h1");
            var nodeSongs = htmlDoc.DocumentNode.SelectNodes(songsXPath);

            return new Playlist
            {
                Name = nodePlaylist.GetAttributeValue("headline",""),
                Avatar = nodePlaylist.GetAttributeValue("image-src", ""),
                Description = nodePlaylist.GetAttributeValue("secondary-text", ""),

                Songs = nodeSongs.Select(c => new Song
                {
                    AlbumName = c.GetAttributeValue("secondary-text-2", ""),
                    ArtistName = c.GetAttributeValue("secondary-text-1", ""),
                    SongName = c.GetAttributeValue("primary-text", ""),
                    Duration = 123


                })

            };

        }

        public string GetPlaylistTitle(string url) {
            try
            {
                using (HttpClientHandler httpHandler = new HttpClientHandler { AllowAutoRedirect = false, AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.None})
                {
                    using(HttpClient client = new HttpClient(httpHandler))
                    {
                        using (HttpResponseMessage resp = client.GetAsync(url).Result) 
                        {
                            if (resp.IsSuccessStatusCode) {

                                var html = resp.Content.ReadAsStringAsync().Result;
                                if(!string.IsNullOrEmpty(html))
                                {
                                    var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                                    htmlDoc.LoadHtml(html);


                                    var nodePlaylist = htmlDoc.DocumentNode.SelectSingleNode("//body");


                                    return nodePlaylist.OuterHtml;
                                }

                               
                            }
                            return "I`m gATY";
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {

                return ex.Message;
            }

            
        }

        List<Song> GetSongList(HtmlDocument doc, string html)
        {
          
           
            var list = new List<Song>();
            int index = 1;

            for (int i = 1; i < 20; i++)
            {
                if(doc.DocumentNode.SelectSingleNode($"//*[@id=\"contents\"]/ytmusic-responsive-list-item-renderer[{i}]/div[2]/div[3]/yt-formatted-string[1]/a")!= null &&
                   (doc.DocumentNode.SelectSingleNode($"//*[@id=\"contents\"]/ytmusic-responsive-list-item-renderer[{i}]/div[2]/div[3]/yt-formatted-string[2]/a") != null))
                list.Add(new Song
                {
                    ArtistName = doc.DocumentNode.SelectSingleNode($"//*[@id=\"contents\"]/ytmusic-responsive-list-item-renderer[{i}]/div[2]/div[3]/yt-formatted-string[1]/a").InnerHtml,
                    AlbumName = doc.DocumentNode.SelectSingleNode($"//*[@id=\"contents\"]/ytmusic-responsive-list-item-renderer[{i}]/div[2]/div[3]/yt-formatted-string[2]/a").InnerHtml,
                    Duration = 123,
                    SongName = doc.DocumentNode.SelectSingleNode($"//*[@id=\"contents\"]/ytmusic-responsive-list-item-renderer[{i}]/div[2]/div[1]/yt-formatted-string/a").InnerHtml
                });
            }
            
            return list;
        }
     
        public Playlist GetWebPlaylist(string url)
        {
            IWebDriver driver = new ChromeDriver("D:\\3 курс\\chromedriver-win64\\chromedriver.exe");

            try
            {
                driver.Navigate().GoToUrl(url);

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
                var jsResult = jsExecutor.ExecuteScript("return document.documentElement.outerHTML");

                // Отримуємо HTML-код сторінки з Selenium
                string pageSource = jsResult.ToString();

                // Ініціалізуємо парсер HAP та завантажуємо в нього HTML
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(pageSource);

                // Приклад парсингу заголовку сторінки
                var pageTitle = doc.DocumentNode.SelectSingleNode("//*[@id=\"contents\"]/ytmusic-responsive-list-item-renderer[12]/div[2]/div[1]/yt-formatted-string/a");
                return new Playlist
                {
                    Name = doc.DocumentNode.SelectSingleNode("//*[@id=\"header\"]/ytmusic-detail-header-renderer/div/div[2]/h2/yt-formatted-string").InnerHtml,
                    Avatar = doc.DocumentNode.SelectSingleNode("//*[@id=\"img\"]").GetAttributeValue("src", ""),
                    Description = doc.DocumentNode.SelectSingleNode("//*[@id=\"header\"]/ytmusic-detail-header-renderer/div/div[2]/yt-formatted-string[1]/span[1]").InnerHtml,
                   Songs = GetSongList(doc, pageSource)
                };
            }
            catch (Exception ex)
            {
                return new Playlist { Name = ex.Message};
            }
            finally
            {
                driver.Quit(); // Закрити драйвер після виконання всіх операцій
            }
        }

   


        public string GetPlaylist1(string url)
         {
            IWebDriver driver = new ChromeDriver("D:\\3 курс\\chromedriver-win64\\chromedriver.exe");
            int index = 2;
            try
            {
                driver.Navigate().GoToUrl(url);

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
                var jsResult = jsExecutor.ExecuteScript("return document.documentElement.outerHTML");

                // Отримуємо HTML-код сторінки з Selenium
                string pageSource = jsResult.ToString();

                // Ініціалізуємо парсер HAP та завантажуємо в нього HTML
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(pageSource);

               
                // Приклад парсингу заголовку сторінки
                var pageTitle = doc.DocumentNode.SelectSingleNode("//*[@id=\"img\"]").GetAttributeValue("src", "");
                return pageTitle;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                driver.Quit(); // Закрити драйвер після виконання всіх операцій
            }
        }
        
    }
}
