# ApiFluentResults

A lightweight solution to **map `FluentResults` to HTTP responses** in ASP.NET Core using **ProblemDetails**, keeping controllers clean and aligned with **Clean Architecture** and the **Result Pattern**.

The solution is split into **two NuGet packages** to support modular and layered architectures.

---

## Packages

### ApiFluentResults.Domain

Contains **standard and base error types** that can be used from any layer (Domain / Application) without depending on ASP.NET Core.

This package is intended to be referenced from **core layers** in Clean Architecture.

**Included errors:**
- `NotFoundError`
- `ConflictError`
- `ValidationError`
- `UnauthorizedError`
- `ForbiddenError`
- `DomainError` (base class for business errors)

```bash
dotnet add package ApiFluentResults.Domain
```

### ApiFluentResults.AspNetCore

Provides ASP.NET Core integration to:

- Automatically map FluentResults errors
- Resolve the correct HTTP status code
- Return standardized ProblemDetails responses
- Support custom domain errors without controller boilerplate

This package depends on ApiFluentResults.Domain.

```bash
dotnet add package ApiFluentResults.AspNetCore
```

## Getting Started

Base Controller

Inherit from FluentResultController to enable result mapping:

```csharp
public class FluentResultController : ControllerBase
{
    protected ResultMapper<T> MapResult<T>(Result<T> result) => new(result, this);

    protected ResultMapper<object> MapResult(Result result) =>
        new(result.ToResult<object>(), this);
}
 ```


## Controller Usage Example

```csharp
Public class OrdersController : FluentResultController
{
    // ...
    [HttpPost("{id}/pay")]
    public async Task<IActionResult> PayOrder(
        [FromRoute] long id,
        [FromBody] List<PayOrderCommand.PaymentDto> payOrders
    )
    {
        var command = new PayOrderCommand(id, payOrders);
        var result = await Mediator.Send(command);

        return MapResult(result)
            .ConflictFor<OrderAlreadyPaidError>()
            .Ok();
    }
}

```
No switch statements
No if/else chains
No HTTP concerns leaking into Application or Domain layers

## Supported Success Responses

```csharp
Ok()
.OkEmpty()
.Created()
.CreatedAtAction()
.NoContent()
.Accepted()
.Status(StatusCode...)
```

Example:

```csharp
return MapResult(result)
    .Created();
```

# Default Error Mappings

The following error types are mapped automatically:

| Error Type           | HTTP Status |
|----------------------|-------------|
| NotFoundError        | 404         |
| ConflictError        | 409         |
| ValidationError      | 400         |
| UnauthorizedError    | 401         |
| ForbiddenError       | 403         |
| DomainError          | 422         |

---

## Custom Domain Errors

You can define your own business errors by inheriting from `BaseFluentError`:

```csharp
public class OrderAlreadyPaidError : BaseFluentError
{
    public OrderAlreadyPaidError(string orderNumber)
        : base(
            $"Order {orderNumber} has already been paid.",
            "ORDER_ALREADY_PAID"
        )
    {
        Metadata.Add("orderId", orderNumber);
    }
}
```
Then map the error to an HTTP status code in the controller:

```csharp
return MapResult(result)
    .ConflictFor<OrderAlreadyPaidError>()
    .Ok();
```

## Custom Status Code Mapping
If there is no predefined method for the HTTP status you need, you can use CustomErrorFor:
```csharp
return MapResult(result)
    .CustomErrorFor<OrderAlreadyPaidError>(418)
    .Ok();
```
This allows full control over the returned HTTP status code.    

### ProblemDetails Response Format

All error responses follow the ProblemDetails standard, with additional support for:

- code (business error code)
- metadata
- errors (validation errors)

Example response:

```json
{
  "type": "https://httpstatuses.com/409",
  "title": "Conflict",
  "status": 409,
  "detail": "Order 123 has already been paid.",
  "instance": "/api/orders/123/pay",
  "code": "ORDER_ALREADY_PAID",
  "metadata": {
    "orderId": "123"
  }
}
```

### Benefits

- Clean and declarative controllers
- Clear separation between domain logic and HTTP concerns
- Fully compatible with FluentResults
- Ideal for Clean, Hexagonal, and Modular Architectures
- Consistent and standardized HTTP error responses

### When to Use

- ASP.NET Core APIs
- Applications using the Result Pattern
- Systems with explicit business rules
- Teams looking to reduce controller boilerplate