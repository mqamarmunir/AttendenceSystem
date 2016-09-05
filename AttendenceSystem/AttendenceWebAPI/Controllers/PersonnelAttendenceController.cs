using AttendenceWebAPI.Models;
using BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace AttendenceWebAPI.Controllers
{
    public class PersonnelAttendenceController : ApiController
    {
        // GET: api/PersonnelAttendence
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/PersonnelAttendence/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/PersonnelAttendence
        public HttpResponseMessage Post(personnelAttendence paobj)
        {
            int countinsert = 0;
            string ErrorMessages=String.Empty;
            HttpResponseMessage response;
            //JavaScriptSerializer objserializer = new JavaScriptSerializer();
            //var lstpersonnelattendencerecords=objserializer.Deserialize<List<personnelAttendence>>(attendencerecordsjson);
            // System.IO.File.WriteAllText(@"D:\Writeme.txt", data);
            PersonnelAttendence objAttendence = new PersonnelAttendence();
                
                
                    objAttendence.BranchID = paobj.branch_Id;
                    objAttendence.date_time = paobj.EnteredOn.ToString("dd/MM/yyyy hh:mm:ss tt");
                    objAttendence.EmployeeId = paobj.Emp_Id;

            if (!objAttendence.Insert())
            {
                response = Request.CreateResponse(HttpStatusCode.ExpectationFailed, objAttendence.StrErrorMessage);
            }
            else
                response = Request.CreateResponse(HttpStatusCode.Created);
            return response;

           
            //return new HttpResponseMessage(HttpStatusCode.Created);
        }

        // PUT: api/PersonnelAttendence/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/PersonnelAttendence/5
        public void Delete(int id)
        {
        }
    }
}
