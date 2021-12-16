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
using System.Xml.Linq;

namespace MovieDatabaseAPITesting
{
    class TestTitleParameter
    {
        RestClient client;
        string url = "https://movie-database-imdb-alternative.p.rapidapi.com/";
        string host = "movie-database-imdb-alternative.p.rapidapi.com";
        string apiKey = "a7a4498059mshb397be3ba3c8cffp1ed061jsnc41d2ee900b9";

        
        [SetUp]
        public void Setup()
        {
            client = new RestClient($"{url}");
        }

        [Test]
        public void CheckResponseIgnoringCaseSensitivityInTitle()
        {
            // Given
            var request = new Helpers().GETRequest(host, apiKey);

            request.AddParameter("s", "the hateful eight");
            request.AddParameter("y", "2015");

            // When
            var response = client.Execute(request);
            JObject actual = JObject.Parse(response.Content);

            // Expected
            var expected = new Helpers().PrepareResponseModelFromJSON(@"..\..\..\ResponseModels\ValidResponse.json");

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(JToken.DeepEquals(expected, actual), "Title doesn't ignore the case anymore!");
        }

        [Test]
        public void CheckResponseForSearchingByAFewWords()
        {
            // Given
            var request = new Helpers().GETRequest(host, apiKey);

            string title = "The Hateful";
            request.AddParameter("s", $"{title}");

            // When
            var response = client.Execute(request);
            var responseObject = JsonConvert.DeserializeObject<Root>(response.Content);

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.IsTrue(new Helpers().CheckPartOfTitleInResponse(title, responseObject), "Search by a few separate words doesn't work!");
        }

        [Test]
        public void CheckResponseBySpecifyingTheWrongWordsOrderInTitle()
        {
            // Given
            var request = new Helpers().GETRequest(host, apiKey);

            string title = "Eight The Hateful";
            request.AddParameter("s", $"{title}");

            // When
            var response = client.Execute(request);
            JObject actual = JObject.Parse(response.Content);

            // Expected
            var expected = new Helpers().PrepareResponseModelFromJSON(@"..\..\..\ResponseModels\ValidResponse.json");

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(JToken.DeepEquals(expected, actual), "Title doesn't ignore the wrong order of words anymore!");
        }

        [Test]
        public void CheckSpeialCharactersInsteadOfSpacesBetweenWordsInTitle()
        {
            // Given
            var request = new Helpers().GETRequest(host, apiKey);

            string title = "The//Hateful-*Eight";
            request.AddParameter("s", $"{title}");

            // When
            var response = client.Execute(request);
            JObject actual = JObject.Parse(response.Content);

            // Expected
            var expected = new Helpers().PrepareResponseModelFromJSON(@"..\..\..\ResponseModels\ValidResponse.json");

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(JToken.DeepEquals(expected, actual), "Title doesn't ignore special characters instead of spaces between words anymore!");
        }

        [Test]
        public void GetErrorForUsingPartOfAKeywordInTheTitle()
        {
            // Given
            var request = new Helpers().GETRequest(host, apiKey);

            string title = "The Hatef";
            request.AddParameter("s", $"{title}");

            // When
            var response = client.Execute(request);
            JObject actual = JObject.Parse(response.Content);

            // Expected
            var expected = new Helpers().PrepareResponseModelFromJSON(@"..\..\..\ResponseModels\ErrorResponse.json");

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(JToken.DeepEquals(expected, actual), "Now the title can be determined by the part of the keyword!");
        }

        [Test]
        public void GetErrorForUsingAFewSpacesBetweenWordsInTheTitle()
        {
            // Given
            var request = new Helpers().GETRequest(host, apiKey);

            string title = "The Hateful  Eight";
            request.AddParameter("s", $"{title}");

            // When
            var response = client.Execute(request);
            JObject actual = JObject.Parse(response.Content);

            // Expected
            var expected = new Helpers().PrepareResponseModelFromJSON(@"..\..\..\ResponseModels\ErrorResponse.json");

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(JToken.DeepEquals(expected, actual), "Now the title can be determined when there are two or more spaces between words!");
        }

        [Test]
        public void GetErrorForUsingASpaceInTheBeginningOfTheTitle()
        {
            // Given
            var request = new Helpers().GETRequest(host, apiKey);

            string title = " Hateful";
            request.AddParameter("s", $"{title}");

            // When
            var response = client.Execute(request);
            JObject actual = JObject.Parse(response.Content);

            // Expected
            var expected = new Helpers().PrepareResponseModelFromJSON(@"..\..\..\ResponseModels\ErrorResponse.json");

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(JToken.DeepEquals(expected, actual), "Now the title can be determined when there's a space between first word!");
        }

        [Test]
        public void GetErrorForUsingASpaceInTheEndOfTheTitle()
        {
            // Given
            var request = new Helpers().GETRequest(host, apiKey);

            string title = "Hateful ";
            request.AddParameter("s", $"{title}");

            // When
            var response = client.Execute(request);
            JObject actual = JObject.Parse(response.Content);

            // Expected
            var expected = new Helpers().PrepareResponseModelFromJSON(@"..\..\..\ResponseModels\ErrorResponse.json");

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(JToken.DeepEquals(expected, actual), "Now the title can be determined when there's a space after last word!");
        }

        [Test]
        public void GetErrorForUsingNonValidTitle()
        {
            // Given
            var request = new Helpers().GETRequest(host, apiKey);
            request.AddParameter("s", "testdummydata");

            // When
            var response = client.Execute(request);
            JObject actual = JObject.Parse(response.Content);   // запитати Оксану, як писати When, куди писати actual

            // Expected
            var expected = new Helpers().PrepareResponseModelFromJSON(@"..\..\..\ResponseModels\ErrorResponse.json");

            // Then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusDescription, Is.EqualTo("OK"));
            Assert.That(JToken.DeepEquals(expected, actual), "Error JSON Model Is Not Correct");
        }
    }
}
