Kasiopea API
===
[![GitHub (pre-)release](https://img.shields.io/github/release/Sorashi/KasiopeaApi/all.svg)](https://github.com/Sorashi/KasiopeaApi/releases/latest)

This is an unoffical .NET web-scrapping API for the [Kasiopea](https://kasiopea.matfyz.cz) competition.

## This API allows you to

- [x] Require the input open-data, post your answer and know the result (success/fail)
- [x] Read the competition tasks – name and description (even from the archive section)
- [x] Read the competition result list and iterate through it
- [x] Satisfy your OCD

## Get started

### Installation

Download the `.nupkg` from [Releases](https://github.com/Sorashi/KasiopeaApi/releases/latest) and use Package Manager Console from Visual Studio. Run `Install-Package "path-to-the.nupkg"`

![](https://user-images.githubusercontent.com/6270283/29934511-c7bf6f84-8e7b-11e7-9188-c54966a24d4e.png)

[comment]: # (Fallback image: https://a.doko.moe/awjfpp.png)

If you don't use Visual Studio, you either have to unpack the .nupkg (it's a zip), reference the DLL and install the [dependencies](#dependencies) yourself, or you need to find [another way](https://stackoverflow.com/questions/10240029/how-to-install-a-nuget-package-nupkg-file-locally).

*I don't want to redistribute the dependencies, since this software doesn't have any license yet. That's why I use a NuGet package.*

### Usage

The fastest way to get started is using the relative URL to the task you are solving. For example `/archiv/2016/doma/D/` refers to [this task](https://kasiopea.matfyz.cz/archiv/2016/doma/D/).

```csharp
var kasiopeaInterface = new KasiopeaInterface();
var loginSucceeded = await kasiopeaInterface.Login("email", "password");
var task = await KasiopeaTask.FromUrl("/archiv/2016/doma/D/");
var version = KasiopeaTask.InputVersion.Easy; // or InputVersion.Hard (hard input has more difficult constraints)
var reader = await task.GetInputReaderAsync(version, kasiopeaInterface);
var writer = task.GetOutputWriter();

// read the input with
reader.ReadLine();
// write the output with
writer.WriteLine();
// or clear the output and start fresh
task.ClearStreamWriter();

// post the output and get the KasiopeaTask.OutputCheckResult (Success, Fail, Timeout, MissingFile, Unknown)
var result = await task.PostOutputAsync(version, kasiopeaInterface);
if(result == KasiopeaTask.OutputCheckResult.Success)
	Console.WriteLine("Hurray");
else
	Console.WriteLine(result.ToString());
```

How to use await from the application entry-point?
```csharp
static void Main(){
	try{
		MainAsync().Wait();
	}
	catch(Exception e){
		while(e is AggregateException) e = e.InnerException;
		throw e;
	}
}
static async Task MainAsync(){
	// do your async stuff
}
```

## Dependencies

- [HtmlAgilityPack](https://www.nuget.org/packages/HtmlAgilityPack/)
- [RestSharp](https://www.nuget.org/packages/RestSharp/)