// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using JPSoftworks.CodeTools.Analyzers.Shared;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPSoftworks.CodeTools.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NewGuidCodeFixProvider))]
    [Shared]
    public class NewGuidCodeFixProvider : CodeFixProvider
    {
        public const string NewGuidCodeActionKey = "NewGuid";
        public const string EmptyCodeActionKey = "Empty";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
                WellKnownDiagnosticIds.JPX0001_NewGuidAnalyzer,
                WellKnownDiagnosticIds.JPX0002_DefaultGuidAnalyzer);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic:
            // JPX0001
            // - ObjectCreationExpressionSyntax (new Guid())
            // - ImplicitObjectCreationExpressionSyntax (new())
            // JPX0002
            // - DefaultExpressionSyntax (default(Guid))
            // - LiteralExpressionSyntax (default)

            var ancestorsAndSelf = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().ToList();
            var declaration = ancestorsAndSelf
                                  .OfType<BaseObjectCreationExpressionSyntax>().FirstOrDefault()
                              ?? ancestorsAndSelf
                                  .OfType<ExpressionSyntax>().FirstOrDefault();

            if (declaration == null)
                return;

            // Register a code action that will invoke the fix.
            Debug.Assert(CodeFixResources.UseGuidEmpty != null, "CodeFixResources.UseGuidEmpty != null");
            Debug.Assert(CodeFixResources.UseGuidNewGuid != null, "CodeFixResources.UseGuidNewGuid != null");

            context.RegisterCodeFix(
                CodeAction.Create(
                    CodeFixResources.UseGuidEmpty,
                    c => ReplaceWithGuidEmpty(context.Document, declaration, c),
                    EmptyCodeActionKey),
                diagnostic);
            context.RegisterCodeFix(
                CodeAction.Create(
                    CodeFixResources.UseGuidNewGuid,
                    c => ReplaceWithGuidNewGuid(context.Document, declaration, c),
                    NewGuidCodeActionKey),
                diagnostic);
        }

        private static async Task<Document> ReplaceWithGuidEmpty(Document document, ExpressionSyntax invocation, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newRoot = root.ReplaceNode(invocation, SyntaxFactory.ParseExpression("Guid.Empty"));
            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> ReplaceWithGuidNewGuid(Document document, ExpressionSyntax invocation, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newRoot = root.ReplaceNode(invocation, SyntaxFactory.ParseExpression("Guid.NewGuid()"));
            return document.WithSyntaxRoot(newRoot);
        }
    }
}