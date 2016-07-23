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
            int hoursRun = 0;
            bool folderCreated = false;
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
                    //get feeds
                    News bbcNewsFeed = ProcessFeedHelper.GetFeeds(bbcURI);

                    // if this is the first run go ahead and create a json file.
                    if (hoursRun == 0 && !folderCreated)
                    {
                        ProcessFeedHelper.CreateFolder(feedsFolderPath);

                        ProcessFeedHelper.CreateJsonFile(bbcNewsFeed, feedsFolderPath);
                        folderCreated = true;
                    }
                    else
                    {
                        //if it is the second time then we need to check for duplicates.
                        ProcessFeedHelper.RemoveDuplicatesFeeds(bbcNewsFeed, feedsFolderPath);
                        ProcessFeedHelper.CreateJsonFile(bbcNewsFeed, feedsFolderPath);
                    }

                    // if it is the 23rd hour then we need to reset the counter and detele all files in folder.
                    if (hoursRun == 23)
                    {
                        hoursRun = 0;

                        ProcessFeedHelper.DeleteFilesInDirectory(feedsFolderPath);
                    }
                        //others increment the hoursrun.
                    else
                        hoursRun++;

                }
            }
        }
    }
}