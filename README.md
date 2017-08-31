Kasiopea API
===

This is an unoffical .NET web-scrapping API for the [Kasiopea](https://kasiopea.matfyz.cz) competition.

## This API allows you to

- [x] Require the input open-data, post your answer and know the result (success/fail)
- [x] Read the competition tasks – name and description (even from the archive section)
- [x] Read the competition result list and iterate through it
- [x] Satisfy your OCD

## Get started

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