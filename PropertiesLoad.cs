using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace RestApiClient
{
    public class PropertiesLoad{
        
        public static Dictionary<string,string> properitesParser(string path){
            var data = new Dictionary<string, string>();
            foreach (var row in File.ReadAllLines(path)){
               data.Add(row.Split("=")[0], string.Join("=",row.Split("=").Skip(1).ToArray()));
        }
        return data;

        }

        public static JObject loadToJObject(Dictionary<string,string> myprop_dictionary){
            JObject jObjectBody=new JObject();
            
            foreach(KeyValuePair<string,string> entry in myprop_dictionary){
                jObjectBody.Add(entry.Key,entry.Value);
            }
            return jObjectBody;
        }

        public static void postProperties(string PropertiesPath,string Target,RestClient client){
            
            var myprop_dictionary=PropertiesLoad.properitesParser(PropertiesPath);
            var jObjectBody=PropertiesLoad.loadToJObject(myprop_dictionary);


            RestRequest postResquest = new RestRequest(Target,Method.POST);
            postResquest.RequestFormat=DataFormat.Json;
            postResquest.AddParameter("application/json",jObjectBody,ParameterType.RequestBody);
            IRestResponse postResponse=client.Execute(postResquest);
            Console.WriteLine("Status code: " + postResponse.StatusCode);
            Console.WriteLine ("Status message " + postResponse.Content);
        }


        public static void  postAllProperties(string Paths_Targets,RestClient client){
            
            var Paths_Targets_Dictionary = properitesParser(Paths_Targets);
            foreach(KeyValuePair<string,string> entry in Paths_Targets_Dictionary){
                postProperties(entry.Key,entry.Value,client);
            }
        }

        public static Dictionary<string, string>  getProperiesFromEnv(string Target,RestClient client){
            RestRequest request = new RestRequest(Target,Method.GET);
         IRestResponse response=client.Execute(request);
            var data = (JObject)JsonConvert.DeserializeObject(response.Content);
            Dictionary<string,string> projects=new Dictionary<string, string>();
            /*
            foreach(var item in data["project"]){
                   projects.Add((string) item["id"],(string) item["name"]);


                    //Console.WriteLine (item["id"].Type);
            }*/
            projects.Add("id", (string) data["project"][4]["id"]);
            projects.Add("name",(string) data["project"][4]["id"]);

            return projects;
        }

        public static Dictionary<string,Tuple<string, string>> properitesCompare(string PropertiesPath,string Target,RestClient client){
            var myprop_dictionary=PropertiesLoad.properitesParser(PropertiesPath);
            var envprop_dictionary= getProperiesFromEnv(Target,client);
            var diff_Dictionary= new Dictionary<string,Tuple<string, string>>();

            foreach(KeyValuePair<string,string> entry in envprop_dictionary){
                
                //myprop_dictionary.TryGetValue((string)entry.Key, out value) & 
                if(!myprop_dictionary[entry.Key].Equals(entry.Value)){
                    Tuple<string, string> OldAndNewValues=new Tuple<string, string>(entry.Value,myprop_dictionary[entry.Key]);
                    Console.WriteLine("#################");
                    Console.WriteLine(OldAndNewValues);
                    Console.WriteLine("#################");
                    
                    diff_Dictionary.Add((string)entry.Key,OldAndNewValues);
                }
            }
            //printDictionary(myprop_dictionary);
            //Console.WriteLine("#################");
            //printDictionary(diff_Dictionary);
            
            return diff_Dictionary;
            

        }

        public static void printDictionary(Dictionary<string,string> dictionary){
            foreach(KeyValuePair<string,string> entry in dictionary){
                Console.WriteLine("key  :"+entry.Key);
                Console.WriteLine("value  :"+entry.Value);
            }
        }

         public static string ConvertDataTableToHTML(string[] headers,Dictionary<string,Tuple<string, string>> diff_dictionary){
             
             string html="";

            
            
            html +="<!DOCTYPE html><html><head><style>table {  font-family: arial, sans-serif;  border-collapse: collapse;  width: 100%;}td, th {  border: 1px solid #dddddd;  text-align: left;  padding: 8px;}tr:nth-child(even) {background-color: #dddddd;}</style></head><body>";
             
             
            html+= "<h2>Configuration Report</h2>";    
             html+="<table>";
             //add header row
            html += "<tr>";
             foreach(string header in headers)
                 html+="<td>"+header+"</td>";
                 html += "</tr>";
             //add rows
            foreach (KeyValuePair<string,Tuple<string, string>>entry in diff_dictionary)
             {
                html += "<tr>";
              
                html += "<td>" + entry.Key + "</td>";
                html += "<td>" + entry.Value.Item1 + "</td>";
                html += "<td>" + entry.Value.Item2 + "</td>";
                
                html += "</tr>";
            }

            html += "</table>";
            html+="</body> </html>";
            return html;
            
        }

        public static void WriteHTMLReport(string html,string Target){
                    
            string path = Target + ".html";
                    
            try
            {
             StreamWriter sw = new StreamWriter(path);
             sw.WriteLine(html); 
             sw.Close();
            }
             catch(Exception e)
            {
             Console.WriteLine("Exception: " + e.Message);
             }
            finally
            {
             Console.WriteLine("Report Generated : "+ path);
            }
        }
    }
}