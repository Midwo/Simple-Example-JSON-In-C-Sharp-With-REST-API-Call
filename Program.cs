using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Cache;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Simple_Example_JSON_In_C__With_REST_API_Call
{
    internal class Program
    {
        private static HttpClient Client = new HttpClient();
        public static async Task Main(string[] args)
        {
            //Declaration of looking for a patient with ID = 1 and an unknown number of pages of data
            //Original link to json https://jsonmock.hackerrank.com/api/medical_records?userId=<userId>&page=<page>		
            //Basic variables for the https link (WebClient/HttpClient userId = and &page= )
            int idUser;
            int actualPageUser = 1;
            bool tempBoolCorrectIntValue = true;
            do
            {
                Console.Write("Patient ID: ");
                var tempStringReadLine = Console.ReadLine();
                tempBoolCorrectIntValue = int.TryParse(tempStringReadLine, out idUser);
                if (!tempBoolCorrectIntValue)
                {
                    Console.Clear();
                    Console.WriteLine("You have entered an incorrect Patient ID");
                }
            } while (!tempBoolCorrectIntValue);
            Console.Clear();

            ////Selection of data retrieval method, newer HttpClient and old, not recommended WebClient
            ////WebClient() call with https link. Changing<user Id> to the idUser variable, and<page> to the pageUser variable.
            //// var stringWebsiteJson = new WebClient().DownloadString("https://jsonmock.hackerrank.com/api/medical_records?userId=" + idUser + "&page=" + actualPageUser + "\t\t");
            var stringWebsiteJson = await Client.GetStringAsync("https://jsonmock.hackerrank.com/api/medical_records?userId=" + idUser + "&page=" + actualPageUser + "\t\t");

            ////Add to project "Newtonsoft.Json"  How? Project -> Manage NuGet Packages -> Newtonsoft.Json
            ////Program.Root x = Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(stringWebsiteJson);
            var deserializeJson = Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(stringWebsiteJson, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
         

            if (deserializeJson != null)
            {
                //Variables needed to define the number of pages of patient data and the number of entries.
                //Assigning the appropriate data to the variable in the next stage of the code
                int totalPages = deserializeJson.total_pages;
                int totalEntries = deserializeJson.total;

                //DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                //dateTime = dateTime.AddSeconds(Math.Round(1568550058964 / 1000d)).ToLocalTime();
                //Console.WriteLine(dateTime);
                List<long> listTimeStamp = new List<long>();
                do
                {
                    if (actualPageUser > 1)
                    {
                        ////Select the new HttpClient method or the old WebClient method
                        //stringWebsiteJson = new WebClient().DownloadString("https://jsonmock.hackerrank.com/api/medical_records?userId=" + idUser + "&page=" + actualPageUser + "\t\t");
                        stringWebsiteJson = await Client.GetStringAsync("https://jsonmock.hackerrank.com/api/medical_records?userId=" + idUser + "&page=" + actualPageUser + "\t\t");

                        deserializeJson = Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(stringWebsiteJson, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });
                    }
                    //else if (actualPageUser == 1)
                    //{
                    //    Console.WriteLine(deserializeJson.data[0].timestamp);
                    //}
                    //if (actualPageUser == totalPages)
                    //{
                    //    Console.WriteLine(deserializeJson.data[deserializeJson.data.Count()-1].timestamp);
                    //}

                    if (deserializeJson != null)
                    {
                        foreach (var item in deserializeJson.data)
                        {
                            listTimeStamp.Add((long)item.timestamp);
                            Console.WriteLine(item.meta.weight);
                            Console.WriteLine(item.meta.height);
                            Console.WriteLine(item.meta.bmi);
                            RetrievedDataFromDeserialization tempValueClass = new RetrievedDataFromDeserialization { bodyTemperature = item.vitals.bodyTemperature };
                            sss.Add(tempValueClass);
                        }
                    }
                    else
                    {
                        break;
                    }
                    actualPageUser++;
                } while (actualPageUser <= totalPages);
                Console.WriteLine(UnixTimeStampToDateTime(listTimeStamp.Max()));
                Console.WriteLine(UnixTimeStampToDateTime(listTimeStamp.Min()));
                
            }
            foreach (var item in sss)
            {         
                Console.WriteLine(item.bodyTemperature);       
            }
        }
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(Math.Round(unixTimeStamp / 1000d)).ToLocalTime();
            return dateTime;
        }
        //List with deserialized data
        public static List<RetrievedDataFromDeserialization> sss = new List<RetrievedDataFromDeserialization>();

        //Class in list with data deserialization
        public class RetrievedDataFromDeserialization
        {
            public double bodyTemperature { get; set; }
        }


        //To write the structure you can use: https://json2csharp.com/ or write it yourself.
        public class InsideListData
        {
            public int id { get; set; }
            public long timestamp { get; set; }
            public Diagnosis? diagnosis { get; set; }
            public Vitals? vitals { get; set; }
            public Doctor? doctor { get; set; }
            public int userId { get; set; }
            public string? userName { get; set; }
            public string? userDob { get; set; }
            public Meta? meta { get; set; }
        }

        public class Diagnosis
        {
            public int id { get; set; }
            public string? name { get; set; }
            public int severity { get; set; }
        }

        public class Doctor
        {
            public int id { get; set; }
            public string? name { get; set; }
        }

        public class Meta
        {
            public int height { get; set; }
            public int weight { get; set; }
            public decimal? bmi
            {
                get { return Math.Round(Convert.ToDecimal((weight) / 2.20462262185 )/ ((Convert.ToDecimal(height) / 100 * Convert.ToDecimal(height) / 100)),2); }
                set { bmi = value; }
            }
        }

        public class Root
        {
            public int page { get; set; }
            public int per_page { get; set; }
            public int total { get; set; }
            public int total_pages { get; set; }
            public List<InsideListData>? data { get; set; }
        }

        public class Vitals
        {
            public int bloodPressureDiastole { get; set; }
            public int bloodPressureSystole { get; set; }
            public int pulse { get; set; }
            public int breathingRate { get; set; }
            public double bodyTemperature { get; set; }
        }
    }
}
