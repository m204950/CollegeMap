using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;

namespace CollegeMap.Models.CollegeMapModels
{
    public class Csv_CollegeImport
    {
        /*
        public string UNITID { get; set; }
        public string OPEID { get; set; }
        public string OPEID6 { get; set; }
        public string INSTNM { get; set; }
        public string CITY { get; set; }
        public string STABBR { get; set; }
        public string ZIP { get; set; }
        public string INSTURL { get; set; }
        public string NPCURL { get; set; }
        public string HIGHDEG { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
        public string ADM_RATE  { get; set; }
        public string ACTCMMID { get; set; }
        public string CONTROL { get; set; }
        public string UGDS { get; set; }
        public string TUITIONFEE_IN { get; set; }
        public string TUITIONFEE_OUT { get; set; }
        public string NetPrice { get; set; }
        */

        public int UNITID { get; set; }
        public int OPEID { get; set; }
        public int OPEID6 { get; set; }
        public string INSTNM { get; set; }
        public string CITY { get; set; }
        public string STABBR { get; set; }
        public string ZIP { get; set; }
        public string INSTURL { get; set; }
        public int HIGHDEG { get; set; }
        public float LATITUDE { get; set; }
        public float LONGITUDE { get; set; }
        public float ADM_RATE { get; set; }
        public int ACTCMMID { get; set; }
        public int CONTROL { get; set; }
        public int UGDS { get; set; }
        public int TUITIONFEE_IN { get; set; }
        public int TUITIONFEE_OUT { get; set; }
        public int NetPrice { get; set; }
    }

}
