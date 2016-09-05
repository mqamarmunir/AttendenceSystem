using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Threading;
using DPUruNet;
using System.Reflection;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Attendence
{
    public partial class MainForm : Form
    {
        public static int personid { get; set; }
        List<Fmd> preenrollmentFmds;
        int count;
        public MainForm()
        {
            InitializeComponent();
            pictheader.ImageLocation = System.Configuration.ConfigurationSettings.AppSettings["HeaderImage"].ToString().Trim();
            if (StaticData.Persondatalist != null)
                dgPersons.DataSource = StaticData.Persondatalist;

            _readers = ReaderCollection.GetReaders();
            currentReader = _readers[0];
            attendence1._sender = this;


        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
              //  FetchUserDetails(textBox1.Text.Trim());
               
            }
            catch { }
        }

        private void FetchUserDetails(int Personid)
        {
            if (StaticData.Persondatalist != null && StaticData.Persondatalist.Count > 0)
            {
                //string jsondata = File.ReadAllText(Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/persons.txt");

                //var personslist = new List<Person>();                                                     //Console.WriteLine("{0}\t${1}\t{2}", product.Name, product.Price, product.Category);
                //JavaScriptSerializer JSserializer = new JavaScriptSerializer();
                //personslist = JSserializer.Deserialize<List<Person>>(jsondata);
                //tring login = loginName;

                Person person = null;
                person = StaticData.Persondatalist.SingleOrDefault(c => c.UserId == Personid);
                if (person != null)
                {
                    txtName.Text = person.FirstName + " " +person.LastName;
                    txtDesignamtion.Text = person.CellNo;
                    //textBox1.Text = person.Login;
                    personid = person.UserId;
                    btnThumbRegistration.Enabled = true;
                }
                else
                {
                    MessageBox.Show("No record found");
                    ResetForm();
                }
            }
            else
            {
                MessageBox.Show("No record on Web Server");
            }
        }

        private void ResetForm()
        {
            txtDesignamtion.Text = "";
            txtName.Text = "";
            txtOther.Text = "";
            textBox1.Text = "";
            btnThumbRegistration.Enabled = false;
            personid = 0;

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void dgPersons_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgPersons.Rows[e.RowIndex].Cells["FingerPrint"].Value.ToString().Trim() == "")
            {
                int userid = Convert.ToInt32(dgPersons.Rows[e.RowIndex].Cells["userid"].Value.ToString().Trim());
                FetchUserDetails(userid);

            }
            else
            {
                MessageBox.Show("User already registered.");
            }
        }

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
                        string s = Fmd.SerializeXml(Imp);

                        //System.IO.File.WriteAllText(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/personfingerprint.txt", s);

                       

                        var ss=JSserializer.Serialize(new PersonFingerPrint());
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

        private void btnThumbRegistration_Click(object sender, EventArgs e)
        {
            //txtEnroll.Text = string.Empty;
            //preenrollmentFmds = new List<Fmd>();
            //count = 0;

            //SendMessage(Action.SendMessage, "Place a finger on the reader.");

            //if (!OpenReader())
            //{
            //    this.Close();
            //}

            //if (!StartCaptureAsync(this.OnCaptured))
            //{
            //  //  this.Close();
            //}
            //{
            frmThumbRegistration obj = new frmThumbRegistration();
            obj._sender = this;
            var result = obj.ShowDialog();
            if (result == DialogResult.OK)
            {
                ///d
                /// dgPersons.DataSource = StaticData.Persondatalist;
                /// 
                ResetForm();
                dgPersons.DataSource = null;
                dgPersons.DataSource = StaticData.Persondatalist;
                

            }
        }

        public bool Reset
        {
            get { return reset; }
            set { reset = value; }
        }
        private bool reset;
        public Reader CurrentReader
        {
            get { return currentReader; }
            set
            {
                currentReader = value;
               // SendMessage(Action.UpdateReaderState, value);
            }
        }
        private static Reader currentReader;
        private ReaderCollection _readers;
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

    
       

        private void button2_Click(object sender, EventArgs e)
        {
            frmIdentification objin = new frmIdentification();
            objin._sender = this;
            objin.ShowDialog();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //string apiUrl = "http://localhost:58949/api/person/";
            //var client = new HttpClient();
            //var values = new JObject();
            ////values.Add("PersonID", PersonFingerPrint.PersonID);
            ////values.Add("FingerPrintDataXML", PersonFingerPrint.FingerPrintDataXML);
            //// var content = new FormUrlEncodedContent(values);
            //HttpContent content = new StringContent(values.ToString(), Encoding.UTF8, "application/json");
            //var response =  client.PostAsync(apiUrl, content).Result;
        }

        private void tabControl1_TabIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                _readers = ReaderCollection.GetReaders();
                currentReader = _readers[0];
                //OpenReader();
                //StartCaptureAsync();
                attendence1._sender = this;
                attendence1.InitializeDevice();
            }
            else
            {
                CurrentReader.Dispose();
                CurrentReader = null;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CurrentReader != null)
            {
                CurrentReader.Dispose();
                CurrentReader = null;
            }
        }
    }
}
