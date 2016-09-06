using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchServiceLibrary
{
    public class Result<T>
    {
        public long TotalCount { get; set; }    

        public T Data { get; set; }

        public TimeSpan Second { get; set; }

        public List<KeyValuePair<string, long?>> Category { get; set; }
    }
}
