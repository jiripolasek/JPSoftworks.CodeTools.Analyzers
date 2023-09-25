// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using JPSoftworks.CodeTools.Analyzers.Shared;

using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Threading.Tasks;

using CodeFixVerifier = JPSoftworks.CodeTools.Analyzers.Test.CSharpCodeFixVerifier<
    JPSoftworks.CodeTools.Analyzers.NewGuidAnalyzer,
    JPSoftworks.CodeTools.Analyzers.NewGuidCodeFixProvider>;

namespace JPSoftworks.CodeTools.Analyzers.Test
{
    [TestClass]
    public class NewGuidAnalyzerUnitTests : ConventionsBasedUnitTestsBase
    {
        [TestMethod]
        public async Task ConvertsPrimaryCtorToGuidEmptyConst()
        {
            var test = await ReadTestDataTextAsync(@"0_Case_ParameterlessCtor.cs");
            var fixExpected = await ReadTestDataTextAsync(@"0_Fixed_Empty.cs");

            var expected = CodeFixVerifier
                .Diagnostic(WellKnownDiagnosticIds.JPX0001_NewGuidAnalyzer)
                .WithSpan(12, 18, 12, 28);
            await CodeFixVerifier.VerifyCodeFixAsync(test, expected,
                fixExpected, NewGuidCodeFixProvider.EmptyCodeActionKey);
        }

        [TestMethod]
        public async Task ConvertsPrimaryCtorToGuidFactoryMethod()
        {
            var test = await ReadTestDataTextAsync("0_Case_ParameterlessCtor.cs")!;
            var fixExpected = await ReadTestDataTextAsync("0_Fixed_NewGuid.cs")!;

            var expected = CodeFixVerifier
                .Diagnostic(WellKnownDiagnosticIds.JPX0001_NewGuidAnalyzer)
                .WithSpan(12, 18, 12, 28);
            await CodeFixVerifier.VerifyCodeFixAsync(test, expected,
                fixExpected, NewGuidCodeFixProvider.NewGuidCodeActionKey);
        }

        [TestMethod]
        public async Task ConvertsImplicitCtorToGuidFactoryMethod()
        {
            var test = await ReadTestDataTextAsync("0_Case_ImplicitCtor.cs")!;
            var fixExpected = await ReadTestDataTextAsync("0_Fixed_NewGuid.cs")!;

            var expected = CodeFixVerifier
                .Diagnostic(WellKnownDiagnosticIds.JPX0001_NewGuidAnalyzer)
                .WithSpan(12, 18, 12, 23);
            await CodeFixVerifier
                .VerifyCodeFixAsync(test, expected, fixExpected, NewGuidCodeFixProvider.NewGuidCodeActionKey);
        }

        [TestMethod]
        public async Task ConvertsImplicitCtorToGuidEmptyConstant()
        {
            var test = await ReadTestDataTextAsync("0_Case_ImplicitCtor.cs")!;
            var fixExpected = await ReadTestDataTextAsync("0_Fixed_Empty.cs")!;

            var expectedDiagnostics = CodeFixVerifier
                .Diagnostic(WellKnownDiagnosticIds.JPX0001_NewGuidAnalyzer)
                .WithSpan(12, 18, 12, 23);
            await CodeFixVerifier
                .VerifyCodeFixAsync(test, expectedDiagnostics, fixExpected, NewGuidCodeFixProvider.EmptyCodeActionKey);
        }

        [TestMethod]
        public async Task ParametricCtorDoesNotTriggerDiagnostic()
        {
            var test = await ReadTestDataTextAsync("1_Case_ParametricCtor.cs")!;
            var fixExpected = await ReadTestDataTextAsync("1_Fixed.cs")!;

            await CodeFixVerifier.VerifyCodeFixAsync(
                test,
                Array.Empty<DiagnosticResult>(),
                fixExpected, null);
        }

        [TestMethod]
        public async Task GuidParseMethodDoesNotTriggerDiagnostic()
        {
            var test = await ReadTestDataTextAsync("2_Case_GuidParse.cs")!;
            var fixExpected = await ReadTestDataTextAsync("2_Fixed.cs")!;

            await CodeFixVerifier.VerifyCodeFixAsync(
                test,
                Array.Empty<DiagnosticResult>(),
                fixExpected, null);
        }

        [TestMethod]
        public async Task ConvertsDefaultExpressionToGuidEmpty()
        {
            var test = await ReadTestDataTextAsync(@"100_Case_DefaultExpression.cs");
            var fixExpected = await ReadTestDataTextAsync(@"0_Fixed_Empty.cs");

            var expected = CodeFixVerifier
                .Diagnostic(WellKnownDiagnosticIds.JPX0002_DefaultGuidAnalyzer)
                .WithSpan(12, 18, 12, 25);
            await CodeFixVerifier.VerifyCodeFixAsync(test, expected,
                fixExpected, NewGuidCodeFixProvider.EmptyCodeActionKey);
        }

        [TestMethod]
        public async Task ConvertsDefaultExpressionToGuidFactoryMethod()
        {
            var test = await ReadTestDataTextAsync(@"100_Case_DefaultExpression.cs");
            var fixExpected = await ReadTestDataTextAsync(@"0_Fixed_NewGuid.cs");

            var expected = CodeFixVerifier
                .Diagnostic(WellKnownDiagnosticIds.JPX0002_DefaultGuidAnalyzer)
                .WithSpan(12, 18, 12, 25);
            await CodeFixVerifier.VerifyCodeFixAsync(test, expected,
                fixExpected, NewGuidCodeFixProvider.NewGuidCodeActionKey);
        }

        [TestMethod]
        public async Task ConvertsDefaultExpressionToGuidEmptyConstant()
        {
            var test = await ReadTestDataTextAsync(@"100_Case_DefaultExpression.cs");
            var fixExpected = await ReadTestDataTextAsync(@"0_Fixed_Empty.cs");

            var expected = CodeFixVerifier
                .Diagnostic(WellKnownDiagnosticIds.JPX0002_DefaultGuidAnalyzer)
                .WithSpan(12, 18, 12, 25);
            await CodeFixVerifier.VerifyCodeFixAsync(test, expected,
                fixExpected, NewGuidCodeFixProvider.EmptyCodeActionKey);
        }
    }
}