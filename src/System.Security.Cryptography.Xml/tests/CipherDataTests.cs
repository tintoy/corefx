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
    }
}
