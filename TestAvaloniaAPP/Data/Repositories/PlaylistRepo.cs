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
       

        List<Song> GetSongList(HtmlDocument doc, string html)
        {
          
           
            var list = new List<Song>();
            

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


        
    }
}
