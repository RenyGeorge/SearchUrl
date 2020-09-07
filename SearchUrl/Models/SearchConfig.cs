using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchUrl.Models
{
    public class SearchConfig
    {
        public string name
        {
            get;
            set;
        }
        public string url
        {
            get;
            set;
        }

        public string queryParam
        {
            get;
            set;
        }
        public string pageCountParam
        {
            get;
            set;
        }
    }
}
