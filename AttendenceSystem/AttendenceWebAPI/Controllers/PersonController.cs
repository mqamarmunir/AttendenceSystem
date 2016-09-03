using AttendenceWebAPI.Models;
using BusinessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AttendenceWebAPI.Controllers
{
    public class PersonController : ApiController
    {
        // GET: api/Person
        public IEnumerable<Person> Get()
        {
            List<Person> lstPerson = new List<Person>();
            Personnel objPersonnel = new Personnel();
            DataView dv = objPersonnel.GetAll(1);
            if (dv.Count > 0)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    Person objPerson = new Person
                    {
                        FirstName = dv[i]["FirstName"].ToString(),
                        LastName = dv[i]["LastName"].ToString(),
                        Designation = dv[i]["Designation"].ToString(),
                        FullName = dv[i]["FullName"].ToString(),
                        Login = dv[i]["Login"].ToString(),
                        Org_BranchId = dv[i]["BranchID"].ToString(),
                        UserId = Convert.ToInt32(dv[i]["UserID"].ToString())
                    };
                    lstPerson.Add(objPerson);
                }
            }
            return lstPerson;

        }

        // GET: api/Person/5
        public List<Person> Get(int id)
        {
            List<Person> lstPerson = new List<Person>();
            Personnel objPersonnel = new Personnel();
            DataView dv = objPersonnel.GetAll(1);
            if (dv.Count > 0)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    Person objPerson = new Person
                    {
                        FirstName = dv[i]["FirstName"].ToString(),
                        LastName = dv[i]["LastName"].ToString(),
                        Designation = dv[i]["Designation"].ToString(),
                        FullName = dv[i]["FullName"].ToString(),
                        Login = dv[i]["Login"].ToString(),
                        Org_BranchId = dv[i]["BranchID"].ToString(),
                        UserId = Convert.ToInt32(dv[i]["UserID"].ToString())
                    };
                    lstPerson.Add(objPerson);
                }
            }
            return lstPerson;
        }

        // POST: api/Person
        public void Post(personFingerPrint personfingerprint)
        {
            var x = personfingerprint;
            int orgid = personfingerprint.Orgid;
            int branchid = personfingerprint.BranchId;
            int id = personfingerprint.PersonID;
            string data = personfingerprint.FingerPrintDataXML;
            System.IO.File.WriteAllText(@"D:\Writeme.txt", data);
        }

        // PUT: api/Person/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Person/5
        public void Delete(int id)
        {
        }
    }
}
