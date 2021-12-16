using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MovieDatabaseAPITesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NUnit;
using NUnit.Framework;
using RestSharp;

namespace MovieDatabaseAPITesting
{
    class TestAPIUsingHelpers
    {
        string url = "https://movie-database-imdb-alternative.p.rapidapi.com/";
        string host = "movie-database-imdb-alternative.p.rapidapi.com";
        string apiKey = "a7a4498059mshb397be3ba3c8cffp1ed061jsnc41d2ee900b9";
        RestClient client;
        
        [SetUp]
        public void Setup()
        {
            client = new RestClient($"{url}");
        }

        [Test]
        public void UseDataInExpectedResult()
        {
            // Given
            var request = new Helpers().GETRequest(host, apiKey);

            request.AddParameter("s", "Kill Bill");
            request.AddParameter("r", "json");
            request.AddParameter("type", "movie");
            request.AddParameter("y", "2003");
            request.AddParameter("page", "1");

            // When
            var response = client.Execute(request);
            JObject actual = JObject.Parse(response.Content);
            
            // Expected
            var killBill1 = new Search
                ("Kill Bill: Vol. 1", 
                "2003", 
                "tt0266697", 
                "movie", 
                "https://m.media-amazon.com/images/M/MV5BNzM3NDFhYTAtYmU5Mi00NGRmLTljYjgtMDkyODQ4MjNkMGY2XkEyXkFqcGdeQXVyNzkwMjQ5NzM@._V1_SX300.jpg");
            var killBill2 = new Search
                ("The Making of 'Kill Bill'", 
                "2003", 
                "tt0412956", 
                "movie", 
                "https://m.media-amazon.com/images/M/MV5BY2VmNThlMzEtMWI5My00ZDJkLTkwMGEtYWRlMmJjNDAzMWQzXkEyXkFqcGdeQXVyMTMyNTQwNTg@._V1_SX300.jpg");
            
            List<Search> movieList = new List<Search> { killBill1, killBill2 };

            var expected = new Helpers().PrepareResponseModel(movieList, movieList.Count.ToString(), "True");

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(JToken.DeepEquals(expected, actual), "JSON Model Is Not Correct");
        }

        [Test]
        public void UseDataFromCSV()
        {
            // Given
            var request = new Helpers().GETRequest(host, apiKey);

            request.AddParameter("s", "Kill Bill");
            request.AddParameter("r", "json");
            request.AddParameter("type", "movie");
            request.AddParameter("y", "2003");
            request.AddParameter("page", "1");

            // When
            var response = client.Execute(request);

            JObject actual = JObject.Parse(response.Content);

            // Expected
            string pathToFile = @"..\..\..\csvTestData\TestData.csv";

            var expected = new Helpers().PrepareResponseModelFromCSVUsingMapping(pathToFile);

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(JToken.DeepEquals(expected, actual), "JSON Model Is Not Correct");
        }

        [Test]
        public void UseDataFromCSVUsingMapping()
        {
            // Given
            var request = new Helpers().GETRequest(host, apiKey);

            request.AddParameter("s", "Kill Bill");
            request.AddParameter("r", "json");
            request.AddParameter("type", "movie");
            request.AddParameter("y", "2003");
            request.AddParameter("page", "1");

            // When
            var response = client.Execute(request);
            JObject actual = JObject.Parse(response.Content);

            // Expected
            string pathToFile = @"..\..\..\csvTestData\TestData.csv";

            var expected = new Helpers().PrepareResponseModelFromCSVUsingMapping(pathToFile);

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(JToken.DeepEquals(expected, actual), "JSON Model Is Not Correct");
        }

        [Test]
        public void CheckPagination()
        {
            // Given
            var request = new Helpers().GETRequest(host, apiKey);

            request.AddParameter("s", "Pulp Fiction");
            request.AddParameter("page", "2");

            // When
            var response = client.Execute(request);
            JObject actual = JObject.Parse(response.Content);

            // Expected
            string pathToFile = @"..\..\..\csvTestData\DataForPaginationTesting.csv";

            var expected = new Helpers().PrepareSeparatePageModelFromCSV(pathToFile, 10, 7);

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(JToken.DeepEquals(expected, actual), "JSON Model Is Not Correct");
        }

        [Test]
        public void CheckNonValidApiKey()
        {
            // Given
            var request = new RestRequest(Method.GET);                      // уточнити

            request.AddHeader("x-rapidapi-host", "movie-database-imdb-alternative.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "test");
            request.AddParameter("s", "The Hateful Eight");

            // When
            var response = client.Execute(request);
            JObject actual = JObject.Parse(response.Content);

            // Expected
            JObject expected = new JObject
            {
                { "message", "You are not subscribed to this API."}
            };

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            Assert.That(response.StatusDescription, Is.EqualTo("Forbidden"));
            Assert.That(JToken.DeepEquals(expected, actual), "HTTP Status Code != 403");
        }

        [Test]
        public void CheckNonValidHost()
        {
            // Given
            var request = new RestRequest(Method.GET);                      // уточнити

            request.AddHeader("x-rapidapi-host", "test");
            request.AddHeader("x-rapidapi-key", "a7a4498059mshb397be3ba3c8cffp1ed061jsnc41d2ee900b9");
                                   
            request.AddParameter("s", "The Hateful Eight");

            // When
            var response = client.Execute(request);
            JObject actual = JObject.Parse(response.Content);

            // Expected
            JObject expected = new JObject
            {
                { "messages", "The host you've provided is invalid. If you have difficulties, contact the RapidAPI support team, support@rapidapi.com"}
            };

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.StatusDescription, Is.EqualTo("Bad Request"));
            Assert.That(JToken.DeepEquals(expected, actual), "HTTP Status Code != 400");
        }
    }
}
