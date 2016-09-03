using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BusinessLayer
{
    public class Personnel
    {
        #region Class Variables

        private const string Default = "~!@";
        private const string TableName = "dc_tu_accessgroup";
        private string StrErrorMessage = "";



        #endregion
        public DataView GetAll(int flag)
        {
            DataView dv = null;
            switch (flag)
            {
                case 1:
                    DataTable dt = new DataTable();
                    dt.Columns.Add("FirstName");
                    dt.Columns.Add("LastName");
                    dt.Columns.Add("FullName");
                    dt.Columns.Add("Designation");
                    dt.Columns.Add("UserId");
                    dt.Columns.Add("Login");
                    dt.Columns.Add("BranchId");

                    dt.AcceptChanges();

                    DataRow dr = dt.NewRow();
                    dr["FirstName"] = "Muhammad";
                    dr["LastName"] = "Qamar";

                    dr["FullName"] = "Muhammad Qamar";
                    dr["Designation"] = "CEO";
                    dr["UserId"] = "1";
                    dr["Login"] = "mqamarmunir";
                    dr["BranchId"] = "1";
                    dt.Rows.Add(dr);

                    dr = dt.NewRow();
                    dr["FirstName"] = "Asif";
                    dr["LastName"] = "Rauf";

                    dr["FullName"] = "Asif Rauf";
                    dr["Designation"] = "Team Lead";
                    dr["UserId"] = "2";
                    dr["Login"] = "asifrauf";
                    dr["BranchId"] = "1";
                    dt.Rows.Add(dr);

                    dr = dt.NewRow();
                    dr["FirstName"] = "Ali";
                    dr["LastName"] = "Rehman";

                    dr["FullName"] = "Ali Rehman";
                    dr["Designation"] = "SE";
                    dr["UserId"] = "3";
                    dr["Login"] = "alirehman";
                    dr["BranchId"] = "1";
                    dt.Rows.Add(dr);

                    dt.AcceptChanges();
                    dv = new DataView(dt);
                    break;
            }
            return dv;
        }
    }

}
