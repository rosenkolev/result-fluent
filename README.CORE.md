
# FluentResult

1. [Models](#models)
1. [Creating a Result](#creating-a-result)
1. [Map and MapAsync](#map-and-mapasync)
1. [Validating](#validating)
1. [Switch Mapping](#switch-mapping)
1. [Ensure successful result](#ensure-successful-result)
1. [Catch Exception](#catch-exception)
1. [ResultOfItems](#resultofitems)
1. [Combine and CombineAsync](#combine-and-combineasync)
1. [Async helper class](#async-helper-class)
1. [Using FluentValidation](#using-fluentvalidation)
1. [Deserialize the Result](#deserialize-the-result)

## Models

```csharp
class Result<TModel>
{
    TModel Data { get; }
    ResultComplete Status { get; }
    ICollection<string> Messages { get; }
    bool IsSuccessfulStatus();
}

class ResultOfItems<TItem> : Result<IEnumerable<TItem>>
{
    ResultMetadata Metadata { get; }
}
```

## Creating a Result

A Result can store Data, Messages, and a Status code.

_Create a result which indicates success_

```csharp
Result<Model> successResult =
  new Result<Model>(
    new Model(),
    ResultComplete.Success,
    messages);

/* {
 *   "Data" : { ... },
 *   "Status": "Success",
 *   "Messages": [ ... ]
 * }
 */
```

_Create a result which indicates success (short version)_

```csharp
Result<Model> successResult =
  new Result<Model>(new Model()); 

Result<Model> successResult2 =
  Result.Create(new Model());

// Both will create the same success model:
/* {
 *   "Data" : { ... },
 *   "Status": "Success",
 *   "Messages": null
 * }
 */
```

_Create a result which indicates success (short version with message)_

```csharp
Result<Model> successResult =
  Result.Create(model, "OK");

/* {
 *   "Data" : { ... },
 *   "Status": "Success",
 *   "Messages": ["OK"]
 * }
 */
```

_Create a result which indicates error_

```csharp
Result<Model> errorResult =
  new Result<Model>(
    model,
    ResultComplete.InvalidArgument,
    new [] { "Model identifier must be a positive number" });

/* {
 *   "Data" : { ... },
 *   "Status": "InvalidArgument",
 *   "Messages": ["Model identifier must be a positive number"]
 * }
 */

Result<Model> errorResult =
  Result.CreateResultWithError<Model>(
    ResultComplete.NotFound,
    "Model not found");
    
/* {
 *   "Data": null,
 *   "Status": "NotFound",
 *   "Messages": ["Model not found"]
 * }
 */
```

## Map and MapAsync

Maps the `Data` model to another model.

_Basic synchronous mapping_

```csharp
Result<string> result =
    Result
        .Create(5)
        .Map(value => value * 2)
        .Map(value => $"value is {value}");

/* {
 *   "Data": "value is 10",
 *   "Status": "Success",
 *   "Messages": null
 * }
 */
```

_Basic asynchronous mapping_

```csharp
Result<UserModel> result =
    await Result
        .Create(requestedUserId)
        .MapAsync(async userId => await _userRepository.GetByIdAsync(userId))
        .MapAsync(user => ConvertUserToUserModel(user));
```

> :warning: **In case the `Status` is not `Success`**, the mapper function is not executed and the Data becomes the `default(TResult)`.
> 
```csharp
Result<int> result =
    new Result<string>("1100", ResultComplete.InvalidArgument, new [] { "Invalid argument" })
        .Map<int>(value => int.Parse(value));

/* {
 *   "Data": 0,
 *   "Status": "InvalidArgument",
 *   "Messages": [ "Invalid argument" ]
 * }
 */
```

## Validating

A simple implementation of validation. You can validate static values or validate the `Data` model.

_Simple direct validation_

```csharp
int userId = -1;
string userName = string.Empty;
Result<bool> validateResult =
    Result
        .Validate(userId > 0, ResultComplete.InvalidArgument, "User identifier must be positive number")
        .Validate(userName.Length > 0, ResultComplete.InvalidArgument, "User name is required");

/* {
 *   "Data": false,
 *   "Status": "InvalidArgument",
 *   "Messages": [
 *     "User identifier must be positive number",
 *     "User name is required"
 *   ]
 * }
 */
```

_Simple validation of the `.Data` model_

```csharp
string userName = "someLongUserName";
Result<bool> validateResult =
    Result
        .Create(userName)
        .Validate(
            x => x.Length < 5,
            ResultComplete.InvalidArgument,
            x => $"User name must be less than 5 symbols, the provided value was {x.Length} symbols.")

/* {
 *   "Data": "someLongUserName",
 *   "Status": "InvalidArgument",
 *   "Messages": [ "User name must be less than 5 symbols, the provided value was 12 symbols." ]
 * }
 */
```

_Validate can be skipped based on the result's `Status` property and the `skipOnInvalidResult` argument_

```
UserModel userModel = null;
Result<bool> validateResult =
    Result
        .Create(userModel)
        .Validate(user => user != null, ResultComplete.InvalidArgument, "User model is null")
        .Validate(user => user.UserId > 0, ResultComplete.InvalidArgument, "User identifier is required", skipOnInvalidResult: true);

/* {
 *   "Data": false,
 *   "Status": "InvalidArgument",
 *   "Messages": [ "User model is null" ]
 * }
 */
```

_Validate nullable objects with the `ValidateNotNull` method_

```
Result<bool> validateResult =
    Result
        .Create<string?>(name)
        .ValidateNotNull(ResultComplete.InvalidArgument, "Name is null")
        .Validate(name => /** name is not null */, ResultComplete.InvalidArgument, "");
```

_Chain validation and mapping_

```csharp
Task<Result<User>> UpdateUserAsync(int userId, string userName)
{
    var userUpdateResult = Result
        .Validate(
            userId > 0,
            ResultComplete.InvalidArgument,
            "User identifier must be positive number")
        .Validate(
            userName.Length > 0,
            ResultComplete.InvalidArgument,
            "User name is required")
        .MapAsync(
            isValid => _userRepository.GetByIdAsync(userId))
        .ValidateAsync(
            user => user != null,
            ResultComplete.NotFound,
            "User doesn't exist")
        .MapAsync(
            async user =>
            {
                user.UserName = userName;
                var updatedUser = await _userRepository.UpdateUserAsync(user);
                return updatedUser;
            })
        .ValidateAsync(
            updatedUser => updatedUser != null,
            ResultComplete.OperationFailed,
            "User was not updated");

    return userUpdateResult;
}

await UpdateUserAsync(10, "");

/* {
 *   "Data": null,
 *   "Status": "InvalidArgument",
 *   "Messages": [ "User name is required" ]
 * }
 */

await UpdateUserAsync(100, "Rosen");

/* 
 * In case `_userRepository.GetByIdAsync()` returns `null`:
 * {
 *   "Data": null,
 *   "Status": "NotFound",
 *   "Messages": [ "User doesn't exist" ]
 * }
 *
 * In case `_userRepository.UpdateUserAsync()` returns `null`:
 * {
 *   "Data": null,
 *   "Status": "OperationFailed",
 *   "Messages": [ "User was not updated" ]
 * }
 *
 * In case everything is successful:
 * {
 *   "Data": {
 *     "UserId": 100,
 *     "UserName": "Rosen"
 *   },
 *   "Status": "Success",
 *   "Messages": null
 * }
 */
```

## Switch Mapping

You may need to use multiple methods to return `Result`. For that reason, there is the `Switch` method.

_The `Switch` method uses another Result as a source of mapping_

```csharp
Result<int> Multiply(int a, int b) =>
    Result.Create(a + b);

Result<int> AddAndDouble(int a, int b) =>
    Result
        .Create(a + b)
        .Switch(value => Multiply(value, 2))
        .Map(value => $"The result is {value}");

AddAndDouble(2, 3)

/* {
 *   "Data": "The result is 10",
 *   "Status": "Success",
 *   "Messages": null
 * }
 */
```

_The `SwitchAsync` is the same as `Switch` but uses `Task`_

```csharp
Result<int> validateResult =
    await Result
        .Create(5)
        .SwitchAsync(value =>
            Task.FromResult(
                Result.Create(value * 2)));

/* {
 *   "Data": 10,
 *   "Status": "Success",
 *   "Messages": null
 * }
 */
```

## Ensure successful result

The method `AsValidData` returns the `Data` property or throws the [ResultValidationException](src/FluentResult/ResultValidationException.cs) when invalid.

_Throws the `ResultValidationException` exception_

```csharp
Result
  .Validate(false, ResultComplete.OperationFailed, "Invalid result")
  .AsValidData();
```

_Returns the Data model_

```csharp
User theSameAsUser =
    Result.Create(user).AsValidData();

// We can also use `AsValidDataAsync` for `Task<Result<TSource>>` and `Task<ResultOfItems<TSource>>`.

Result.Create(5)
  .MapAsync(Task.FromResult)
  .AsValidDataAsync();
```

## Catch Exception

We can catch async exceptions by using the Catch extensions.

```csharp
Result<User> result =
    Result
        .Create(requestedUserId)
        .MapAsync(userId => getUserAsync(userId))
        .CatchAsync(ex => Result.CreateResultWithError<User>(ResultComplete.OperationFailed, ex.Message));
```

## ResultOfItems

The result of items is a result that contains metadata for a collection of items.

```csharp
ResultOfItems<int> result = new ResultOfItems<int>(
    items: new [] { 4, 5 },
    status: ResultComplete.Success,
    messages: null,
    totalCount: 5,
    pageSize: 3,
    pageIndex: 1,
    count: 2
);

// is the same as
ResultOfItems<int> result = Result.CreateResultOfItems(
    items: new [] { 4, 5 },
    totalCount: 5,
    pageSize: 3,
    pageIndex: 1);

// or may be
ResultOfItems<Item> GetItemsByPage(int pageIndex, int pageSize) =>
    Result
        .Validate(pageIndex >= 0, ResultComplete.InvalidArgument, "Page index is invalid")
        .Validate(pageSize > 0, ResultComplete.InvalidArgument, "Page size is invalid")
        .MapAsync(
            isValid => _itemsRepository.GetByPageAsync(pageIndex, pageSize))
        .ToResultOfItemsAsync(
            data => Result.CreateResultOfItems(data.Items, data.TotalCount, pageSize, pageIndex));

// or simply
ResultOfItems<Item> GetItemsByPage(int pageIndex, int pageSize) =>
    Result
        .Validate(pageIndex >= 0, ResultComplete.InvalidArgument, "Page index is invalid")
        .Validate(pageSize > 0, ResultComplete.InvalidArgument, "Page size is invalid")
        .MapAsync(_ => _itemsRepository.GetByPageAsync(pageIndex, pageSize))
        .ToResultOfItemsAsync();
```

## Combine and CombineAsync

```csharp
// We can combine from 1 to 5 results.
var helloWorld = Result.Create("Hello").Combine(
	number => Result.Create("World"),
	(a, b) => a + b);

// Sum is 9
var sum = Result.Create(2).Combine(
	number => (
		Result.Create(3),
		Result.Create(4)),
	(a, b, c) => a + b + c);

// async example
Task<Result<Classroom>> UpdateClassroomAsync(UpdateClassroomRequest request) =>
    Result
		.Validate(request != null, ResultComplete.InvalidArgument, "The request must not be null")
	    .MapAsync(_ => _classroomRepository.GetByIdAsync(request.Id))
		.CombineAsync(
		    classroom => (
			    _schoolRepository.GetByIdAsync(request.SchoolId),
				_userRepository.GetByIdAsync(request.TeacherId)),
			async (classroom, school, teacher) =>
			{
			    classroom.School = school;
				classroom.Teacher = teacher;
				await _classroomRepository.UpdateAsync(classroom);
			});
```

## Async helper class

The [Async](src/FluentResult/Async.cs) is a helper structure to reduce code definitions.

```csharp
Task<Result<IReadOnlyList<string>>> GetNamesAsync();
// Becomes
Async<IReadOnlyList<string>> GetNamesAsync();
```

We can use `.AsAsync()` extension method.

```csharp
Async task = Result.Create(5).MapAsync(Task.FromResult).ToAsync();
```

## Using FluentValidation

We can extend the `Result` class to support `FluentValidation` as well.

```csharp
using FluentResult;
using FluentValidation;

public static class FluentValidationExtensions
{
  /// <summary>Validates based on FluentValidation.</summary>
  /// <typeparam name="TResult">The type of the result.</typeparam>
  [DebuggerStepThrough]
  public static Result<TResult> Validate<TResult>(
    [NotNull] this Result<TResult> result,
    [NotNull] IValidator<TResult> validator,
    ResultComplete status)
  {
    var res = validator.Validate(result.Data);
    return res.IsValid
      ? result
      : new Result<TResult>(
          result.Data,
          status,
          (result.Messages ?? []).Concat(res.Errors.Select(it => it.ErrorMessage)).ToArray());
  }

  /// <summary>Validates based on FluentValidation.</summary>
  /// <typeparam name="TResult">The type of the result.</typeparam>
  [DebuggerStepThrough]
  public static Result<TResult> Validate<TResult>(
    [NotNull] this Result<TResult> result,
    [NotNull] IValidator<TResult> validator,
    ResultComplete status,
    bool skipOnInvalidResult) =>
    skipOnInvalidResult && !result.IsSuccessfulStatus()
      ? result
      : Validate(result, validator, status);
}
```

Then use it in the flow:

```csharp
/*
 * class UpdateUserValidator : AbstractValidator<UserModel> { }
 */

/// <inheritdoc/>
public Task<Result<UserModel>> UpdateAsync(UserModel updateModel) =>
  Result
    .Create(updateModel)
    .Validate(new UpdateUserValidator(), ResultComplete.InvalidArgument);
```

## Deserialize the Result

To deserialize it we need to add `JsonConstructorAttribute`, because all properties are with a private set.
For this to happen we need to use System.Test.Json or Newtonsoft.Json.
This library does not include it, because we do not want to depend on specific serialization. It can be achieved by inhering the class like:

```csharp
// my /Result{TResult}.cs
using Newtonsoft.Json;
// ...

public class Result<TResult> : FluentResult.Result<TResult>
{
    [JsonConstructor]
    public Result(TResult data, FluentResult.ResultComplete status, ICollection<string> messages)
        : base(data, status, messages)
    {
    }
}
```

or

```csharp
using System.Text.Json.Serialization;
// ...

public class Result<TResult> : FluentResult.Result<TResult>
{
    [JsonConstructor]
    public Result(TResult data, FluentResult.ResultComplete status, ICollection<string> messages)
        : base(data, status, messages)
    {
    }
}
```
