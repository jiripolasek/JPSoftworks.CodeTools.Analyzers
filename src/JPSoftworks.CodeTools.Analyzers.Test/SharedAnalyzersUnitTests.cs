// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

using VerifyCS = JPSoftworks.CodeTools.Analyzers.Test.CSharpCodeFixVerifier<
    JPSoftworks.CodeTools.Analyzers.NewGuidAnalyzer,
    JPSoftworks.CodeTools.Analyzers.NewGuidCodeFixProvider>;

namespace JPSoftworks.CodeTools.Analyzers.Test
{
    [TestClass]
    public class JPSoftworksCodeToolsAnalyzersUnitTest
    {
        //No diagnostics expected to show up
        [TestMethod]
        public async Task NothingToTest()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}