using AzureBlob1.Dtos;
using AzureBlob1.Helpers;
using FluentValidation;

namespace AzureBlob1.Validators;

public class VideoValidator : AbstractValidator<VideoDto>
{
    public VideoValidator()
    {
        RuleFor(dto => dto.Title).NotEmpty().NotNull().WithMessage("Title is requeird")
            .MinimumLength(3).WithMessage("Title must be more than 3 characters")
            .MaximumLength(50).WithMessage("Title must be less than 50 characters");

        int maxImageSizeMB = 3;
        RuleFor(dto => dto.VideoImage).NotEmpty().NotNull().WithMessage("Image field is required");
        RuleFor(dto => dto.VideoImage.Length).LessThan(maxImageSizeMB * 1024 * 1024 + 1).WithMessage($"Image size must be less than {maxImageSizeMB} MB");
        RuleFor(dto => dto.VideoImage.FileName).Must(predicate =>
        {
            FileInfo fileInfo = new FileInfo(predicate);
            return MediaHelper.GetImageExtensions().Contains(fileInfo.Extension);
        }).WithMessage("This file type is not IMAGE file");

        RuleFor(dto => dto.Video).NotEmpty().NotNull().WithMessage("Video field is required");
        RuleFor(dto => dto.Video.FileName).Must(predicate =>
        {
            FileInfo fileInfo = new FileInfo(predicate);
            return MediaHelper.GetVideoExtensions().Contains(fileInfo.Extension);
        }).WithMessage("This file type is not VIDEO file");

    }
}
