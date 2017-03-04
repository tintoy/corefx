// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
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
                XmlPreloadedResolver resolver = new XmlPreloadedResolver();
                resolver.Add(
                    new Uri("doc.dtd", UriKind.Relative),
                    "<!-- presence required, not contents -->"
                );
                resolver.Add(
                    new Uri("world.txt", UriKind.Relative),
                    "world"
                );

                return resolver;
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
    }
}
