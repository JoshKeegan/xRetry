using System;

// ReSharper disable once CheckNamespace
// ReSharper disable once IdentifierTypo
namespace Xunit;

/// <summary>
///     Do not use.
///     Exists purely as a marker to replicate the exception thrown by Xunit.SkippableFact that Reqnroll.xUnit
///     makes use of. That way we can intercept the exception that is throwing without also having our own runtime
///     plugin, or adding a direct dependency on either of these other libraries.
/// </summary>
public class SkipException : Exception
{
}