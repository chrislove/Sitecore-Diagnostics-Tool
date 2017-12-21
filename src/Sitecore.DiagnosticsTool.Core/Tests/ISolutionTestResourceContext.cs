﻿namespace Sitecore.DiagnosticsTool.Core.Tests
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.DiagnosticsTool.Core.Resources.Database;

  /// <summary>
  ///   The base context interface for providing access to solution-wide resources.
  /// </summary>
  public interface ISolutionTestResourceContext : IReadOnlyDictionary<string, ITestResourceContext>, ITestResourceContextBase
  {
    /// <summary>
    ///   System information.
    /// </summary>
    [NotNull]
    ISystemContext System { get; }
  }
}