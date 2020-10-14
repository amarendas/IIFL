using System;
using VendorOpenAPIWebApp;
using System.Text;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleApp1
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //Client Profile
            string AppName = "IIFLMarAMAREN P";
            string UserKey = "okGtXuTMbGG7Bdo6J5cPDTXVeoNYMnae";
            string UserID = "i0QvHdSOthN";
            string UserPassword = "R6vDqH0zZNt";
            string EncryptionKey = "HMuDXUm24cevoNikKNkkuywv5wa8Pd8xQV5utrCatdr7ZuPBgmBlDhSPs6Bw2lFT";
            string Email_id = "amaren.das@gmail.com";
            string ContactNumber = "9757257604";
            string ClientCode = Email_id;// "AMARDAS1";
            string Password = "Disco3dancer";
            string DOB = "21101975";
            //Client Profile Ends

            VendorLogin(AppName, UserKey, UserID, UserPassword, EncryptionKey, Email_id, ContactNumber);
            ClientLogin(AppName, UserKey, UserID, UserPassword, EncryptionKey, ClientCode, Password, DOB);
            Holding(AppName, UserKey, UserID, UserPassword, ClientCode);
            Console.ReadKey(); // wait for key



        }// end of main


   

        private static void VendorLogin(string AppName, string UserKey, string UserID, string UserPassword, string EncryptionKey, string Email_id, string ContactNumber)
        {
            CommonCode obj = new CommonCode();

            var encoding2 = new UTF8Encoding();

            byte[] UserIDEncryptReturn = { };
            byte[] UserPasswordReturn = { };

            string UserIDPass;
            string UserPasswordPass;

            //Encription algorithem
            obj.Encrypt_Vendor(encoding2.GetBytes(UserID), EncryptionKey, ref UserIDEncryptReturn);
            UserIDPass = Convert.ToBase64String(UserIDEncryptReturn);
            obj.Encrypt_Vendor(encoding2.GetBytes(UserPassword), EncryptionKey, ref UserPasswordReturn);
            UserPasswordPass = Convert.ToBase64String(UserPasswordReturn);
            /* For Testing only
           
            Console.WriteLine("UserIDPass: " + UserIDPass);
            Console.WriteLine("UserPasswordPas:{0}", UserPasswordPass);
            Console.WriteLine("IP Adderss" + obj.GetIPAddress());

            */

            var _data = new CommonCode.LoginRequestMobileReq();
            CommonCode.LoginRequestMobileRes objFinal = new CommonCode.LoginRequestMobileRes();

            string ReturnData = string.Empty; // recievd data
            string postData = string.Empty;  // data to send
            string mobileServiceURL = "https://dataservice.iifl.in/openapi/prod/LoginRequestMobileForVendor";
            HttpWebRequest request = WebRequest.Create(mobileServiceURL) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers["Ocp-Apim-Subscription-Key"] = "fc714d8e5b82438a93a95baa493ff45b"; // added a custome header
            _data.head.requestCode = "IIFLMarRQLoginForVendor";
            _data.head.key = UserKey;
            _data.head.appVer = "1.0";
            _data.head.appName = AppName;
            _data.head.osName = "WEB";
            _data.head.userId = UserIDPass;
            _data.head.password = UserPasswordPass;
            _data.body.Email_id = Email_id;
            _data.body.ContactNumber = ContactNumber;
            _data.body.LocalIP = obj.GetIPAddress();
            _data.body.PublicIP = _data.body.LocalIP;
            postData = JsonConvert.SerializeObject(_data);
            var bytes = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = bytes.Length;
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(string.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));
                }

                Stream stream1 = response.GetResponseStream();
                var sr = new StreamReader(stream1);
                ReturnData = sr.ReadToEnd();
                //Console.WriteLine(ReturnData);

                string[] reponseURI = response.Headers.AllKeys;

                string jasoObject = JsonConvert.SerializeObject(ReturnData);
                Console.WriteLine(" Vendor Reply *******************");
                Console.WriteLine(JToken.Parse(ReturnData).ToString()); // prints a pritty formated string
               

            }
        }
        private static void ClientLogin(string AppName, string UserKey, string UserID, string UserPassword, string EncryptionKey, string ClientCode, string Password, string DOB)
        {
            CommonCode obj = new CommonCode();

            var encoding2 = new UTF8Encoding();

            byte[] DOBEncryptReturn = { };
            byte[] PswdEncryptReturn = { };
            byte[] CCEncryptReturn = { };
            byte[] UserIDEncryptReturn = { };
            byte[] UserPasswordReturn = { };

            string ClientCodePass;
            string PswdPass;
            string DOBPass;
            string UserIDPass;
            string UserPasswordPass;

            obj.Encrypt_Vendor(encoding2.GetBytes(ClientCode), EncryptionKey, ref CCEncryptReturn);
            ClientCodePass = Convert.ToBase64String(CCEncryptReturn);
            obj.Encrypt_Vendor(encoding2.GetBytes(Password), EncryptionKey, ref PswdEncryptReturn);
            PswdPass = Convert.ToBase64String(PswdEncryptReturn);
            obj.Encrypt_Vendor(encoding2.GetBytes(DOB), EncryptionKey, ref DOBEncryptReturn);
            DOBPass = Convert.ToBase64String(DOBEncryptReturn);
            obj.Encrypt_Vendor(encoding2.GetBytes(UserID), EncryptionKey, ref UserIDEncryptReturn);
            UserIDPass = Convert.ToBase64String(UserIDEncryptReturn);
            obj.Encrypt_Vendor(encoding2.GetBytes(UserPassword), EncryptionKey, ref UserPasswordReturn);
            UserPasswordPass = Convert.ToBase64String(UserPasswordReturn);

            var _data = new CommonCode.LoginRequestV2Req();
            string ReturnData = string.Empty;
            string postData = string.Empty;
            string mobileServiceURL = "https://dataservice.iifl.in/openapi/prod/LoginRequest";
            HttpWebRequest request = WebRequest.Create(mobileServiceURL) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers["Ocp-Apim-Subscription-Key"] = "fc714d8e5b82438a93a95baa493ff45b"; // added a custome header

            _data.head.requestCode = "IIFLMarRQLoginRequestV2";
            _data.head.key = UserKey;
            _data.head.appVer = "1.0";
            _data.head.appName = AppName;
            _data.head.osName = "WEB";
            _data.head.userId = UserID;
            _data.head.password = UserPassword;

            _data.body.ClientCode = ClientCodePass;
            _data.body.Password = PswdPass;
            _data.body.LocalIP = obj.GetIPAddress();
            _data.body.PublicIP = _data.body.LocalIP;
            _data.body.HDSerialNumber = "";
            _data.body.MACAddress = "";
            _data.body.MachineID = "";
            _data.body.VersionNo = "1.0.16.0";
            _data.body.RequestNo = "1";
            _data.body.My2PIN = DOBPass;
            _data.body.ConnectionType = "1";

            postData = Newtonsoft.Json.JsonConvert.SerializeObject(_data);
            var bytes = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = bytes.Length;
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(string.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));
                }

                Stream stream1 = response.GetResponseStream();
                var sr = new StreamReader(stream1);
                ReturnData = sr.ReadToEnd();
                string jasoObject = JsonConvert.SerializeObject(ReturnData);
                Console.WriteLine(" Client Reply **************************");
                Console.WriteLine(JToken.Parse(ReturnData).ToString()); // prints a pritty formated string
                

            }
        }
        private static void Holding(string AppName, string UserKey, string UserID, string UserPassword, string ClientCode)
        {
            CommonCode obj = new CommonCode();

            var _data = new CommonCode.CommonReq();
            string ReturnData = string.Empty;
            string postData = string.Empty;
            string mobileServiceURL = "https://dataservice.iifl.in/openapi/prod/Holding";
            HttpWebRequest request = WebRequest.Create(mobileServiceURL) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers["Ocp-Apim-Subscription-Key"] = "fc714d8e5b82438a93a95baa493ff45b"; // added a custome header

            _data.head.requestCode = "IIFLMarRQHoldingV2";
            _data.head.key = UserKey;
            _data.head.appName = AppName;
            _data.head.appVer = "1.0";
            _data.head.osName = "Android";
            _data.head.userId = UserID;
            _data.head.password = UserPassword;
            _data.body.ClientCode = ClientCode;

            postData = Newtonsoft.Json.JsonConvert.SerializeObject(_data);
            var bytes = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = bytes.Length;
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(string.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));
                }

                Stream stream1 = response.GetResponseStream();
                var sr = new StreamReader(stream1);
                ReturnData = sr.ReadToEnd();
                string jasoObject = JsonConvert.SerializeObject(ReturnData);
                Console.WriteLine(" Holding Reply **********************");
                Console.WriteLine(JToken.Parse(ReturnData).ToString()); // prints a pritty formated string
               
            }
        }

    }//End of Class Prog
} // End of namespace Console1
