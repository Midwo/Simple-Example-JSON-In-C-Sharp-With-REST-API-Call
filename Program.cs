using System.ComponentModel;
using System.Net;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Simple_Example_JSON_In_C__With_REST_API_Call
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Declaration of looking for a patient with ID = 1 and an unknown number of pages of data
            //Original link to json https://jsonmock.hackerrank.com/api/medical_records?userId=<userId>&page=<page>		
            //Basic variables for the https link (WebClient userId = and &page= )
            int idUser = 1;
            int pageUser = 1;

            //Variables needed to define the number of pages of patient data and the number of entries.
            //Assigning the appropriate data to the variable in the next stage of the code
            int pageMaxUser = 0;
            int total = 0;

            //WebClient() call with https link. Changing<user Id> to the idUser variable, and<page> to the pageUser variable.
            var json = new WebClient().DownloadString("https://jsonmock.hackerrank.com/api/medical_records?userId=" + idUser + "&page=" + pageUser + "\t\t");


            
        }

        //To write the structure you can use: https://json2csharp.com/ or write it yourself.
        public class InsideListData
        {
            public int id { get; set; }
            public object timestamp { get; set; }
            public Diagnosis diagnosis { get; set; }
            public Vitals vitals { get; set; }
            public Doctor doctor { get; set; }
            public int userId { get; set; }
            public string userName { get; set; }
            public string userDob { get; set; }
            public Meta meta { get; set; }
        }

        public class Diagnosis
        {
            public int id { get; set; }
            public string name { get; set; }
            public int severity { get; set; }
        }

        public class Doctor
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class Meta
        {
            public int height { get; set; }
            public int weight { get; set; }
        }

        public class Root
        {
            public int page { get; set; }
            public int per_page { get; set; }
            public int total { get; set; }
            public int total_pages { get; set; }
            public List<InsideListData> data { get; set; }
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
