using Soenneker.Validators.Validator.Abstract;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Validators.Email.Disposable.Abstract;

/// <summary>
/// Checks email domains against the disposable-domain data packaged with the application.
/// </summary>
public interface IEmailDisposableValidator : IValidator, IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Validates if the given email is disposable/temporary.
    /// </summary>
    /// <param name="email">The email to be validated.</param>
    /// <param name="log">Whether to log the full email when it matches the disposable-domain list.</param>
    /// <param name="cancellationToken">A token used while loading the domain data.</param>
    /// <returns>False if the email is disposable/temporary, true otherwise.</returns>
    ValueTask<bool> Validate(string? email, bool log = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if the given domain is disposable/temporary.
    /// </summary>
    /// <param name="domain">The domain to check using case-insensitive exact matching.</param>
    /// <param name="log">Whether to log the domain when it matches the disposable-domain list.</param>
    /// <param name="cancellationToken">A token used while loading the domain data.</param>
    /// <returns>False if the domain is listed as disposable/temporary, true otherwise.</returns>
    ValueTask<bool> ValidateDomain(string? domain, bool log = false, CancellationToken cancellationToken = default);
}
