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
        public string LATITUDE { get; set; }   // some entries NULL.  Need to convert to float
        public string LONGITUDE { get; set; }  // some entries NULL.  Need to convert to float
        public string ADM_RATE { get; set; }   // some entries NULL.  Need to convert to float
        public string ACTCMMID { get; set; }   // some entries NULL.  Need to convert to int
        public int CONTROL { get; set; }
        // undergraduate enrollment
        public string UGDS { get; set; }   // some entries NULL.  Need to convert to int
        // percentage of students that are white
        public string UGDS_WHITE { get; set; }   // some entries NULL.  Need to convert to float
        // in state tuition
        public string TUITIONFEE_IN { get; set; }  // some entries NULL.  Need to convert to int
        // out of state tuition
        public string TUITIONFEE_OUT { get; set; }  // some entries NULL.  Need to convert to int
        // average tuition paid
        public string TUITFTE { get; set; }  // some entries NULL.  Need to convert to int
        // completion rate
        public string C150_4_POOLED_SUPP { get; set; }  // some entries NULL.  Need to convert to float
    }

}
