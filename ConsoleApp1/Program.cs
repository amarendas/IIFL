using System;
using VendorOpenAPIWebApp; // refer to CommonCode.cs
using System.Text;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static string Mycooki = string.Empty;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //Client credientials
            string AppName = "IIFLMarAMAREN P";
            string AppSource = "554";
            string UserKey = "okGtXuTMbGG7Bdo6J5cPDTXVeoNYMnae";
            string UserID = "i0QvHdSOthN";
            string UserPassword = "R6vDqH0zZNt";
            string EncryptionKey = "HMuDXUm24cevoNikKNkkuywv5wa8Pd8xQV5utrCatdr7ZuPBgmBlDhSPs6Bw2lFT";
            string Email_id = "amaren.das@gmail.com";
            string ContactNumber = "9757257604";
            string ClientCode = "AMARDAS1";
            string Password = "Disco3dancer";
            string DOB = "19751021";
            //Client Profile Ends

            CookieContainer cookieContainer = new CookieContainer();
            CommonCode obj = new CommonCode();

            PrintHelp();
            int choice = 0;

            do
            {
                choice = Convert.ToInt32(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        VendorLogin(cookieContainer, obj, AppName, UserKey, UserID, UserPassword, EncryptionKey, Email_id, ContactNumber);
                        ClientLogin( obj, AppName, UserKey, UserID, UserPassword, EncryptionKey, ClientCode, Password, DOB);
                        break;
                    case 2:
                        Holding( AppName, UserKey, UserID, UserPassword, ClientCode);
                        break;
                    case 3:
                        OrderBookV2( AppName, UserKey, UserID, UserPassword, ClientCode);
                        break;
                    case 4:
                        TradeBook( AppName, UserKey, UserID, UserPassword, ClientCode);
                        break;
                    default:
                        PrintHelp();
                        break;

                }
                Console.WriteLine(" Wait for 8 sec. API call requirement");
                System.Threading.Thread.Sleep(8000);
                PrintHelp();
            }
            while (choice != 0);
            Console.WriteLine("Programme exited");

           
            




        }// end of main

        private static void PrintHelp()
        {
            Console.WriteLine("Press 0 to: Exit");
            Console.WriteLine("Press 1 to: Login");
            Console.WriteLine("Press 2 to: Get Holding list");
            Console.WriteLine("Press 3 to: Get Order list");
        }

        private static string Encrypt(string Data, string EncryptionKey, CommonCode obj)
        {
            var encoding2 = new UTF8Encoding();
            byte[] DataEncryptReturn = { };
            string EncriptedData;
            obj.Encrypt_Vendor(encoding2.GetBytes(Data), EncryptionKey, ref DataEncryptReturn);
            EncriptedData = Convert.ToBase64String(DataEncryptReturn);
            return EncriptedData;
        }

        private static void CreateHTTP_Header(HttpWebRequest request)
        {
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers["Ocp-Apim-Subscription-Key"] = "fc714d8e5b82438a93a95baa493ff45b"; // added a custome header
        }

        private static void SendHTTP_Request(string postData, HttpWebRequest request)
        {
            
            var bytes = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = bytes.Length;
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }
        }

        private static string RecieveHTTP_Responce(HttpWebRequest request)
        {
            string ReturnData;           
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(string.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));
                }

                Stream stream1 = response.GetResponseStream();
                var sr = new StreamReader(stream1);
                ReturnData = sr.ReadToEnd();

            }
            return ReturnData;
        }

        private static void VendorLogin(CookieContainer cookieContainer,CommonCode obj, string AppName, string UserKey, string UserID, string UserPassword, string EncryptionKey, string Email_id, string ContactNumber)
        {

            string UserIDPass;
            string UserPasswordPass;

            //Encription of data
            UserIDPass = Encrypt(UserID, EncryptionKey, obj);
            UserPasswordPass = Encrypt(UserPassword, EncryptionKey, obj);

            var _data = new CommonCode.LoginRequestMobileReq();
            //CommonCode.LoginRequestMobileRes objFinal = new CommonCode.LoginRequestMobileRes();

            string ReturnData = string.Empty; // recievd data
            string postData = string.Empty;  // data to send
            string mobileServiceURL = "https://dataservice.iifl.in/openapi/prod/LoginRequestMobileForVendor";
            HttpWebRequest request = WebRequest.Create(mobileServiceURL) as HttpWebRequest;
            request.CookieContainer = cookieContainer;
            CreateHTTP_Header(request);
            //***************** Create body   ************************
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
            //************************************************
            postData = JsonConvert.SerializeObject(_data);
            SendHTTP_Request(postData, request);
            ReturnData = RecieveHTTP_Responce(request);
            //Console.WriteLine(ReturnData);
            //string[] reponseURI = response.Headers.AllKeys;
            Console.WriteLine(" Vendor Reply *******************");
            string jasoObject = JsonConvert.SerializeObject(ReturnData);
            Console.WriteLine(JToken.Parse(ReturnData).ToString()); // prints a pritty formated string
        }
        private static void ClientLogin(CommonCode obj, string AppName, string UserKey, string UserID, string UserPassword, string EncryptionKey, string ClientCode, string Password, string DOB)
        {
            string ClientCodePass;
            string PswdPass;
            string DOBPass;

            // Encrypt Data
            ClientCodePass = Encrypt(ClientCode, EncryptionKey, obj); 
            PswdPass = Encrypt(Password, EncryptionKey, obj); 
            DOBPass = Encrypt(DOB, EncryptionKey, obj); 


            var _data = new CommonCode.LoginRequestV2Req();
            string ReturnData = string.Empty;
            string postData = string.Empty;
            string mobileServiceURL = "https://dataservice.iifl.in/openapi/prod/LoginRequest";
            HttpWebRequest request = WebRequest.Create(mobileServiceURL) as HttpWebRequest;
            //request.CookieContainer = cookieContainer;
            CreateHTTP_Header(request);

            //***************** Create body   ************************
            _data.head.appName = AppName;
            _data.head.appVer = "1.0";
            _data.head.key = UserKey;
            _data.head.requestCode = "IIFLMarRQLoginRequestV2";
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
            _data.body.RequestNo = "2";
            _data.body.My2PIN = DOBPass;
            _data.body.ConnectionType = "1";

            postData = JsonConvert.SerializeObject(_data);
            var bytes = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = bytes.Length;
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }
            var CookieValue = "";
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(string.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));
                }

                Stream stream1 = response.GetResponseStream();
                var sr = new StreamReader(stream1);
                ReturnData = sr.ReadToEnd();

                string[] reponseURI = response.Headers.AllKeys;
                string value1 = response.Headers.Get("Set-Cookie");
                var value2 = value1.Split(';');
                var final = value2[0].Split('=');
                CookieValue = final[1];
                Mycooki = CookieValue;

                Console.WriteLine(" Client Reply **************************");
                string jasoObject = JsonConvert.SerializeObject(ReturnData);
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
            var cookie = new Cookie("IIFLMarcookie", Mycooki);
            var CookieContainer = new CookieContainer();
            cookie.Domain = "openapi.indiainfoline.com";
            CookieContainer.Add(cookie);
            request.CookieContainer = CookieContainer;
            
            CreateHTTP_Header(request);

            _data.head.requestCode = "IIFLMarRQHoldingV2";
            _data.head.key = UserKey;
            _data.head.appName = AppName;
            _data.head.appVer = "1.0";
            _data.head.osName = "Android";
            _data.head.userId = UserID;
            _data.head.password = UserPassword;
            _data.body.ClientCode = ClientCode;

            postData = JsonConvert.SerializeObject(_data);
            SendHTTP_Request( postData, request);

            ReturnData=RecieveHTTP_Responce(request);
                string jasoObject = JsonConvert.SerializeObject(ReturnData);
                Console.WriteLine(" Holding Reply **********************");
                Console.WriteLine(JToken.Parse(ReturnData).ToString()); // prints a pritty formated string
               
            
        }
        public static void OrderBookV2( string AppName, string UserKey, string UserID, string UserPassword, string ClientCode)
        {

            CommonCode obj = new CommonCode();
            var _data = new CommonCode.CommonReq();
            string ReturnData = string.Empty;
            string postData = string.Empty;
            string mobileServiceURL = "https://dataservice.iifl.in/openapi/prod/OrderBookV2";
            HttpWebRequest request = WebRequest.Create(mobileServiceURL) as HttpWebRequest;
            var cookie = new Cookie("IIFLMarcookie", Mycooki);
            var CookieContainer = new CookieContainer();
            cookie.Domain = "openapi.indiainfoline.com";
            CookieContainer.Add(cookie);
            request.CookieContainer = CookieContainer;

            CreateHTTP_Header(request);

            _data.head.requestCode = "IIFLMarRQOrdBkV2";
            _data.head.key = UserKey;
            _data.head.appName = AppName;
            _data.head.appVer = "1.0";
            _data.head.osName = "WEB";
            _data.head.userId = UserID;
            _data.head.password = UserPassword;
            _data.body.ClientCode = ClientCode;

            postData = JsonConvert.SerializeObject(_data);
            SendHTTP_Request(postData, request);

            ReturnData = RecieveHTTP_Responce(request);

            Console.WriteLine(" Order  Reply **************************");
            string jasoObject = JsonConvert.SerializeObject(ReturnData);
            Console.WriteLine(JToken.Parse(ReturnData).ToString()); // prints a pritty formated string   
        }
        private static void TradeBook( string AppName, string UserKey, string UserID, string UserPassword, string ClientCode)
        {
            CommonCode obj = new CommonCode();

            var _data = new CommonCode.CommonReq();
            string ReturnData = string.Empty;
            string postData = string.Empty;
            string mobileServiceURL = "https://dataservice.iifl.in/openapi/prod/TradeBook";
            HttpWebRequest request = WebRequest.Create(mobileServiceURL) as HttpWebRequest;
            var cookie = new Cookie("IIFLMarcookie", Mycooki);
            var CookieContainer = new CookieContainer();
            cookie.Domain = "openapi.indiainfoline.com";
            CookieContainer.Add(cookie);
            request.CookieContainer = CookieContainer;

            CreateHTTP_Header(request);

            _data.head.requestCode = "IIFLMarRQTrdBkV1";
            _data.head.key = UserKey;
            _data.head.appName = AppName;
            _data.head.appVer = "1.0";
            _data.head.osName = "Android";
            _data.head.userId = UserID;
            _data.head.password = UserPassword;
            _data.body.ClientCode = ClientCode;

            postData = JsonConvert.SerializeObject(_data);
            SendHTTP_Request( postData, request);

            ReturnData = RecieveHTTP_Responce(request);
            string jasoObject = JsonConvert.SerializeObject(ReturnData);
            Console.WriteLine(" Trade book Reply **********************");
            Console.WriteLine(JToken.Parse(ReturnData).ToString()); // prints a pritty formated string


        }
    }//End of Class Prog
} // End of namespace Console1
