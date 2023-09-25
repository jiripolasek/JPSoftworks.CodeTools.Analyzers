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
using Microsoft.CodeAnalysis.Editing;

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace JPSoftworks.CodeTools.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EventArgsCodeFixProvider)), Shared]
    public class EventArgsCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(WellKnownDiagnosticIds.JPX0003_EventArgsEmpty);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var objectCreation = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ObjectCreationExpressionSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    "Use EventArgs.Empty",
                    c => UseEventArgsEmptyAsync(context.Document, objectCreation, c),
                    nameof(EventArgsCodeFixProvider)),
                diagnostic);
        }


        private async Task<Document> UseEventArgsEmptyAsync(Document document, ObjectCreationExpressionSyntax objectCreation, System.Threading.CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken);

            var emptySyntax = SyntaxFactory.ParseExpression("EventArgs.Empty")
                .WithLeadingTrivia(objectCreation.GetLeadingTrivia())
                .WithTrailingTrivia(objectCreation.GetTrailingTrivia());

            editor.ReplaceNode(objectCreation, emptySyntax);

            return editor.GetChangedDocument();
        }
    }

}
