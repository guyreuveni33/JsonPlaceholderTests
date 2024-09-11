using NUnit.Framework;
using RestSharp;
using System;
using Newtonsoft.Json.Linq;

namespace JsonPlaceholderTests
{
    public class BlogPostTests
    {
        private RestClient client;

        [SetUp]
        public void Setup()
        {
            client = new RestClient("https://jsonplaceholder.typicode.com");
        }

        [Test]
        public void GetAllPosts_ShouldReturnListOfPosts()
        {
            var request = new RestRequest("/posts", Method.Get);
            var response = client.Execute(request);
            Assert.IsTrue(response.IsSuccessful);
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsNotEmpty(response.Content, "The response does not contain any posts.");
        }

        [Test]
        public void GetPostById_ShouldReturnPost_WhenIdIsValid()
        {
            var valid_request = new RestRequest("/posts/1");
            var valid_response = client.Execute(valid_request);
            var jsonResponse = JObject.Parse(valid_response.Content);
            Assert.IsNotNull(jsonResponse["userId"], "userId should not be null.");
            Assert.IsTrue(valid_response.IsSuccessful);
            Assert.AreEqual(200, (int)valid_response.StatusCode);

            // Invalid ID case
            var invalid_request = new RestRequest("/posts/999999", Method.Get); // Use a non-existing ID
            var invalid_response = client.Execute(invalid_request);
            Assert.AreEqual(404, (int)invalid_response.StatusCode, "Expected status code 404 for non-existent post.");
        }

        [Test]
        public void CreatePost_ShouldReturnCreatedPost()
        {
            var request = new RestRequest("/posts", Method.Post);
            request.AddJsonBody(new { userId = "101", title = "foo", body = "bar" });
            var response = client.Execute(request);

            // Assert: Verify the post was created successfully
            Assert.AreEqual(201, (int)response.StatusCode, "Expected status code 201 for resource creation.");
            Assert.IsTrue(response.Content.Contains("foo"), "The title of the created post is not correct.");
            Assert.IsTrue(response.Content.Contains("bar"), "The body of the created post is not correct.");
            Assert.IsTrue(response.Content.Contains("id"), "The response does not contain an id for the created post.");
        }
/* tests for a valid API that repond well for bad reuqests
        [Test]
        public void CreatePost_ShouldReturnBadRequest_WhenDataIsInvalid()
        {
            var request = new RestRequest("/posts", Method.Post);
            request.AddJsonBody(new { title = "", body = "", userId = "" }); // Invalid data
            var response = client.Execute(request);

            Assert.AreEqual(400, (int)response.StatusCode, "Expected status code 400 for bad request with invalid data.");
        }*/

        [Test]
        public void UpdatePost_ShouldReturnUpdatedPost_WhenIdIsValid()
        {
            var request = new RestRequest("/posts/1", Method.Put);
            request.AddJsonBody(new { body = "hello bar" });
            var response = client.Execute(request);

            Assert.IsTrue(response.IsSuccessful);
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsTrue(response.Content.Contains("hello bar"), "The post body was not updated correctly.");
        }

        [Test]
        public void UpdatePost_ShouldReturnNotFound_WhenIdIsInvalid()
        {
            var request = new RestRequest("/posts/999999", Method.Put); // Use a non-existing ID
            request.AddJsonBody(new { body = "hello bar" });
            var response = client.Execute(request);

            Assert.AreEqual(404, (int)response.StatusCode, "Expected status code 404 for updating a non-existent post.");
        }

        [Test]
        public void DeletePost_ShouldReturnOk_WhenIdIsValid()
        {
            var request = new RestRequest("/posts/1", Method.Delete);
            var response = client.Execute(request);

            Assert.IsTrue(response.IsSuccessful);
            Assert.AreEqual(200, (int)response.StatusCode);

            // Optionally check the post is actually deleted
            var getRequest = new RestRequest("/posts/1", Method.Get);
            var getResponse = client.Execute(getRequest);
            Assert.AreEqual(404, (int)getResponse.StatusCode, "The post was not deleted successfully.");
        }

        [Test]
        public void DeletePost_ShouldReturnNotFound_WhenIdIsInvalid()
        {
            var request = new RestRequest("/posts/999999", Method.Delete); // Use a non-existing ID
            var response = client.Execute(request);

            Assert.AreEqual(404, (int)response.StatusCode, "Expected status code 404 for deleting a non-existent post.");
        }

        [Test]
        public void GetAllPosts_ShouldRespondWithinTimeout()
        {
            var request = new RestRequest("/posts", Method.Get);
            var response = client.Execute(request);

            Assert.IsTrue(response.IsSuccessful);
        }

        [Test]
        public void GetPost_ShouldReturnJsonContentType()
        {
            var request = new RestRequest("/posts/1", Method.Get);
            var response = client.Execute(request);

            Assert.AreEqual("application/json; charset=utf-8", response.ContentType, "Expected content type to be JSON.");
        }

        [TearDown]
        public void Teardown()
        {
            client.Dispose();
        }
    }
}
