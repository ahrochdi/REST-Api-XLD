using System;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RestApiClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("HTTP Injector!");
            var client = new RestClient("http://localhost:8111");
            client.Authenticator = new HttpBasicAuthenticator("ahrochdi","ROCH21021996.");
            RestRequest request = new RestRequest("app/rest/projects",Method.GET);
            IRestResponse response=client.Execute(request);
            var data = (JObject)JsonConvert.DeserializeObject(response.Content);
            Console.WriteLine("Status code: " + response.StatusCode);
            Dictionary<object,object> projects=new Dictionary<object, object>();
            /*
            foreach(var item in data["project"]){
                   projects.Add(item["id"],item["name"]);
                    Console.WriteLine (item["id"].Type);
            }*/
            //var projects =(JObject) JsonConvert.DeserializeObject(data["project"][0]);
            //Console.WriteLine("Status code: " + data["project"]);
            

            var diff_dictionary = PropertiesLoad.properitesCompare("myfile.properties","app/rest/projects",client);
            
            //string Paths_Targets="./path_Targets.properties";

           //PropertiesLoad.postAllProperties(Paths_Targets,client);
           //PropertiesLoad.getProperiesFromEnv("app/rest/projects",client);
            string[] headers= {"key","Old Value", "New Value"};
            string html = PropertiesLoad. ConvertDataTableToHTML(headers,diff_dictionary);
            PropertiesLoad.WriteHTMLReport(html,"test");

            
        }
    }
}
