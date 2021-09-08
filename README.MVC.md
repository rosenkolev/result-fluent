# FluentResult.Mvc

**This is a MVC extensions methods for returning an HttpAction from Result.**

You can install [ResultFluent.Mvc with NuGet](https://www.nuget.org/packages/ResultFluent.Mvc/):

```shell
dotnet add package ResultFluent.Mvc
```

## Use
```csharp
    // using FluentResult;

    /// <summary>Gets an item by id.</summary>
    /// <response code="200">Gets an item by id successfully.</response>
    /// <response code="400">Id is invalid.</response>
    /// <response code="404">Item is not found.</response>
    [HttpGet]
    [ProducesResponseType(typeof(Result<Item>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetItemByIdAsync([Required] int id) =>
         _itemsService.GetItemByIdAsync(id).ToActionResultAsync(this);
```
