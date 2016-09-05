using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DPUruNet;
using System.Web.Script.Serialization;
using System.Threading;

namespace Attendence
{
    public partial class frmThumbRegistration : Form
    {
        public MainForm _sender;

        private bool reset;

        List<Fmd> preenrollmentFmds;
        int count;

        private static Reader currentReader;
        private ReaderCollection _readers;
        public Reader CurrentReader
        {
            get { return currentReader; }
            set
            {
                currentReader = value;
                // SendMessage(Action.UpdateReaderState, value);
            }
        }
        public frmThumbRegistration()
        {
            InitializeComponent();
            _readers = ReaderCollection.GetReaders();
            currentReader = _readers[0];
        }


        private void frmThumbRegistration_Load(object sender, EventArgs e)
        {
            txtEnroll.Text = string.Empty;
            preenrollmentFmds = new List<Fmd>();
            count = 0;

            SendMessage(Action.SendMessage, "Place a finger on the reader.");

            if (!OpenReader())
            {
                this.Close();
            }

            if (!StartCaptureAsync(this.OnCaptured))
            {
                this.Close();
            }
        }
        public bool StartCaptureAsync(Reader.CaptureCallback OnCaptured)
        {
            // Activate capture handler
            currentReader.On_Captured += new Reader.CaptureCallback(OnCaptured);

            // Call capture
            if (!CaptureFingerAsync())
            {
                return false;
            }

            return true;
        }
        public void GetStatus()
        {
            Constants.ResultCode result = currentReader.GetStatus();

            if ((result != Constants.ResultCode.DP_SUCCESS))
            {
                reset = true;
                throw new Exception("" + result);
            }

            if ((currentReader.Status.Status == Constants.ReaderStatuses.DP_STATUS_BUSY))
            {
                Thread.Sleep(50);
            }
            else if ((currentReader.Status.Status == Constants.ReaderStatuses.DP_STATUS_NEED_CALIBRATION))
            {
                currentReader.Calibrate();
            }
            else if ((currentReader.Status.Status != Constants.ReaderStatuses.DP_STATUS_READY))
            {
                throw new Exception("Reader Status - " + currentReader.Status.Status);
            }
        }
        public bool CaptureFingerAsync()
        {
            try
            {
                GetStatus();

                Constants.ResultCode captureResult = currentReader.CaptureAsync(Constants.Formats.Fid.ANSI, Constants.CaptureProcessing.DP_IMG_PROC_DEFAULT, currentReader.Capabilities.Resolutions[0]);
                if (captureResult != Constants.ResultCode.DP_SUCCESS)
                {
                    reset = true;
                    throw new Exception("" + captureResult);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:  " + ex.Message);
                return false;
            }
        }
        public void CancelCaptureAndCloseReader(Reader.CaptureCallback OnCaptured)
        {
            if (currentReader != null)
            {
                // Dispose of reader handle and unhook reader events.
                currentReader.Dispose();

                if (reset)
                {
                    CurrentReader = null;
                }
            }
        }
        public bool OpenReader()
        {
            

            reset = false;
            Constants.ResultCode result = Constants.ResultCode.DP_DEVICE_FAILURE;

            // Open reader
            result = currentReader.Open(Constants.CapturePriority.DP_PRIORITY_EXCLUSIVE);

            if (result != Constants.ResultCode.DP_SUCCESS)
            {
                MessageBox.Show("Error:  " + result);
                reset = true;
                return false;
            }

            return true;
        }
        public bool CheckCaptureResult(CaptureResult captureResult)
        {
            if (captureResult.Data == null)
            {
                if (captureResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
                {
                    reset = true;
                    throw new Exception(captureResult.ResultCode.ToString());
                }

                // Send message if quality shows fake finger
                if ((captureResult.Quality != Constants.CaptureQuality.DP_QUALITY_CANCELED))
                {
                    throw new Exception("Quality - " + captureResult.Quality);
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// Handler for when a fingerprint is captured.
        /// </summary>
        /// <param name="captureResult">contains info and data on the fingerprint capture</param>
        private void OnCaptured(CaptureResult captureResult)
        {
            try
            {
                // Check capture quality and throw an error if bad.
                if (!CheckCaptureResult(captureResult)) return;

                count++;

                DataResult<Fmd> resultConversion = FeatureExtraction.CreateFmdFromFid(captureResult.Data, Constants.Formats.Fmd.ANSI);

                SendMessage(Action.SendMessage, "A finger was captured.  \r\nCount:  " + (count));

                if (resultConversion.ResultCode != Constants.ResultCode.DP_SUCCESS)
                {
                    //_sender.Reset = true;
                    throw new Exception(resultConversion.ResultCode.ToString());
                }

                preenrollmentFmds.Add(resultConversion.Data);

                if (count >= 4)
                {
                    DataResult<Fmd> resultEnrollment = DPUruNet.Enrollment.CreateEnrollmentFmd(Constants.Formats.Fmd.ANSI, preenrollmentFmds);

                    if (resultEnrollment.ResultCode == Constants.ResultCode.DP_SUCCESS)
                    {

                        // save in db code here

                        var Imp = resultEnrollment.Data;
                        JavaScriptSerializer JSserializer = new JavaScriptSerializer();
                        string serializedFmd = Fmd.SerializeXml(Imp);

                        //System.IO.File.WriteAllText(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/personfingerprint.txt", s);
                        PersonFingerPrint fp = new PersonFingerPrint { BranchId = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["BranchID"].ToString().Trim()),
                            Orgid = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["OrgId"].ToString().Trim()),
                            PersonID = MainForm.personid,
                            FingerPrintDataXML= serializedFmd
                        };

                        if(!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + " /PersonsData/PersonFingerPrints"))
                            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + " /PersonsData/PersonFingerPrints");


                        var ss = JSserializer.Serialize(fp);

                        System.IO.File.WriteAllText(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/PersonFingerPrints/personfingerprint("+fp.PersonID+").txt", ss);

                        var list = StaticData.Persondatalist;
                        var thisperson = list.FirstOrDefault(c => c.UserId == MainForm.personid);

                        list.Remove(thisperson);
                        thisperson.FingerPrint = serializedFmd;
                        list.Add(thisperson);
                        StaticData.Persondatalist = list;
                        string personupdated = JSserializer.Serialize(list);
                        System.IO.File.WriteAllText(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/persons.txt", personupdated);

                       CurrentReader.Reset();
                        if (!this.InvokeRequired)
                            btnOk.PerformClick();
//                        _sender.CurrentReader.Dispose();
  //                      btnOk.PerformClick();
                        // var str = JSserializer.Serialize();


                        SendMessage(Action.SendMessage, "An enrollment FMD was successfully created.");
                        SendMessage(Action.SendMessage, "Place a finger on the reader.");
                        preenrollmentFmds.Clear();
                        count = 0;
                        return;
                    }
                    else if (resultEnrollment.ResultCode == Constants.ResultCode.DP_ENROLLMENT_INVALID_SET)
                    {
                        SendMessage(Action.SendMessage, "Enrollment was unsuccessful.  Please try again.");
                        SendMessage(Action.SendMessage, "Place a finger on the reader.");
                        preenrollmentFmds.Clear();
                        count = 0;
                        return;
                    }
                }

                SendMessage(Action.SendMessage, "Now place the same finger on the reader.");
            }
            catch (Exception ex)
            {
                // Send error message, then close form
                SendMessage(Action.SendMessage, "Error:  " + ex.Message);
            }
        }

        /// <summary>
        /// Close window.
        /// </summary>
        private void btnBack_Click(System.Object sender, System.EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Close window.
        /// </summary>
        private void Enrollment_Closed(object sender, System.EventArgs e)
        {
           _sender.CancelCaptureAndCloseReader(this.OnCaptured);
        }

        #region SendMessage
        private enum Action
        {
            SendMessage
        }

        private delegate void SendMessageCallback(Action action, string payload);
        private void SendMessage(Action action, string payload)
        {
            try
            {
                if (this.txtEnroll.InvokeRequired)
                {
                    SendMessageCallback d = new SendMessageCallback(SendMessage);
                    this.Invoke(d, new object[] { action, payload });
                }
                else
                {
                    switch (action)
                    {
                        case Action.SendMessage:
                            txtEnroll.Text += payload + "\r\n\r\n";
                            txtEnroll.SelectionStart = txtEnroll.TextLength;
                            txtEnroll.ScrollToCaret();
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void btnFirstImpression_Click(object sender, EventArgs e)
        {

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            CurrentReader.Dispose();
            CurrentReader = null;
        }
    }
}