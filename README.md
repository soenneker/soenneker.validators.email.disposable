[![](https://img.shields.io/nuget/v/soenneker.validators.email.disposable.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.validators.email.disposable/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.validators.email.disposable/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.validators.email.disposable/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.validators.email.disposable.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.validators.email.disposable/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.validators.email.disposable/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.validators.email.disposable/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Validators.Email.Disposable
Checks email domains against a packaged disposable/temporary-domain list without making network requests.

## Installation

```bash
dotnet add package Soenneker.Validators.Email.Disposable
```

## Registration

```csharp
using Soenneker.Validators.Email.Disposable.Registrars;

services.AddEmailDisposableValidatorAsSingleton();
```

Singleton registration is recommended because the domain file is loaded lazily into a `HashSet` once per validator instance. Scoped registration is available but reloads the data for each scope that uses the validator.

## Validate an email

```csharp
using Soenneker.Validators.Email.Disposable.Abstract;

bool accepted = await validator.Validate(
    "person@example.com",
    cancellationToken: cancellationToken);
```

The result is `false` when the input is blank, does not contain non-empty text on both sides of its last `@`, or its extracted domain is present in the packaged list. It returns `true` when the domain is not listed.

This is not email syntax validation. For example, the domain is obtained from the text after the last `@`; no mailbox grammar, internationalized-domain normalization, or deliverability check is performed. Combine it with a syntax validator when accepting user addresses.

## Validate a domain directly

```csharp
bool accepted = await validator.ValidateDomain(
    "mailinator.com",
    cancellationToken: cancellationToken);
```

Matching is case-insensitive and exact. A parent domain match does not automatically reject a subdomain, and input is not trimmed or converted from Unicode to Punycode. Normalize domains before calling when those behaviors are required.

## Data and failure behavior

The list comes from the installed `Soenneker.Data.Email.Disposables` package and is not refreshed over the network at runtime. Updating the application dependency is what updates the packaged data. A `true` result means only “not present in this list”; providers can appear, disappear, or evade list-based detection.

The first validation loads the resource file. Missing-resource and file-read failures propagate, as does cancellation during that load.

Set `log: true` only when it is acceptable to write the matched email address or domain to logs. Blank-input warnings are logged regardless of that flag. Email addresses are personal data in many environments, so prefer direct-domain validation or leave match logging disabled unless operationally necessary.

The validator implements synchronous and asynchronous disposal for its lazy resource holder. Dependency injection disposes registered instances automatically.
