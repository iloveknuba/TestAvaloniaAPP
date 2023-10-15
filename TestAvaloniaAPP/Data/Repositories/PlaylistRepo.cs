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
        HashSet<Song> GetSongsFromDoc(List<Song> allsongs)
        {
            var uniqueSongs = new HashSet<Song>(new NodeComparer());
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
            string playlistType;


            Playlist playlist  = new Playlist();
           
            try
            {
                driver.Navigate().GoToUrl(url);

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"atf\"]/music-detail-header")));



                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
                var jsResult = jsExecutor.ExecuteScript("return document.documentElement.outerHTML");

           

                // Отримуємо HTML-код сторінки з Selenium
                string pageSource = jsResult.ToString();

                // Ініціалізуємо парсер HAP та завантажуємо в нього HTML
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(pageSource);

                var playlistNode = doc.DocumentNode.SelectSingleNode("//*[@id=\"atf\"]/music-detail-header");



                var srs = "//*[@id=\"Web.TemplatesInterface.v1_0.Touch.DetailTemplateInterface.DetailTemplate_1\"]/music-container/div/div/div[2]/div/div/music-image-row[1]";
                playlistType = doc.DocumentNode.SelectSingleNode(srs) == null ? "text" : "image";


                var allSongs = new List<Song>();
                #region Getting songs by cycle



                int index = 1;
                int countGap = 10;
                while (true)
                {
                 
                    jsResult = (string)jsExecutor.ExecuteScript("return document.documentElement.outerHTML");

                    pageSource = jsResult.ToString();

                    var fragmentDoc = new HtmlDocument();

                    fragmentDoc.LoadHtml(pageSource);

                    doc.DocumentNode.AppendChild(fragmentDoc.DocumentNode);


                    var allsongs = doc.DocumentNode.SelectNodes("//*[@id=\"Web.TemplatesInterface.v1_0.Touch.DetailTemplateInterface.DetailTemplate_1\"]/music-container/div/div/div[2]/div/div/music-image-row");


                    jsExecutor.ExecuteScript("window.scrollBy(0, 1100);");
                    if (fragmentDoc.DocumentNode.SelectSingleNode($"//*[@id=\"Web.TemplatesInterface.v1_0.Touch.DetailTemplateInterface.DetailTemplate_1\"]/music-container/div/div/div[2]/div/div/music-image-row[{index}]") == null &&
                        fragmentDoc.DocumentNode.SelectSingleNode($"//*[@id=\"Web.TemplatesInterface.v1_0.Touch.DetailTemplateInterface.DetailTemplate_1\"]/music-container/div/div/div[2]/div/div/music-text-row[{index}]") == null)
                    {
                        break;
                    }

                   
                    for (int i=1;i<countGap;i++) { 

                        allSongs.Add(new Song
                        {
                            AlbumName = fragmentDoc.DocumentNode.SelectSingleNode($"/html/body/div/div[3]/div/music-app/div/div/div/div/music-container/music-container/div/div/div[2]/div/div/music-{playlistType}-row[{i}]/div/div[3]/music-link")?.GetAttributeValue("title", ""),
                            ArtistName = fragmentDoc.DocumentNode.SelectSingleNode($"/html/body/div/div[3]/div/music-app/div/div/div/div/music-container/music-container/div/div/div[2]/div/div/music-{playlistType}-row[{i}]/div/div[2]/music-link")?.GetAttributeValue("title", ""),
                            SongName = fragmentDoc.DocumentNode.SelectSingleNode($"/html/body/div/div[3]/div/music-app/div/div/div/div/music-container/music-container/div/div/div[2]/div/div/music-{playlistType}-row[{i}]/div/div[1]/music-link")?.GetAttributeValue("title", ""),
                            Duration = fragmentDoc.DocumentNode.SelectSingleNode($"/html/body/div/div[3]/div/music-app/div/div/div/div/music-container/music-container/div/div/div[2]/div/div/music-{playlistType}-row[{i}]/div/div[4]/music-link")?.GetAttributeValue("title", ""),
                        });
                       
                    }
                    countGap += 10;
                    index++;
                    Thread.Sleep(100);
                  
                };

                #endregion

                var uniqueSongs = GetSongsFromDoc(allSongs);

          
             
  
         
            return new Playlist()
            {
                Name = playlistNode?.GetAttributeValue("headline", "") ?? "",
                Avatar = playlistNode?.GetAttributeValue("image-src", "") ?? "",
                Description = playlistNode?.GetAttributeValue("primary-text", "") ?? "",
                Songs = uniqueSongs.ToList()
         
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
