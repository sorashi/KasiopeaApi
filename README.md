Kasiopea API 
===
[![CZ](https://download.kde.org/extra/flags/png/cz.png)](./README.md) | [![EN](https://download.kde.org/extra/flags/png/gb.png)](./README-en.md)

[![Discord](https://img.shields.io/discord/404269352335048704.svg)][discord]

[![NuGet](https://img.shields.io/nuget/dt/KasiopeaApi.svg)](https://www.nuget.org/packages/KasiopeaApi/)
[![NuGet](https://img.shields.io/nuget/v/KasiopeaApi.svg)](https://www.nuget.org/packages/KasiopeaApi/)
[![AppVeyor branch](https://img.shields.io/appveyor/ci/Sorashi/KasiopeaApi/master.svg)](https://ci.appveyor.com/project/Sorashi/kasiopeaapi)
[![GitHub issues](https://img.shields.io/github/issues/Sorashi/KasiopeaApi.svg)](https://github.com/Sorashi/KasiopeaApi/issues)
[![Trello](https://img.shields.io/badge/board-on%20trello-brightgreen.svg)](https://trello.com/b/g63MxKDP)
[![license](https://img.shields.io/github/license/sorashi/KasiopeaApi.svg)](./LICENSE)

Neoficiální .NET web-scraping API pro soutěž [Kasiopea](https://kasiopea.matfyz.cz).

```powershell
Install-Package KasiopeaApi
```

## Pomocí tohoto API můžeš

- [x] Získat vstup, odeslat svůj výstup a zjistit úspěšnost (success/fail)
- [x] Číst názvy a zadání úloh (i ze sekce Archiv)
- [x] Číst a iterovat tabulku výsledků
- [x] Uspokojit svou OCD

## Jak začít

### Instalace

Nejjednodušší je použít Package Manager Console ve Visual Studiu. Spusť `Install-Package KasiopeaApi`

Jelikož používáme [SemVer 2.0.0](https://semver.org/spec/v2.0.0.html), musíš mít alespoň Visual Studio 2017 (verze 15.3) nebo NuGet client 4.3.0 a výš.

![](https://user-images.githubusercontent.com/6270283/29934511-c7bf6f84-8e7b-11e7-9188-c54966a24d4e.png)

[comment]: # (Fallback image: https://a.doko.moe/awjfpp.png)

Pokud nepoužíváš Visual Studio, musíš mít [NuGet console](https://chocolatey.org/packages/NuGet.CommandLine) a spustit `nuget install KasiopeaApi -PreRelease` ve složce s projektem.

### Použití

Nejrychlejší způsob, jak začít, je použít relativní URL na úlohu, kterou řešíš. Například `/archiv/2016/doma/D/` odkazuje na [tuto úlohu](https://kasiopea.matfyz.cz/archiv/2016/doma/D/).

Příklad

```csharp
var kasiopeaInterface = new KasiopeaInterface();
var loginSucceeded = await kasiopeaInterface.Login("email", "password");
var task = await KasiopeaTask.FromUrl("/archiv/2016/doma/D/");
var version = KasiopeaTask.InputVersion.Easy; // nebo InputVersion.Hard (těžší vstup)
var reader = await task.GetInputReaderAsync(version, kasiopeaInterface);
var writer = task.GetOutputWriter();

// čti vstup pomocí
reader.ReadLine();
// piš výstup pomocí
writer.WriteLine();
// nebo vyčisti výstup a začni odznovu
task.ClearStreamWriter();

// odešli výstup a zkontroluj KasiopeaTask.OutputCheckResult (Success, Fail, Timeout, MissingFile, Unknown)
var result = await task.PostOutputAsync(version, kasiopeaInterface);
if(result == KasiopeaTask.OutputCheckResult.Success)
	Console.WriteLine("Hurá");
else
	Console.WriteLine(result.ToString());
```

<details>
<summary><strike>Jak použít await ze vstupního bodu aplikace?</strike></summary>

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

Od C# 7.1 lze označit vstupní bod jako asynchronní metodu.
```csharp
static async Task Main() {
	// do your async stuff
}
```

Pokud máš jakékoli dotazy, zeptej se na našem [Discord serveru][discord].

## Závislosti

- [HtmlAgilityPack](https://www.nuget.org/packages/HtmlAgilityPack/)
- [RestSharp](https://www.nuget.org/packages/RestSharp/)

## Pre-release

Pokud rád žiješ *na hraně*, instaluj pre-release verzi. Obsahuje nejnovější binárky, které ještě nebyly označeny za stabilní 😱

```powershell
Install-Package KasiopeaApi -IncludePrerelease
```

## Licence

[MIT](./LICENSE) licence

[discord]: https://discord.gg/bGzPAvy