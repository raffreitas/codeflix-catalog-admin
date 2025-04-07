using Bogus;

using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Validations;

using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Validations;

public class DomainValidationTest
{
    private Faker Faker { get; set; } = new Faker();

    [Fact(DisplayName = nameof(NotNullOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullOk()
    {
        var value = Faker.Commerce.ProductName();
        var fieldName = Faker.Lorem.Word();

        var action = () => DomainValidation.NotNull(value, fieldName);

        action.Should().NotThrow();
    }

    [Fact(DisplayName = nameof(NotNullThrowWhenNull))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullThrowWhenNull()
    {
        string? value = null;
        var fieldName = Faker.Lorem.Word();

        var action = () => DomainValidation.NotNull(value, fieldName);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should not be null.");
    }

    [Theory(DisplayName = nameof(NotNulOrEmptyThrowWhenEmpty))]
    [Trait("Domain", "DomainValidation - Validation")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void NotNulOrEmptyThrowWhenEmpty(string? target)
    {
        var fieldName = Faker.Lorem.Word();

        var action = () => DomainValidation.NotNullOrEmpty(target, fieldName);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should not be null or empty.");
    }

    [Fact(DisplayName = nameof(NotNulOrEmptyOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNulOrEmptyOk()
    {
        var target = Faker.Lorem.Word();
        var fieldName = Faker.Lorem.Word();

        var action = () => DomainValidation.NotNullOrEmpty(target, fieldName);

        action.Should().NotThrow();
    }

    public static IEnumerable<object[]> GetValuesSmallerThanMin(int numberOfTests = 5)
    {
        var faker = new Faker();
        for (int i = 0; i < numberOfTests; i++)
        {
            string example = faker.Commerce.ProductName();
            int minLength = example.Length + new Random().Next(1, 20);
            yield return new object[] { example, minLength };
        }
    }

    [Theory(DisplayName = nameof(MinLengthThrowWhenLess))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValuesSmallerThanMin), parameters: 10)]
    public void MinLengthThrowWhenLess(string target, int minLength)
    {
        var fieldName = Faker.Lorem.Word();
        var action = () => DomainValidation.MinLength(target, minLength, fieldName);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should have at least {minLength} characters.");
    }

    public static IEnumerable<object[]> GetValuesGreaterThanMin(int numberOfTests = 5)
    {
        var faker = new Faker();
        for (int i = 0; i < numberOfTests; i++)
        {
            string example = faker.Commerce.ProductName();
            int minLength = example.Length - new Random().Next(1, 20);
            yield return new object[] { example, minLength };
        }
    }

    [Theory(DisplayName = nameof(MinLengthOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValuesGreaterThanMin), parameters: 5)]
    public void MinLengthOk(string target, int minLength)
    {
        var fieldName = Faker.Lorem.Word();

        var action = () => DomainValidation.MinLength(target, minLength, fieldName);

        action.Should().NotThrow();
    }

    public static IEnumerable<object[]> GetValuesGreaterThanMax(int numberOfTests = 5)
    {
        var faker = new Faker();
        for (int i = 0; i < numberOfTests; i++)
        {
            string example = faker.Commerce.ProductName();
            int maxLength = example.Length - new Random().Next(1, 20);
            yield return new object[] { example, maxLength };
        }
    }

    [Theory(DisplayName = nameof(MaxLengthThrowWhenGreater))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValuesGreaterThanMax), parameters: 5)]
    public void MaxLengthThrowWhenGreater(string target, int maxLength)
    {
        var fieldName = Faker.Lorem.Word();

        var action = () => DomainValidation.MaxLength(target, maxLength, fieldName);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should be less or equal {maxLength} characters.");
    }

    public static IEnumerable<object[]> GetValuesSmallerThanMax(int numberOfTests = 5)
    {
        var faker = new Faker();
        for (int i = 0; i < numberOfTests; i++)
        {
            string example = faker.Commerce.ProductName();
            int maxLength = example.Length + new Random().Next(1, 20);
            yield return new object[] { example, maxLength };
        }
    }

    [Theory(DisplayName = nameof(MaxLengthOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValuesSmallerThanMax), parameters: 5)]
    public void MaxLengthOk(string target, int maxLength)
    {
        var fieldName = Faker.Lorem.Word();

        var action = () => DomainValidation.MaxLength(target, maxLength, fieldName);

        action.Should().NotThrow();
    }
}