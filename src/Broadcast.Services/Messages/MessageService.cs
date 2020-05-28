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

        #region Insert

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
                    msgTag.Add(t ?? new Tag { TagName = tag });
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

        #endregion

        #region Update

        internal async Task<Message> ProcessUpdateMessageAsync(string slug, MessageDto dto)
        {
            var msgRepo = _worker.GetRepositoryAsync<Message>();
            var projRepo = _worker.GetRepositoryAsync<Project>();

            var message = await msgRepo.SingleAsync(msg => msg.Slug == slug);
            if (message == null) throw new RestException(HttpStatusCode.NotFound, new { Message = $"Message {Constants.NotFound}" });

            message.Project = await projRepo.SingleAsync(p => p.Slug == dto.ProjectDto.Project) ?? message.Project;
            message.Title = dto.AboutDto.Title ?? message.Title;
            message.Description = dto.AboutDto.Description ?? message.Description;
            message.StartDate = dto.AboutDto.StartDate != DateTime.MinValue ? dto.AboutDto.StartDate : message.StartDate;
            message.EndDate = dto.AboutDto.EndDate != DateTime.MinValue ? dto.AboutDto.EndDate : message.EndDate;
            message.Body = dto.DetailsDto?.Editor ?? message.Body;
            message.Slug = message.Slug ?? dto.AboutDto.Title?.GenerateSlug();
            message.Author = _userAccessor?.CurrentUser;
            message.UpdatedAt = DateTime.UtcNow;

            return message;
        }

        internal async Task ProcessUpdateTagAsync(AboutDto dto, Message message)
        {
            message.MessageTags.Clear();

            if (dto?.Tags?.Length > 0)
            {
                var tagRepo = _worker.GetRepositoryAsync<Tag>();
                foreach (var tag in dto.Tags)
                {
                    var t = await tagRepo.SingleAsync(tg => tg.TagName == tag);
                    message.AddMessageTag(new MessageTag {Tag = t ?? new Tag {TagName = tag}});
                }
            }
        }

        internal async Task ProcessUpdateCategoryAsync(AboutDto dto, Message message)
        {
            if (dto?.Categories?.Length > 0)
            {
                var catRepo = _worker.GetRepositoryAsync<Category>();
                foreach (var category in await catRepo.GetQueryableAsync())
                {
                    if (dto.Categories.Contains(category.Type))
                    {
                        if (message.MessageCategories.Count(cat => cat.CategoryId == category.Id) == 0)
                            message.AddMessageCategory(new MessageCategory { Category = category });
                    }
                    else
                    {
                        if (message.MessageCategories.Count(cat => cat.CategoryId == category.Id) > 0)
                            message.RemoveMessageCategory(message.MessageCategories.FirstOrDefault(mc => mc.CategoryId == category.Id));
                    }
                }
            }
        }

        internal async Task ProcessUpdateFileAsync(ExtrasDto dto, Message message)
        {
            message.Files.Clear();

            if (dto?.ExtraFiles?.Length > 0)
            {
                var fileRepo = _worker.GetRepositoryAsync<File>();
                foreach (var file in dto.ExtraFiles)
                {
                    var f = await fileRepo.SingleAsync(fl => fl.FileName == file.FileName && fl.FileType == file.FileType);
                    await fileRepo.AddAsync(new File
                    {
                        Message = message,
                        FileName = f.FileName ?? file.FileName,
                        FileType = f.FileType ?? file.FileType,
                        FileSize = f.FileSize > 0 ? f.FileSize : file.FileSize,
                        FileContent = f.FileContent ?? Convert.FromBase64String(file.FileContent),
                    });
                }
            }
        }

        #endregion

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

        public async Task<Message> UpdateMessageAsync(string slug, MessageDto dto)
        {
            var message = await ProcessUpdateMessageAsync(slug, dto);
 
            await ProcessUpdateTagAsync(dto.AboutDto, message);
        
            await ProcessUpdateCategoryAsync(dto.AboutDto, message);
        
            await ProcessUpdateFileAsync(dto.ExtrasDto, message);

            await _worker.SaveChangesAsync();

            return message;
        }
    }
}