using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Timers;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Text;
using System.IO;

namespace Attendence
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            System.Timers.Timer timerpostAttendence = new System.Timers.Timer(60000);
            timerpostAttendence.Elapsed += new System.Timers.ElapsedEventHandler(PostAttendenceRecordsTimer);
            timerpostAttendence.Start();
           
            SaveFingerPrintsasync().Wait();
            FetchUserData().Wait();
           
            //System.Timers.Timer timer = new System.Timers.Timer(Convert.ToDouble(System.Configuration.ConfigurationSettings.AppSettings["FingerPrintsUploadTimeinMinutes"].ToString().Trim()) * 1000 * 60 * 60);
            //timer.Enabled = true;
            //timer.Elapsed += new System.Timers.ElapsedEventHandler(SaveFingerPrints);
            Application.Run(new MainForm());

           
        }

        private static void PostAttendenceRecordsTimer(object sender, ElapsedEventArgs e)
        {
            PostAttendenceRecords().Wait();
        }

        private static void SaveFingerPrints(object sender, ElapsedEventArgs e)
        {
            try
            {
               SaveFingerPrintsasync().Wait();
            }
            catch (Exception exp)
            {
               // ErrorLogger.WriteError("UploadReportService", "StartService", exp.ToString());
            }
        }

        private static async Task SaveFingerPrintsasync()
        {
            List<PersonFingerPrint> listfp=new List<PersonFingerPrint>();
            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + " /PersonsData/PersonFingerPrints"))
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + " /PersonsData/PersonFingerPrints");


            string[] allfiles = System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/PersonFingerPrints");
            for (int i = 0; i < allfiles.Length; i++)
            {
                if (!System.IO.File.Exists(allfiles[i]))
                    continue;

                string alldatainfile = System.IO.File.ReadAllText(allfiles[i]);
                JavaScriptSerializer objserializer = new JavaScriptSerializer();
                PersonFingerPrint fp = objserializer.Deserialize<PersonFingerPrint>(alldatainfile);
                listfp.Add(fp);
            }

            
            //listfp = objserializer.Deserialize<List<PersonFingerPrint>>(alldatainfile);
            if (listfp != null && listfp.Count > 0)
            {
                Task<Boolean>[] objtaskss = new Task<Boolean>[listfp.Count];
                int i = 0;
                foreach (PersonFingerPrint pf in listfp)
                {
                    objtaskss[i] = Posttoserver(pf);
                    i++;
                }
                await Task.WhenAll(objtaskss);
            }

           
            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri("http://localhost:8080/");
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    // TODO - Send HTTP requests
            //    string _BranchId = System.Configuration.ConfigurationSettings.AppSettings["BranchID"].ToString().Trim();

            //    HttpResponseMessage response = await client.GetAsync("api/person/" + _BranchId);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        var responsea = await response.Content.ReadAsStringAsync();// ReadAsAsync<List<int>> ();
            //        if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/"))
            //            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/");

            //        System.IO.File.WriteAllText(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/persons.txt", responsea);
            //        var person = new List<Person>();                                                     //Console.WriteLine("{0}\t${1}\t{2}", product.Name, product.Price, product.Category);
            //        JavaScriptSerializer JSserializer = new JavaScriptSerializer();
            //        person = JSserializer.Deserialize<List<Person>>(responsea);
            //        StaticData.Persondatalist = person;
            //        //var xyz = 1;
            //    }
            //}
        }

        private static async Task PostAttendenceRecords()
        {
            List<personnelAttendence> listfp = new List<personnelAttendence>();
            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + " /PersonsData/PersonnelAttendence"))
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + " /PersonsData/PersonnelAttendence");


            string[] allfiles = System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/PersonnelAttendence");
            for (int i = 0; i < allfiles.Length; i++)
            {
                if (!System.IO.File.Exists(allfiles[i]))
                    continue;

                string alldatainfile = System.IO.File.ReadAllText(allfiles[i]);
                JavaScriptSerializer objserializer = new JavaScriptSerializer();
                personnelAttendence fp = objserializer.Deserialize<personnelAttendence>(alldatainfile);
                listfp.Add(fp);
            }


            //listfp = objserializer.Deserialize<List<PersonFingerPrint>>(alldatainfile);
            if (listfp != null && listfp.Count > 0)
            {
                //JavaScriptSerializer objserializer = new JavaScriptSerializer();
                //var lstjson = objserializer.Serialize(listfp);
                //  await PostAttendencetoserver(lstjson);
                Task<Boolean>[] objtaskss = new Task<Boolean>[listfp.Count];
                int i = 0;
                foreach (personnelAttendence pf in listfp)
                {
                    objtaskss[i] = PostAttendencetoserver(pf);
                    i++;
                }
                await Task.WhenAll(objtaskss);
            }


            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri("http://localhost:8080/");
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    // TODO - Send HTTP requests
            //    string _BranchId = System.Configuration.ConfigurationSettings.AppSettings["BranchID"].ToString().Trim();

            //    HttpResponseMessage response = await client.GetAsync("api/person/" + _BranchId);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        var responsea = await response.Content.ReadAsStringAsync();// ReadAsAsync<List<int>> ();
            //        if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/"))
            //            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/");

            //        System.IO.File.WriteAllText(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/persons.txt", responsea);
            //        var person = new List<Person>();                                                     //Console.WriteLine("{0}\t${1}\t{2}", product.Name, product.Price, product.Category);
            //        JavaScriptSerializer JSserializer = new JavaScriptSerializer();
            //        person = JSserializer.Deserialize<List<Person>>(responsea);
            //        StaticData.Persondatalist = person;
            //        //var xyz = 1;
            //    }
            //}
        }
        private static async Task<bool> PostAttendencetoserver(personnelAttendence pf)
        {
            try
            {
                HttpClientHandler httpClientHandler = null;
                if (System.Configuration.ConfigurationSettings.AppSettings["IsNetworkProxyEnabled"].ToString().Trim().ToLower() == "true")
                {
                    httpClientHandler = new HttpClientHandler
                    {
                        Proxy = new WebProxy(System.Configuration.ConfigurationSettings.AppSettings["proxyServerAddress"].ToString().Trim(), true),
                        UseProxy = true
                    };

                }
                else
                {
                    httpClientHandler = new HttpClientHandler();
                }
                using (var client = new HttpClient(httpClientHandler))
                {

                    client.BaseAddress = new Uri(System.Configuration.ConfigurationSettings.AppSettings["ServiceBaseUri"].ToString().Trim());
                    var requestUri = System.Configuration.ConfigurationSettings.AppSettings["ServiceBaseUri"].ToString().Trim() + "/api/PersonnelAttendence";
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //                client.DefaultRequestHeaders.Authorization =
                    //new AuthenticationHeaderValue(
                    //    "Basic",
                    //    Convert.ToBase64String(
                    //        System.Text.ASCIIEncoding.ASCII.GetBytes(
                    //            string.Format("{0}:{1}", System.Configuration.ConfigurationSettings.AppSettings["user"].ToString().Trim(), System.Configuration.ConfigurationSettings.AppSettings["pass"].ToString().Trim()))));
                    // client.DefaultRequestHeaders.Add("Authorization", System.Configuration.ConfigurationSettings.AppSettings["Authorization"].ToString().Trim());
                    //  client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                    // var jsonstring=JavaScriptSe
                    //var values = new JObject();
                    //values.Add("attendencerecordsjson",pf);

                    //HttpContent content = new StringContent(values.ToString());//, Encoding.UTF8, "application/json"
                    var values = new JObject();
                   // values.Add("Orgid", System.Configuration.ConfigurationSettings.AppSettings["Orgid"].ToString().Trim());
                    values.Add("branch_id", System.Configuration.ConfigurationSettings.AppSettings["BranchID"].ToString().Trim());
                    values.Add("Emp_Id", pf.Emp_Id);
                    values.Add("EnteredOn", pf.EnteredOn);
                 //   values.Add("FingerPrintDataXML", pf.FingerPrintDataXML);
                    HttpContent content = new StringContent(values.ToString(), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(requestUri, content);
                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/PersonnelAttendence/ArchivedAttendence"))
                            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/PersonnelAttendence/ArchivedAttendence");

                        //string[] allfilesinattendencefolder = System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/PersonnelAttendence", "*.txt");
                        //foreach (string filepath in allfilesinattendencefolder)
                        //{
                        // string filename = System.IO.Path.GetFileName(filepath);
                        string filenametobemoved = System.DateTime.Now.AddMinutes(-5).ToString("ddMMyyyy") + "(" + pf.Emp_Id.ToString() + ").txt";
                        if (File.Exists(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/PersonnelAttendence/ArchivedAttendence/" + filenametobemoved + ".bak"))
                        {
                            File.Delete(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/PersonnelAttendence/ArchivedAttendence/" + filenametobemoved + ".bak");
                        }
                        
                       System.IO.File.Move(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/PersonnelAttendence/"+filenametobemoved, System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/PersonnelAttendence/ArchivedAttendence/" + filenametobemoved+".bak");
                        //}//createdcaseid = await response.Content.ReadAsStringAsync();
                        //  UpdatelocalDB(caserecord.supplierCallRef, CadCaseID, createdcaseid);
                        return true;
                    }
                    else
                    {
                        // ErrorLogger.WriteActivity("UploadReportService", "PostToService", await response.Content.ReadAsStringAsync());
                        return false;

                    }

                }
            }

            catch (Exception ee)
            {
                //   ErrorLogger.WriteError("UploadReportService", "PostToService", ee.ToString());
                return false;
            }
        }
        private static async Task<bool> Posttoserver(PersonFingerPrint pf)
        {
            try
            {
                HttpClientHandler httpClientHandler = null;
                if (System.Configuration.ConfigurationSettings.AppSettings["IsNetworkProxyEnabled"].ToString().Trim().ToLower() == "true")
                {
                    httpClientHandler = new HttpClientHandler
                    {
                        Proxy = new WebProxy(System.Configuration.ConfigurationSettings.AppSettings["proxyServerAddress"].ToString().Trim(), true),
                        UseProxy = true
                    };

                }
                else
                {
                    httpClientHandler = new HttpClientHandler();
                }
                using (var client = new HttpClient(httpClientHandler))
                {
                    
                    client.BaseAddress = new Uri(System.Configuration.ConfigurationSettings.AppSettings["ServiceBaseUri"].ToString().Trim());
                    var requestUri = System.Configuration.ConfigurationSettings.AppSettings["ServiceBaseUri"].ToString().Trim() + "/api/person";
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    //                client.DefaultRequestHeaders.Authorization =
    //new AuthenticationHeaderValue(
    //    "Basic",
    //    Convert.ToBase64String(
    //        System.Text.ASCIIEncoding.ASCII.GetBytes(
    //            string.Format("{0}:{1}", System.Configuration.ConfigurationSettings.AppSettings["user"].ToString().Trim(), System.Configuration.ConfigurationSettings.AppSettings["pass"].ToString().Trim()))));
                    // client.DefaultRequestHeaders.Add("Authorization", System.Configuration.ConfigurationSettings.AppSettings["Authorization"].ToString().Trim());
                    //  client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                    // var jsonstring=JavaScriptSe
                    var values = new JObject();
                    values.Add("Orgid", System.Configuration.ConfigurationSettings.AppSettings["Orgid"].ToString().Trim());
                    values.Add("BranchId", System.Configuration.ConfigurationSettings.AppSettings["BranchId"].ToString().Trim());
                    values.Add("PersonID", pf.PersonID);
                    values.Add("FingerPrintDataXML", pf.FingerPrintDataXML);
                    HttpContent content = new StringContent(values.ToString(), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(requestUri,content);
                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        if(!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/PersonFingerPrints/ArchivedFingerPrints"))
                            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/PersonFingerPrints/ArchivedFingerPrints");

                        System.IO.File.Move(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/PersonFingerPrints/personfingerprint(" + pf.PersonID + ").txt", System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/PersonFingerPrints/ArchivedFingerPrints/personfingerprint(" + pf.PersonID + ").txt.bak");
                        //createdcaseid = await response.Content.ReadAsStringAsync();
                        //  UpdatelocalDB(caserecord.supplierCallRef, CadCaseID, createdcaseid);
                        return true;
                    }
                    else
                    {
                       // ErrorLogger.WriteActivity("UploadReportService", "PostToService", await response.Content.ReadAsStringAsync());
                        return false;

                    }

                }
            }

            catch (Exception ee)
            {
             //   ErrorLogger.WriteError("UploadReportService", "PostToService", ee.ToString());
                return false;
            }
        }

        private static void Timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //DataTable dt=
                FetchUserData().Wait();
            //UpdateLocalData(dt);
        }

        private static void UpdateLocalData(DataTable dt)
        {
            //throw new NotImplementedException();
        }

        private static async Task FetchUserData()
        {
            HttpClientHandler httpClientHandler = null;
            if (System.Configuration.ConfigurationSettings.AppSettings["IsNetworkProxyEnabled"].ToString().Trim().ToLower() == "true")
            {
                httpClientHandler = new HttpClientHandler
                {
                    Proxy = new WebProxy(System.Configuration.ConfigurationSettings.AppSettings["proxyServerAddress"].ToString().Trim(), true),
                    UseProxy = true
                };

            }
            else
            {
                httpClientHandler = new HttpClientHandler();
            }
            using (var client = new HttpClient(httpClientHandler))
            {
                client.BaseAddress = new Uri(System.Configuration.ConfigurationSettings.AppSettings["ServiceBaseUri"].ToString().Trim());
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                // TODO - Send HTTP requests
                string _BranchId = System.Configuration.ConfigurationSettings.AppSettings["BranchID"].ToString().Trim();

                HttpResponseMessage response = await client.GetAsync("api/person/"+_BranchId);
                if (response.IsSuccessStatusCode)
                {
                    var responsea = await response.Content.ReadAsStringAsync();// ReadAsAsync<List<int>> ();
                    if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/"))
                        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/PersonsData/");

                    System.IO.File.WriteAllText(System.IO.Path.GetDirectoryName(Application.ExecutablePath)+"/PersonsData/persons.txt", responsea);
                    var person = new List<Person>();                                                     //Console.WriteLine("{0}\t${1}\t{2}", product.Name, product.Price, product.Category);
                    JavaScriptSerializer JSserializer = new JavaScriptSerializer();
                    person = JSserializer.Deserialize<List<Person>>(responsea);
                    StaticData.Persondatalist = person;
                    //var xyz = 1;
                }
            }
        }
    }
}
