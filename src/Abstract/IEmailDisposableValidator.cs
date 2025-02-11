using Soenneker.Validators.Validator.Abstract;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Validators.Email.Disposable.Abstract;

/// <summary>
/// A validation module checking if a given email's domain is disposable/temporary, updated daily (if available)
/// </summary>
public interface IEmailDisposableValidator : IValidator, IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Validates if the given email is disposable/temporary.
    /// </summary>
    /// <param name="email">The email to be validated.</param>
    /// <param name="log"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>False if the email is disposable/temporary, true otherwise.</returns>
    ValueTask<bool> Validate(string email, bool log = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if the given domain is disposable/temporary.
    /// </summary>
    /// <param name="domain">The domain to be validated. It is lowered within this method before validating.</param>
    /// <param name="log"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>False if the email is disposable/temporary, true otherwise.</returns>
    ValueTask<bool> ValidateDomain(string domain, bool log = false, CancellationToken cancellationToken = default);
}
