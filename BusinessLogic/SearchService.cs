using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
public    class SearchService
    {
        public DataSet searchResult{ get; set; }
        public List<string> parameters { get; set; }

        public string keyword { get; set; }
    }
}
