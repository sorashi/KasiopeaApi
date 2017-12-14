Kasiopea API
===
[![GitHub (pre-)release](https://img.shields.io/github/release/Sorashi/KasiopeaApi/all.svg)](https://github.com/Sorashi/KasiopeaApi/releases/latest)
[![AppVeyor branch](https://img.shields.io/appveyor/ci/Sorashi/KasiopeaApi/master.svg)](https://ci.appveyor.com/project/Sorashi/kasiopeaapi) [CZ](./README.md)/[EN](./README-en.md)

This is an unoffical .NET web-scrapping API for the [Kasiopea](https://kasiopea.matfyz.cz) competition.

> `Install-Package KasiopeaApi -IncludePrerelease`

## This API allows you to

- [x] Require the input open-data, post your answer and know the result (success/fail)
- [x] Read the competition tasks – name and description (even from the archive section)
- [x] Read the competition result list and iterate through it
- [x] Satisfy your OCD

## Get started

### Installation

Use Package Manager Console from Visual Studio. Run `Install-Package KasiopeaApi -IncludePrerelease`

Since we're using [SemVer 2.0.0](https://semver.org/spec/v2.0.0.html), you need at least Visual Studio 2017 (version 15.3) or NuGet client 4.3.0 or above.

![](https://user-images.githubusercontent.com/6270283/29934511-c7bf6f84-8e7b-11e7-9188-c54966a24d4e.png)

[comment]: # (Fallback image: https://a.doko.moe/awjfpp.png)

If you don't use Visual Studio, you need to have the [NuGet console](https://chocolatey.org/packages/NuGet.CommandLine) installed and execute `nuget install KasiopeaApi -PreRelease` in the project directory.

### Usage

The fastest way to get started is using the relative URL to the task you are solving. For example `/archiv/2016/doma/D/` refers to [this task](https://kasiopea.matfyz.cz/archiv/2016/doma/D/).

Example

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

<details>
<summary><strike>How to use await from the application entry-point?</strike></summary>

```csharp
static void Main() {
	try {
		MainAsync().Wait();
	}
	catch(Exception e) {
		while(e is AggregateException) e = e.InnerException;
		throw e;
	}
}
static async Task MainAsync() {
	// do your async stuff
}
```
</details>

Since C# 7.1 you can make the entry point async.
```csharp
static async Task Main() {
	// do your async stuff
}
```

## Dependencies

- [HtmlAgilityPack](https://www.nuget.org/packages/HtmlAgilityPack/)
- [RestSharp](https://www.nuget.org/packages/RestSharp/)

## License

[MIT](https://rawgit.com/Sorashi/KasiopeaApi/master/LICENSE) license