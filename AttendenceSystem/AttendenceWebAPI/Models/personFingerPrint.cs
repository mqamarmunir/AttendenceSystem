using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendenceWebAPI.Models
{
    public class personFingerPrint
    {
        public int Orgid { get; set; }
        public int BranchId { get; set; }
        public  int PersonID { get; set; }
        public  string FingerPrintDataXML { set; get; }
    }
}