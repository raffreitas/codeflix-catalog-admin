using FC.Codeflix.Catalog.Api.Models.Genres;
using FC.Codeflix.Catalog.Api.Models.Responses;
using FC.Codeflix.Catalog.Application.UseCases.Genres.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genres.CreateGenre;
using FC.Codeflix.Catalog.Application.UseCases.Genres.DeleteGenre;
using FC.Codeflix.Catalog.Application.UseCases.Genres.GetGenre;
using FC.Codeflix.Catalog.Application.UseCases.Genres.ListGenres;
using FC.Codeflix.Catalog.Application.UseCases.Genres.UpdateGenre;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class GenresController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GenreModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var output = await mediator.Send(new GetGenreInput(id), cancellationToken);
        return Ok(new ApiResponse<GenreModelOutput>(output));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        await mediator.Send(new DeleteGenreInput(id), cancellationToken);
        return NoContent();
    }

    [HttpPost()]
    [ProducesResponseType(typeof(ApiResponse<GenreModelOutput>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateGenre(
        [FromBody] CreateGenreInput input,
        CancellationToken cancellationToken
    )
    {
        var output = await mediator.Send(input, cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { id = output.Id },
            new ApiResponse<GenreModelOutput>(output)
        );
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GenreModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateGenre(
        [FromBody] UpdateGenreApiInput apiInput,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var output = await mediator.Send(
            new UpdateGenreInput(
                id,
                apiInput.Name,
                apiInput.IsActive,
                apiInput.CategoriesId
            ),
            cancellationToken
        );
        return Ok(new ApiResponse<GenreModelOutput>(output));
    }


    [HttpGet]
    [ProducesResponseType(typeof(ListGenresOutput), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        CancellationToken cancellationToken,
        [FromQuery] int? page = null,
        [FromQuery(Name = "per_page")] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery] SearchOrder? dir = null
    )
    {
        var input = new ListGenresInput();
        if (page is not null) input.Page = page.Value;
        if (perPage is not null) input.PerPage = perPage.Value;
        if (!string.IsNullOrWhiteSpace(search)) input.Search = search;
        if (!string.IsNullOrWhiteSpace(sort)) input.Sort = sort;
        if (dir is not null) input.Dir = dir.Value;

        var output = await mediator.Send(input, cancellationToken);
        return Ok(
            new ApiResponseList<GenreModelOutput>(output)
        );
    }
}