# Light.SharedCore

*Provides general abstractions, algorithms, and data structures for .NET*

![Light Logo](light-logo.png)

[![License](https://img.shields.io/badge/License-MIT-green.svg?style=for-the-badge)](https://github.com/feO2x/Light.SharedCore/blob/main/LICENSE)
[![NuGet](https://img.shields.io/badge/NuGet-3.0.0-blue.svg?style=for-the-badge)](https://www.nuget.org/packages/Light.SharedCore/)

# How to install

Light.SharedCore is compiled against [.NET Standard 2.0 and 2.1](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) and thus supports all major platforms like .NET and .NET Framework, Mono, Xamarin, UWP, or Unity.

Light.SharedCore is available as a [NuGet package](https://www.nuget.org/packages/Light.SharedCore/) and can be installed via:

- **Package Reference in csproj**: `<PackageReference Include="Light.SharedCore" Version="3.0.0" />`
- **dotnet CLI**: `dotnet add package Light.SharedCore`
- **Visual Studio Package Manager Console**: `Install-Package Light.SharedCore`

# What does Light.SharedCore offer you?

## Base classes for entities

Light.SharedCore offers you four base classes for entities. These are `Int32Entity`, `Int64Entity`, `GuidEntity`, and `StringEntity`. All of them offer an `Id` property of the corresponding type which is immutable by default. Also, all these classes implement `IEntity<T>` (this interface is part of Light.SharedCore) and `IEquatable<T>` for you (two instances are considered equal when they have equal ID values). These base classes are specifically tailored to be used with Object-Relational Mappers or serialization frameworks. They are immutable by default, although you can use the `IMutableEntity<T>` interface to change the ID after initialization.

### Deriving from the base classes

A class that derives from these entities could look like this:

```csharp
public sealed class Address : Int32Entity
{
    // Id property is not needed, it comes with the base class
    public string Street { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}
```

Your class can then be instantiated like so:

```csharp
var address = new Address
{
    Id = 1, // Or leave it out when the ID is generated by the database
    Street = "123 Lane Street",
    ZipCode = "49230",
    Location = "London"
};
```

The base classes also offer a parameterized constructor, so you could also make your class immutable via constructor injection (check if your ORM and serialization framework supports this - Entity Framework Core and System.Text.Json do support this, for example):

```csharp
public sealed class Address : Int32Entity
{
    public Address(int id, string street, string zipCode, string location)
        : base(id)
    {
        Street = street;
        ZipCode = zipCode;
        Location = location;
    }

    public string Street { get; }
    public string ZipCode { get; }
    public string Location { get; }
}
```

### ID range for Int32Entity and Int64Entity

By default, the `Int32Entity` and `Int64Entity` base classes will only allow IDs that are greater or equal to 1.

```csharp
// This statement will throw because Int32Entity and Int64Entity
// only allow positive IDs by default
var address = new Address { Id = 0 };
```

You can customize this behavior by using the static `AllowIdZero` and `AllowNegativeIds` properties:

```csharp
Int32Entity.AllowIdZero = true;
var address = new Address { Id = 0 };
```

There is also a handy `AllowZeroAndNegativeIds` method to set both `AllowIdZero` and `AllowNegativeIds` to true with one call.

> BE CAREFUL: all entities that derive from `Int32Entity` are affected by setting the `AllowIdZero` property or the `AllowNegativeIds` property to `true`. If you want to limit these settings to a specific type, you should derive from `Int32Entity<T>`, like so:

```csharp
public sealed class Address : Int32Entity<Address>
{
    // Members omitted for brevity's sake
}
```

You can then e.g. only allow zero as a valid ID for addresses:

```csharp
// other entities are not affected, because they do not derive from Int32Entity<Address>
Address.AllowIdZero = true; 
var address = new Address { Id = 0 };
var contact = new Contact { Id = 0 }; // This would throw
```

### GuidEntity and empty GUIDs

Similarly to the other types, you can derive from `GuidEntity` or `GuidEntity<T>`:

```csharp
public sealed class Bill : GuidEntity
{
    // Id property is not needed, it comes with the base class
    public decimal AmountInDollar {get; init; }
}
```

By default, `GuidEntity` does not allow empty GUIDs:

```csharp
// The next statement will throw
var bill = new Bill { Id = Guid.EmptyGuid };
```

Similarly to other entity base classes, you can change that by setting the `AllowEmptyGuid` static property:

```csharp
GuidEntity.AllowEmptyGuids = true;
var bill = new Bill { Id = Guid.EmptyGuid }; // This does not throw
```

As with `AllowIdZero` and `AllowNegativeIds`, the above code would affect all entities deriving from `GuidEntity`. To limit the effect to a single type, you should derive from `GuidEntity<T>`.

### StringEntity, validation, and case-sensitivity

The string entity has the same basic functionality as the other entity base classes. The IDs that are passed to it are validated with the following rules:

- The string must not be null, empty, or contain only white space
- It must be trimmed, i.e. the first and last character must not be white space
- It must have a maximum length of 200 characters

You can customize this behavior by supplying a delegate to the static `ValidateId` property. As always, if you want to limit this to one entity type, consider deriving from `StringEntity<T>` (instead of just `StringEntity`).

Furthermore, by default, an entity operates in case-sensitive mode (to be precise: `StringComparison.Ordinal`). You can change this mode by setting the static `ComparisonMode` property to another value of the `StringComparison` enum. As always: if you want to limit this to certain entity types, consider deriving from `StringEntity<T>`.

> BE CAREFUL: you should only change the comparison mode at the beginning of your application (in the composition root) before any of the entities are instantiated. Otherwise, subtle bugs can start to occur (e.g. when the ID is already used as a key in a dictionary), because the `Equals` and `GetHashCode` implementation rely on the `ComparisonMode` value.

The default value for `Id` for a string entity is `null`. You can change this behavior by using the static `IsDefaultValueNull` property whose default value is `true`.

### Changing the ID of an entity after initialization

By default, all ID properties of the entity base classes are immutable. However, there is a back door that you can use to change the ID after the entity is already fully initialized. The usual scenario where this is necessary is when the ID is created by a database so that the ID is only available after an I/O call:

```csharp
var address = new Address
{
    Street = "123 Lane Street",
    ZipCode = "49230",
    Location = "London"
};
var idOfNewAddress = await session.InsertAsync(address);
await session.SaveChangesAsync();
address.ToMutable().SetId(idOfNewAddress); // This will set the ID after initialization
```

To change the ID after initialization, simply call `entity.ToMutable().SetId(newId)`. `ToMutable` is an extension method which will not pollute the public API of your entities.

> BE CAREFUL: you must not change the ID of an entity when it is already supposed to be immutable. This might lead to subtle bugs e.g. when the ID is used as a key in a dictionary.

## Parsing strings to floating point values

.NET already offers many `TryParse` methods when it comes to parsing text to floating point values, but all of them have the issue that they interpret points and commas in a dedicated way (either as decimal sign or as thousand-delimiter sign, depending on the current or provided `CultureInfo`).

But often (and especially in a German context), commas and points might be mixed up, e.g. when users enter text into a text box, or when some IoT devices numbers in the German format, but others in the English format.

You can use the `DoubleParser.TryParse` method which analyses the input string for points and commas and then chooses either the invariant culture or the German culture to parse the string, depending on the number of points and commas and where they are placed. Check out the following code:

```csharp
DoubleParser.TryParse("15.0", out var value);        // value = 15.0
DoubleParser.TryParse("15,0", out var value);        // value = 15.0
DoubleParser.TryParse("200,575.833", out var value); // value = 200575.833
DoubleParser.TryParse("200.575,833", out var value); // value = 200575.833
```

> BE CAREFUL: if you have a number with only a single thousand-delimiter sign (i.e. no decimal sign), this number will not be parsed correctly. The thousand-delimiter sign will be interpreted as the decimal sign. We recognize that this scenario is rare, as especially human input will most likely never use the thousand-delimiter sign. Howevery, if this scenario applies, then please use the .NET `TryParse` methods and specify the correct culture info by yourself.

Light.SharedCore also offers you the `FloatParser` and the `DecimalParser`. Furthermore, the .NET Standard 2.1 version of this library has support for `ReadOnlySpan<char>`.

## Abstract from DateTime.UtcNow by using IClock

> While .NET 8 introduced the `TimeProvider` class, we think that this API is too verbose (including its timer functionality). This is why we stick to the `IClock` interface of Light.SharedCore and promote using it.

Light.SharedCore provides the `IClock` interface that abstracts calls to `DateTime.Now` and `DateTime.UtcNow`. This is usually required when testing your code, and you want to supply dedicated `DateTime` values to better control your tests. `IClock` has a method called `GetTime` that you can use to obtain the current time stamp.

There are three implementations for `IClock`:

- `UtcClock` will return `DateTime.UtcNow` when calling `GetTime`. This should be the default clock that you use as the resulting value is unambiguous.
- `LocalClock` will return the local time. Be aware that this might lead to ambiguous time stamps, e.g. when a change from standard time to daylight saving time happens.
- `TestClock` can be used in unit test scenarios to control the time programmatically.

You typically register the clock as a singleton with the DI container:

```csharp
services.AddUtcClock();
```

The clock can then be injected into a client:

```csharp
public sealed class UdpateJob
{
    public UpdateJob(IClock clock, INotificationService notificationService)
    {
        Clock = clock;
        NotificationService = notificationService;
    }

    private IClock Clock { get; }
    private INotificationService NotificationService { get; }

    public async Task ExecuteAsync()
    {
        var now = Clock.GetTime();

        // Do something here

        var finished = Clock.GetTime();
        if ((finished - now) >= TimeSpan.FromMinutes(2))
            await NotificationService.CreateMessage("The update took unusually long - please check the log files for irregularities.");
    }
}
```

The usage of `IClock` in your production code lets us now write the tests way easier:

```csharp
public sealed class UpdateJobTests
{
    [Fact]
    public async Task CreateNotificationOnLongExecutionTime()
    {
        var initialTime = DateTime.UtcNow;
        var secondTime = initialTime.AddMinutes(2);
        var testClock = new TestClock(initialTime, secondTime);
        var notificationService = new NotificationServiceMock();
        var job = new UpdateJob(testClock, notificationService);

        await job.ExecuteAsync();

        notificationService.CreateMessageMustHaveBeenCalled();
    }
}
```

In the example above, two `DateTime` instances are created, where the second one is two minutes later than the initial one. They are passed to the test clock which will return them on subsequent calls to `GetTime`. This allows us to easily check if the notification service is called properly by our job implementation.

`TestClock` also provides you with a `AdvanceTime` method that will change the current time. This can be used in scenarios where flow control returns to the test method in between calls to `GetTime`.

Prefer UTC time stamps, especially in services and when saving date and time values. They are unambiguous, especially when it comes to changes in daylight saving time or to political decisions. You can convert your UTC time stamp to local time in the UI layer.

## Database access abstractions

This package offers a new base interface for designing abstractions for database access. The `ISession` interface represents the [Unit-of-Work Design Pattern](https://www.martinfowler.com/eaaCatalog/unitOfWork.html) and offers a `Task SaveChangesAsync(CancellationToken)` method to explicitly trigger a commit to the underlying database access technology. This could be committing a transaction when using ADO.NET or Micro-ORMs like LinqToDB, Dapper, or MongoDB.Driver, as well as calling `SaveChangesAsync` on an Entity Framework Core `DbContext` for Full-ORMs. The `ISession` interface also derives from `IAsyncDisposable`. When the session is disposed before `SaveChangesAsync` is called, a rollback will be automatically executed by underlying implementations.

If you have been using Light.SharedCore in previous versions, you might wonder where the `IAsyncReadOnlySession` interface has gone. It has been removed in favor of simply using `IAsyncDisposable`. If you design database sessions that essentially behave like a client (without explicit transaction management by the caller), then simply derive your abstraction from `IAsyncDisposable` instead of the old `IAsyncReadOnlySession`. Also, all synchronous database interfaces as well as the sessions supporting transactions are gone (the current recommendation is to completely hide transaction objects like `DbTransaction` from business logic by implemeting specialized sessions).

 All database I/O should be executed in an asynchronous fashion to avoid threads being blocked during database queries. This is especially important when you try to scale service apps. Incoming requests will normally be handled by executing code on the .NET Thread Pool (e.g. in ASP.NET Core) which in turn will create new threads when it sees that its worker threads are blocked. With a high number of concurrent requests, you might end up in a situation where your service app responds really slowly because of all the overhead of new threads being created and the context switches between them (thread starvation).