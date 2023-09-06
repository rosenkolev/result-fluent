
# FluentResult

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

A Result can store Data, Messages and Status code.

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
 *   "Data" : null,
 *   "Status": "NotFound",
 *   "Messages": ["Model not found"]
 * }
 */
```

## Map and MapAsync

```csharp
// simple map
Result<string> result =
    Result
        .Create(5)
        .Map(value => value * 2)
        .Map(value => $"value is {value}");
// result.Data == "value is 10"

// async map
Result<UserModel> result =
    await Result
        .Create(requestedUserId)
        .MapAsync(async userId => await _userRepository.GetByIdAsync(userId))
        .MapAsync(user => ConvertUserToUserModel(user));
```

## Validating

Static validation

```csharp
int userId = -1;
string userName = string.Empty;
Result<bool> validateResult =
    Result
        .Validate(userId > 0, ResultComplete.InvalidArgument, "User identifier must be positive number")
        .Validate(userName.Length > 0, ResultComplete.InvalidArgument, "User name is required");

// validateResult:
//  Data = false
//  Status = InvalidArgument
//  Messages = string[] { "User identifier must be positive number", "User name is required" }

string userName = "someLongUserName";
Result<bool> validateResult =
    Result
        .Create(userName)
        .Validate(
            x => x.Length < 5,
            ResultComplete.InvalidArgument,
            x => $"User name must be less than 5 symbols, the provided value was {x.Length} symbols.")

// validateResult:
//  Data = "someLongUserName"
//  Status = InvalidArgument
//  Messages = string[] { "User name must be less than 5 symbols, the provided value was 12 symbols." }
```

Validate can skip rules on fail

```
Result<bool> validateResult =
    Result
        .Create(userModel)
        .Validate(user => user != null, ResultComplete.InvalidArgument, "User model is null")
        .Validate(user => user.UserId > 0, ResultComplete.InvalidArgument, "User identifier is required", skipOnInvalidResult: true);
```

Validate nullable

```
Result<bool> validateResult =
    Result
        .Create<string?>(name)
        .ValidateNotNull(ResultComplete.InvalidArgument, "Name is null")
        .Validate(name => /** name is not null */, ResultComplete.InvalidArgument, "");
```

Chain validation and mapping

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
            "User doesn't exists")
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
// Status=InvalidArgument, Messages=["User name is required"], Data=null

await UpdateUserAsync(100, "Rosen");
// One of the following:
//
// Status=NotFound, Messages=["User doesn't exists"], Data=null
// Status=OperationFailed, Messages=["User was not updated"], Data=null
// Status=Success, Messages=null, Data={ UserId=100, UserName="Rosen" }
```

## Switch Mapping

You may need to use multiple method returning Result. For that reason there is Switch method.

```csharp
Result<int> Multiply(int a, int b) =>
    Result.Create(a + b);

Result<int> AddAndDouble(int a, int b) =>
    Result
        .Create(a + b)
        .Switch(value => Multiply(value, 2))
        .Map(value => $"The result is {value}");

AddAndDouble(2, 3) // Data = "The result is 10"
```

## Ensure successful result and return data

```csharp
// return user or throw exception
User validatedUser =
    Result
        .Create(user)
		.ValidateNotNull(ResultComplete.InvalidArgument, string.Empty)
		.AsValidData();

// When invalid we throw [ResultValidationException](src/FluentResult/ResultValidationException.cs).
Result
  .Validate(false, ResultComplete.OperationFailed, "Invalid result")
  .AsValidData();

// We can also use `AsValidDataAsync` for `Task<Result<TSource>>` and `Task<ResultOfItems<TSource>>`.
Result.Create(5)
  .MapAsync(Task.FromResult)
  .AsValidDataAsync();
```

## Catch Exception

We can catch async exception by using the Catch extensions.

```csharp
Result<User> result =
    Result
        .Create(requestedUserId)
        .MapAsync(userId => getUserAsync(userId))
        .CatchAsync(ex => Result.CreateResultWithError<User>(ResultComplete.OperationFailed, ex.Message));
```

## ResultOfItems

The result of items is a result that contain metadata for a collection of items.

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

## Deserialize the as Result<TResult>

In order to deserialize it we need to add `JsonConstructorAttribute`, because all properties are with private set.
For this to happen we need to use System.Test.Json or Newtonsoft.Json.
This library do not include it, because we do not want to depend on specific serialization. It can be achieved by inhering the class like:

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
