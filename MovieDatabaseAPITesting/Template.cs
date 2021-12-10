using System;
using System.Collections.Generic;
using NUnit.Framework;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serialization.Json;
using RestSharp.Serializers.NewtonsoftJson;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using RestSharp.Authenticators;
using System.IO;
using System.Net;
using Newtonsoft.Json.Schema;
using CsvHelper;

namespace MovieDatabaseAPITesting
{
    public class Template
    {
        RestClient client;
        RestRequest request;

        [SetUp]
        public void Setup()
        {
            client = new RestClient("https://movie-database-imdb-alternative.p.rapidapi.com/");

            request = new RestRequest(Method.GET);

            request.AddHeader("x-rapidapi-host", "movie-database-imdb-alternative.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "a7a4498059mshb397be3ba3c8cffp1ed061jsnc41d2ee900b9");

            request.AddParameter("s", "test");
        }

        [Test]
        public void Test()
        {
            request.AddParameter("year", "2019");

            // get full URL
            var fullURL = client.BuildUri(request);

            var response = client.Execute(request, Method.GET);

            // convert to MovieDatabaseAPITesting.Root object
            var test = JsonConvert.DeserializeObject<Root>(response.Content);

            // convert to Newtonsoft.Json.Linq.JObject
            var test1 = JsonConvert.DeserializeObject(response.Content);

            // convert to RestSharp.Serialization.Json.JsonDeserializer
            var deserialize = new JsonDeserializer();
            var outputDictionary = deserialize.Deserialize<Dictionary<string, string>>(response);    // Dictionary<string, string>
            var outputObject = deserialize.Deserialize<Root>(response);                              // Root

            // read JSON from file + parse to JObject
            StreamReader r = new StreamReader(@"..\..\..\ResponseModels\ErrorResponse.json");
            string json = r.ReadToEnd();
            JObject expected = JObject.Parse(json);
            JObject actual = JObject.Parse(response.Content);

            // check response DeepEquals
            Assert.That(JToken.DeepEquals(expected, actual), "JSON Model Is Not Correct");
        }

        [Test]
        public void Test1()
        {
            /*
            client = new RestClient("https://movie-database-imdb-alternative.p.rapidapi.com/");

            request = new RestRequest(Method.GET);

            request.AddHeader("x-rapidapi-host", "movie-database-imdb-alternative.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "a7a4498059mshb397be3ba3c8cffp1ed061jsnc41d2ee900b9");
            */
            // parse ValidResponse.json to compare JSON response model
            StreamReader validResponseRead = new StreamReader(@"..\..\..\ResponseModels\ValidResponseModel.json");
            string validJson = validResponseRead.ReadToEnd();
            var validResponse = JToken.Parse(validJson);

            var test = validResponse;
            Console.WriteLine(test);

        }

        [Test]
        public  void Test2()
        {
            client = new RestClient("https://movie-database-imdb-alternative.p.rapidapi.com/");

            request = new RestRequest(Method.GET);

            request.AddHeader("x-rapidapi-host", "movie-database-imdb-alternative.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "a7a4498059mshb397be3ba3c8cffp1ed061jsnc41d2ee900b9");

            // Given
            request.AddParameter("s", "The Hateful Eight");
            request.AddParameter("r", "json");
            request.AddParameter("type", "movie");
            request.AddParameter("y", "2015");
            request.AddParameter("page", "1");

            // When
            var response = client.Execute(request, Method.GET);
            JObject actual = JObject.Parse(response.Content);

            StreamReader validResponseRead = new StreamReader(@"..\..\..\ResponseModels\ValidResponse.json");
            string validJson = validResponseRead.ReadToEnd();
            var validResponse = JToken.Parse(validJson);

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(JToken.DeepEquals(validResponse, actual), "JSON Model Is Not Correct");
        }

        [Test]
        public void CheckJSONSchema()
        {
            // Given
            var request = new Helpers().GETRequest();

            request.AddParameter("s", "Kill Bill");
            request.AddParameter("r", "json");
            request.AddParameter("type", "movie");
            request.AddParameter("y", "2003");
            request.AddParameter("page", "1");

            // When
            var response = client.Execute(request);
            JObject actual = JObject.Parse(response.Content);

            // Expected
            var schema = JsonSchema.Parse(@"{
            'type': 'object',
            'properties': {
                'hobbies': {
                    'type': 'object',
                    'properties': {
                        'Title': { 'type': 'string' },
                        'Year': { 'type': 'string' },
                        'imdbID': { 'type': 'string' },
                        'Type': { 'type': 'string' },
                        'Poster': { 'type': 'string' }
                        },
                'totalResults': { 'type': 'string' },
                'Response': { 'type': 'string' }
                },
            }}");

            // Then
            Assert.IsTrue(actual.IsValid(schema));
        }
    }
}