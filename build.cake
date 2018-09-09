using System.Text.RegularExpressions;

#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var solution = "./Anywhere.ArcGIS.sln";

var version = "1.8.0";
var versionSuffix = Environment.GetEnvironmentVariable("VERSION_SUFFIX");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./src/Anywhere.ArcGIS/bin") + Directory(configuration);
var artifactDir = Directory("./artifacts");
var outputDir = artifactDir + Directory("output");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Build")
    .IsDependentOn("Update-Version")
    .Does(() =>
    {
        DotNetCoreBuild(
            solution,
            new DotNetCoreBuildSettings
            {
                Configuration = configuration,
                ArgumentCustomization = args => args.Append($"/p:CI=true  /v:n"),
                OutputDirectory = outputDir
            });
    });


Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
    CleanDirectory(artifactDir);
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetCoreRestore(solution);
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var projects = GetFiles("./tests/**/*.csproj");
        foreach(var project in projects)
        {
            DotNetCoreTool(
                projectPath: project.FullPath, 
                command: "xunit", 
                arguments: $"-configuration {configuration} -diagnostics -stoponfail"
            );
        }
    });

Task("Update-Version")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        Information("Setting version to " + version + versionSuffix);

        var file = GetFiles("./src/Anywhere.ArcGIS/Anywhere.ArcGIS.csproj").First();

        var project = System.IO.File.ReadAllText(file.FullPath, Encoding.UTF8);

        var projectVersion = new Regex(@"<Version>.+<\/Version>");

        project = projectVersion.Replace(project, string.Concat("<Version>", version + versionSuffix, "</Version>"));

        System.IO.File.WriteAllText(file.FullPath, project, Encoding.UTF8);
    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
