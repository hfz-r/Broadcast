using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Core;
using Broadcast.Core.Caching;
using Broadcast.Core.Domain.Security;
using Broadcast.Core.Domain.Users;
using Broadcast.Core.Infrastructure;
using Broadcast.Services.Security;
using Broadcast.Services.Users;
using Broadcast.Test;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Broadcast.Services.Test.Security
{
    [TestFixture]
    public class PermissionServiceTests
    {
        private IPermissionService _permissionService;
        private IStaticCacheManager _cacheManager;
        private Mock<IUnitOfWork> _worker;
        private Mock<IUnitOfWork> _worker2;
        private Mock<IUserService> _userService;
        private Mock<ICurrentUserAccessor> _userAccessor;
        private Mock<IRepositoryAsync<Role>> _roleRepository;
        private Mock<IRepositoryAsync<PermissionRole1>> _permissionRoleRepository;
        private Mock<DbContext> _dbContext;
        private Mock<DbContext> _dbContext2;

        private IList<JsonToObject> _objects;
        private IList<Role> _roles;
        private IList<PermissionRole1> _permissionRoles;

        [SetUp]
        public void SetUp()
        {
            _cacheManager = new TestCacheManager();
            _worker = new Mock<IUnitOfWork>();
            _worker2 =new Mock<IUnitOfWork>();
            _userService = new Mock<IUserService>();
            _userService = new Mock<IUserService>();
            _userAccessor = new Mock<ICurrentUserAccessor>();
            _roleRepository = new Mock<IRepositoryAsync<Role>>();
            _permissionRoleRepository = new Mock<IRepositoryAsync<PermissionRole1>>();
            _dbContext = new Mock<DbContext>();
            _dbContext2 = new Mock<DbContext>();

            //target data - should be deserialized into this form
            _objects = new List<JsonToObject>
            {
                new JsonToObject {Role = "Admin", Permissions = new[] {Permission1.AccessAll}},
                new JsonToObject
                {
                    Role = "Tester",
                    Permissions = new[] {Permission1.WidgetRead, Permission1.AnnouncementRead}
                }
            };

            #region Test-case 

            //PermissionRole
            var prOperator = new PermissionRole1 {Id = 2, RoleId = 2, Permission = Permission1.WidgetRead};
            _permissionRoles = new List<PermissionRole1> {prOperator};

            //Role
            var adminRole = new Role
            {
                Id = 1,
                Name = "Admin",
                PermissionRoles1 = { } //no permission
            };
            var operatorRole = new Role
            {
                Id = 2,
                Name = "Operator",
                PermissionRoles1 = {prOperator}
            };
            _roles = new List<Role> {adminRole, operatorRole};

            #endregion

            #region Setup

            //Role 
            var rDataSet = _roles.BuildMockDbSet();
            _dbContext.Setup(x => x.Set<Role>()).Returns(rDataSet.Object);
            //_dbContext.Setup(x => x.Set<Role>().AddAsync(It.IsAny<Role>(), CancellationToken.None))
            //    .Callback((Role r, CancellationToken c) => _roles.Add(r));

            _worker.Setup(x => x.GetRepositoryAsync<Role>()).Returns(() => new RepositoryAsync<Role>(_dbContext.Object));

            _roleRepository.Setup(x => x.GetQueryableAsync(null, null, null, false)).ReturnsAsync(_roles.AsQueryable());

            //PermissionRole
            var prDataSet = _permissionRoles.BuildMockDbSet();
            _dbContext.Setup(x => x.Set<PermissionRole1>()).Returns(prDataSet.Object);
            _worker.Setup(x => x.GetRepositoryAsync<PermissionRole1>()).Returns(() => new RepositoryAsync<PermissionRole1>(_dbContext.Object));

            _permissionRoleRepository
                .Setup(x => x.GetQueryableAsync(It.IsAny<Expression<Func<PermissionRole1, bool>>>(), null, null, false))
                .ReturnsAsync(_permissionRoles.AsQueryable());

            //Role add
            var dataSet2 = _roles.BuildMockDbSet();
            _dbContext2.Setup(x => x.Set<Role>()).Returns(dataSet2.Object);
            _dbContext2.Setup(x => x.AddAsync(It.IsAny<Role>(), CancellationToken.None)).Callback((Role r, CancellationToken c) => _roles.Add(r));
            _worker2.Setup(x => x.GetRepositoryAsync<Role>()).Returns(() => new RepositoryAsync<Role>(_dbContext2.Object));
            
            #endregion

            _userService.Setup(x => x.GetRoleByNameAsync(It.IsAny<string>())).ReturnsAsync((string r) => _roles.FirstOrDefault(x => x.Name == r));

            _permissionService = new PermissionService(_cacheManager, _worker.Object, _userService.Object, _userAccessor.Object);
        }

        [Test]
        public void Can_install_permission()
        {
            //arrange
            _userService.Setup(x => x.InsertRoleAsync(It.IsAny<Role>())).Callback((Role r) => _roles.Add(r)).Returns(Task.CompletedTask);

            //act
            _permissionService.InstallPermissionsAsync1(_objects);

            //assert
            _roles.Count.Should().Be(3);
        }
    }
}