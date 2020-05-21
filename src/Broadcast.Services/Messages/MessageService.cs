using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Broadcast.Core;
using Broadcast.Core.Caching;
using Broadcast.Core.Domain.Messages;
using Broadcast.Core.Domain.Projects;
using Broadcast.Core.Domain.Tags;
using Broadcast.Core.Dtos.Messages;
using Broadcast.Core.Infrastructure;
using Broadcast.Core.Infrastructure.Errors;
using Broadcast.Core.Infrastructure.Paging;

namespace Broadcast.Services.Messages
{
    public class MessageService : IMessageService
    {
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IUnitOfWork _worker;
        private readonly ICurrentUserAccessor _userAccessor;

        public MessageService(
            IStaticCacheManager staticCacheManager,
            IUnitOfWork worker,
            ICurrentUserAccessor userAccessor)
        {
            _staticCacheManager = staticCacheManager;
            _worker = worker;
            _userAccessor = userAccessor;
        }

        protected virtual async Task<Project> ProcessProjectAsync(MessageDto vm, Func<Project, Project> execFunc)
        {
            var msgRepo = _worker.GetRepositoryAsync<Message>();
            var prjRepo = _worker.GetRepositoryAsync<Project>();

            if ((await msgRepo.GetQueryableAsync(m =>
                m.Project.Slug == vm.ProjectDto.Project &&
                m.Title.Equals(vm.AboutDto.Title, StringComparison.InvariantCultureIgnoreCase))).Any())
                throw new RestException(HttpStatusCode.BadRequest, new {Project = $"Project {Constants.Existed}"});

            return execFunc(await prjRepo.SingleAsync(p => p.Slug == vm.ProjectDto.Project) ??
                            throw new ArgumentNullException(nameof(vm.ProjectDto.Project)));
        }

        //internal mod. start
        internal async Task<Message> ProcessMessageAsync(MessageDto vm)
        {
            var message = new Message
            {
                Project = await ProcessProjectAsync(vm, p =>
                {
                    var modP = p;
                    modP.ModifiedBy = _userAccessor.CurrentUser.Name;
                    modP.ModifiedOn = DateTime.UtcNow;
                    return modP;
                }),
                Title = vm.AboutDto.Title,
                Description = vm.AboutDto.Description,
                StartDate = vm.AboutDto.StartDate,
                EndDate = vm.AboutDto.EndDate,
                Body = vm.DetailsDto?.Editor,
                Slug = vm.AboutDto.Title?.GenerateSlug(),
                Author = _userAccessor?.CurrentUser,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            var msgRepo = _worker.GetRepositoryAsync<Message>();
            await msgRepo.AddAsync(message);

            return message;
        }

        internal async Task ProcessTagAsync(AboutDto vm, Message message)
        {
            if (vm?.Tags?.Length > 0)
            {
                var tagRepo = _worker.GetRepositoryAsync<Tag>();
                var mTagRepo = _worker.GetRepositoryAsync<MessageTag>();

                var msgTag = new List<Tag>();
                foreach (var tag in vm.Tags)
                {
                    var t = await tagRepo.SingleAsync(tg => tg.TagName == tag);
                    msgTag.Add(t ?? new Tag {TagName = tag});
                }

                await mTagRepo.AddAsync(msgTag.Select(t => new MessageTag
                {
                    Message = message,
                    Tag = t
                }));
            }
        }

        internal async Task ProcessCategoryAsync(AboutDto vm, Message message)
        {
            if (vm?.Categories?.Length > 0)
            {
                var catRepo = _worker.GetRepositoryAsync<Category>();
                var mCatRepo = _worker.GetRepositoryAsync<MessageCategory>();

                await mCatRepo.AddAsync(vm.Categories.Select(cat => new MessageCategory
                {
                    Message = message,
                    Category = AsyncHelper.RunSync(() => catRepo.SingleAsync(c => c.Type == cat))
                }));
            }
        }

        internal async Task ProcessFileAsync(ExtrasDto vm, Message message)
        {
            if (vm?.ExtraFiles?.Length > 0)
            {
                var fileRepo = _worker.GetRepositoryAsync<File>();

                await fileRepo.AddAsync(vm.ExtraFiles.Select(file => new File
                {
                    FileName = file.FileName,
                    FileType = file.FileType,
                    FileSize = file.FileSize,
                    FileContent = Convert.FromBase64String(file.FileContent),
                    Message = message
                }));
            }
        }
        //internal mod. end

        public async Task<IPaginate<Message>> FetchMessagesAsync(string tag, string author, int? limit, int? offset)
        {
            var repo = _worker.GetRepositoryAsync<Message>();

            var messages = await repo.GetQueryableAsync(queryExp: query =>
                {
                    if (!string.IsNullOrEmpty(tag))
                        query = query.Where(m => m.MessageTags.Any(mt => mt.Tag.TagName == tag));
                    if (!string.IsNullOrEmpty(author))
                        query = query.Where(m => m.Author.Name == author);
                    return query;
                },
                orderBy: msg => msg.OrderByDescending(m => m.CreatedAt));

            if (messages == null)
                throw new RestException(HttpStatusCode.NotFound, new {Messages = $"Messages {Constants.NotFound}"});

            return await messages.ToPaginateAsync(offset ?? 0, limit ?? 20);
        }

        public async Task<Message> InsertMessageAsync(MessageDto dto)
        {
            var message = await ProcessMessageAsync(dto);
            //process tag
            await ProcessTagAsync(dto.AboutDto, message);
            //process category
            await ProcessCategoryAsync(dto.AboutDto, message);
            //process file
            await ProcessFileAsync(dto.ExtrasDto, message);

            await _worker.SaveChangesAsync();

            return message;
        }
    }
}