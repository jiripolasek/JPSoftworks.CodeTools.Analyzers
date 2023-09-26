//------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
//------------------------------------------------------------

using JetBrains.Annotations;

using JPSoftworks.CodeTools.Analyzers.Shared;
using JPSoftworks.CodeTools.Analyzers.Utils;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace JPSoftworks.CodeTools.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
public class EnforcedSuffixAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = WellKnownDiagnosticIds.JPX0101_EnforcedTypeNameSuffix;

    private static readonly LocalizableString Title = Resources.EnforcedSuffixAnalyzer_Title;
    private static readonly LocalizableString MessageFormat = Resources.EnforcedSuffixAnalyzer_MessageFormat;
    private static readonly LocalizableString Description = Resources.EnforcedSuffixAnalyzer_Description;

    private const string Category = WellKnownCategories.Naming;
    public const string RequiredSuffixPropertyKey = "RequiredSuffix";

    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

        context.RegisterCompilationStartAction(analysisContext =>
        {
            Debug.Assert(analysisContext != null, nameof(analysisContext) + " != null");

            // Initialize the map here
            var baseClassToSuffixMap = new Dictionary<string, List<string>?>
            {
                { "System.Exception", new List<string> { "Exception", "$base" } },
                { "System.Attribute", new List<string> { "Attr", "Attribute" } },
                { "System.EventArgs", null }
            };

            // Expand $base token
            foreach (var key in baseClassToSuffixMap.Keys)
            {
                Debug.Assert(key != null, nameof(key) + " != null");

                // 3. Remove invalid entries
                baseClassToSuffixMap[key] = baseClassToSuffixMap[key]?.Where(t => !string.IsNullOrWhiteSpace(t)).ToList();

                // 1. Empty list -> add the class itself
                // 2. Replace $base with the class name

                if (baseClassToSuffixMap[key] == null || baseClassToSuffixMap[key].Count == 0)
                {
                    baseClassToSuffixMap[key] = new List<string> { RoslynHelper.GetSimpleName(key) };
                }
                else
                {
                    baseClassToSuffixMap[key] = baseClassToSuffixMap[key]
                        .Select(suffix => suffix == "$base" ? RoslynHelper.GetSimpleName(key) : suffix)
                        .ToList();
                }

                // 4. Deduplicate
                baseClassToSuffixMap[key] = baseClassToSuffixMap[key].Distinct().ToList();
            }

            analysisContext.RegisterSymbolAction(c => AnalyzeSymbol(c, baseClassToSuffixMap), SymbolKind.NamedType);
        });
    }

    private void AnalyzeSymbol(SymbolAnalysisContext context, [NotNull] Dictionary<string, List<string>> baseClassToSuffixMap)
    {
        var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

        foreach (var baseClassRule in baseClassToSuffixMap)
        {
            if (!RoslynHelper.InheritsFrom(namedTypeSymbol, baseClassRule.Key))
            {
                continue;
            }

            bool hasValidSuffix = false;

            foreach (var suffix in baseClassRule.Value)
            {
                if (namedTypeSymbol.Name.EndsWith(suffix))
                {
                    hasValidSuffix = true;
                    break;
                }
            }

            if (!hasValidSuffix)
            {
                var allowedSuffixes = string.Join(", ", baseClassRule.Value);
                var expectedNames = string.Join(", ", baseClassRule.Value.Select(t => namedTypeSymbol.Name + t));
                var messageArgs = new object[]
                {
                    namedTypeSymbol.Name, // 0
                    allowedSuffixes, // 1
                    expectedNames, // 2
                    baseClassRule.Key //3
                };
                var properties = new Dictionary<string, string> { { RequiredSuffixPropertyKey, baseClassRule.Value.FirstOrDefault() } };
                var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0],
                    properties: properties.ToImmutableDictionary()!,
                    messageArgs: messageArgs);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}