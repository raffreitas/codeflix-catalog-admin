using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Videos.Common;
using FC.Codeflix.Catalog.Application.UseCases.Videos.UpdateVideo;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Extensions;
using FC.Codeflix.Catalog.Domain.Repositories;

using FluentAssertions;

using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Videos.UpdateVideo;

[Trait("Application", "UpdateVideo - Use Cases")]
public sealed class UpdateVideoTest : IClassFixture<UpdateVideoTestFixture>
{
    private readonly Mock<IVideoRepository> _videoRepositoryMock = new();
    private readonly Mock<IGenreRepository> _genreRepositoryMock = new();
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();
    private readonly Mock<ICastMemberRepository> _castMemberRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IStorageService> _storageServiceMock = new();

    private readonly UpdateVideoTestFixture _fixture;
    private readonly UpdateVideoUseCase _useCase;

    public UpdateVideoTest(UpdateVideoTestFixture fixture)
    {
        _fixture = fixture;
        _useCase = new UpdateVideoUseCase(
            _videoRepositoryMock.Object,
            _genreRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _castMemberRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _storageServiceMock.Object
        );
    }

    [Fact]
    public async Task UpdateVideosBasicInfo()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var input = _fixture.CreateValidInput(exampleVideo.Id);
        _videoRepositoryMock.Setup(repository =>
            repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleVideo);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(repository => repository.Update(
            It.Is<Video>(video =>
                ((video.Id == exampleVideo.Id) &&
                 (video.Title == input.Title) &&
                 (video.Description == input.Description) &&
                 (video.Rating == input.Rating) &&
                 (video.YearLaunched == input.YearLaunched) &&
                 (video.Opened == input.Opened) &&
                 (video.Published == input.Published) &&
                 (video.Duration == input.Duration))), It.IsAny<CancellationToken>()
        ), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithoutRelationsWithRelations))]
    public async Task UpdateVideosWithoutRelationsWithRelations()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var examplesGenreIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var examplesCastMembersIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var examplesCategoriesIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var input = _fixture.CreateValidInput(exampleVideo.Id,
            genreIds: examplesGenreIds,
            categoryIds: examplesCategoriesIds,
            castMemberIds: examplesCastMembersIds);
        _videoRepositoryMock.Setup(repository =>
                repository.Get(
                    It.Is<Guid>(id => id == exampleVideo.Id),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _genreRepositoryMock.Setup(x =>
                x.GetIdsListByIds(
                    It.Is<List<Guid>>(idsList =>
                        idsList.Count == examplesGenreIds.Count &&
                        idsList.All(id => examplesGenreIds.Contains(id))),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(examplesGenreIds);
        _castMemberRepositoryMock.Setup(x =>
                x.GetIdsListByIds(
                    It.Is<List<Guid>>(idsList =>
                        idsList.Count == examplesCastMembersIds.Count &&
                        idsList.All(id => examplesCastMembersIds.Contains(id))),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(examplesCastMembersIds);
        _categoryRepositoryMock.Setup(x =>
                x.GetIdsListByIds(
                    It.Is<List<Guid>>(idsList =>
                        idsList.Count == examplesCategoriesIds.Count &&
                        idsList.All(id => examplesCategoriesIds.Contains(id))),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(examplesCategoriesIds);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.VerifyAll();
        _genreRepositoryMock.VerifyAll();
        _categoryRepositoryMock.VerifyAll();
        _castMemberRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(repository => repository.Update(
                It.Is<Video>(video =>
                    (video.Id == exampleVideo.Id) &&
                    (video.Title == input.Title) &&
                    (video.Description == input.Description) &&
                    (video.Rating == input.Rating) &&
                    (video.YearLaunched == input.YearLaunched) &&
                    (video.Opened == input.Opened) &&
                    (video.Published == input.Published) &&
                    (video.Duration == input.Duration) &&
                    video.Genres.All(genreId => examplesGenreIds.Contains(genreId)) &&
                    (video.Genres.Count == examplesGenreIds.Count) &&
                    video.Categories.All(id => examplesCategoriesIds.Contains(id)) &&
                    (video.Categories.Count == examplesCategoriesIds.Count) &&
                    video.CastMembers.All(id => examplesCastMembersIds.Contains(id)) &&
                    (video.CastMembers.Count == examplesCastMembersIds.Count)
                ), It.IsAny<CancellationToken>())
            , Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Genres.Select(genre => genre.Id).ToList()
            .Should().BeEquivalentTo(examplesGenreIds);
        output.Categories.Select(category => category.Id).ToList()
            .Should().BeEquivalentTo(examplesCategoriesIds);
        output.CastMembers.Select(castMember => castMember.Id).ToList()
            .Should().BeEquivalentTo(examplesCastMembersIds);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithGenreIds))]
    public async Task UpdateVideosWithGenreIds()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var examplesGenreIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var input = _fixture.CreateValidInput(exampleVideo.Id, examplesGenreIds);
        _videoRepositoryMock.Setup(repository =>
                repository.Get(
                    It.Is<Guid>(id => id == exampleVideo.Id),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _genreRepositoryMock.Setup(x =>
                x.GetIdsListByIds(
                    It.Is<List<Guid>>(idsList =>
                        idsList.Count == examplesGenreIds.Count &&
                        idsList.All(id => examplesGenreIds.Contains(id))),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(examplesGenreIds);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.VerifyAll();
        _genreRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(repository => repository.Update(
                It.Is<Video>(video =>
                    ((video.Id == exampleVideo.Id) &&
                     (video.Title == input.Title) &&
                     (video.Description == input.Description) &&
                     (video.Rating == input.Rating) &&
                     (video.YearLaunched == input.YearLaunched) &&
                     (video.Opened == input.Opened) &&
                     (video.Published == input.Published) &&
                     (video.Duration == input.Duration) &&
                     video.Genres.All(genreId => examplesGenreIds.Contains(genreId) &&
                                                 (video.Genres.Count == examplesGenreIds.Count))))
                , It.IsAny<CancellationToken>())
            , Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Genres.Select(genre => genre.Id).ToList()
            .Should().BeEquivalentTo(examplesGenreIds);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithRelationsToOtherRelations))]
    public async Task UpdateVideosWithRelationsToOtherRelations()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var examplesGenreIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var examplesCastMembersIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var examplesCategoriesIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var input = _fixture.CreateValidInput(exampleVideo.Id,
            genreIds: examplesGenreIds,
            categoryIds: examplesCategoriesIds,
            castMemberIds: examplesCastMembersIds);
        _videoRepositoryMock.Setup(repository =>
                repository.Get(
                    It.Is<Guid>(id => id == exampleVideo.Id),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _genreRepositoryMock.Setup(x =>
                x.GetIdsListByIds(
                    It.Is<List<Guid>>(idsList =>
                        idsList.Count == examplesGenreIds.Count &&
                        idsList.All(id => examplesGenreIds.Contains(id))),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(examplesGenreIds);
        _castMemberRepositoryMock.Setup(x =>
                x.GetIdsListByIds(
                    It.Is<List<Guid>>(idsList =>
                        idsList.Count == examplesCastMembersIds.Count &&
                        idsList.All(id => examplesCastMembersIds.Contains(id))),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(examplesCastMembersIds);
        _categoryRepositoryMock.Setup(x =>
                x.GetIdsListByIds(
                    It.Is<List<Guid>>(idsList =>
                        idsList.Count == examplesCategoriesIds.Count &&
                        idsList.All(id => examplesCategoriesIds.Contains(id))),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(examplesCategoriesIds);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.VerifyAll();
        _genreRepositoryMock.VerifyAll();
        _categoryRepositoryMock.VerifyAll();
        _castMemberRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(repository => repository.Update(
                It.Is<Video>(video =>
                    (video.Id == exampleVideo.Id) &&
                    (video.Title == input.Title) &&
                    (video.Description == input.Description) &&
                    (video.Rating == input.Rating) &&
                    (video.YearLaunched == input.YearLaunched) &&
                    (video.Opened == input.Opened) &&
                    (video.Published == input.Published) &&
                    (video.Duration == input.Duration) &&
                    video.Genres.All(genreId => examplesGenreIds.Contains(genreId)) &&
                    (video.Genres.Count == examplesGenreIds.Count) &&
                    video.Categories.All(id => examplesCategoriesIds.Contains(id)) &&
                    (video.Categories.Count == examplesCategoriesIds.Count) &&
                    video.CastMembers.All(id => examplesCastMembersIds.Contains(id)) &&
                    (video.CastMembers.Count == examplesCastMembersIds.Count)
                ), It.IsAny<CancellationToken>())
            , Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Genres.Select(genre => genre.Id).ToList()
            .Should().BeEquivalentTo(examplesGenreIds);
        output.Categories.Select(category => category.Id).ToList()
            .Should().BeEquivalentTo(examplesCategoriesIds);
        output.CastMembers.Select(castMember => castMember.Id).ToList()
            .Should().BeEquivalentTo(examplesCastMembersIds);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithRelationsRemovingRelations))]
    public async Task UpdateVideosWithRelationsRemovingRelations()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.CreateValidInput(exampleVideo.Id,
            genreIds: [],
            categoryIds: [],
            castMemberIds: []);
        _videoRepositoryMock.Setup(repository =>
                repository.Get(
                    It.Is<Guid>(id => id == exampleVideo.Id),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Genres.Should().BeEmpty();
        output.Categories.Should().BeEmpty();
        output.CastMembers.Should().BeEmpty();
        _videoRepositoryMock.VerifyAll();
        _genreRepositoryMock.Verify(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
        _categoryRepositoryMock.Verify(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
        _castMemberRepositoryMock.Verify(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);

        _videoRepositoryMock.Verify(repository => repository.Update(
                It.Is<Video>(video =>
                    (video.Id == exampleVideo.Id) &&
                    (video.Title == input.Title) &&
                    (video.Description == input.Description) &&
                    (video.Rating == input.Rating) &&
                    (video.YearLaunched == input.YearLaunched) &&
                    (video.Opened == input.Opened) &&
                    (video.Published == input.Published) &&
                    (video.Duration == input.Duration) &&
                    (video.Genres.Count == 0) &&
                    (video.Categories.Count == 0) &&
                    (video.CastMembers.Count == 0)
                ), It.IsAny<CancellationToken>())
            , Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithRelationsKeepRelationWhenReceiveNullInRelations))]
    public async Task UpdateVideosWithRelationsKeepRelationWhenReceiveNullInRelations()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.CreateValidInput(exampleVideo.Id,
            genreIds: null,
            categoryIds: null,
            castMemberIds: null);
        _videoRepositoryMock.Setup(repository =>
                repository.Get(
                    It.Is<Guid>(id => id == exampleVideo.Id),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Genres.Select(genre => genre.Id).ToList()
            .Should().BeEquivalentTo(exampleVideo.Genres);
        output.Categories.Select(category => category.Id).ToList()
            .Should().BeEquivalentTo(exampleVideo.Categories);
        output.CastMembers.Select(castMember => castMember.Id).ToList()
            .Should().BeEquivalentTo(exampleVideo.CastMembers);
        _videoRepositoryMock.VerifyAll();
        _genreRepositoryMock.Verify(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
        _categoryRepositoryMock.Verify(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
        _castMemberRepositoryMock.Verify(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
        _videoRepositoryMock.Verify(repository => repository.Update(
                It.Is<Video>(video =>
                    (video.Id == exampleVideo.Id) &&
                    (video.Title == input.Title) &&
                    (video.Description == input.Description) &&
                    (video.Rating == input.Rating) &&
                    (video.YearLaunched == input.YearLaunched) &&
                    (video.Opened == input.Opened) &&
                    (video.Published == input.Published) &&
                    (video.Duration == input.Duration) &&
                    (video.Genres.Count == exampleVideo.Genres.Count) &&
                    (video.Genres.All(id => exampleVideo.Genres.Contains(id))) &&
                    (video.Categories.Count == exampleVideo.Categories.Count) &&
                    (video.Categories.All(id => exampleVideo.Categories.Contains(id))) &&
                    (video.CastMembers.Count == exampleVideo.CastMembers.Count) &&
                    (video.CastMembers.All(id => exampleVideo.CastMembers.Contains(id)))
                ), It.IsAny<CancellationToken>())
            , Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithCategoryIds))]
    public async Task UpdateVideosWithCategoryIds()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var exampleIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var input = _fixture.CreateValidInput(exampleVideo.Id, categoryIds: exampleIds);
        _videoRepositoryMock.Setup(repository =>
                repository.Get(
                    It.Is<Guid>(id => id == exampleVideo.Id),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _categoryRepositoryMock.Setup(x =>
                x.GetIdsListByIds(
                    It.Is<List<Guid>>(idsList =>
                        idsList.Count == exampleIds.Count &&
                        idsList.All(id => exampleIds.Contains(id))),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleIds);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.VerifyAll();
        _genreRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(repository => repository.Update(
            It.Is<Video>(video =>
                ((video.Id == exampleVideo.Id) &&
                 (video.Title == input.Title) &&
                 (video.Description == input.Description) &&
                 (video.Rating == input.Rating) &&
                 (video.YearLaunched == input.YearLaunched) &&
                 (video.Opened == input.Opened) &&
                 (video.Published == input.Published) &&
                 (video.Duration == input.Duration) &&
                 video.Categories.All(genreId => exampleIds.Contains(genreId) &&
                                                 (video.Categories.Count == exampleIds.Count)))),
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Categories.Select(category => category.Id).ToList()
            .Should().BeEquivalentTo(exampleIds);
    }

    [Fact(DisplayName = nameof(UpdateVideosThrowsWhenInvalidGenreId))]
    public async Task UpdateVideosThrowsWhenInvalidGenreId()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var examplesGenreIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var invalidGenreId = Guid.NewGuid();
        var inputInvalidIdsList = examplesGenreIds
            .Concat(new List<Guid>() { invalidGenreId }).ToList();
        var input = _fixture.CreateValidInput(exampleVideo.Id, inputInvalidIdsList);
        _videoRepositoryMock.Setup(repository =>
                repository.Get(
                    It.Is<Guid>(id => id == exampleVideo.Id),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _genreRepositoryMock.Setup(x =>
                x.GetIdsListByIds(
                    It.IsAny<List<Guid>>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(examplesGenreIds);

        var action = () => _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related genre id (or ids) not found: {invalidGenreId}.");
        _videoRepositoryMock.VerifyAll();
        _genreRepositoryMock.VerifyAll();
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithCastMemberIds))]
    public async Task UpdateVideosWithCastMemberIds()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var exampleIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var input = _fixture.CreateValidInput(exampleVideo.Id, castMemberIds: exampleIds);
        _videoRepositoryMock.Setup(repository =>
                repository.Get(
                    It.Is<Guid>(id => id == exampleVideo.Id),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _castMemberRepositoryMock.Setup(x =>
                x.GetIdsListByIds(
                    It.Is<List<Guid>>(idsList =>
                        idsList.Count == exampleIds.Count &&
                        idsList.All(id => exampleIds.Contains(id))),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleIds);

        var output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.VerifyAll();
        _genreRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(repository => repository.Update(
                It.Is<Video>(video =>
                    ((video.Id == exampleVideo.Id) &&
                     (video.Title == input.Title) &&
                     (video.Description == input.Description) &&
                     (video.Rating == input.Rating) &&
                     (video.YearLaunched == input.YearLaunched) &&
                     (video.Opened == input.Opened) &&
                     (video.Published == input.Published) &&
                     (video.Duration == input.Duration) &&
                     video.CastMembers.All(genreId => exampleIds.Contains(genreId) &&
                                                      (video.CastMembers.Count == exampleIds.Count))))
                , It.IsAny<CancellationToken>())
            , Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.CastMembers.Select(castMember => castMember.Id).ToList()
            .Should().BeEquivalentTo(exampleIds);
    }

    [Fact(DisplayName = nameof(UpdateVideosThrowsWhenInvalidGenreId))]
    public async Task UpdateVideosThrowsWhenInvalidCastMemberId()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var examplesGenreIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var invalidGenreId = Guid.NewGuid();
        var inputInvalidIdsList = examplesGenreIds
            .Concat(new List<Guid>() { invalidGenreId }).ToList();
        var input = _fixture.CreateValidInput(exampleVideo.Id, inputInvalidIdsList);
        _videoRepositoryMock.Setup(repository =>
                repository.Get(
                    It.Is<Guid>(id => id == exampleVideo.Id),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _genreRepositoryMock.Setup(x =>
                x.GetIdsListByIds(
                    It.IsAny<List<Guid>>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(examplesGenreIds);

        var action = () => _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related genre id (or ids) not found: {invalidGenreId}.");
        _videoRepositoryMock.VerifyAll();
        _videoRepositoryMock.VerifyAll();
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory(DisplayName = nameof(UpdateVideosThrowsWhenReceiveInvalidInput))]
    [ClassData(typeof(UpdateVideoTestDataGenerator))]
    public async Task UpdateVideosThrowsWhenReceiveInvalidInput(
        UpdateVideoInput invalidInput,
        string expectedExceptionMessage)
    {
        var exampleVideo = _fixture.GetValidVideo();
        _videoRepositoryMock.Setup(repository => repository
                .Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var action = () => _useCase.Handle(invalidInput, CancellationToken.None);

        var exceptionAssertion = await action.Should().ThrowAsync<EntityValidationException>()
            .WithMessage("There are validation errors");
        exceptionAssertion.Which.Errors!.ToList()[0].Message.Should()
            .Be(expectedExceptionMessage);
        _videoRepositoryMock.VerifyAll();
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(UpdateVideosThrowsWhenVideoNotFound))]
    public async Task UpdateVideosThrowsWhenVideoNotFound()
    {
        var input = _fixture.CreateValidInput(Guid.NewGuid());
        _videoRepositoryMock.Setup(repository =>
                repository.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Video not found"));

        var action = () => _useCase.Handle(input, CancellationToken.None);
        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Video not found");

        _videoRepositoryMock.Verify(repository => repository.Update(
                It.IsAny<Video>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithBannerWhenVideoHasNoBanner))]
    public async Task UpdateVideosWithBannerWhenVideoHasNoBanner()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var input = _fixture.CreateValidInput(exampleVideo.Id, banner: _fixture.GetValidImageFileInput());
        var bannerPath = $"storage/banner.{input.Banner!.Extension}";
        _videoRepositoryMock.Setup(repository =>
            repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleVideo);
        _storageServiceMock.Setup(x => x.Upload(
            It.Is<string>(filename =>
                filename == StorageFileName.Create(
                    exampleVideo.Id,
                    nameof(exampleVideo.Banner),
                    input.Banner!.Extension)),
            It.IsAny<MemoryStream>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(bannerPath);

        var output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.BannerFileUrl.Should().Be(bannerPath);
        _storageServiceMock.VerifyAll();
        _videoRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(repository => repository.Update(
                It.Is<Video>(video =>
                    (video.Id == exampleVideo.Id) &&
                    (video.Title == input.Title) &&
                    (video.Description == input.Description) &&
                    (video.Rating == input.Rating) &&
                    (video.YearLaunched == input.YearLaunched) &&
                    (video.Opened == input.Opened) &&
                    (video.Published == input.Published) &&
                    (video.Duration == input.Duration) &&
                    (video.Banner!.Path == bannerPath)
                ), It.IsAny<CancellationToken>())
            , Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact(DisplayName = nameof(UpdateVideosKeepBannerWhenReceiveNull))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosKeepBannerWhenReceiveNull()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.CreateValidInput(exampleVideo.Id,
            banner: null);
        _videoRepositoryMock.Setup(repository =>
                repository.Get(
                    It.Is<Guid>(id => id == exampleVideo.Id),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.BannerFileUrl.Should().Be(exampleVideo.Banner!.Path);
        _videoRepositoryMock.VerifyAll();
        _storageServiceMock.Verify(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>())
            , Times.Never);
        _videoRepositoryMock.Verify(repository => repository.Update(
                It.Is<Video>(video =>
                    (video.Id == exampleVideo.Id) &&
                    (video.Title == input.Title) &&
                    (video.Description == input.Description) &&
                    (video.Rating == input.Rating) &&
                    (video.YearLaunched == input.YearLaunched) &&
                    (video.Opened == input.Opened) &&
                    (video.Published == input.Published) &&
                    (video.Duration == input.Duration) &&
                    (video.Banner!.Path == exampleVideo.Banner!.Path)
                ), It.IsAny<CancellationToken>())
            , Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithThumbWhenVideoHasNoThumb))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosWithThumbWhenVideoHasNoThumb()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var input = _fixture.CreateValidInput(exampleVideo.Id,
            thumb: _fixture.GetValidImageFileInput());
        var path = $"storage/thumb.{input.Thumb!.Extension}";
        _videoRepositoryMock.Setup(repository =>
                repository.Get(
                    It.Is<Guid>(id => id == exampleVideo.Id),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _storageServiceMock.Setup(x => x.Upload(
            It.Is<string>(name => (name == StorageFileName.Create(
                exampleVideo.Id,
                nameof(exampleVideo.Thumb),
                input.Thumb!.Extension))
            ),
            It.IsAny<MemoryStream>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(path);

        var output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.ThumbFileUrl.Should().Be(path);
        _videoRepositoryMock.VerifyAll();
        _storageServiceMock.VerifyAll();
        _videoRepositoryMock.Verify(repository => repository.Update(
                It.Is<Video>(video =>
                    (video.Id == exampleVideo.Id) &&
                    (video.Title == input.Title) &&
                    (video.Description == input.Description) &&
                    (video.Rating == input.Rating) &&
                    (video.YearLaunched == input.YearLaunched) &&
                    (video.Opened == input.Opened) &&
                    (video.Published == input.Published) &&
                    (video.Duration == input.Duration) &&
                    (video.Thumb!.Path == path)
                ), It.IsAny<CancellationToken>())
            , Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideosKeepThumbWhenReceiveNull))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosKeepThumbWhenReceiveNull()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.CreateValidInput(exampleVideo.Id,
            thumb: null);
        _videoRepositoryMock.Setup(repository =>
                repository.Get(
                    It.Is<Guid>(id => id == exampleVideo.Id),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.ThumbFileUrl.Should().Be(exampleVideo.Thumb!.Path);
        _videoRepositoryMock.VerifyAll();
        _storageServiceMock.Verify(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>())
            , Times.Never);
        _videoRepositoryMock.Verify(repository => repository.Update(
                It.Is<Video>(video =>
                    (video.Id == exampleVideo.Id) &&
                    (video.Title == input.Title) &&
                    (video.Description == input.Description) &&
                    (video.Rating == input.Rating) &&
                    (video.YearLaunched == input.YearLaunched) &&
                    (video.Opened == input.Opened) &&
                    (video.Published == input.Published) &&
                    (video.Duration == input.Duration) &&
                    (video.Thumb!.Path == exampleVideo.Thumb!.Path)
                ), It.IsAny<CancellationToken>())
            , Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithThumbHalfWhenVideoHasNoThumbHalf))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosWithThumbHalfWhenVideoHasNoThumbHalf()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var input = _fixture.CreateValidInput(exampleVideo.Id,
            thumbHalf: _fixture.GetValidImageFileInput());
        var path = $"storage/thumb-half.{input.ThumbHalf!.Extension}";
        _videoRepositoryMock.Setup(repository =>
                repository.Get(
                    It.Is<Guid>(id => id == exampleVideo.Id),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _storageServiceMock.Setup(x => x.Upload(
            It.Is<string>(name => (name == StorageFileName.Create(
                exampleVideo.Id,
                nameof(exampleVideo.ThumbHalf),
                input.ThumbHalf!.Extension))
            ),
            It.IsAny<MemoryStream>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(path);

        var output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.ThumbHalfFileUrl.Should().Be(path);
        _videoRepositoryMock.VerifyAll();
        _storageServiceMock.VerifyAll();
        _videoRepositoryMock.Verify(repository => repository.Update(
                It.Is<Video>(video =>
                    (video.Id == exampleVideo.Id) &&
                    (video.Title == input.Title) &&
                    (video.Description == input.Description) &&
                    (video.Rating == input.Rating) &&
                    (video.YearLaunched == input.YearLaunched) &&
                    (video.Opened == input.Opened) &&
                    (video.Published == input.Published) &&
                    (video.Duration == input.Duration) &&
                    (video.ThumbHalf!.Path == path)
                ), It.IsAny<CancellationToken>())
            , Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideosKeepThumbHalfWhenReceiveNull))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosKeepThumbHalfWhenReceiveNull()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.CreateValidInput(exampleVideo.Id,
            thumbHalf: null);
        _videoRepositoryMock.Setup(repository =>
                repository.Get(
                    It.Is<Guid>(id => id == exampleVideo.Id),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.ThumbHalfFileUrl.Should().Be(exampleVideo.ThumbHalf!.Path);
        _videoRepositoryMock.VerifyAll();
        _storageServiceMock.Verify(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>())
            , Times.Never);
        _videoRepositoryMock.Verify(repository => repository.Update(
                It.Is<Video>(video =>
                    (video.Id == exampleVideo.Id) &&
                    (video.Title == input.Title) &&
                    (video.Description == input.Description) &&
                    (video.Rating == input.Rating) &&
                    (video.YearLaunched == input.YearLaunched) &&
                    (video.Opened == input.Opened) &&
                    (video.Published == input.Published) &&
                    (video.Duration == input.Duration) &&
                    (video.ThumbHalf!.Path == exampleVideo.ThumbHalf!.Path)
                ), It.IsAny<CancellationToken>())
            , Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
}