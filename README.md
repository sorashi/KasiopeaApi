
# Kasiopea API

[![Discord](https://img.shields.io/discord/404269352335048704.svg)][discord]

[![NuGet](https://img.shields.io/nuget/dt/KasiopeaApi.svg)](https://www.nuget.org/packages/KasiopeaApi/)
[![NuGet](https://img.shields.io/nuget/v/KasiopeaApi.svg)](https://www.nuget.org/packages/KasiopeaApi/)
[![GitHub issues](https://img.shields.io/github/issues/Sorashi/KasiopeaApi.svg)](https://github.com/Sorashi/KasiopeaApi/issues)
[![license](https://img.shields.io/github/license/sorashi/KasiopeaApi.svg)](./LICENSE)

Neoficiální .NET (C#) API klient pro soutěž [Kasiopea](https://kasiopea.matfyz.cz).

## Jak začít

### Instalace

Nainstaluj nuget balík `KasiopeaApi`, případně pre-release verzi.

```sh
dotnet add package KasiopeaApi
```

nebo

```powershell
Install-Package KasiopeaApi
```

### Použití

Úloha je určena rokem, typem kola (domácí/finále) a písmenem. Například 2020, doma, A.

Příklad

```csharp
var k = new KasiopeaInterface("email", "password");
// nebo new KasiopeaInterface("email", "password", "https://kasiopea.matfyz.cz");
await k.SelectTaskAsync(2020, CourseKind.Home, 'A');
var reader = await k.GetInputReaderAsync(Difficulty.Easy);
var writer = k.GetOutputWriter();

// čti vstup pomocí
reader.ReadLine();
// piš výstup pomocí
writer.WriteLine();

// odešli výstup a zkontroluj KasiopeaTask.OutputCheckResult (Success, Fail, Timeout, MissingFile, Unknown)
var result = await k.PostOutputAsync();

if(result == ApiAttemptState.Success)
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

- [RestSharp](https://www.nuget.org/packages/RestSharp/)

## Licence

[MIT](./LICENSE) licence

[discord]: https://discord.gg/bGzPAvy