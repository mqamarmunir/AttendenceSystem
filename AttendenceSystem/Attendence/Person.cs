using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attendence
{
    public class Person
    {
        public int UserId { get; set; }
        public string Login { get; set; }
        public string Org_BranchId { get; set;}
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Designation { get; set; }
        public string FingerPrint { get; set;}

        public byte[] Picture { get; set; }

    }
}