// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace System.Security.Cryptography.Xml.Tests
{
    /// <summary>
    ///     The base class for test suites that need temporary local files available for their tests.
    /// </summary>
    public abstract class TestWithFilesBase
        : IDisposable
    {
        private DirectoryInfo BaseDirectory { get; }

        private Dictionary<string, TempDirectory> TempDirectories { get; } = new Dictionary<string, TempDirectory>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="TestWithFilesBase"/> class.
        /// </summary>
        protected TestWithFilesBase()
        {
            BaseDirectory = new DirectoryInfo(
                Path.Combine(Directory.GetCurrentDirectory(), "TempFiles", GetType().Name)
            );
        }

        /// <summary>
        ///     Dispose of resources being used by the test suite.
        /// </summary>
        public void Dispose()
        {
            foreach (TempDirectory tempDirectory in TempDirectories.Values)
                tempDirectory.Dispose();

            TempDirectories.Clear();
        }

        /// <summary>
        ///     Enter the temporary directory for the current test.
        /// </summary>
        /// <param name="testMethodName">
        ///     The name of the current test method (optional; usually supplied by the compiler).
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> that, when disposed, returns to the previous directory.
        /// </returns>
        protected IDisposable EnterTempDirectory([CallerMemberName] string testMethodName = null)
        {
            return new PushDirectory(
                GetTempDirectory(testMethodName)
            );
        }

        /// <summary>
        ///     Get the temporary directory for the current test.
        /// </summary>
        /// <param name="testMethodName">
        ///     The name of the current test method (optional; usually supplied by the compiler).
        /// </param>
        /// <returns>
        ///     A <see cref="DirectoryInfo"/> representing the temporary directory.
        /// </returns>
        protected DirectoryInfo GetTempDirectory([CallerMemberName] string testMethodName = null)
        {
            TempDirectory tempDirectory;
            if (!TempDirectories.TryGetValue(testMethodName, out tempDirectory))
            {
                tempDirectory = new TempDirectory(
                    Path.Combine(BaseDirectory.FullName, testMethodName)
                );
                TempDirectories.Add(testMethodName, tempDirectory);
            }

            return new DirectoryInfo(tempDirectory.Path);
        }

        /// <summary>
        ///     Create a "doc.dtd" file for use in the current test.
        /// </summary>
        /// <param name="testMethodName">
        ///     The name of the current test method (optional; usually supplied by the compiler).
        /// </param>
        /// <returns>
        ///     A <see cref="FileInfo"/> representing doc.dtd.
        /// </returns>
        protected FileInfo CreateTestDtdFile([CallerMemberName] string testMethodName = null)
        {
            return CreateTempFile("doc.dtd", "<!-- presence, not content, required -->", testMethodName);
        }

        /// <summary>
        ///     Create a "world.txt" file for use in the current test.
        /// </summary>
        /// <param name="testMethodName">
        ///     The name of the current test method (optional; usually supplied by the compiler).
        /// </param>
        /// <returns>
        ///     A <see cref="FileInfo"/> representing world.txt.
        /// </returns>
        protected FileInfo CreateTestTextFile([CallerMemberName] string testMethodName = null)
        {
            return CreateTempFile("world.txt", "world", testMethodName);
        }

        /// <summary>
        ///     Create a temporary file.
        /// </summary>
        /// <param name="fileName">
        ///     The name of the file to create.
        /// </param>
        /// <param name="content">
        ///     Optional content for the file.
        /// 
        ///     If non-<c>null</c>, then the file will created with this content.
        /// </param>
        /// <param name="testMethodName">
        ///     The name of the current test method (optional; usually supplied by the compiler).
        /// </param>
        /// <returns>
        ///     A <see cref="FileInfo"/> representing the temporary file.
        /// </returns>
        /// <remarks>
        ///     The file will be deleted after the test has run.
        /// </remarks>
        protected FileInfo CreateTempFile(string fileName, string content = null, [CallerMemberName] string testMethodName = null)
        {
            FileInfo tempFile = CreateTempFileCore(testMethodName, fileName);
            
            if (content != null)
                File.WriteAllText(tempFile.FullName, content);

            return tempFile;
        }

        FileInfo CreateTempFileCore(string testMethodName, string fileName)
        {
            DirectoryInfo tempDirectory = GetTempDirectory(testMethodName);

            return new FileInfo(
                Path.Combine(tempDirectory.FullName, fileName)
            );
        }

        /// <summary>
        ///     Temporarily enters a directory until disposed.
        /// </summary>
        class PushDirectory
            : IDisposable
        {
            readonly string _originalDirectory;
            readonly string _targetDirectory;

            public PushDirectory(DirectoryInfo targetDirectory)
                : this(targetDirectory.FullName)
            {
            }

            public PushDirectory(string targetDirectory)
            {
                _originalDirectory = Directory.GetCurrentDirectory();
                _targetDirectory = targetDirectory;
            }

            public void Dispose()
            {
                Directory.SetCurrentDirectory(_originalDirectory);
            }
        }
    }
}
