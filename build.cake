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

Information("Searching for solutions:");
var solutions = GetFiles("./**/*.sln").ToList();
int counter = 0;
solutions.ForEach(solution => Information("[{1}] Solution: {0}", solution, counter++));
Information("Found {0} solutions.", counter);

int restoreErrors = 0;
int buildErrors = 0;

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
                try
                {
                    Information("Restoring: {0}", solution);
                    NuGetRestore(solution, new NuGetRestoreSettings {
                        ToolTimeout = TimeSpan.FromMinutes(toolTimeout)
                    });
                }
                catch (Exception)
                {
                    Information("Failed to restore: {0}", solution);
                    restoreErrors++;
                }
            });

        });
})
.Finally(() =>
{  
    Information("Failed to restore {0} solutions.", restoreErrors);
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
        solutions.ForEach(solution => {
            try
            {
                Information("Building: {0}", solution);
                MSBuild(solution, settings => {
                    settings.SetConfiguration(configuration);
                    settings.WithProperty("Platform", "\"" + platform + "\"");
                    settings.SetVerbosity(Verbosity.Minimal);
                });
            }
            catch (Exception)
            {
                Information("Failed to build: {0}", solution);
                buildErrors++;
            }
        });
    }
    else
    {
        solutions.ForEach(solution => {
            try
            {
                Information("Building: {0}", solution);
                XBuild(solution, settings => {
                    settings.SetConfiguration(configuration);
                    settings.WithProperty("Platform", "\"" + platform + "\"");
                    settings.SetVerbosity(Verbosity.Minimal);
                });
            }
            catch (Exception)
            {
                Information("Failed to build: {0}", solution);
                buildErrors++;
            }
        });
    }
})
.Finally(() =>
{
    Information("Failed to build {0} solutions.", buildErrors);
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
