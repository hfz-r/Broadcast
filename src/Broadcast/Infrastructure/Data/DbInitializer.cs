using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using Broadcast.Domain;
using Broadcast.Domain.Messages;
using Broadcast.Domain.Tags;
using Broadcast.Domain.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Broadcast.Infrastructure.Data
{
    public static class DbInitializer
    {
        private static void Faker()
        {
            Randomizer.Seed = new Random(8675309);

            #region Users

            var userFaker = new Faker<User>()
                //.RuleFor(u => u.Id, f => f.IndexFaker)
                .RuleFor(u => u.Username, f => f.Name.FirstName())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.Username))
                .RuleFor(u => u.Bio, f => f.Rant.Review())
                .RuleFor(u => u.Image, f => f.Image.PicsumUrl())
                .RuleFor(u => u.Hash, f => f.Random.Bytes(1))
                .RuleFor(u => u.Salt, f => f.Random.Bytes(1));

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
        }

        private static async Task AddAsync<T>(IUnitOfWork worker, IList<T> src) where T : BaseEntity
        {
            var repo = worker.GetRepositoryAsync<T>();
            await repo.AddAsync(src);
        }

        private static async Task Init(IServiceProvider service)
        {
            var worker = service.GetRequiredService<IUnitOfWork>();

            await AddAsync(worker, Users);
            await AddAsync(worker, Tags);
            await AddAsync(worker, Messages);
            await AddAsync(worker, MessageTag);

            await worker.SaveChangesAsync();
        }

        public static void SeedData(IServiceProvider service)
        {
            Task.Run(async () =>
            {
                Faker();
                await Init(service);
            }).Wait();
        }

        public static IList<User> Users { get; private set; }
        public static IList<Tag> Tags { get; private set; }
        public static IList<Message> Messages { get; private set; }
        public static IList<MessageTag> MessageTag { get; private set; }
    }
}