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
public class EventArgsAnalyzer : DiagnosticAnalyzer
{
    private const string Category = WellKnownCategories.Design;

    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.EventArgsAnalyzerTitle), Resources.ResourceManager!, typeof(Resources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.EventArgsAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.EventArgsAnalyzerDescription), Resources.ResourceManager, typeof(Resources));

    private static readonly DiagnosticDescriptor Rule = new(WellKnownDiagnosticIds.JPX0003_EventArgsEmpty, Title!, MessageFormat!, Category, DiagnosticSeverity.Warning, true, Description)!;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ObjectCreationExpression);
    }

    private void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var objectCreation = (ObjectCreationExpressionSyntax)context.Node;

        // Check if the instance created is of type EventArgs and has no initializer
        if (objectCreation.ArgumentList == null
            || objectCreation.ArgumentList.Arguments.Count != 0
            || objectCreation.Type.ToFullString().Trim() != nameof(EventArgs)
            || objectCreation.Initializer != null)
        {
            return;
        }

        var typeInfo = context.SemanticModel.GetTypeInfo(objectCreation);
        var typeSymbol = typeInfo.Type;
        var eventArgsSymbol = context.Compilation.GetTypeByMetadataName(typeof(EventArgs).FullName!);

        if (typeSymbol != null && SymbolEqualityComparer.Default.Equals(typeSymbol, eventArgsSymbol))
        {
            var diagnostic = Diagnostic.Create(Rule!, objectCreation.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}