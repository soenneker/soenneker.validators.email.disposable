using Microsoft.Extensions.Logging;
using Soenneker.Validators.Email.Disposable.Abstract;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Soenneker.Utils.AsyncSingleton;
using Soenneker.Utils.File.Abstract;
using System;
using System.Threading;
using Soenneker.Extensions.Enumerable.String;
using Soenneker.Extensions.String;
using Soenneker.Utils.String.Abstract;
using Soenneker.Extensions.ValueTask;

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
            IEnumerable<string> enumerable = (await fileUtil.ReadAsLines(Path.Combine(AppContext.BaseDirectory, "Resources", "data-email-disposables.txt"), true, token).NoSync()).ToLower();
            return [.. enumerable];
        });
    }

    public async ValueTask<bool> Validate(string email, bool log = false, CancellationToken cancellationToken = default)
    {
        string? domain = _stringUtil.GetDomainFromEmail(email);

        if (domain == null)
            return true;

        domain = domain.ToLowerInvariantFast();

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
        domain = domain.ToLowerInvariantFast();

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