using FluentValidation;
using Microsoft.Extensions.Localization;
using Shared.Application.Common.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Application.Common.Models
{
    public class FileUploadRequest
    {
        public string Name { get; set; } = default!;
        public string Extension { get; set; } = default!;
        public string Data { get; set; } = default!;
    }

    public class FileUploadRequestValidator : CustomValidator<FileUploadRequest>
    {
        public FileUploadRequestValidator(IStringLocalizer<FileUploadRequestValidator> T)
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .WithMessage(T["Image Name cannot be empty!"])
                .MaximumLength(150);

            RuleFor(p => p.Extension)
                .NotEmpty()
                .WithMessage(T["Image Extension cannot be empty!"])
                .MaximumLength(5);

            RuleFor(p => p.Data)
                .NotEmpty()
                .WithMessage(T["Image Data cannot be empty!"]);
        }
    }
}
