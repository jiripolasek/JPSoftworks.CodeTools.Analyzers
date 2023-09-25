// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;
using System.Linq;

namespace JPSoftworks.CodeTools.Analyzers.Utils;

public static class RoslynHelper
{
    /// <summary>
    /// Determines if a class symbol inherits from another class, either directly or indirectly.
    /// </summary>
    /// <param name="derivedClass">The derived class symbol.</param>
    /// <param name="baseClassName">The name of the base class to check.</param>
    /// <returns>True if the derived class inherits from the base class, otherwise false.</returns>
    public static bool InheritsFrom(INamedTypeSymbol derivedClass, string baseClassName)
    {
        if (derivedClass == null || string.IsNullOrEmpty(baseClassName))
        {
            return false;
        }

        INamedTypeSymbol currentBase = derivedClass.BaseType;

        while (currentBase != null)
        {
            if (currentBase.ToDisplayString() == baseClassName)
            {
                return true;
            }

            currentBase = currentBase.BaseType;
        }

        return false;
    }

    /// <exception cref="ArgumentNullException"><paramref name="baseClassNames"/> is <see langword="null"/></exception>
    public static bool InheritsFrom(INamedTypeSymbol derivedClass, IEnumerable<string> baseClassNames)
    {
        if (baseClassNames == null)
        {
            throw new ArgumentNullException(nameof(baseClassNames));
        }

        return baseClassNames.Any(baseClassName => InheritsFrom(derivedClass, baseClassName));
    }

    public static string? GetSimpleName(string fullyQualifiedName)
    {
        // Parse the string into a SyntaxNode
        var syntaxNode = SyntaxFactory.ParseName(fullyQualifiedName);

        // If the node is a QualifiedNameSyntax, you can get the rightmost identifier
        if (syntaxNode is QualifiedNameSyntax qualifiedName)
        {
            return qualifiedName.Right.Identifier.Text;
        }

        // If the node is an IdentifierNameSyntax, then the name is already simple
        if (syntaxNode is IdentifierNameSyntax identifierName)
        {
            return identifierName.Identifier.Text;
        }

        // If you end up here, the string might not have been a valid type name
        return null;
    }


}