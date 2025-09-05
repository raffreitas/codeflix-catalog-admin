using FC.Codeflix.Catalog.Domain.Enum;
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

    public void Update(string title, string description, int yearLaunched, int duration, bool opened, bool published)
    {
        Title = title;
        Description = description;
        YearLaunched = yearLaunched;
        Duration = duration;
        Opened = opened;
        Published = published;
    }

    public void UpdateThumb(string path)
        => Thumb = new Image(path);

    public void UpdateThumbHalf(string path)
        => ThumbHalf = new Image(path);

    public void UpdateBanner(string path)
        => Banner = new Image(path);

    public void Validate(ValidationHandler handler)
        => new VideoValidator(this, handler).Validate();
}