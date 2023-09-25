// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using JetBrains.Annotations;

using Microsoft.CodeAnalysis.Diagnostics;

using System;
using System.Collections.Generic;
using System.Linq;

namespace JPSoftworks.CodeTools.Analyzers.Utils;

public static class EditorConfigHelper
{
    public static List<string> ReadCommaSeparatedList(AnalyzerConfigOptions options, string propertyName, List<string> defaultValue, string separator = ",")
    {
        if (options.TryGetValue(propertyName, out var propertyValue))
        {
            return propertyValue.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
        }
        return defaultValue;
    }

    public static HashSet<string> ReadCommaSeparatedSet(AnalyzerConfigOptions options, string propertyName, HashSet<string> defaultValue, string separator = ",")
    {
        if (options.TryGetValue(propertyName, out var propertyValue))
        {
            return new HashSet<string>(propertyValue.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()));
        }
        return defaultValue;
    }
}

public static class AnalyzerConfigOptionsExtensions
{
    [NotNull]
    public static OptionsReader GetReaderForSubsection(this AnalyzerConfigOptions options, string path)
    {
        return new OptionsReader(options, new[] { path });
    }

    [NotNull]
    public static OptionsReader GetReaderForSubsection(this AnalyzerConfigOptions options, string[] path)
    {
        return new OptionsReader(options, path);
    }
}

public class OptionsReader
{
    private readonly AnalyzerConfigOptions _options;
    private readonly string[] _rootParts;

    public OptionsReader([NotNull] AnalyzerConfigOptions options, [NotNull] string[] rootParts)
    {
        if (options is null)
            throw new ArgumentNullException(nameof(options));
        if (rootParts is null)
            throw new ArgumentNullException(nameof(rootParts));

        _options = options;
        _rootParts = rootParts;
    }

    [CanBeNull]
    public string GetString(string[] pathParts, string defaultValue)
    {
        if (_options.TryGetValue(BuildKey(pathParts), out var value))
        {
            return value;
        }

        return defaultValue;
    }

    [CanBeNull]
    [ContractAnnotation("defaultValue:notnull => notnull")]
    public ISet<string> GetStringSet(string path, IEnumerable<string> defaultValue, string separator = ",")
    {
        return GetStringSet(new[] { path }, defaultValue, separator);
    }

    [CanBeNull]
    [ContractAnnotation("defaultValue:notnull => notnull")]
    public ISet<string> GetStringSet(string[] pathParts, IEnumerable<string> defaultValue, string separator = ",")
    {
        var s = GetString(pathParts, null);
        if (string.IsNullOrWhiteSpace(s))
            return new HashSet<string>(defaultValue);
        return new HashSet<string>(s.Split(new[] { separator }, StringSplitOptions.None));
    }

    [NotNull]
    private string BuildKey(string[] pathParts)
    {
        return string.Join(".", _rootParts.Concat(pathParts));
    }
}