///////////////////////////////////////////////////////////////////////////////
// ADDINS
///////////////////////////////////////////////////////////////////////////////

#addin "nuget:?package=Polly&version=4.2.0"

///////////////////////////////////////////////////////////////////////////////
// USINGS
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using Polly;

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var platform = Argument("platform", "Any CPU");
var configuration = Argument("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// SOLUTIONS
///////////////////////////////////////////////////////////////////////////////

var solutions = GetFiles("./**/*.sln").ToList();
solutions.ForEach(solution => Information("Solution: {0}", solution));

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Restore-NuGet-Packages")
    .Does(() =>
{
    var maxRetryCount = 5;
    var toolTimeout = 1d;
    Policy
        .Handle<Exception>()
        .Retry(maxRetryCount, (exception, retryCount, context) => {
            if (retryCount == maxRetryCount)
            {
                throw exception;
            }
            else
            {
                Verbose("{0}", exception);
                toolTimeout+=0.5;
            }})
        .Execute(()=> {
            solutions.ForEach(solution => {
                Information("Restoring: {0}", solution);
                NuGetRestore(solution, new NuGetRestoreSettings {
                    ToolTimeout = TimeSpan.FromMinutes(toolTimeout)
                });
            });

        });
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    solutions.ForEach(solution => {
        Information("Building: {0}", solution);
        MSBuild(solution, settings => {
            settings.SetConfiguration(configuration);
            settings.WithProperty("Platform", "\"" + platform + "\"");
            settings.SetVerbosity(Verbosity.Minimal);
        });
    });
});

///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
  .IsDependentOn("Build");

///////////////////////////////////////////////////////////////////////////////
// EXECUTE
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);
