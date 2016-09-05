using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DPUruNet;
using System.Web.Script.Serialization;

namespace Attendence.UserControls
{
    public partial class Attendence : UserControl
    {

        public MainForm _sender;
        private const int DPFJ_PROBABILITY_ONE = 0x7fffffff;
        private DPCtlUruNet.IdentificationControl identificationControl;
        Dictionary<int, Fmd> objfmd = new Dictionary<int, Fmd>();
        public Attendence()
        {
            InitializeComponent();
        }
        public void Attendence_Load(object sender, EventArgs e)
        {
            InitializeDevice();
        }
        public void InitializeDevice()
        {
            if (identificationControl != null)
            {
                identificationControl.Reader = _sender.CurrentReader;
            }
            else
            {
                ////////////


                //string s = System.IO.File.ReadAllText(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/personfingerprint.txt");
                try
                {
                    List<Person> objpersonlistwithfingerprints = StaticData.Persondatalist.Where(c => c.FingerPrint != null && c.FingerPrint.Trim() != "").ToList();
                    for (int i = 0; i < objpersonlistwithfingerprints.Count(); i++)
                    {
                        int personid = objpersonlistwithfingerprints[i].UserId;
                        string fingerprintdata = objpersonlistwithfingerprints[i].FingerPrint;
                        try
                        {
                            Fmd dmd = Fmd.DeserializeXml(fingerprintdata);
                            objfmd.Add(personid, dmd);
                        }
                        catch { }
                    }
                    //JavaScriptSerializer JSserializer = new JavaScriptSerializer();
                    //  object x = JSserializer.Deserialize<object>(s);
                    //Fmd[] fmd = new Fmd[1];


                    //objfmd.Add(dmd);


                    // objfmd.Add(x);
                }
                catch (Exception ee)
                {
                    throw;
                }
                ////////



                // See the SDK documentation for an explanation on threshold scores.
                int thresholdScore = DPFJ_PROBABILITY_ONE * 1 / 100000;

                identificationControl = new DPCtlUruNet.IdentificationControl(_sender.CurrentReader, objfmd.Values, thresholdScore, 1, Constants.CapturePriority.DP_PRIORITY_EXCLUSIVE);
                identificationControl.Location = new System.Drawing.Point(3, 3);
                identificationControl.Name = "identificationControl";
                identificationControl.Size = new System.Drawing.Size(397, 128);
                identificationControl.TabIndex = 0;
                identificationControl.OnIdentify += new DPCtlUruNet.IdentificationControl.FinishIdentification(this.identificationControl_OnIdentify);

                // Be sure to set the maximum number of matches you want returned.
                identificationControl.MaximumResult = 1;

                this.Controls.Add(identificationControl);
            }

            identificationControl.StartIdentification();
        }
        private void identificationControl_OnIdentify(DPCtlUruNet.IdentificationControl IdentificationControl, IdentifyResult IdentificationResult)
        {
            if (IdentificationResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
            {
                if (IdentificationResult.Indexes == null)
                {
                    if (IdentificationResult.ResultCode == Constants.ResultCode.DP_INVALID_PARAMETER)
                    {
                        MessageBox.Show("Warning: Fake finger was detected.");
                    }
                    else if (IdentificationResult.ResultCode == Constants.ResultCode.DP_NO_DATA)
                    {
                        MessageBox.Show("Warning: No finger was detected.");
                    }
                    else
                    {
                        if (_sender.CurrentReader != null)
                        {
                            _sender.CurrentReader.Dispose();
                            _sender.CurrentReader = null;
                        }
                    }
                }
                else
                {
                    if (_sender.CurrentReader != null)
                    {
                        _sender.CurrentReader.Dispose();
                        _sender.CurrentReader = null;
                    }

                    MessageBox.Show("Error:  " + IdentificationResult.ResultCode);
                }
            }
            else
            {
                //_sender.CurrentReader = IdentificationControl.Reader;
                if (IdentificationResult.Indexes.Length > 0)
                {
                    int matchedindex = IdentificationResult.Indexes[0][0];
                    int personid = objfmd.Keys.ElementAt(matchedindex);

                    var personfound = StaticData.Persondatalist.FirstOrDefault(c => c.UserId == personid);

                    if(!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + " /PersonsData/PersonnelAttendence"))
                        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + " /PersonsData/PersonnelAttendence");
                    var objpersonnelattendence = new personnelAttendence();
                    objpersonnelattendence.Emp_Id = personid;
                    objpersonnelattendence.branch_Id = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["BranchId"].ToString().Trim());
                    objpersonnelattendence.EnteredOn = System.DateTime.Now;

                    JavaScriptSerializer objserializer = new JavaScriptSerializer();

                    var serializedpersonnelattendence = objserializer.Serialize(objpersonnelattendence);

                    
                    System.IO.File.WriteAllText(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + " /PersonsData/PersonnelAttendence/" + System.DateTime.Now.ToString("ddMMyyyy") + "(" + personid.ToString() + ").txt", serializedpersonnelattendence);
                    //MessageBox.Show(personfound.FirstName+ " "+personfound.LastName + " checked.");
                    txtMessage.Text = txtMessage.Text + personfound.FirstName + " " + personfound.LastName + " checked.";
                }

                else
                {
                    txtMessage.Text = txtMessage.Text + "OnIdentify:  " + (IdentificationResult.Indexes.Length.Equals(0) ? "No " : "One or more ") + "matches.  Try another finger.\r\n\r\n";
                }
            
            }

            txtMessage.SelectionStart = txtMessage.TextLength;
            txtMessage.ScrollToCaret();
        }
    }
}
