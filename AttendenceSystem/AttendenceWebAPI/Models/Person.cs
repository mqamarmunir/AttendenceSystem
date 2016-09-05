using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendenceWebAPI.Models
{
    public class Person
    {
        public int UserId { get; set; }
       
        public string Org_BranchId { get; set;}
        public string FirstName { get; set; }
        public string LastName { get; set; }
       
        public string CellNo { get; set; }
        public string FingerPrint { get; set;}

        

    }
}