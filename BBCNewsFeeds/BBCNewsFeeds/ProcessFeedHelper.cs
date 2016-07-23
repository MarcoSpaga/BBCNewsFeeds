using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Syndication;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.IO;
using Newtonsoft.Json;

namespace BBCNewsFeeds
{
    class ProcessFeedHelper
    {
        /// <summary>
        /// A Method that gets RSSFeeds returns a News object
        /// </summary>
        /// <param name="aURI"></param>
        /// <returns>News</returns>
        public static News GetFeeds(String aURI)
        {
            News newsFeed;

            try
            {
                //instantiate xmlreader and point to uri
                using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(aURI))
                {
                    //load  the feed into SyndicationFeed Object
                    SyndicationFeed feed = SyndicationFeed.Load(reader);

                    newsFeed = new News();

                    foreach (var item in feed.Items)
                    {
                        // BBC Feed parent element titles change throughout the day but I have not managed to get them all.
                        // Could potentially break however, the logic is correct.
                        // Here we create the parent element object.
                        if (item.Title.Text == "BBC News Channel" || item.Title.Text == "BBC News at Ten")
                        {
                            newsFeed.title = item.Title.Text;
                            newsFeed.link = item.Id;
                            newsFeed.description = item.Summary.Text;
                        }
                        // Create the child elements.
                        else
                        {
                            NewsItem newsItem = new NewsItem();
                            newsItem.title = item.Title.Text;
                            newsItem.link = item.Id;
                            newsItem.description = item.Summary.Text;
                            newsItem.publishDate = FormatDate(item.PublishDate.ToString());

                            //Add it to parent object.
                            newsFeed.items.Add(newsItem);
                        }
                    }
                    //close reader once we have finished reading feed and return feed object.
                    reader.Close();
                    return newsFeed;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected Error" + ex.Message);
            }


            return null;
        }
        /// <summary>
        /// Creates a folder at a specified path.
        /// </summary>
        /// <param name="aPath"></param>
        public static void CreateFolder(string aPath)
        {
            System.IO.Directory.CreateDirectory(aPath);
        }
        /// <summary>
        /// Creates a Json formatted file based on a news object passed through.
        /// </summary>
        /// <param name="aNews"></param>
        /// <param name="aPath"></param>
        public static void CreateJsonFile(News aNews, string aPath)
        {
            try
            {
                string filePath = aPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd-HH") + ".json";
                //serialises objects in news Object and appends a file.
                string jsonFile = JsonConvert.SerializeObject(aNews, formatting: Newtonsoft.Json.Formatting.Indented);
                File.AppendAllText(@filePath, jsonFile);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected Error");
            }
        }

        /// <summary>
        /// Removes Duplicate news articles in new feeds if they are already stored in files.
        /// </summary>
        /// <param name="aNews"></param>
        /// <param name="aPath"></param>
        public static void RemoveDuplicatesFeeds(News aNews, string aPath)
        {
            try
            {
                //get paths to all files.
                string[] filesInDirectory = Directory.GetFiles(aPath);

                List<News> newsInFiles = new List<News>();
                News newsInFile;

                // loop through files in directory.
                foreach (string aFile in filesInDirectory)
                {
                    //Read files file and deserialise the news object putting it in a news collection.
                    StreamReader reader = new StreamReader(aFile);
                    string fileContent = reader.ReadToEnd();
                    newsInFile = Newtonsoft.Json.JsonConvert.DeserializeObject<News>(fileContent);

                    newsInFiles.Add(newsInFile);
                    reader.Close();
                }
                //only go in here if there is the recent feed has news items.
                if (aNews.items.Count > 0)
                {
                    foreach (News aNewsInFile in newsInFiles)
                    {
                        // put news list into new news list so the next loop doesn't crash.
                        List<NewsItem> tempNewsList = new List<NewsItem>(aNews.items);
                        foreach (NewsItem aNewsItemFromCurrentFeed in tempNewsList)
                        {
                            //check that the current news item is not already in files saved.
                            var newsItemAlreadyExists = from nItems in aNewsInFile.items
                                                        where nItems.title == aNewsItemFromCurrentFeed.title
                                                        where nItems.publishDate == aNewsItemFromCurrentFeed.publishDate
                                                        where nItems.link == aNewsItemFromCurrentFeed.link
                                                        where nItems.description == aNewsItemFromCurrentFeed.description
                                                        select nItems;
                            // if item already stored in file then we must remove it as we don't want it.
                            if (newsItemAlreadyExists.First() != null)
                            {
                                if (aNews.items.Contains(aNewsItemFromCurrentFeed))
                                {
                                    aNews.items.Remove(aNewsItemFromCurrentFeed);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected Error");
            }
        }
        /// <summary>
        /// Deletes all the files in a directory(path specified in parameter).
        /// </summary>
        /// <param name="directoryPath"></param>
        public static void DeleteFilesInDirectory(string directoryPath)
        {
            try
            {
                //create files collection and directory object.
                List<FileInfo> importFiles = new List<FileInfo>();
                DirectoryInfo tempDirectory = new DirectoryInfo(directoryPath);

                //get all files in directory.
                importFiles.AddRange(tempDirectory.GetFiles());

                //if the number of files in the directory are greater than zero then delete them.
                if (importFiles.Count > 0)
                {
                    for (int i = 0; i < importFiles.Count; i++)
                        importFiles[i].Delete();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected Error");
            }
        }
        /// <summary>
        /// Formats a string to ddd, mm yyyy hh:ss gmt
        /// </summary>
        /// <param name="aDate"></param>
        /// <returns></returns>
        private static String FormatDate(String aDate)
        {
            try
            {
                //split string 
                char[] delimiters = { ' ', ',', ':', '/' };
                string[] tokens = aDate.Split(delimiters);
                int year = int.Parse(tokens[2]);
                int month = int.Parse(tokens[1]);
                int day = int.Parse(tokens[0]);
                int hh = int.Parse(tokens[3]);
                int mm = int.Parse(tokens[4]);
                int ss = int.Parse(tokens[5]);

                //create date time object. and add gmt to end of string.
                DateTime date = new DateTime(year, month, day, hh, mm, ss);
                return date.ToUniversalTime().ToString("r");
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected Error");
            }
            return "";
        }
    }
}
