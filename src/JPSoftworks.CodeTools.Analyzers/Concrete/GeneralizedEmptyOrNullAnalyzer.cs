// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using JetBrains.Annotations;

using JPSoftworks.CodeTools.Analyzers.Shared;
using JPSoftworks.CodeTools.Analyzers.Utils;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using System;
using System.Collections.Immutable;
using System.Linq;

namespace JPSoftworks.CodeTools.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
public class GeneralizedEmptyOrNullAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = WellKnownDiagnosticIds.JPX0004_GeneralizedEmptyOrNullAnalyzer;

    private static readonly LocalizableString Title = Resources.GeneralizedEmptyOrNullAnalyzer_Title;
    private static readonly LocalizableString MessageFormat = Resources.GeneralizedEmptyOrNullAnalyzer_MessageFormat;
    private static readonly LocalizableString Description = Resources.GeneralizedEmptyOrNullAnalyzer_Description;

    private const string Category = WellKnownCategories.Usage;

    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title!, MessageFormat!, Category, DiagnosticSeverity.Info, true, Description!);

    private static string[] DefaultSet => new[] { "Null", "Empty" };

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ClassDeclaration);
    }

    private void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        // Get AnalyzerConfigOptionsProvider directly from the syntaxContext
        AnalyzerConfigOptionsProvider optionsProvider = context.Options.AnalyzerConfigOptionsProvider;
        AnalyzerConfigOptions options = optionsProvider.GetOptions(context.Node.SyntaxTree);

        var optionsReader = new OptionsReader(options, new[] { "jpx_rules.null_pattern" });
        var nullPatternNamesSet = optionsReader.GetStringSet("member_names", DefaultSet);
        var nullPatternCheckedTypes = optionsReader.GetStringSet("allowed_types", Array.Empty<string>());

        ClassDeclarationSyntax classDeclaration = (ClassDeclarationSyntax)context.Node;
        INamedTypeSymbol classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);

        if (!RoslynHelper.InheritsFrom(classSymbol, nullPatternCheckedTypes))
        {
            return;
        }

        // Check if parameterless constructor exists
        IMethodSymbol parameterlessConstructor = classSymbol?.InstanceConstructors.FirstOrDefault(c =>
            c!.Parameters.Length == 0 && c.DeclaredAccessibility == Accessibility.Public);
        if (parameterlessConstructor == null)
        {
            return;
        }

        // Check if Empty or Null static member exists
        ISymbol nullPatternMemberSymbol = classSymbol.GetMembers().FirstOrDefault(m => m is { IsStatic: true } && nullPatternNamesSet.Any(t => t == m.Name));
        if (nullPatternMemberSymbol == null)
        {
            var diagnostic = Diagnostic.Create(Rule!, classDeclaration.Identifier.GetLocation(), classSymbol.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}