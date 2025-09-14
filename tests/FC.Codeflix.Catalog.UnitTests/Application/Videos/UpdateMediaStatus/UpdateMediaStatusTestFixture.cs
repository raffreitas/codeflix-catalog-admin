﻿using FC.Codeflix.Catalog.Application.UseCases.Videos.UpdateMediaStatus;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.UnitTests.Common.Fixtures;

namespace FC.Codeflix.Catalog.UnitTests.Application.Videos.UpdateMediaStatus;

public sealed class UpdateMediaStatusTestFixture : VideoTestFixtureBase
{
    public UpdateMediaStatusInput GetSucceededEncodingInput(Guid videoId) => new(
        videoId,
        MediaStatus.Completed,
        EncodedPath: GetValidMediaPath());

    public UpdateMediaStatusInput GetFailedEncodingInput(Guid videoId) => new(
        videoId,
        MediaStatus.Error,
        ErrorMessage: "There was an error while trying to encode video.");

    public UpdateMediaStatusInput GetInvalidStatusInput(Guid videoId) => new(
        videoId,
        MediaStatus.Processing,
        ErrorMessage: "There was an error while trying to encode video.");
}