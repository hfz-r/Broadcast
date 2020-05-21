using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Core;
using Broadcast.Core.Caching;
using Broadcast.Core.Domain.Messages;
using Broadcast.Core.Domain.Projects;
using Broadcast.Core.Domain.Tags;
using Broadcast.Core.Domain.Users;
using Broadcast.Core.Dtos.Messages;
using Broadcast.Core.Dtos.Projects;
using Broadcast.Core.Infrastructure;
using Broadcast.Services.Messages;
using Broadcast.Test;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using NUnit.Framework;

namespace Broadcast.Services.Test.Messages
{
    [TestFixture]
    public class MessageServiceTests
    {
        private IStaticCacheManager _cacheManager;
        private MessageService _messageService;
        private Mock<DbContext> _dbContext;
        private Mock<DbSet<Message>> _mockDbSetMsg;
        private Mock<IRepositoryAsync<Message>> _msgRepo;
        private Mock<IRepositoryAsync<Project>> _projRepo;
        private Mock<IRepositoryAsync<Tag>> _tagRepo;
        private Mock<IUnitOfWork> _worker;
        private Mock<ICurrentUserAccessor> _userAccessor;

        private IList<Message> _messages;
        private IList<Project> _projects;
        private IList<Tag> _tags;
        private List<MessageTag> _messageTags;

        [SetUp]
        public void SetUp()
        {
            _cacheManager = new TestCacheManager();
            _dbContext = new Mock<DbContext>();
            _mockDbSetMsg = new Mock<DbSet<Message>>();
            _msgRepo = new Mock<IRepositoryAsync<Message>>();
            _projRepo = new Mock<IRepositoryAsync<Project>>();
            _tagRepo = new Mock<IRepositoryAsync<Tag>>();
            _worker = new Mock<IUnitOfWork>();
            _userAccessor = new Mock<ICurrentUserAccessor>();

            #region Test data

            //Project
            var project1 = new Project
            {
                Id = 1,
                Name = "Project 1",
                Description = "Description of Project 1",
            };
            var project2 = new Project
            {
                Id = 2,
                Name = "Project 2",
                Description = "Description of Project 2",
            };
            var project3 = new Project
            {
                Id = 3,
                Name = "Project 3",
                Description = "Description of Project 3",
            };
            _projects = new List<Project> {project1, project2, project3};

            //Message
            var message1 = new Message
            {
                Project = project1,
                Title = "Unit test on Project 1",
                Description = string.Empty,
                Slug = "unit-test1",
                Author = new User
                {
                    Guid = Guid.NewGuid(),
                    AccountName = "user_one",
                    Name = "User One"
                }
            };
            var message2 = new Message
            {
                Project = project2,
                Title = "Unit test on Project 2",
                Description = string.Empty,
                Slug = "unit-test2",
                Author = new User
                {
                    Guid = Guid.NewGuid(),
                    AccountName = "user_two",
                    Name = "User Two"
                }
            };
            _messages = new List<Message> {message1, message2};

            //Tag
            var tag1 = new Tag
            {
                Id = 1,
                TagName = "covid-19",
            };
            var tag2 = new Tag
            {
                Id = 2,
                TagName = "#stayhome"
            };
            _tags = new List<Tag> {tag1, tag2};

            //MessageTag
            var mtag1 = new MessageTag
            {
                Id = 1,
                Message = message1,
                MessageId = message1.Id,
                Tag = tag1,
                TagId = tag1.Id
            };
            var mtag2 = new MessageTag
            {
                Id = 2,
                Message = message1,
                MessageId = message1.Id,
                Tag = tag2,
                TagId = tag2.Id
            };
            _messageTags = new List<MessageTag> {mtag1, mtag2};

            //Setup Encoding
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding.GetEncoding("Cyrillic"); //slug accent remover

            #endregion

            //Arrange
            //User Context
            _userAccessor.Setup(x => x.CurrentUser).Returns(new User
            {
                Guid = Guid.NewGuid(),
                AccountName = "test_admin",
                Name = "Test Admin",
                Email = "test@brdcst.com",
            });

            _mockDbSetMsg = _messages.BuildMockDbSet();
            //var msgDataSet = _messages.BuildMockDbSet();
            var projDbSet = _projects.BuildMockDbSet();
            var tagDbSet = _tags.BuildMockDbSet();
            var mTagsDbSet = _messageTags.BuildMockDbSet();

            _dbContext.Setup(x => x.Set<Message>()).Returns(_mockDbSetMsg.Object);
            _worker.Setup(x => x.GetRepositoryAsync<Message>()).Returns(() => new RepositoryAsync<Message>(_dbContext.Object));

            _dbContext.Setup(x => x.Set<Project>()).Returns(projDbSet.Object);
            _worker.Setup(x => x.GetRepositoryAsync<Project>()).Returns(() => new RepositoryAsync<Project>(_dbContext.Object));

            _dbContext.Setup(x => x.Set<Tag>()).Returns(tagDbSet.Object);
            _worker.Setup(x => x.GetRepositoryAsync<Tag>()).Returns(() => new RepositoryAsync<Tag>(_dbContext.Object));

            _dbContext.Setup(x => x.Set<MessageTag>()).Returns(mTagsDbSet.Object);
            _worker.Setup(x => x.GetRepositoryAsync<MessageTag>()).Returns(() => new RepositoryAsync<MessageTag>(_dbContext.Object));

            _messageService = new MessageService(_cacheManager, _worker.Object, _userAccessor.Object);
        }

        [Test]
        public void Should_be_able_to_insert_raw_MessageDto()
        {
            //Arrange
            _mockDbSetMsg.Setup(x => x.AddAsync(It.IsAny<Message>(), CancellationToken.None))
                .Callback((Message message, CancellationToken c) => _messages.Add(message))
                .Returns((Message message, CancellationToken c) => Task.FromResult((EntityEntry<Message>) null));

            _msgRepo.Setup(x => x.GetQueryableAsync(It.IsAny<Expression<Func<Message, bool>>>(), null, null, false))
                .ReturnsAsync(_messages.AsQueryable);

            _projRepo.Setup(x => x.SingleAsync(It.IsAny<Expression<Func<Project, bool>>>(), null, null, false));

            //Act
            var dto = new MessageDto
            {
                MessageId = 1,
                ProjectDto = new ProjectDto
                {
                    Project = "Project 3",
                    Description = "Description of Project 3",
                    CreatedBy = _userAccessor.Object.CurrentUser.Name,
                    ModifiedBy = _userAccessor.Object.CurrentUser.Name,
                },
                AboutDto = new AboutDto
                {
                    Title = "Covid-19 Deployment Delay",
                    Description = "Project 3 deployment delay info and actions",
                    Categories = new[] {"info", "delayed"},
                    Tags = new[] {"covid-19", "#stayhome"},
                    EndDate = DateTime.Now.AddDays(1),
                    StartDate = DateTime.Now,
                },
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var res = _messageService.ProcessMessageAsync(dto).Result;

            //Assert
            res.Project.Name.Should().BeOneOf(_projects.FirstOrDefault(x => x.Name == res.Project.Name)?.Name);

            _messages.Count.Should().Be(3);

            _mockDbSetMsg.Verify(x => x.AddAsync(It.IsAny<Message>(), CancellationToken.None), Times.Once);
            _mockDbSetMsg.Verify(
                x => x.AddAsync(It.Is<Message>(y => y.Project.Name == dto.ProjectDto.Project), CancellationToken.None),
                Times.Once);
        }

        [Test]
        public void Should_be_able_to_insert_raw_tags()
        {
            //Arrange
            _tagRepo.Setup(x => x.SingleAsync(It.IsAny<Expression<Func<Tag, bool>>>(), null, null, false));

            _dbContext.Setup(x => x.Set<MessageTag>().AddRangeAsync(It.IsAny<IEnumerable<MessageTag>>(), CancellationToken.None))
                .Callback((IEnumerable<MessageTag> mt, CancellationToken c) =>
                {
                    var newList = new List<MessageTag>(mt);
                    _messageTags.AddRange(newList);
                })
                .Returns((IEnumerable<MessageTag> mt, CancellationToken c) => Task.CompletedTask);

            //Act
            var dto = new AboutDto {Tags = new[] {"covid-19", "#stayhome", "newtag"}};
            var message = new Message
            {
                Project = new Project
                {
                    Id = 2,
                    Name = "Project 2",
                    Description = "Description of Project 2",
                },
                Title = "Unit test on Project 2",
                Description = string.Empty,
                Slug = "unit-test2",
                Author = new User
                {
                    Guid = Guid.NewGuid(),
                    AccountName = "user_two",
                    Name = "User Two"
                }
            };

            _messageService.ProcessTagAsync(dto, message).GetAwaiter().GetResult();

            //Assert
            _messageTags.Count.Should().Be(5);
            _messageTags.Select(x => x.Tag.TagName).Should().Contain("newtag");
        }

        [Test]
        public void Can_fetch_messages()
        {
            //Arrange
            _msgRepo.Setup(x => x.GetQueryableAsync(It.IsAny<Expression<Func<Message, bool>>>(), null, null, false)).ReturnsAsync(_messages.AsQueryable);

            //Act
            var res = _messageService.FetchMessagesAsync(null, null, null, null).Result;

            //Assert
            res.Count.Should().Be(2);
            res.Items.Select(x => x.Project.Id).Should()
                .Equal(_projects.Where(x => x.Name != "Project 3").Select(x => x.Id));
        }
    }
}