using FluentValidation;

namespace KITT.Core.Validators;

public class StreamingValidator : AbstractValidator<Streaming>
{
    private readonly KittDbContext _context;

    const string ScheduleStreamingAction = "ScheduleStreaming";

    public StreamingValidator(KittDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        DefineRules();
    }

    public void ValidateForScheduleStreaming(Streaming streaming) => this.Validate(streaming, options =>
    {
        options.ThrowOnFailures();
        options.IncludeRuleSets(ScheduleStreamingAction);
    });

    public void ValidateForUpdateStreaming(Streaming streaming) => this.Validate(streaming, options =>
    {
        options.ThrowOnFailures();
    });

    private void DefineRules()
    {
        RuleFor(streaming => streaming.TwitchChannel).MaximumLength(50).NotEmpty();
        RuleFor(streaming => streaming.Title).MaximumLength(255).NotEmpty();
        RuleFor(streaming => streaming.Slug).MaximumLength(255).NotEmpty();
        RuleFor(streaming => streaming.HostingChannelUrl).NotEmpty().MaximumLength(255);
        RuleFor(streaming => streaming.YouTubeVideoUrl).MaximumLength(255);

        RuleSet(ScheduleStreamingAction, () =>
        {
            RuleFor(streaming => streaming.Slug).Must(slug => !_context.Streamings.Any(s => s.Slug == slug));
        });
    }
}
