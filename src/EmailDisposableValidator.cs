using Microsoft.Extensions.Logging;
using Soenneker.Validators.Email.Disposable.Abstract;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Soenneker.Utils.AsyncSingleton;
using Soenneker.Utils.File.Abstract;
using System;
using System.Threading;
using Soenneker.Utils.String.Abstract;
using Soenneker.Extensions.ValueTask;
using Soenneker.Extensions.String;

namespace Soenneker.Validators.Email.Disposable;

/// <inheritdoc cref="IEmailDisposableValidator"/>
public sealed class EmailDisposableValidator : Validator.Validator, IEmailDisposableValidator
{
    private readonly IStringUtil _stringUtil;
    private readonly AsyncSingleton<HashSet<string>> _emailDomainsSet;

    public EmailDisposableValidator(IFileUtil fileUtil, IStringUtil stringUtil, ILogger<EmailDisposableValidator> logger) : base(logger)
    {
        _stringUtil = stringUtil;

        _emailDomainsSet = new AsyncSingleton<HashSet<string>>(async (token, _) =>
        {
            return await fileUtil.ReadToHashSet(Path.Combine(AppContext.BaseDirectory, "Resources", "data-email-disposables.txt"),
                                     StringComparer.InvariantCultureIgnoreCase, cancellationToken: token)
                                 .NoSync();
        });
    }

    public async ValueTask<bool> Validate(string email, bool log = false, CancellationToken cancellationToken = default)
    {
        if (email.IsNullOrWhiteSpace())
        {
            Logger.LogWarning("Email is null or whitespace, failing");
            return false;
        }

        string? domain = _stringUtil.GetDomainFromEmail(email);

        if (domain.IsNullOrWhiteSpace())
        {
            Logger.LogWarning("Domain is null or whitespace, failing");
            return false;
        }

        // The reason for not calling ValidateDomain() here is so we can full the full email
        if ((await _emailDomainsSet.Get(cancellationToken).NoSync()).Contains(domain))
        {
            if (log)
                Logger.LogWarning("Email ({email}) detected as disposable", email);

            return false;
        }

        return true;
    }

    public async ValueTask<bool> ValidateDomain(string domain, bool log = false, CancellationToken cancellationToken = default)
    {
        if (domain.IsNullOrWhiteSpace())
        {
            Logger.LogWarning("Domain is null or whitespace, failing");
            return false;
        }

        if ((await _emailDomainsSet.Get(cancellationToken).NoSync()).Contains(domain))
        {
            if (log)
                Logger.LogWarning("Domain ({domain}) detected as disposable", domain);

            return false;
        }

        return true;
    }

    public ValueTask DisposeAsync()
    {
        return _emailDomainsSet.DisposeAsync();
    }

    public void Dispose()
    {
        _emailDomainsSet.Dispose();
    }
}