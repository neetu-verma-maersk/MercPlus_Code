using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercPurgeTool
{
    public class PurgeModel
    {
        public string FactPath { get; set; }
        public string EdiPath { get; set; }
        public string Facttoken { get; set; }
        public string Editoken { get; set; }
        public string Factdate { get; set; }
        public string Edidate { get; set; }
        public string Path { get; set; }

        public PurgeModel Empty()
        {
            PurgeModel pM = new PurgeModel();
            pM.FactPath = "";
            pM.EdiPath = "";
            pM.Facttoken = "";
            pM.Editoken = "";
            pM.Factdate = "";
            pM.Edidate = "";
            pM.Path = "";

            return pM;
        }
    }
}
