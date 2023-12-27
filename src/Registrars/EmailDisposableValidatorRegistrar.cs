using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Utils.File.Registrars;
using Soenneker.Utils.String.Registrars;
using Soenneker.Validators.Email.Disposable.Abstract;

namespace Soenneker.Validators.Email.Disposable.Registrars;

/// <summary>
/// A validation module checking if a given email's domain is disposable/temporary, updated daily (if available)
/// </summary>
public static class EmailDisposableValidatorRegistrar
{
    /// <summary>
    /// Adds <see cref="IEmailDisposableValidator"/> as a singleton service. Recommended if you don't want to load the resource every time the validator is instantiated.
    /// </summary>
    public static void AddEmailDisposableValidatorAsSingleton(this IServiceCollection services)
    {
        services.TryAddSingleton<IEmailDisposableValidator, EmailDisposableValidator>();
        services.AddFileUtilAsSingleton();
        services.AddStringUtilAsSingleton();
    }

    /// <summary>
    /// Adds <see cref="IEmailDisposableValidator"/> as a scoped service. <para/>
    /// </summary>
    public static void AddEmailDisposableValidatorAsScoped(this IServiceCollection services)
    {
        services.TryAddScoped<IEmailDisposableValidator, EmailDisposableValidator>();
        services.AddFileUtilAsScoped();
        services.AddStringUtilAsScoped();
    }
}
