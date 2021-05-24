# FluentResult

**This is a lightweight .NET library, that can be used for returning and validating Result without relaying on exceptions.**

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

```csharp
Model model = new Model();

// create a result which indicates success
Result<Model> successResult = new Result<Model>(model, ResultComplete.Success, messages);
// or
Result<Model> successResult = new Result<Model>(model); // Status = Success, Messages = null
// or just
Result<Model> successResult = Result.Create(model);

// create a result which indicates error
Result<Model> errorResult = new Result<Model>(model, ResultComplete.InvalidArgument, new [] { "Model identifier must be a positive number" });
// or
Result<Model> errorResult = Result.CreateResultWithError<Model>(ResultComplete.NotFound, "Model not found");
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
int userName = string.Empty;
Result<bool> validateResult =
    Result
        .Validate(userId > 0, ResultComplete.InvalidArgument, "User identifier must be positive number")
        .Validate(userName.Length > 0, ResultComplete.InvalidArgument, "User name is required");

// validateResult:
//  Data = false
//  Status = InvalidArgument
//  Messages = string[] { "User identifier must be positive number", "User name is required" }
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
                var updatedUser = _userRepository.UpdateUserAsync(user);
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

await UpdateUserAsync(100, "Jhon");
// One of the following:
//
// Status=NotFound, Messages=["User doesn't exists"], Data=null
// Status=OperationFailed, Messages=["User was not updated"], Data=null
// Status=Success, Messages=null, Data={ UserId=100, UserName="Jhon" }
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
```
