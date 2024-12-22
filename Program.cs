using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Cache;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Serialization;
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

            int totalPages = 0;
            int totalEntries = 0;
            do
            {
                Console.Clear();
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
                    totalPages = deserializeJson.total_pages;
                    totalEntries = deserializeJson.total;

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
                                RetrievedDataFromDeserialization tempValueClass = new RetrievedDataFromDeserialization
                                {
                                    bodyTemperature = item.vitals.bodyTemperature,
                                    bmi = item.meta.bmi,
                                    DateExamination = UnixTimeStampToDateTime((long)item.timestamp),
                                    height = item.meta.height,
                                    weight = item.meta.weight
                                };
                                listRetrievedDataFromDeseralization.Add(tempValueClass);
                            }
                        }
                        else
                        {
                            break;
                        }
                        actualPageUser++;
                    } while (actualPageUser <= totalPages);

                }
                List<RetrievedDataFromDeserialization> SortedList = listRetrievedDataFromDeseralization.OrderBy(s => s.DateExamination).ToList();

                Console.WriteLine("-----------------------------------------------------------------------------------\n" +
                                  "------------------Welcome to the hospital register \"Quick help\"--------------------\n" +
                                  "-----------------------------------------------------------------------------------\n");
                Console.WriteLine("Patient ID: " + idUser + "\n" +
                                  "Total number of entries: " + totalEntries + "\n" +
                                  "Date of first examination: " + SortedList.Min(s => s.DateExamination) + "\n" +
                                  "Date of last examination: " + SortedList.Max(s => s.DateExamination) + "\n \n" +
                                  "Patient condition: \n" +
                                  "Current growth: " + SortedList[totalEntries - 1].height + "\n" +
                                  "Current weight: " + SortedList[totalEntries - 1].weight + "\n" +
                                  "Previous weight: " + SortedList[totalEntries - 2].weight + "\n" +
                                  "Current body temperature: " + SortedList[totalEntries - 1].bodyTemperature + "\n" +
                                  "Previous body temperature: " + SortedList[totalEntries - 2].bodyTemperature + "\n\n" +
                                  "Weight difference: " + (SortedList[totalEntries - 1].weight - SortedList[totalEntries - 2].weight) + "\n" +
                                  "Body temperature difference: " + (SortedList[totalEntries - 1].bodyTemperature - SortedList[totalEntries - 2].bodyTemperature) + "\n" +
                                  "Current BMI: " + SortedList[totalEntries - 1].bmi + "\n\n\n" +
                                  "Recommendations :");
                var tempHowManyRecommendations = 0;
                var tempBmi = SortedList[totalEntries - 1].bmi;
                var tempTemperatureBody = SortedList[totalEntries - 1].bodyTemperature;
                List<string> listRecommendations = new List<string>();

                if (tempBmi < (decimal)18.5)
                {
                    listRecommendations.Add(" - He was found to be underweight. Please increase the weight.");
                }
                else if (tempBmi <= (decimal)24.9)
                {
                    listRecommendations.Add(" - Normal weight was found. Please maintain your weight.");
                }
                else
                {
                    listRecommendations.Add(" - Overweight was diagnosed. Please reduce your weight.");
                }
                if (tempTemperatureBody < (decimal)36.5)
                {
                    listRecommendations.Add(" - Reduced body temperature was found. The patient's temperature should be increased.");
                }
                else if (tempTemperatureBody <= (decimal)37)
                {
                    listRecommendations.Add(" - Correct body temperature was found. The temperature must be maintained.");
                }
                else
                {
                    listRecommendations.Add(" - High body temperature was detected. The patient's body temperature should be reduced.");
                }

                foreach (var item in listRecommendations)
                {
                    Console.WriteLine(item);
                }

                Console.WriteLine("\n\n\n\n To search for another patient, press \"enter\"");
            } while (Console.ReadKey().Key == ConsoleKey.Enter);
        }
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(Math.Round(unixTimeStamp / 1000d)).ToLocalTime();
            return dateTime;
        }
        //List with deserialized data
        public static List<RetrievedDataFromDeserialization> listRetrievedDataFromDeseralization = new List<RetrievedDataFromDeserialization>();

        //Class in list with data deserialization
        public class RetrievedDataFromDeserialization
        {
            public decimal? bmi { get; set; }
            public decimal bodyTemperature { get; set; }
            public DateTime DateExamination { get; set; }
            public int height { get; set; }
            public decimal weight { get; set; }
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
            private decimal _weight;
            public int height { get; set; }
            public decimal weight {
                get { return _weight; }
                set { _weight = Math.Round(Convert.ToDecimal((value) / (decimal)2.20462262185), 1); } 
            }
            public decimal? bmi
            {
                get { return Math.Round((weight)/ ((Convert.ToDecimal(height) / 100 * Convert.ToDecimal(height) / 100)),2); }
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
            private decimal _bodyTemperature;
            public int bloodPressureDiastole { get; set; }
            public int bloodPressureSystole { get; set; }
            public int pulse { get; set; }
            public int breathingRate { get; set; }
            public decimal bodyTemperature
            {
                get { return _bodyTemperature; }
                set { _bodyTemperature = Math.Round(((value - 32) / (decimal)1.8),1); }
            }
        }
    }
}
