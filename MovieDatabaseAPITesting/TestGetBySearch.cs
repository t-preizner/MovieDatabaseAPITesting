using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;

namespace MovieDatabaseAPITesting
{
    public class TestGetBySearch
    {
        RestClient client;
        RestRequest request;
        JObject validResponse;
        JObject errorResponse;

        [SetUp]
        public void Setup()
        {
            client = new RestClient("https://movie-database-imdb-alternative.p.rapidapi.com/");

            request = new RestRequest(Method.GET);

            request.AddHeader("x-rapidapi-host", "movie-database-imdb-alternative.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "a7a4498059mshb397be3ba3c8cffp1ed061jsnc41d2ee900b9");

            // parse ValidResponse.json to compare JSON response model
            StreamReader errorResponseRead = new StreamReader("C:/Users/tpreizner/Documents/test_projects/movie-database-api-tests/MovieDatabaseAPITesting/MovieDatabaseAPITesting/MovieDatabaseAPITesting/ResponseModels/ValidResponse.json");
            string errorJson = errorResponseRead.ReadToEnd();
            validResponse = JObject.Parse(errorJson);

            // parse ErrorResponse.json to compare JSON response model
            StreamReader validResponseRead = new StreamReader("C:/Users/tpreizner/Documents/test_projects/movie-database-api-tests/MovieDatabaseAPITesting/MovieDatabaseAPITesting/MovieDatabaseAPITesting/ResponseModels/ErrorResponse.json");
            string validJson = validResponseRead.ReadToEnd();
            errorResponse = JObject.Parse(validJson);
        }

        [Test]
        public void CheckRequestWithAllValidParameters()
        {
            // Given
            request.AddParameter("s", "The Hateful Eight");
            request.AddParameter("r", "json");
            request.AddParameter("type", "movie");
            request.AddParameter("y", "2015");
            request.AddParameter("page", "1");

            // When
            var response = client.Execute(request, Method.GET);
            JObject actual = JObject.Parse(response.Content);

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(JToken.DeepEquals(validResponse, actual), "JSON Model Is Not Correct");
        }

        [Test]
        public void CheckRequestWithOnlyRequiredParameters()
        {
            // Given
            request.AddParameter("s", "The Hateful Eight");

            // When
            var response = client.Execute(request, Method.GET);
            JObject actual = JObject.Parse(response.Content);

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(JToken.DeepEquals(validResponse, actual), "JSON Model Is Not Correct");
        }

        [Test]
        public void CheckRequestWithoutRequiredParameters()
        {
            // Given

            // When
            var response = client.Execute(request, Method.GET);
            var actual = JsonConvert.DeserializeObject(response.Content);

            // Expected
            var expected = "Incorrect IMDb ID.";

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(actual.ToString().Contains(expected));
        }

        [Test]
        public void CheckRequestWithTooManyResultsResponse()
        {
            // Given
            request.AddParameter("s", "The");

            // When
            var response = client.Execute(request, Method.GET);
            JObject actual = JObject.Parse(response.Content);

            // Expected 
            JObject expected = new JObject
            {
                { "Responce", "False"},
                { "Error", "Too many results."}
            };

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(JToken.DeepEquals(expected, actual));        // розібратися!!!!
        }

        // ============================== check title =====================================

        [Test]
        public void CheckRequestWithAPartOfTitle()
        {
            // Given
            request.AddParameter("s", "Eight");

            // When
            var response = client.Execute(request, Method.GET);
            var actual = JsonConvert.DeserializeObject<Root>(response.Content);

            // Expected
            var expected = "Eight";

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(actual.Search[1].Title.Contains(expected));
        }

        [Test]
        public void CheckRequestWithNonValidTitle()
        {
            // Given
            request.AddParameter("s", "testdummydata");

            // When
            var response = client.Execute(request, Method.GET);
            JObject actual = JObject.Parse(response.Content);

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(JToken.DeepEquals(errorResponse, actual), "JSON Model Is Not Correct");
        }

        // ============================== check data type =====================================

        [Test]
        public void CheckRequestWithJSONDataType()
        {
            // Given
            request.AddParameter("s", "The Hateful Eight");
            request.AddParameter("r", "json");

            // When
            var response = client.Execute(request, Method.GET);
            JObject actual = JObject.Parse(response.Content);

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(JToken.DeepEquals(validResponse, actual), "JSON Model Is Not Correct");
        }

        [Test]
        public void CheckRequestWithXMLDataType()
        {
            // Given
            request.AddParameter("s", "The Hateful Eight");
            request.AddParameter("r", "xml");

            // When
            var response = client.Execute(request, Method.GET);
            JObject actual = JObject.Parse(response.Content);

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(JToken.DeepEquals(validResponse, actual), "JSON Model Is Not Correct");
        }

        [Test]
        public void CheckRequestWithNonValidDataType()
        {
            // Given
            request.AddParameter("s", "The Hateful Eight");
            request.AddParameter("r", "test");

            // When
            var response = client.Execute(request, Method.GET);
            JObject actual = JObject.Parse(response.Content);

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(JToken.DeepEquals(validResponse, actual), "JSON Model Is Not Correct");
        }

        // ===================== check type = movie, series, episode =============================

        [TestCase("movie", "movie", TestName = "CheckMovie")]
        [TestCase("series", "series", TestName = "CheckSeries")]
        [TestCase("episode", "episode", TestName = "CheckEpisode")]
        public void CheckRequestForDifferentValidTypes(string movie, string type)
        {
            // Given
            request.AddParameter("s", "Man");
            request.AddParameter("type", movie);

            // When
            var response = client.Execute(request, Method.GET);
            var actual = JsonConvert.DeserializeObject<Root>(response.Content);

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(actual.Search[0].Type, Is.EqualTo(type));
        }

        [Test]
        public void CheckRequestWithNonValidTypeParameter()
        {
            // Given
            request.AddParameter("s", "The Hateful Eight");
            request.AddParameter("type", "test");

            // When
            var response = client.Execute(request, Method.GET);
            JObject actual = JObject.Parse(response.Content);

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status Code != 200");
            Assert.That(response.StatusDescription, Is.EqualTo("OK"), "Status Message != OK");
            Assert.That(JToken.DeepEquals(errorResponse, actual), "JSON Model Is Not Correct");
        }

        // ============================== check year =====================================


        [Test]
        public void CheckRequestForValidYear()
        {
            // Given
            request.AddParameter("s", "The Hateful Eight");
            request.AddParameter("y", "2015");

            // When
            var response = client.Execute(request, Method.GET);
            var actual = JsonConvert.DeserializeObject<Root>(response.Content);

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(actual.Search[0].Year, Is.EqualTo("2015"));
        }

        [Test]
        public void CheckRequestForNonValidYear()
        {
            // Given
            request.AddParameter("s", "The Hateful Eight");
            request.AddParameter("y", "2000");

            // When
            var response = client.Execute(request, Method.GET);
            JObject actual = JObject.Parse(response.Content);

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(JToken.DeepEquals(errorResponse, actual), "JSON Model Is Not Correct");
        }

        [Test]
        public void CheckRequestForNonValidDatainYearString()
        {
            // Given
            request.AddParameter("s", "The Hateful Eight");
            request.AddParameter("y", "test");

            // When
            var response = client.Execute(request, Method.GET);
            JObject actual = JObject.Parse(response.Content);

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(JToken.DeepEquals(validResponse, actual), "JSON Model Is Not Correct");
        }

        // ============================== check page =====================================

        [Test]
        public void CheckRequestForValidPageNumber()
        {
            // Given
            request.AddParameter("s", "Kill Bill");
            request.AddParameter("page", "2");

            // When
            var response = client.Execute(request, Method.GET);
            var actual = JsonConvert.DeserializeObject<Root>(response.Content);

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(actual.Search[0].Year, Is.EqualTo("2020"));
        }

        [Test]
        public void CheckRequestForPageNumberMoreThatLastOne()
        {
            // Given
            request.AddParameter("s", "Kill Bill");
            request.AddParameter("page", "3");

            // When
            var response = client.Execute(request, Method.GET);
            JObject actual = JObject.Parse(response.Content);

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(JToken.DeepEquals(errorResponse, actual), "JSON Model Is Not Correct");
        }

        [Test]
        public void CheckRequestForNonValidPageNumber()
        {
            // Given
            request.AddParameter("s", "Kill Bill");
            request.AddParameter("page", "test");

            // When
            var response = client.Execute(request, Method.GET);
            var actual = JsonConvert.DeserializeObject<Root>(response.Content);

            // Expected
            var expected = "Kill Bil";

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(actual.Search[0].Title.ToString().Contains(expected));
        }

        // ============================== check header parameters =====================================

        [Test]
        public void CheckNonValidApiKey()
        {
            // Given
            request.AddHeader("x-rapidapi-key", "test");
            request.AddParameter("s", "The Hateful Eight");


            // When
            var response = client.Execute(request, Method.GET);
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
            client = new RestClient("https://movie-database-imdb-alternative.p.rapidapi.com/");

            request = new RestRequest(Method.GET);

            request.AddHeader("x-rapidapi-host", "test-movie-database-imdb-alternative.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "a7a4498059mshb397be3ba3c8cffp1ed061jsnc41d2ee900b9");
            request.AddParameter("s", "The Hateful Eight");

            // When
            var response = client.Execute(request, Method.GET);
            JObject actual = JObject.Parse(response.Content);

            // Expected
            JObject expected = new JObject
            {
                { "message", "API doesn't exists"}
            };

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(response.StatusDescription, Is.EqualTo("Not Found"));
            Assert.That(JToken.DeepEquals(expected, actual), "HTTP Status Code != 404");
        }

        [Test]
        public void CheckNonValidHost1()
        {
            // Given
            client = new RestClient("https://movie-database-imdb-alternative.p.rapidapi.com/");

            request = new RestRequest(Method.GET);

            request.AddHeader("x-rapidapi-host", "test");
            request.AddHeader("x-rapidapi-key", "a7a4498059mshb397be3ba3c8cffp1ed061jsnc41d2ee900b9");
            request.AddParameter("s", "The Hateful Eight");

            // When
            var response = client.Execute(request, Method.GET);
            JObject actual = JObject.Parse(response.Content);

            // Expected
            JObject expected = new JObject
            {
                { "message", "The host you've provided is invalid. If you have difficulties, contact the RapidAPI support team, support@rapidapi.com"}
            };

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.StatusDescription, Is.EqualTo("Bad Request"));
            Assert.That(JToken.DeepEquals(expected, actual), "HTTP Status Code != 400"); // розібратися
        }
    }
}
