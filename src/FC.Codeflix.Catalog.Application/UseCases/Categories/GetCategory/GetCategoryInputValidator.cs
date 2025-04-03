using FluentValidation;

namespace FC.Codeflix.Catalog.Application.UseCases.Categories.GetCategory;
public class GetCategoryInputValidator : AbstractValidator<GetCategoryInput>
{
    public GetCategoryInputValidator()
        => RuleFor(x => x.Id).NotEmpty();
}
