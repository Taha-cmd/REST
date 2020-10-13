using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using REST;
using System.Net.Sockets;

namespace UnitTests
{
    public class HttpResponseTestClass
    {

        [SetUp]
        public void SetUp()
        {

        }


        [Test]
        public void TestHttpResponseHasNoNullValues()
        {
            HttpResponse response = new HttpResponse();


            Assert.IsNotEmpty(response.Protocol);
            Assert.IsNotEmpty(response.Status);
            Assert.IsNotEmpty(response.StatusMessage);

            Assert.IsNotNull(response.Protocol);
            Assert.IsNotNull(response.StatusMessage);
            Assert.IsNotNull(response.Status);

        }

       [Test]
        public void TestHttpResponseHeaderAbilityToOverrideValues()
        {
            HttpResponse response = new HttpResponse();

            response.AddHeader("Content-Type", "text");
            
            Assert.DoesNotThrow(() => response.AddHeader("Content-Type", "text/html"));

            Assert.AreEqual("text/html", response.Values["Content-Type"]);
        }

        [Test]
        public void Test()
        {
            Assert.Ignore();
        }


        

    }
}
