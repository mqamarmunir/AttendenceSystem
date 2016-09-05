using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendenceWebAPI.Models
{
    public class personnelAttendence
    {
        public int Emp_Id { get; set; }
        public int branch_Id { get; set; }
        public DateTime EnteredOn { get; set; }
    }
}