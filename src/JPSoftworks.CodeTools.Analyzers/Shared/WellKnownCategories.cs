// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

namespace JPSoftworks.CodeTools.Analyzers.Shared;

/// <summary>
/// Contains well-known diagnostic rule categories.
/// </summary>
public static class WellKnownCategories
{
    /// <summary>
    /// Design category. Design rules support adherence to the Framework design guidelines.
    /// </summary>
    public const string Design = "Design";

    /// <summary>
    /// Naming category. Naming rules support adherence to the naming conventions of the .NET design guidelines.
    /// </summary>
    public const string Naming = "Naming";

    /// <summary>
    /// Usage category. Usage rules support proper usage of .NET.
    /// </summary>
    public const string Usage = "Usage";

    /// <summary>
    /// Style rules support consistent code style in your codebase. These rules start with the "IDE" prefix.
    /// </summary>
    public const string Style = "Style";
}