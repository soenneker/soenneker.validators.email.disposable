using System.Threading.Tasks;
using AwesomeAssertions;
using Soenneker.Validators.Email.Disposable.Abstract;
using Soenneker.Tests.HostedUnit;


namespace Soenneker.Validators.Email.Disposable.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class EmailDisposableValidatorTests : HostedUnitTest
{
    private readonly IEmailDisposableValidator _validator;

    public EmailDisposableValidatorTests(Host host) : base(host)
    {
        _validator = Resolve<IEmailDisposableValidator>(true);
    }

    [Test]
    public async Task Should_NotHaveValidationError_When_EmailIsNotDisposable()
    {
        const string email = "test@gmail.com";

        bool result = await _validator.Validate(email, cancellationToken: System.Threading.CancellationToken.None);

        result.Should().BeTrue();
    }

    [Test]
    public async Task Should_HaveValidationError_When_EmailIsDisposable()
    {
        const string email = "test@mailinator.com";

        bool result = await _validator.Validate(email, cancellationToken: System.Threading.CancellationToken.None);

        result.Should().BeFalse();
    }
}
