// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace JPSoftworks.CodeTools.Analyzers.Shared;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class WellKnownDiagnosticIds
{
    public const string JPX0001_NewGuidAnalyzer = "JPX0001";
    public const string JPX0002_DefaultGuidAnalyzer = "JPX0002";

    public const string JPX0003_EventArgsEmpty = "JPX0003";

    public const string JPX0004_GeneralizedEmptyOrNullAnalyzer = "JPX0004";

    public const string JPX0101_EnforcedTypeNameSuffix = "JPX0101";
}