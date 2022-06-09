using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Wordle.Api.Controllers;
using Wordle.Api;
using System.Text.Json;

namespace Wordle.Api.Tests
{
    [TestClass]
    [TestCategory("Integration")]
    public class TokenIntegrationTests: IntegrationTestBase
    {
        /// <summary>
        /// test the controller with integration test harness
        /// </summary>
        [TestMethod]
        public async Task TokenGet()
        {
            HttpContent payload = new StringContent(
                @"{
                  ""email"": ""admin@intellitect.com"",
                  ""username"": ""admin@intellitect.com"",
                  ""password"": ""P@ssw0rd123""
                }", null, "application/json");

            var response = await Client.PostAsync("/token/CreateUser", payload);

            //Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);


            payload = new StringContent(
                @"{
                  ""email"": ""admin@intellitect.com"",
                  ""password"": ""P@ssw0rd123""
                }", null, "application/json");

            response = await Client.PostAsync("/token/GetToken", payload);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Length > 10);

            var json = JsonSerializer.Deserialize<TokenDto>(content);

            Assert.IsTrue(json.token.Length > 10);
            Assert.IsTrue(json.token.Split(".").Count() == 3);

        }
    }

    public class TokenDto
    {
        public string token { get; set; }
    }
}
