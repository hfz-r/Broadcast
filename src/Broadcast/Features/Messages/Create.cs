using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Domain.Messages;
using Broadcast.Domain.Tags;
using Broadcast.Domain.Users;
using Broadcast.Dtos.Messages;
using Broadcast.Dtos.Users;
using Broadcast.Infrastructure;
using Broadcast.Infrastructure.Data;
using Broadcast.Infrastructure.Errors;
using Broadcast.Infrastructure.Mapper;
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
                var userRepo = _worker.GetRepositoryAsync<User>();
                var messageRepo = _worker.GetRepositoryAsync<Message>();
                var tagRepo = _worker.GetRepositoryAsync<Tag>();
                var messageTagRepo = _worker.GetRepositoryAsync<MessageTag>();
                var fileRepo = _worker.GetRepositoryAsync<File>();

                if ((await messageRepo.GetQueryableAsync(m =>
                    m.Project == request.Message.Project && m.Title == request.Message.AboutDto.Title)).Any())
                    throw new RestException(HttpStatusCode.BadRequest,
                        new {Project = Constants.Existed, Title = Constants.Existed});

                // todo: user mgmt
                // var author = await userRepo.SingleAsync(user => user.Username == _currentUser.GetCurrentUsername());
                var author = request.Message.AuthorDto;
                var user  = author.ToEntity(new User());
                if (!string.IsNullOrEmpty(user.Username) &&
                    await userRepo.SingleAsync(u => u.Username == author.Username) == null)
                {
                    // temp create user
                    await userRepo.AddAsync(user, cancellationToken);
                }

                var message = new Message
                {
                    Project = request.Message.Project,
                    Title = request.Message.AboutDto.Title,
                    Description = request.Message.AboutDto?.Description,
                    StartDate = request.Message.AboutDto.StartDate,
                    EndDate = request.Message.AboutDto.EndDate,
                    Body = request.Message.DetailsDto?.Editor,
                    Slug = request.Message.AboutDto.Title.GenerateSlug(),
                    Author = user,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                await messageRepo.AddAsync(message, cancellationToken);

                //handle tags
                var tgs = request.Message.AboutDto.Tags;
                if (tgs?.Length > 0)
                {
                    var tags = new List<Tag>();
                    foreach (var tag in tgs)
                    {
                        var t = await tagRepo.SingleAsync(tg => tg.TagName == tag);
                        if (t == null) tags.Add(new Tag {TagName = tag});
                    }

                    await tagRepo.AddAsync(tags, cancellationToken);
                    await messageTagRepo.AddAsync(tags.Select(t => new MessageTag
                    {
                        Message = message,
                        Tag = t
                    }), cancellationToken);
                }

                //handle files
                var fls = request.Message.ExtrasDto?.ExtraFiles;
                if (fls?.Length > 0)
                {
                    var files = fls.Select(file => new File
                        {
                            FileName = file.FileName,
                            FileType = file.FileType,
                            FileSize = file.FileSize,
                            FileContent = Convert.FromBase64String(file.FileContent),
                            Message = message
                        })
                        .ToList();

                    await fileRepo.AddAsync(files, cancellationToken);
                }

                await _worker.SaveChangesAsync();

                return new MessageEnvelope(message.ToDto<MessageDto>());
            }
        }
    }
}