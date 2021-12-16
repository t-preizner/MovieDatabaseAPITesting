using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using NUnit.Framework;
using Newtonsoft.Json;
using RestSharp;

namespace MovieDatabaseAPITesting
{
    public class Helpers
    {
        public RestRequest GETRequest(string host, string apiKey)
        {
            RestRequest request = new RestRequest(Method.GET);

            request.AddHeader("x-rapidapi-host", $"{host}");
            request.AddHeader("x-rapidapi-key", $"{apiKey}");

            return request;
        }

        public List<Search> PrepareMoviesListFromCSVUsingAttributes(string pathToFile)
        {
            var streamReader = new StreamReader($"{pathToFile}");
            var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
            List<Search> movieList = csvReader.GetRecords<Search>().ToList();

            return movieList;
        }

        public List<Search> PrepareMoviesListFromCSVUsingMapping(string pathToFile)
        {
            var streamReader = new StreamReader($"{pathToFile}");
            var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
            csvReader.Context.RegisterClassMap<SearchMap>();
            List<Search> movieList = csvReader.GetRecords<Search>().ToList();

            return movieList;
        }

        public JObject PrepareResponseModel(List<Search> movieList, string count, string response)
        {
            var fullResponse = new Root(movieList, count, response);

            var responseString = JsonConvert.SerializeObject(fullResponse);
            JObject model = (JObject)JToken.Parse(responseString);

            return model;
        }

        public JObject PrepareResponseModelFromCSVUsingAttributes(string pathToFile)
        {
            var movieList = new Helpers().PrepareMoviesListFromCSVUsingAttributes(pathToFile);
            var model = new Helpers().PrepareResponseModel(movieList, movieList.Count.ToString(), "True");

            return model;
        }

        public JObject PrepareResponseModelFromCSVUsingMapping(string pathToFile)
        {
            var movieList = new Helpers().PrepareMoviesListFromCSVUsingMapping(pathToFile);
            var model = new Helpers().PrepareResponseModel(movieList, movieList.Count.ToString(), "True");

            return model;
        }

        public JObject PrepareResponseModelFromJSON(string pathToFile)
        {
            StreamReader responseRead = new StreamReader($"{pathToFile}");
            string validJson = responseRead.ReadToEnd();
            var model = JObject.Parse(validJson);

            return model;
        }

        public JObject PrepareSeparatePageModelFromCSV(string pathToFile, int indexOfFirstElement, int countOfElements)
        {
            var movieList = new Helpers().PrepareMoviesListFromCSVUsingAttributes(pathToFile);
            var pageList = movieList.GetRange(indexOfFirstElement, countOfElements);
            var model = new Helpers().PrepareResponseModel(pageList, movieList.Count.ToString(), "True");

            return model;
        }
        
        public bool CheckPartOfTitleInResponse(string title, Root responseObject)
        {
            string[] titleSubstring = title.ToLower().Split(' ');
            //string[] titleSubstring = { "the", "hatefful" };

            List<Search> search = responseObject.Search;
            string[] responseTitles = search.Select(Search => Search.Title.ToLower()).ToArray();

            bool z = true;

            foreach (var item in responseTitles)
            {
                foreach (var substring in titleSubstring)
                {
                    z = (Array.Exists(responseTitles, x => item.Contains(substring)));

                    if (z != true)
                        break;
                }
                if (z != true)
                    break;
            }
            return z;
        }
    }
}
