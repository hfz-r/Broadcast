using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Core;
using Broadcast.Core.Domain.Messages;
using Broadcast.Core.Domain.Tags;
using Broadcast.Dtos.Messages;
using Broadcast.Core.Infrastructure;
using Broadcast.Core.Infrastructure.Errors;
using Broadcast.Core.Infrastructure.Mapper;
using Broadcast.Infrastructure;
using FluentValidation;
using MediatR;

namespace Broadcast.Features.Messages
{
    public class Create
    {
        public class Command : IRequest<MessageEnvelope>
        {
            public MessageDto Message { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Message).NotNull().SetValidator(new MessageValidator());
            }
        }

        public class Handler : IRequestHandler<Command, MessageEnvelope>
        {
            private readonly IUnitOfWork _worker;
            private readonly ICurrentUserAccessor _currentUser;

            public Handler(IUnitOfWork worker, ICurrentUserAccessor currentUser)
            {
                _worker = worker;
                _currentUser = currentUser;
            }

            public async Task<MessageEnvelope> Handle(Command request, CancellationToken cancellationToken)
            {
                var messageRepo = _worker.GetRepositoryAsync<Message>();
                var tagRepo = _worker.GetRepositoryAsync<Tag>();
                var messageTagRepo = _worker.GetRepositoryAsync<MessageTag>();
                var catRepo = _worker.GetRepositoryAsync<Category>();
                var messageCatRepo = _worker.GetRepositoryAsync<MessageCategory>();
                var fileRepo = _worker.GetRepositoryAsync<File>();

                if ((await messageRepo.GetQueryableAsync(m =>
                        m.Project == request.Message.ProjectDto.Project &&
                        m.Title == request.Message.AboutDto.Title))
                    .Any())
                    throw new RestException(HttpStatusCode.BadRequest,
                        new {Project = $"Project {Constants.Existed}", Title = $"Title {Constants.Existed}"});

                var message = new Message
                {
                    Project = request.Message.ProjectDto.Project,
                    Title = request.Message.AboutDto.Title,
                    Description = request.Message.AboutDto?.Description,
                    StartDate = request.Message.AboutDto.StartDate,
                    EndDate = request.Message.AboutDto.EndDate,
                    Body = request.Message.DetailsDto?.Editor,
                    Slug = request.Message.AboutDto.Title.GenerateSlug(),
                    Author = _currentUser.CurrentUser,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                //materialized message
                await messageRepo.AddAsync(message, cancellationToken);

                //handle tags
                var tags = request.Message.AboutDto.Tags;
                if (tags?.Length > 0)
                {
                    var msgTag = new List<Tag>();
                    foreach (var tag in tags)
                    {
                        var t = await tagRepo.SingleAsync(tg => tg.TagName == tag);
                        msgTag.Add(t ?? new Tag {TagName = tag});
                    }

                    await messageTagRepo.AddAsync(msgTag.Select(t => new MessageTag
                    {
                        Message = message,
                        Tag = t
                    }), cancellationToken);
                }

                //handle category
                var categories = request.Message.AboutDto.Categories;
                if (categories?.Length > 0)
                {
                    await messageCatRepo.AddAsync(categories.Select(cat => new MessageCategory
                    {
                        Message = message,
                        Category = AsyncHelper.RunSync(() => catRepo.SingleAsync(c => c.Type == cat))
                    }), cancellationToken);
                }

                //handle files
                var files = request.Message.ExtrasDto?.ExtraFiles;
                if (files?.Length > 0)
                {
                    var fileObj = files.Select(file => new File
                        {
                            FileName = file.FileName,
                            FileType = file.FileType,
                            FileSize = file.FileSize,
                            FileContent = Convert.FromBase64String(file.FileContent),
                            Message = message
                        })
                        .ToList();

                    await fileRepo.AddAsync(fileObj, cancellationToken);
                }

                await _worker.SaveChangesAsync();

                return new MessageEnvelope(message.ToDto<MessageDto>());
            }
        }
    }
}