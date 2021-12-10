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
        RestClient client;
        RestRequest request;
        JObject validResponse;
        JObject errorResponse;

        public RestRequest GETRequest()
        {
            var request = new RestRequest(Method.GET);

            request.AddHeader("x-rapidapi-host", "movie-database-imdb-alternative.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "a7a4498059mshb397be3ba3c8cffp1ed061jsnc41d2ee900b9");

            return request;
        }

        public List<Search> PrepareMoviesListFromSCV(string pathToFile)
        {
            var streamReader = new StreamReader($"{pathToFile}");
            var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
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

        public JObject PrepareResponseModelFromCSV(string pathToFile)
        {
            var movieList = new Helpers().PrepareMoviesListFromSCV(pathToFile);

            var model = new Helpers().PrepareResponseModel(movieList, movieList.Count.ToString(), "True");

            return model;
        }

        public JObject PrepareDifferentPageModel(string pathToFile, int indexOfFirstElement, int countOfElements)
        {
            var movieList = new Helpers().PrepareMoviesListFromSCV(pathToFile);

            var pageList = movieList.GetRange(indexOfFirstElement, countOfElements);  // перейменувати

            var model = new Helpers().PrepareResponseModel(pageList, movieList.Count.ToString(), "True");

            return model;
        }
    }
}
