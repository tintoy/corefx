// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Resolvers;

namespace System.Security.Cryptography.Xml.Tests
{
    internal static class TestHelpers
    {
        public static XmlResolver StaticXmlResolver
        {
            get
            {
                return new TestXmlResolver
                {
                    Data =
                    {
                        ["doc.dtd"] = "<!-- presence required, not contents -->",
                        ["world.txt"] = "world"
                    }
                };
            }
        }

        public static TempFile CreateTestDtdFile(string testName)
        {
            if (testName == null)
                throw new ArgumentNullException(nameof(testName));

            var file = new TempFile(
                Path.Combine(Directory.GetCurrentDirectory(), testName + ".dtd")
            );

            File.WriteAllText(file.Path, "<!-- presence, not content, required -->");

            return file;
        }

        public static TempFile CreateTestTextFile(string testName, string content)
        {
            if (testName == null)
                throw new ArgumentNullException(nameof(testName));

            if (content == null)
                throw new ArgumentNullException(nameof(content));

            var file = new TempFile(
                Path.Combine(Directory.GetCurrentDirectory(), testName + ".txt")
            );

            File.WriteAllText(file.Path, content);

            return file;
        }

        class TestXmlResolver
            : XmlResolver
        {
            public Dictionary<string, string> Data { get; } = new Dictionary<string, string>();

            public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
            {
                string fileName = Path.GetFileName(absoluteUri.LocalPath);

                string data;
                if (!Data.TryGetValue(fileName, out data))
                    return null;

                if (ofObjectToReturn == typeof(Stream))
                {
                    return new MemoryStream(
                        Encoding.UTF8.GetBytes(data)
                    );
                }

                throw new ArgumentException($"Unexpected target type '{ofObjectToReturn.FullName}'.", nameof(ofObjectToReturn));
            }
        }
    }
}
