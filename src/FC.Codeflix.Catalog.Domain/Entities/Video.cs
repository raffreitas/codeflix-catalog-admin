using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Events;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.Validations;
using FC.Codeflix.Catalog.Domain.Validators;
using FC.Codeflix.Catalog.Domain.ValueObjects;

namespace FC.Codeflix.Catalog.Domain.Entities;

public sealed class Video : AggregateRoot
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public int YearLaunched { get; private set; }
    public int Duration { get; private set; }
    public bool Opened { get; private set; }
    public bool Published { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Rating Rating { get; private set; }
    public Image? Thumb { get; private set; }
    public Image? ThumbHalf { get; private set; }
    public Image? Banner { get; private set; }
    public Media? Media { get; private set; }
    public Media? Trailer { get; private set; }

    private readonly List<Guid> _categories = [];
    public IReadOnlyList<Guid> Categories => [.._categories];

    private readonly List<Guid> _genres = [];
    public IReadOnlyList<Guid> Genres => [.._genres];

    private readonly List<Guid> _castMembers = [];
    public IReadOnlyList<Guid> CastMembers => [.._castMembers];

    public Video(
        string title,
        string description,
        int yearLaunched,
        int duration,
        bool opened,
        bool published,
        Rating rating)
    {
        Title = title;
        Description = description;
        YearLaunched = yearLaunched;
        Duration = duration;
        Opened = opened;
        Published = published;
        Rating = rating;

        CreatedAt = DateTime.UtcNow;
    }

    public void Update(
        string title,
        string description,
        int yearLaunched,
        int duration,
        bool opened,
        bool published,
        Rating? rating = null
    )
    {
        Title = title;
        Description = description;
        YearLaunched = yearLaunched;
        Duration = duration;
        Opened = opened;
        Published = published;
        Rating = rating ?? Rating;
    }

    public void UpdateThumb(string path)
        => Thumb = new Image(path);

    public void UpdateThumbHalf(string path)
        => ThumbHalf = new Image(path);

    public void UpdateBanner(string path)
        => Banner = new Image(path);

    public void UpdateMedia(string path)
    {
        Media = new Media(path);
        RaiseEvent(new VideoUploadedEvent(Id, path));
    }

    public void UpdateTrailer(string path)
        => Trailer = new Media(path);

    public void UpdateSentToEncode()
    {
        if (Media is null)
            throw new EntityValidationException("There is no Media");
        Media.UpdateSentToEncode();
    }

    public void UpdateAsEncodingError()
    {
        if (Media is null)
            throw new EntityValidationException("There is no Media");
        Media.UpdateAsEncodingError();
    }

    public void UpdateAsEncoded(string encodedPath)
    {
        if (Media is null)
            throw new EntityValidationException("There is no Media");
        Media.UpdateAsEncoded(encodedPath);
    }

    public void AddCategory(Guid categoryId)
        => _categories.Add(categoryId);

    public void RemoveCategory(Guid categoryId)
        => _categories.Remove(categoryId);

    public void RemoveAllCategories()
        => _categories.Clear();

    public void AddGenre(Guid genreId)
        => _genres.Add(genreId);

    public void RemoveGenre(Guid genreId)
        => _genres.Remove(genreId);

    public void RemoveAllGenres()
        => _genres.Clear();

    public void AddCastMember(Guid castMemberId)
        => _castMembers.Add(castMemberId);

    public void RemoveCastMember(Guid castMemberId)
        => _castMembers.Remove(castMemberId);

    public void RemoveAllCastMembers()
        => _castMembers.Clear();

    public void Validate(ValidationHandler handler)
        => new VideoValidator(this, handler).Validate();
}