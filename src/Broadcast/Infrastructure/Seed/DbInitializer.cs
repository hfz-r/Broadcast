using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using Broadcast.Core.Domain;
using Broadcast.Core.Domain.Messages;
using Broadcast.Core.Domain.Tags;
using Broadcast.Core.Domain.Users;
using Broadcast.Core.Infrastructure;
using Broadcast.Core.Infrastructure.Security;
using Broadcast.Services.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Broadcast.Infrastructure.Seed
{
    public static class DbInitializer
    {
        private static void Faker()
        {
            Randomizer.Seed = new Random(8675309);

            #region Users

            var userFaker = new Faker<User>()
                //.RuleFor(u => u.Id, f => f.IndexFaker)
                .RuleFor(u => u.AccountName, f => f.Name.FirstName())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.AccountName));

            Users = userFaker.Generate(3);

            #endregion

            #region Tags

            var tagFaker = new Faker<Tag>()
                //.RuleFor(t => t.Id, f => f.IndexFaker)
                .RuleFor(t => t.TagName, f => f.Hacker.Verb());

            Tags = tagFaker.Generate(3);

            #endregion

            #region Messages

            var messageFaker = new Faker<Message>()
                //.RuleFor(m => m.Id, f => f.IndexFaker)
                .RuleFor(m => m.Slug, f => f.Lorem.Slug())
                .RuleFor(m => m.Title, f => f.Hacker.Phrase())
                .RuleFor(m => m.Description, f => f.Random.Words())
                .RuleFor(m => m.Body, f => f.Lorem.Sentences())
                .RuleFor(m => m.Author, f => f.PickRandom(Users))
                .RuleFor(m => m.CreatedAt, f => f.Date.Recent())
                .RuleFor(m => m.UpdatedAt, f => f.Date.Recent());

            Messages = messageFaker.Generate(5);

            #endregion

            #region MessageTag

            var messageTagFaker = new Faker<MessageTag>()
                    .RuleFor(mt => mt.Message, f => f.PickRandom(Messages))
                    .RuleFor(mt => mt.MessageId, (f, o) => o.Message.Id)
                    .RuleFor(mt => mt.Tag, f => f.PickRandom(Tags))
                    .RuleFor(mt => mt.TagId, (f, o) => o.Tag.Id);

            MessageTag = messageTagFaker.Generate(3);

            #endregion

            #region Category

            Categories = new List<Category>
            {
                new Category{Type = "Warning"},
                new Category{Type = "Error"},
                new Category{Type = "Info"},
                new Category{Type = "Others"},
            };

            #endregion

            #region Role

            Roles = new List<Role>
            {
                new Role{Name = "Admin"},
                new Role{Name = "Tester"},
                new Role{Name = "DBA"},
                new Role{Name = "QA"},
                new Role{Name = "Analyst"},
            };

            #endregion
        }

        private static async Task AddAsync<T>(IUnitOfWork worker, IList<T> src) where T : BaseEntity
        {
            var repo = worker.GetRepositoryAsync<T>();
            if ((await repo.GetPagedListAsync()).Count == 0) await repo.AddAsync(src);
        }

        private static async Task InitPermissionRole(IServiceProvider service)
        {
            var permissionService = service.GetRequiredService<IPermissionService>();

            var permissionProviders = new List<Type> { typeof(StandardPermissionProvider) };
            foreach (var providerType in permissionProviders)
            {
                var provider = (IPermissionProvider)Activator.CreateInstance(providerType);
                await permissionService.InstallPermissionsAsync(provider);
            }
        }

        private static async Task InitCommonData(IServiceProvider service)
        {
            var worker = service.GetRequiredService<IUnitOfWork>();

            //await AddAsync(worker, Users);
            //await AddAsync(worker, Tags);
            //await AddAsync(worker, Messages);
            //await AddAsync(worker, MessageTag);
            //await AddAsync(worker, Roles);
            await AddAsync(worker, Categories);

            await worker.SaveChangesAsync();
        }

        public static void SeedData(IServiceProvider service)
        {
            Task.Run(async () =>
            {
                Faker();
                await InitCommonData(service);
                await InitPermissionRole(service);
            }).Wait();
        }

        public static IList<User> Users { get; private set; }
        public static IList<Tag> Tags { get; private set; }
        public static IList<Message> Messages { get; private set; }
        public static IList<MessageTag> MessageTag { get; private set; }
        public static IList<Category> Categories { get; private set; }
        public static IList<Role> Roles { get; private set; }
    }
}