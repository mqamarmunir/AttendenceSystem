using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataLayer;

namespace BusinessLayer
{
    public class PersonnelAttendence
    {
        #region Class Variables

        private const string Default = "~!@";
        private const string TableName = "hr_roster_attendance_machine";
        public string StrErrorMessage { get; set; }

        public int EmployeeId { get; set; }
        public string date_time { get; set; }

        public int BranchID { get; set; }

        clsoperation objTrans = new clsoperation();


        private clsdbhims objdbhims = new clsdbhims();
    


        #endregion
        public DataView GetAll(int flag)
        {
            DataView dv = null;
            switch (flag)
            {
                case 1:
                    objdbhims.Query = @"select r.clinic_branch_id Branchid
                                        , r.personnelid UserId
                                        ,r.Firstname
                                        ,ifnull(r.lastname,'') lastname
                                        ,r.cellno
                                        ,fp.thumb_code
                                         from rm_pr_personnel r
                                         left outer join hr_roster_emp_finger_print fp on fp.emp_id=r.personnelid
                                        where lower(active)='y'";
                    break;
            }
            return objTrans.DataTrigger_Get_All(objdbhims);
        } 
        public bool Insert()
        {
           
                try
                {
                    //clsoperation objTrans = new clsoperation();
                    QueryBuilder objQB = new QueryBuilder();
                    objTrans.Start_Transaction();

                   
                   
                        objdbhims.Query = objQB.QBInsert(MakeArray(), TableName);
                        this.StrErrorMessage = objTrans.DataTrigger_Insert(objdbhims);

                        objTrans.End_Transaction();

                        if (this.StrErrorMessage.Equals("True"))
                        {
                            this.StrErrorMessage = objTrans.OperationError;
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    
                  
                }
                catch (Exception e)
                {
                    this.StrErrorMessage = e.Message;
                    return false;
                }
            
           
        }
        private string[,] MakeArray()
        {
            string[,] arySAPS = new string[3, 3];

            

            if (!this.EmployeeId.Equals(Default))
            {
                arySAPS[0, 0] = "employee_id";
                arySAPS[0, 1] = this.EmployeeId.ToString();
                arySAPS[0, 2] = "int";
            }




            if (!this.EmployeeId.Equals(Default))

            {
                arySAPS[1, 0] = "date_time";
                arySAPS[1, 1] = this.date_time;
                arySAPS[1, 2] = "datetime";
            }

            if (!this.BranchID.Equals(Default))
            {
                arySAPS[2, 0] = "Branch_ID";
                arySAPS[2, 1] = this.BranchID.ToString().Trim();
                arySAPS[2, 2] = "int";
            }

            return arySAPS;
        }
    }

}
