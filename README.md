[![](https://img.shields.io/nuget/v/soenneker.validators.email.disposable.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.validators.email.disposable/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.validators.email.disposable/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.validators.email.disposable/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.validators.email.disposable.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.validators.email.disposable/)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Validators.Email.Disposable
### A validation module checking if a given email's domain is disposable/temporary, updated daily (if available)

## Installation

```
dotnet add package Soenneker.Validators.Email.Disposable
```

Register `EmailDisposableValidator` in your `.NET` application using `IServiceCollection`:

```csharp
services.AddEmailDisposableValidatorAsSingleton();
```

## Usage

Inject `IEmailDisposableValidator` and validate emails or domains:

```csharp
public class MyService
{
    private readonly IEmailDisposableValidator _validator;

    public MyService(IEmailDisposableValidator validator)
    {
        _validator = validator;
    }

    // Returns FALSE if disposable (i.e. mailinator.com). TRUE if non-disposable.
    public async Task<bool> IsNonDisposableEmail(string email)
    {
        return await _validator.Validate(email);
    }
}
```