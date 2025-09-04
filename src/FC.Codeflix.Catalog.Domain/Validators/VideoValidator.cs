using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Validations;

namespace FC.Codeflix.Catalog.Domain.Validators;

public class VideoValidator(Video video, ValidationHandler handler) : Validator(handler)
{
    private const int MaxTitleLength = 255;
    private const int MaxDescriptionLength = 4000;

    public override void Validate()
    {
        ValidateTitle();

        if (string.IsNullOrWhiteSpace(video.Description))
            _handler.HandleError("'Description' is required");
        if (video.Description.Length > MaxTitleLength)
            _handler.HandleError(
                $"'{nameof(video.Description)}' should be less or equal {MaxDescriptionLength} characters long");
    }

    private void ValidateTitle()
    {
        if (string.IsNullOrWhiteSpace(video.Title))
            _handler.HandleError($"'{nameof(video.Title)}' is required");

        if (video.Title.Length > 255)
            _handler.HandleError($"'{nameof(video.Title)}' should be less or equal {MaxTitleLength} characters long");
    }
}