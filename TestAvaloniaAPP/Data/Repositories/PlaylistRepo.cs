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
using OpenQA.Selenium.Interactions;
using System.Threading;
using System.Xml.Linq;
using DynamicData;
using Avalonia.Controls;

namespace TestAvaloniaAPP.Data.Repositories
{
    public class PlaylistRepo : IPlaylist
    {
        HashSet<HtmlNode> GetSongsFromDoc(HtmlDocument doc)
        {
            var allsongs = doc.DocumentNode.SelectNodes("//*[@id=\"Web.TemplatesInterface.v1_0.Touch.DetailTemplateInterface.DetailTemplate_1\"]/music-container/div/div/div[2]/div/div/music-image-row");

            var uniqueSongs = new HashSet<HtmlNode>(new NodeComparer());
            foreach (var song in allsongs)
            {
                if (!uniqueSongs.Contains(song))
                {
                    uniqueSongs.Add(song);

                }
            }
            return uniqueSongs;

        }
            
        public Playlist GetWebPlaylist(string url)
        {
            //chromedriver settings
            string projectFolder = AppDomain.CurrentDomain.BaseDirectory;
            string driverPath = Path.Combine(projectFolder, "Chromedriver");
            IWebDriver driver = new ChromeDriver(driverPath);


            Playlist playlist  = new Playlist();
           
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

                var playlistNode = doc.DocumentNode.SelectSingleNode("//*[@id=\"atf\"]/music-detail-header");
           
   
                int index = 1;

                while (true)
                {
                 
                    jsResult = (string)jsExecutor.ExecuteScript("return document.documentElement.outerHTML");

                    pageSource = jsResult.ToString();

                    var fragmentDoc = new HtmlDocument();

                    fragmentDoc.LoadHtml(pageSource);

                    doc.DocumentNode.AppendChild(fragmentDoc.DocumentNode);

                    jsExecutor.ExecuteScript("window.scrollBy(0, 1000);");
                    if (doc.DocumentNode.SelectSingleNode($"//*[@id=\"Web.TemplatesInterface.v1_0.Touch.DetailTemplateInterface.DetailTemplate_1\"]/music-container/div/div/div[2]/div/div/music-image-row[{index}]") == null)
                    {
                        break;
                    }

                    index++;
                    Thread.Sleep(100);
                  
                };

                var uniqueSongs = GetSongsFromDoc(doc);


  
         
            return new Playlist()
            {
                Name = playlistNode?.GetAttributeValue("headline", "") ?? "",
                Avatar = playlistNode?.GetAttributeValue("image-src", "") ?? "",
                Description = playlistNode?.GetAttributeValue("secondary-text", "") ?? "",
                Songs = uniqueSongs.ToList().Select(p => new Song
                {
                    ArtistName = p.GetAttributeValue("secondary-text-1", ""),
                    SongName = p.GetAttributeValue("primary-text", ""),
                    Duration = "3:33",
                    AlbumName = p.GetAttributeValue("secondary-text-2", "")
                })

            };
                
          
            }
            catch (NoSuchWindowException ex)
            {
               return new Playlist { Name = ex.Message };
                driver.Quit();

            }
            catch (Exception ex)
            {
                return new Playlist { Name = ex.Message};
                driver.Quit();
            }
           
            finally
            {
                driver.Quit(); // Закрити драйвер після виконання всіх операцій
            }
        }


        
    }
}
