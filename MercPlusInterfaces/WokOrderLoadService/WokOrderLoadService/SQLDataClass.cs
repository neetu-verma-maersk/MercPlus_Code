using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WokOrderLoadService
{
    public class SQLDataClass
    {
        public int EDI_ID { get; set; }
        public DateTime CRTS { get; set; }
        public int LineNo { get; set; }
        public string LineDetail { get; set; }
    }
}
