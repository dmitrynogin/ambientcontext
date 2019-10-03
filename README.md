## Ambient Context 
### Ambient operation logging and cancellation

[GitHub](https://github.com/dmitrynogin/ambientcontext) and [NuGet](https://www.nuget.org/packages/Ambient.Context/)

How many times you have been writing something like this passing those tedious logger/token parameters?

```csharp
    interface IMyService
    {
        void Method1(…, ILogger logger, CancellationToken token);
        void Method2(…, ILogger logger, CancellationToken token);
        …
    }
```

Enough is enough. 

### Ambient Cancellation

What we about to do is to use a special `Cancellation` helper class like this:

```csharp
        static void Main(string[] args)
        {
            using (new Cancellation())
            {
                Task.Run(PingAsync);
                ReadLine();
                Cancellation.Request();
                ReadLine();
            }
        }

        static async Task PingAsync()
        {
            try
            {
                while (!Cancellation.Requested)
                {
                    await Task.Delay(100, Cancellation.Token);
                    WriteLine("Ping");
                }

                Cancellation.ThrowIfRequested();
            }
            catch(OperationCanceledException)
            {
                WriteLine("Ping cancelled");
            }
        }
```

### Ambient Logging

`Op` class helps tracing execution in a logical order instead of chronological as everybody else :)

For example, the following code:

```csharp
        static async Task MainAsync()
        {
            Op.Log.Subscribe(WriteLine);
            using (new Op())
                await Task.WhenAll(
                    from i in Range(0, 4)
                    select AlphaAsync());
        }

        private static async Task AlphaAsync()
        {
            using (new Op("Alpha function"))
                await BetaAsync();
        }

        private static async Task BetaAsync()
        {
            using (var op = new Op())
            {
                op.Trace("Waiting...");
                await Task.Delay(100);
                op.Trace("Continue");
            }
        }
```

Will generate:

```
    MainAsync took 116 ms
      Alpha function took 109 ms
        BetaAsync took 108 ms
          Waiting... after 0 ms
          Continue after 108 ms
      Alpha function took 109 ms
        BetaAsync took 109 ms
          Waiting... after 0 ms
          Continue after 109 ms
      Alpha function took 109 ms
        BetaAsync took 109 ms
          Waiting... after 0 ms
          Continue after 109 ms
      Alpha function took 111 ms
        BetaAsync took 111 ms
          Waiting... after 0 ms
          Continue after 111 ms
```

Which is a way more readable…

### ASP.NET Core support

Use `AmbientContextAttribute` on controller class or controller action method to support execution tracing and cancellation.
