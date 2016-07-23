using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBCNewsFeeds
{
    class News
    {
        public string title { get; set; }
        public string link{ get; set; }
        public string description{ get; set; }        
        public IList<NewsItem> items{ get; set; }

        public News()
        {
            items = new List<NewsItem>();
        }
        
    }
}
