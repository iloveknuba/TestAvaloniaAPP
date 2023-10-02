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
                if(doc.DocumentNode.SelectSingleNode($"//*[@id=\"Web.TemplatesInterface.v1_0.Touch.DetailTemplateInterface.DetailTemplate_1\"]/music-container/div/div/div[2]/div/div/music-image-row[1]/div/div[3]/music-link/span").InnerHtml != null)
                list.Add(new Song
                {
                    ArtistName = doc.DocumentNode.SelectSingleNode($"//*[@id=\"Web.TemplatesInterface.v1_0.Touch.DetailTemplateInterface.DetailTemplate_1\"]/music-container/div/div/div[2]/div/div/music-image-row[{i}]/div/div[2]/div/music-link[1]/a").InnerHtml,
                    AlbumName = doc.DocumentNode.SelectSingleNode($"//*[@id=\"Web.TemplatesInterface.v1_0.Touch.DetailTemplateInterface.DetailTemplate_1\"]/music-container/div/div/div[2]/div/div/music-image-row[{i}]/div/div[2]/div/music-link[2]/a").InnerHtml,
                    Duration = doc.DocumentNode.SelectSingleNode($"//*[@id=\"Web.TemplatesInterface.v1_0.Touch.DetailTemplateInterface.DetailTemplate_1\"]/music-container/div/div/div[2]/div/div/music-image-row[{i}]/div/div[3]/music-link/span").InnerHtml,
                    SongName = doc.DocumentNode.SelectSingleNode($"//*[@id=\"Web.TemplatesInterface.v1_0.Touch.DetailTemplateInterface.DetailTemplate_1\"]/music-container/div/div/div[2]/div/div/music-image-row[{i}]/div/div[1]/music-link/a").InnerHtml
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

                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"Web.TemplatesInterface.v1_0.Touch.DetailTemplateInterface.DetailTemplate_1\"]/music-container/div/div/div[2]/div/div/music-image-row[1]/div/div[1]/music-link/a")));

                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
                var jsResult = jsExecutor.ExecuteScript("return document.documentElement.outerHTML");

           

                // Отримуємо HTML-код сторінки з Selenium
                string pageSource = jsResult.ToString();

                // Ініціалізуємо парсер HAP та завантажуємо в нього HTML
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(pageSource);

                var playlistXPath = doc.DocumentNode.SelectSingleNode("//*[@id=\"atf\"]/music-detail-header");
                var songsXPath = doc.DocumentNode.SelectNodes("//*[@id=\"Web.TemplatesInterface.v1_0.Touch.DetailTemplateInterface.DetailTemplate_1\"]/music-container/div/div/div[2]/div/div/music-image-row");


                // Приклад парсингу заголовку сторінки


                Playlist playlist = new Playlist
                {
                    Name = playlistXPath?.GetAttributeValue("headline","") ?? "Wrong Url",
                    Avatar = playlistXPath?.GetAttributeValue("image-src", "") ?? "",
                    Description = playlistXPath?.GetAttributeValue("secondary-text","")?? "",
                    Songs = songsXPath.Select(p=> new Song
                    {
                        ArtistName = p.GetAttributeValue("secondary-text-1",""),
                        SongName = p.GetAttributeValue("primary-text",""),
                        Duration = "3:33",
                        AlbumName = p.GetAttributeValue("secondary-text-2","")
                    })
                   
                   //Songs = GetSongList(doc,pageSource)
                };

                return playlist !=null ? playlist : new Playlist
                {
                    Name = "Wrong url"
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
