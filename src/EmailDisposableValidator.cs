using Microsoft.Extensions.Logging;
using Soenneker.Validators.Email.Disposable.Abstract;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Soenneker.Utils.AsyncSingleton;
using Soenneker.Utils.File.Abstract;
using System;
using Soenneker.Extensions.Enumerable.String;
using Soenneker.Utils.String.Abstract;

namespace Soenneker.Validators.Email.Disposable;

/// <inheritdoc cref="IEmailDisposableValidator"/>
public class EmailDisposableValidator : Validator.Validator, IEmailDisposableValidator
{
    private readonly IStringUtil _stringUtil;
    private readonly AsyncSingleton<HashSet<string>> _emailDomainsSet;

    public EmailDisposableValidator(IFileUtil fileUtil, IStringUtil stringUtil, ILogger<EmailDisposableValidator> logger) : base(logger)
    {
        _stringUtil = stringUtil;
        _emailDomainsSet = new AsyncSingleton<HashSet<string>>(async () =>
        {
            IEnumerable<string> enumerable = (await fileUtil.ReadFileAsLines(Path.Combine("Resources", "data-email-disposables.txt"))).ToLower();
            var hashSet = new HashSet<string>(enumerable);
            return hashSet;
        });
    }

    public async ValueTask<bool> Validate(string email, bool log = false)
    {
        string? domain = _stringUtil.GetDomainFromEmail(email);

        if (domain == null)
            return true;

        domain = domain.ToLowerInvariant();

        if ((await _emailDomainsSet.Get()).Contains(domain))
        {
            if (log)
                Logger.LogWarning("Email ({email}) detected as disposable", email);

            return false;
        }

        return true;
    }

    public async ValueTask<bool> ValidateDomain(string domain, bool log = false)
    {
        domain = domain.ToLowerInvariant();

        if ((await _emailDomainsSet.Get()).Contains(domain))
        {
            if (log)
                Logger.LogWarning("Domain ({domain}) detected as disposable", domain);

            return false;
        }

        return true;
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        return _emailDomainsSet.DisposeAsync();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _emailDomainsSet.Dispose();
    }
}