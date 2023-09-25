// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;

namespace JPSoftworks.CodeTools.Analyzers.Test
{
    public abstract class ConventionsBasedUnitTestsBase
    {
        protected async Task<string> ReadTestDataTextAsync(string name, string group = null)
        {
            // Silence the exception analyzer for this. Not nice, but I don't want to pollute tests with exception handling
            // or warnings for these errors.

            // ReSharper disable ExceptionNotDocumented
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var path = Path.Combine("Data", group ?? GetType().Name.Replace("UnitTests", ""), name);
            if (!File.Exists(path))
                throw new FileNotFoundException($"Invalid test data path '{path}'.", path);

            return await File.ReadAllTextAsync(path)!;
            // ReSharper restore ExceptionNotDocumented
        }
    }
}