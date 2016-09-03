using DPUruNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Attendence
{
    public partial class frmIdentification : Form
    {
        public MainForm _sender;
        private const int DPFJ_PROBABILITY_ONE = 0x7fffffff;
        private DPCtlUruNet.IdentificationControl identificationControl;
        public frmIdentification()
        {
            InitializeComponent();
        }

        private void frmIdentification_Load(object sender, EventArgs e)
        {
            if (identificationControl != null)
            {
                identificationControl.Reader = _sender.CurrentReader;
            }
            else
            {
                ////////////
                List<Fmd> objfmd = new List<Fmd>();
                
                string s = System.IO.File.ReadAllText(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/personfingerprint.txt");
                try
                {
                    //JavaScriptSerializer JSserializer = new JavaScriptSerializer();
                  //  object x = JSserializer.Deserialize<object>(s);
                    //Fmd[] fmd = new Fmd[1];
                    Fmd dmd=Fmd.DeserializeXml(s);

                    objfmd.Add(dmd);


                   // objfmd.Add(x);
                }
                catch(Exception ee) {
                    throw;
                }
                ////////



                // See the SDK documentation for an explanation on threshold scores.
                int thresholdScore = DPFJ_PROBABILITY_ONE * 1 / 100000;

                identificationControl = new DPCtlUruNet.IdentificationControl(_sender.CurrentReader, objfmd, thresholdScore, 10, Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);
                identificationControl.Location = new System.Drawing.Point(3, 3);
                identificationControl.Name = "identificationControl";
                identificationControl.Size = new System.Drawing.Size(397, 128);
                identificationControl.TabIndex = 0;
                identificationControl.OnIdentify += new DPCtlUruNet.IdentificationControl.FinishIdentification(this.identificationControl_OnIdentify);

                // Be sure to set the maximum number of matches you want returned.
                identificationControl.MaximumResult = 10;

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
                txtMessage.Text = txtMessage.Text + "OnIdentify:  " + (IdentificationResult.Indexes.Length.Equals(0) ? "No " : "One or more ") + "matches.  Try another finger.\r\n\r\n";
            }

            txtMessage.SelectionStart = txtMessage.TextLength;
            txtMessage.ScrollToCaret();
        }

        
        private void IdentificationControl_Closed(object sender, EventArgs e)
        {
            identificationControl.StopIdentification();
        }
    }
}
