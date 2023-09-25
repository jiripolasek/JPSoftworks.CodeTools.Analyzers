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
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPSoftworks.CodeTools.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnforcedSuffixAnalyzerCodeFix))]
[Shared]
public class EnforcedSuffixAnalyzerCodeFix : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(WellKnownDiagnosticIds.JPX0101_EnforcedTypeNameSuffix);

    public sealed override FixAllProvider GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        Diagnostic diagnostic = context.Diagnostics.First();
        TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

        // Find the type declaration identified by the diagnostic.
        TypeDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf()
            .OfType<TypeDeclarationSyntax>().First();

        context.RegisterCodeFix(
            CodeAction.Create(
                CodeFixResources.AddEnforcedSuffixCodeAction!,
                c => AddRequiredSuffixAsync(context.Document, declaration, diagnostic, c),
                nameof(EnforcedSuffixAnalyzerCodeFix)),
            diagnostic);
    }


    private async Task<Solution> AddRequiredSuffixAsync(Document document, TypeDeclarationSyntax typeDecl,
        Diagnostic diagnostic, CancellationToken cancellationToken)
    {
        // Fetch the required suffix from the diagnostic properties
        string requiredSuffix = diagnostic.Properties[EnforcedSuffixAnalyzer.RequiredSuffixPropertyKey];

        // Compute new name with required suffix
        SyntaxToken identifierToken = typeDecl.Identifier;
        string newName = identifierToken.Text + requiredSuffix;

        // Get the symbol representing the type to be renamed.
        SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken);
        INamedTypeSymbol typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl, cancellationToken);

        // Produce a new solution that has all references to that type renamed, including the declaration.
        Solution originalSolution = document.Project.Solution;
        OptionSet optionSet = originalSolution.Workspace.Options;
        Solution newSolution = await Renamer
            .RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken)
            .ConfigureAwait(false);

        return newSolution;
    }
}