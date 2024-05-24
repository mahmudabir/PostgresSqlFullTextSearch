using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresSqlFullTextSearch.Models
{
    public class AppConfiguration
    {
        public int SeedDataCount { get; set; }
        public int SeedDataChunkCount { get; set; }
        public bool DurationUnitInSecond { get; set; }
    }

}
