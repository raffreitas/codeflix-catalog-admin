using FC.Codeflix.Catalog.Application.UseCases.Categories.Common;
using FC.Codeflix.Catalog.Application.UseCases.Categories.CreateCategory;
using FC.Codeflix.Catalog.Application.UseCases.Categories.DeleteCategory;
using FC.Codeflix.Catalog.Application.UseCases.Categories.GetCategory;
using FC.Codeflix.Catalog.Application.UseCases.Categories.UpdateCategory;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoriesController(IMediator mediator) : ControllerBase
{
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
        [FromBody] UpdateCategoryInput input,
        CancellationToken cancellationToken
    )
    {
        var output = await mediator.Send(input, cancellationToken);
        return Ok(output);
    }
}
