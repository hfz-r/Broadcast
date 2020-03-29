using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Core.Domain.Messages;
using Broadcast.Core.Domain.Tags;
using Broadcast.Core.Domain.Users;
using Broadcast.Dtos.Messages;
using Broadcast.Core.Infrastructure;
using Broadcast.Core.Infrastructure.Errors;
using Broadcast.Core.Infrastructure.Mapper;
using Broadcast.Infrastructure;
using Broadcast.Services.Common;
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
            private readonly IGenericAttributeService _genericAttributeService;
            private readonly ICurrentUserAccessor _currentUser;

            public Handler(IUnitOfWork worker, IGenericAttributeService genericAttributeService,
                ICurrentUserAccessor currentUser)
            {
                _worker = worker;
                _genericAttributeService = genericAttributeService;
                _currentUser = currentUser;
            }

            public async Task<MessageEnvelope> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var userRepo = _worker.GetRepositoryAsync<User>();
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
                            new {Project = Constants.Existed, Title = Constants.Existed});

                    // todo: user mgmt
                    // var author = await userRepo.SingleAsync(user => user.Username == _currentUser.GetCurrentUsername());
                    var author = request.Message.AuthorDto;
                    var user = author.ToEntity(new User());
                    if (!string.IsNullOrEmpty(user.Username) &&
                        await userRepo.SingleAsync(u => u.Username == user.Username) == null)
                    {
                        // temp create user
                        await userRepo.AddAsync(user, cancellationToken);
                    }

                    var message = new Message
                    {
                        Project = request.Message.ProjectDto.Project,
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

                    //handle category
                    var ctgs = request.Message.AboutDto.Categories;
                    if (ctgs?.Length > 0)
                    {
                        await messageCatRepo.AddAsync(ctgs.Select(cat => new MessageCategory
                        {
                            Message = message,
                            Category = AsyncHelper.RunSync(() => catRepo.SingleAsync(c => c.Type == cat))
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

                    //handle location after save
                    var location = request.Message.ProjectDto;
                    if (location != null)
                    {
                        foreach (var prop in location.GetType()
                            .GetProperties(BindingFlags.Public | BindingFlags.Instance))
                        {
                            if (!prop.Name.Contains("project", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var key = string.Format(MessageDefaults.LocationAttribute, prop.Name);
                                await _genericAttributeService.SaveAttributeAsync(message, key,
                                    prop.GetValue(location));
                            }
                        }
                    }

                    return new MessageEnvelope(message.ToDto<MessageDto>());
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }
    }
}