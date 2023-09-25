// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using JetBrains.Annotations;

using JPSoftworks.CodeTools.Analyzers.Shared;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using System;
using System.Collections.Immutable;

namespace JPSoftworks.CodeTools.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
public class NewGuidAnalyzer : DiagnosticAnalyzer
{
    private const string Category = WellKnownCategories.Design;

    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager!, typeof(Resources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager!, typeof(Resources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager!, typeof(Resources));

    private static readonly DiagnosticDescriptor RuleCtor = new(WellKnownDiagnosticIds.JPX0001_NewGuidAnalyzer, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description);
    private static readonly DiagnosticDescriptor RuleDefault = new(WellKnownDiagnosticIds.JPX0002_DefaultGuidAnalyzer, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(RuleCtor, RuleDefault);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // detect
        // - new Guid() invocations (SyntaxKind.ObjectCreationExpression)
        // - Guid g = new() invocations
        context.RegisterSyntaxNodeAction(c => Analyze(c, RuleCtor), SyntaxKind.ObjectCreationExpression, SyntaxKind.ImplicitObjectCreationExpression);

        // detect
        // - default(Guid) invocations
        // - Guid g = default invocation
        context.RegisterSyntaxNodeAction(c => Analyze(c, RuleDefault), SyntaxKind.DefaultExpression, SyntaxKind.DefaultLiteralExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context, DiagnosticDescriptor rule)
    {
        var typeInfo = context.SemanticModel.GetTypeInfo(context.Node);
        if (typeInfo.Type is INamedTypeSymbol namedTypeSymbol)
        {
            AddErrorWhenGuid(context, namedTypeSymbol, rule, context.Node);
        }
    }

    private static void AddErrorWhenGuid(SyntaxNodeAnalysisContext context, INamedTypeSymbol namedTypeSymbol, DiagnosticDescriptor rule, SyntaxNode node)
    {
        if (rule == null)
            throw new ArgumentNullException(nameof(rule));
        if (node == null)
            throw new ArgumentNullException(nameof(node));

        var guidTypeSymbol = context.Compilation.GetTypeByMetadataName(typeof(Guid).FullName!);
        if (!SymbolEqualityComparer.Default.Equals(namedTypeSymbol, guidTypeSymbol))
            return;
        if (node is BaseObjectCreationExpressionSyntax { ArgumentList.Arguments.Count: > 0 })
            return;

        var diagnostic = Diagnostic.Create(rule, node.GetLocation());
        context.ReportDiagnostic(diagnostic);
    }
}