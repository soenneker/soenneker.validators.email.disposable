using System.Threading.Tasks;
using AwesomeAssertions;
using Soenneker.Validators.Email.Disposable.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;


namespace Soenneker.Validators.Email.Disposable.Tests;

[Collection("Collection")]
public class EmailDisposableValidatorTests : FixturedUnitTest
{
    private readonly IEmailDisposableValidator _validator;

    public EmailDisposableValidatorTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _validator = Resolve<IEmailDisposableValidator>(true);
    }

    [Fact]
    public async Task Should_NotHaveValidationError_When_EmailIsNotDisposable()
    {
        const string email = "test@gmail.com";

        bool result = await _validator.Validate(email, cancellationToken: CancellationToken);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Should_HaveValidationError_When_EmailIsDisposable()
    {
        const string email = "test@mailinator.com";

        bool result = await _validator.Validate(email, cancellationToken: CancellationToken);

        result.Should().BeFalse();
    }
}