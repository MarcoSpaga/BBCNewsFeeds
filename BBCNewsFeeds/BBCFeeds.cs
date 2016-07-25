using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BBCNewsFeeds
{
    class BBCFeeds
    {
      
        public static void Main(string[] args)
        {
            // get the starting time of app.
            DateTime startingTime = DateTime.Now;
            int minute = 1;
            // To ensure that app runs on the next hour.
            if (startingTime.Minute >= minute)
            {
                startingTime = startingTime.AddHours(1);
                startingTime = startingTime.AddMinutes(-startingTime.Minute);
            }
            int hoursRun = 0;
            bool folderCreated = false;
            DateTime lastRun = DateTime.MinValue; 
            //this will be the folder path for feeds.
            string feedsFolderPath =  Environment.GetFolderPath(
                       System.Environment.SpecialFolder.Desktop) + "\\feeds";

            // uri for feeds.
            string bbcURI = "http://feeds.bbci.co.uk/news/uk/rss.xml";


            while (true)
            {
                // check the hour and if it is more than 1 minutes past the hour wait for the next hour.
                if (DateTime.Now.Hour == startingTime.AddHours(hoursRun).Hour && DateTime.Now.Minute < minute)
                {
                    Console.WriteLine("Searching for feeds...");
                    //get feeds
                    News bbcNewsFeed = ProcessFeedHelper.GetFeeds(bbcURI);

                    // if this is the first run go ahead and create a json file.
                    if (!folderCreated)
                    {
                        Console.WriteLine("The feed files will be stored in a folder called feeds on your desktop");
                        ProcessFeedHelper.CreateFolder(feedsFolderPath);
                        folderCreated = true;
                    }
                    else
                    {
                        //if it is the second time then we need to check for duplicates.
                        bbcNewsFeed = ProcessFeedHelper.RemoveDuplicatesFeeds(bbcNewsFeed, feedsFolderPath);
                       // ProcessFeedHelper.CreateJsonFile(bbcNewsFeed, feedsFolderPath);
                    }
                    
                    string filePath = feedsFolderPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd-HH") + ".json";

                    //serialises objects in news Object and appends a file.
                    string jsonFile = Newtonsoft.Json.JsonConvert.SerializeObject(bbcNewsFeed, Newtonsoft.Json.Formatting.Indented);

                    File.AppendAllText(@filePath, jsonFile);

                    // if it is the 23 run then we need to reset the counter and detele all files in folder.
                    if (hoursRun == 23)
                    {
                        hoursRun = 0;

                        ProcessFeedHelper.DeleteFilesInDirectory(feedsFolderPath);

                        Console.WriteLine("Applcation has run for 24 hours. All files in feed folder will be deleted.");
                    }
                    else
                    {
                        //others increment the hoursrun.
                        hoursRun++;
                        Console.WriteLine("The application has run for "+hoursRun+" hour(s).");
                    }
                    
                       System.Threading.Thread.Sleep(TimeSpan.FromMinutes(50));
                }
            }
        }
    }
}