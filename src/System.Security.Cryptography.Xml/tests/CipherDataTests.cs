// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xunit;
using Xunit.Extensions;

namespace System.Security.Cryptography.Xml.Tests
{
    public class CipherDataTests
    {
        [Fact]
        public void Constructor_Empty()
        {
            CipherData cipherData = new CipherData();

            Assert.Null(cipherData.CipherReference);
            Assert.Null(cipherData.CipherValue);
            Assert.Throws<CryptographicException>(() => cipherData.GetXml());
        }

        [Fact]
        public void Constructor_CipherValue_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new CipherData((byte[]) null));
        }

        [Theory]
        [InlineData(new byte[0])]
        [InlineData(new byte[] { 1, 2, 3})]
        public void Constructor_CipherValue(byte[] cipherValue)
        {
            CipherData cipherData = new CipherData(cipherValue);

            Assert.Equal(cipherValue, cipherData.CipherValue);
            Assert.Null(cipherData.CipherReference);

            XmlElement xmlElement = cipherData.GetXml();
            Assert.NotNull(xmlElement);
            Assert.Equal(
                string.Format(
                    "<CipherData xmlns=\"http://www.w3.org/2001/04/xmlenc#\"><CipherValue>{0}</CipherValue></CipherData>", 
                    Convert.ToBase64String(cipherValue)), 
                xmlElement.OuterXml);
        }

        [Fact]
        public void Constructor_CipherReference_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new CipherData((CipherReference) null));
        }

        [Theory]
        [MemberData(nameof(Constructor_CipherReference_Source))]
        public void Constructor_CipherReference(CipherReference cipherReference)
        {
            CipherData cipherData = new CipherData(cipherReference);

            Assert.Null(cipherData.CipherValue);
            Assert.Equal(cipherReference, cipherData.CipherReference);

            XmlElement xmlElement = cipherData.GetXml();
            Assert.NotNull(xmlElement);
            if (cipherReference.Uri != string.Empty)
            {
                Assert.Equal(
                    string.Format(
                        "<CipherData xmlns=\"http://www.w3.org/2001/04/xmlenc#\"><CipherReference URI=\"{0}\" /></CipherData>",
                        cipherReference.Uri),
                    xmlElement.OuterXml);
            }
            else 
            {
                Assert.Equal(
                    "<CipherData xmlns=\"http://www.w3.org/2001/04/xmlenc#\"><CipherReference /></CipherData>",
                    xmlElement.OuterXml);
            }
        }

        public static IEnumerable<object[]> Constructor_CipherReference_Source()
        {
            return new object[][]
            {
                new [] { new CipherReference() },
                new [] { new CipherReference("http://dummy.urionly.io") },
                new [] { new CipherReference("http://dummy.uri.transform.io", new TransformChain()) },
            };
        }

        [Fact]
        public void CipherReference_Null()
        {
            CipherData cipherData = new CipherData();
            Assert.Throws<ArgumentNullException>(() => cipherData.CipherReference = null);
        }

        [Fact]
        public void CipherReference_CipherValueSet()
        {
            CipherData cipherData = new CipherData(new byte[0]);
            Assert.Throws<CryptographicException>(() => cipherData.CipherReference = new CipherReference());
        }

        [Fact]
        public void CipherValue_Null()
        {
            CipherData cipherData = new CipherData(new CipherReference());
            Assert.Throws<ArgumentNullException>(() => cipherData.CipherValue = null);
        }

        [Fact]
        public void CipherValue_CipherReferenceSet()
        {
            CipherData cipherData = new CipherData(new CipherReference());
            Assert.Throws<CryptographicException>(() => cipherData.CipherValue = new byte[0]);
        }

        [Fact]
        public void LoadXml_Null()
        {
            CipherData cipherData = new CipherData();
            Assert.Throws<ArgumentNullException>(() => cipherData.LoadXml(null));
        }
    }
}
