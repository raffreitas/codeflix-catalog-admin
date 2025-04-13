using FC.Codeflix.Catalog.Api.Models.Categories;
using FC.Codeflix.Catalog.Application.UseCases.Categories.Common;
using FC.Codeflix.Catalog.Application.UseCases.Categories.CreateCategory;
using FC.Codeflix.Catalog.Application.UseCases.Categories.DeleteCategory;
using FC.Codeflix.Catalog.Application.UseCases.Categories.GetCategory;
using FC.Codeflix.Catalog.Application.UseCases.Categories.ListCategories;
using FC.Codeflix.Catalog.Application.UseCases.Categories.UpdateCategory;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoriesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<ListCategoriesOutput>(StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        CancellationToken cancellationToken,
       [FromQuery] int? page = null,
       [FromQuery] int? perPage = null,
       [FromQuery] string? search = null,
       [FromQuery] string? sort = null,
       [FromQuery] SearchOrder? dir = null
    )
    {
        var input = new ListCategoriesInput();

        if (page is not null) input.Page = page.Value;
        if (perPage is not null) input.PerPage = perPage.Value;
        if (!string.IsNullOrWhiteSpace(search)) input.Search = search;
        if (!string.IsNullOrWhiteSpace(sort)) input.Sort = sort;
        if (dir is not null) input.Dir = dir.Value;

        var output = await mediator.Send(input, cancellationToken);
        return Ok(output);
    }

    [HttpPost]
    [ProducesResponseType<CategoryModelOutput>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryInput input,
        CancellationToken cancellationToken
    )
    {
        var output = await mediator.Send(input, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { output.Id },
            output
        );
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<CategoryModelOutput>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var output = await mediator.Send(new GetCategoryInput(id), cancellationToken);
        return Ok(output);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        await mediator.Send(new DeleteCategoryInput(id), cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<CategoryModelOutput>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromRoute] Guid id,
        [FromBody] UpdateCategoryApiInput apiInput,
        CancellationToken cancellationToken
    )
    {
        var input = new UpdateCategoryInput(
            id, 
            apiInput.Name, 
            apiInput.Description, 
            apiInput.IsActive);

        var output = await mediator.Send(input, cancellationToken);
        return Ok(output);
    }
}
