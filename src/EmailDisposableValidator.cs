using Microsoft.Extensions.Logging;
using Soenneker.Validators.Email.Disposable.Abstract;
using System.Collections.Generic;
using System.Threading.Tasks;
using Soenneker.Utils.AsyncSingleton;
using Soenneker.Utils.File.Abstract;
using Soenneker.Utils.Paths.Resources.Abstract;
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
    private readonly IFileUtil _fileUtil;
    private readonly IResourcesPathUtil _resourcesPathUtil;

    public EmailDisposableValidator(IFileUtil fileUtil, IResourcesPathUtil resourcesPathUtil, IStringUtil stringUtil, ILogger<EmailDisposableValidator> logger) : base(logger)
    {
        _fileUtil = fileUtil;
        _resourcesPathUtil = resourcesPathUtil;
        _stringUtil = stringUtil;

        _emailDomainsSet = new AsyncSingleton<HashSet<string>>(CreateEmailDomainsSet);
    }

    private async ValueTask<HashSet<string>> CreateEmailDomainsSet(CancellationToken token)
    {
        string path = await _resourcesPathUtil.GetResourceFilePath("data-email-disposables.txt", token).NoSync();

        return await _fileUtil.ReadToHashSet(path, StringComparer.InvariantCultureIgnoreCase, cancellationToken: token)
            .NoSync();
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
        if ((await _emailDomainsSet.Get(cancellationToken)
                .NoSync()).Contains(domain))
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

        if ((await _emailDomainsSet.Get(cancellationToken)
                .NoSync()).Contains(domain))
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