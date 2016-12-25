using System;
using Xunit;

namespace System.Security.Cryptography.Xml.Tests
{
    public class SignedXmlTests
    {
        [Fact]
        public void Ctor_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new SignedXml(null));
        }
    }
}