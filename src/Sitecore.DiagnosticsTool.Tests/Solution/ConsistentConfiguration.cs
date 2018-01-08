﻿namespace Sitecore.DiagnosticsTool.Tests.Solution
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.Core.Tests;

  [UsedImplicitly]
  public class ConsistentConfiguration : SolutionTest
  {
    protected const string ShortMessage = "One or several Sitecore instances in the solution have different sets of configuration files in App_Config folder's subfolders. It is recommended to have them idential across all Sitecore instances utilizing Configuration Rules engine";

    public override string Name { get; } = "Consistent configuration";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.Major >= 9;
    }

    protected override bool IsActual(ISolutionResourceContext data)
    {
      return data.Count > 1; // test only makes sense if there are several instances
    }

    public override void Process(ISolutionResourceContext data, ITestOutputContext output)
    {
      var files = new List<string>();
      var identical = CollectionHelper.AreIdenticalByPairs(
        data.Values.ToArray(),
        (instanceA, instanceB) =>

          // compare IncludeFiles of all instances to ensure they are identical
            CollectionHelper.AreIdentical(
              instanceA.SitecoreInfo.IncludeFiles,
              instanceB.SitecoreInfo.IncludeFiles,
              (nameA, nameB) => string.Equals(nameA, nameB, StringComparison.OrdinalIgnoreCase),
              (fileA, fileB) =>
              {
                if (fileA == null)
                {
                  Assert.IsNotNull(fileB);

                  files.Add(fileB.FilePath.Substring(fileB.FilePath.IndexOf("App_Config")));
                  return false;
                }

                if(fileB == null)
                {
                  Assert.IsNotNull(fileA);

                  files.Add(fileA.FilePath.Substring(fileA.FilePath.IndexOf("App_Config")));
                  return false;
                }

                if (!string.Equals(fileA.RawText, fileB.RawText, StringComparison.Ordinal))
                {
                  files.Add(fileA.FilePath.Substring(fileA.FilePath.IndexOf("App_Config")));
                  return false;
                }

                return true;
              }));

      if (!identical)
      {
        output.Warning(ShortMessage, detailed: new DetailedMessage(new BulletedList(files)));
      }
    }
  }
}