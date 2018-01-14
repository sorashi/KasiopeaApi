#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#tool "nuget:?package=GitVersion.CommandLine"

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var solution = "./KasiopeaApi.sln";
var buildDir = Directory("./KasiopeaApi/bin") + Directory(configuration);

var version = GitVersion();
var nugetResultPath = $"./nuget/KasiopeaApi.{version.NuGetVersion}.nupkg";

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
   // Executed BEFORE the first task.
});

Teardown(ctx =>
{
   // Executed AFTER the last task.
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////
Task("Clean")
    .Does(()=>{
        CleanDirectory(buildDir);
        CleanDirectory(Directory("./KasiopeaApi.Tests/bin") + Directory(configuration));
        CleanDirectory("./nuget");
        EnsureDirectoryExists("./KasiopeaApi/Properties");
    });
Task("NuGet-Restore")
    .Does(() => {
        NuGetRestore(solution);
    });
Task("Generate-AssemblyInfo")
    .Does(() => {
        var assemblyInfoPath = "./KasiopeaApi/Properties/AssemblyInfo.cs";
        CreateAssemblyInfo(assemblyInfoPath, new AssemblyInfoSettings {
            Title = "KasiopeaApi",
            Company = "sorashi",
            Product = "KasiopeaApi",
            Copyright = "Copyright © sorashi 2017",
            FileVersion = version.MajorMinorPatch,
            InformationalVersion = version.InformationalVersion,
            Version = version.MajorMinorPatch
        });
        if(AppVeyor.IsRunningOnAppVeyor) {
            // 0.1.2-alpha.12+develop.abcdef0
            AppVeyor.UpdateBuildVersion($"{version.SemVer}+{version.BranchName}.{version.Sha.Substring(0, 7)}");
        }
    });
Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("NuGet-Restore")
    .IsDependentOn("Generate-AssemblyInfo")
    .Does(()=>{
        if(IsRunningOnWindows()) {
            MSBuild(solution, settings => {
                    settings.SetConfiguration(configuration);
                    settings.SetVerbosity(Verbosity.Minimal);
                });
        }else{
            XBuild(solution, settings =>
                settings.SetConfiguration(configuration));
        }
    });
Task("Unit-Tests")
    .IsDependentOn("Build")
    .Does(() => {
        NUnit3("./**/bin/" + configuration + "/*.Tests.dll", new NUnit3Settings {
        NoResults = false});
        if(AppVeyor.IsRunningOnAppVeyor){
            AppVeyor.UploadTestResults("TestResult.xml", AppVeyorTestResultsType.NUnit3);
        }
    });
Task("NuGet-Pack")
    .IsDependentOn("Unit-Tests")
    .IsDependentOn("Clean")
    .Does(() => {
        var settings = new NuGetPackSettings {
            Id = "KasiopeaApi",
            Version = version.NuGetVersion,
            Title = "KasiopeaApi",
            Authors = new [] {"sorashi"},
            Owners = new [] {"sorashi"},
            ProjectUrl = new Uri(@"https://github.com/Sorashi/KasiopeaApi"),
            LicenseUrl = new Uri(@"https://rawgit.com/Sorashi/KasiopeaApi/master/LICENSE"),
            RequireLicenseAcceptance = false,
            Description = "An unoffical .NET web-scrapping API for the kasiopea.matfyz.cz competition.",
            Copyright = "Copyright sorashi 2018",
            Tags = new [] { "Kasiopea" },
            BasePath = buildDir,
            OutputDirectory = "./nuget",
            Files = new [] {
                new NuSpecContent { Source = "KasiopeaApi.dll", Target = "lib" },
                new NuSpecContent { Source = "LICENSE", Target = "Content/Licenses/LICENSE"}
            }
        };
        NuGetPack(settings);
    });
Task("NuGet-Publish")
    .IsDependentOn("NuGet-Pack")
    .Does(() => {
        var key = EnvironmentVariable("nuget_api_key");
        if(key == null) Error("nuget_api_key variable is missing in environment");
        NuGetPush(nugetResultPath, new NuGetPushSettings {
            Source = @"https://www.nuget.org/",
            ApiKey = key
         });
    });
Task("Default")
    .IsDependentOn("NuGet-Pack")
    .Does(() => {
        if(AppVeyor.IsRunningOnAppVeyor) {
            AppVeyor.UploadArtifact(nugetResultPath);
        }
    });

RunTarget(target);
