using FC.Codeflix.Catalog.Application.UseCases.Videos.Common;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Videos.UploadMedias;

public sealed record UploadMediasInput(Guid VideoId, FileInput? VideoFile, FileInput? TrailerFile) : IRequest;