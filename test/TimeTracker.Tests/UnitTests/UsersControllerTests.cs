using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTracker.Controllers;
using TimeTracker.Data;
using TimeTracker.Domain;
using TimeTracker.Models;
using Xunit;

namespace TimeTracker.Tests.UnitTests
{
    public class UsersControllerTests
    {
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            var options = new DbContextOptionsBuilder<TimeTrackerDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var dbContext = new TimeTrackerDbContext(options);
            var logger = new FakeLogger<UsersController>();

            dbContext.Users.Add(new User {Id = 1, Name = "Test User 1", HourRate = 15});
            dbContext.Users.Add(new User {Id = 2, Name = "Test User 2", HourRate = 20});
            dbContext.Users.Add(new User {Id = 3, Name = "Test User 3", HourRate = 25});
            dbContext.SaveChanges();

            _controller = new UsersController(dbContext, logger);
        }

        [Fact]
        public async Task GetById_IdIsNonExisting_ReturnsNotFoundResult()
        {
            var result = await _controller.GetById(0);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetById_IdExists_ReturnsCorrectResult()
        {
            const string expectedName = "Test User 1";

            var result = await _controller.GetById(1);

            Assert.IsType<ActionResult<UserModel>>(result);
            Assert.NotNull(result.Value);
            Assert.Equal(expectedName, result.Value.Name);
        }

        [Fact]
        public async Task GetPage_FirstPage_ReturnsExpectedResult()
        {
            const int expectedCount = 3;
            const int expectedTotalCount = 3;

            var result = await _controller.GetPage(1, 10);

            Assert.IsType<ActionResult<PagedList<UserModel>>>(result);
            Assert.NotNull(result.Value);
            Assert.Equal(expectedCount, result.Value.Items.Count());
            Assert.Equal(expectedTotalCount, result.Value.TotalCount);
        }

        [Fact]
        public async Task GetPage_SecondPage_ReturnsExpectedResult()
        {
            const int expectedTotalCount = 3;

            var result = await _controller.GetPage(2, 10);

            Assert.IsType<ActionResult<PagedList<UserModel>>>(result);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value.Items);
            Assert.Equal(expectedTotalCount, result.Value.TotalCount);
        }

        [Fact]
        public async Task Delete_IdIsNotExisting_ReturnsNotFoundResult()
        {
            var result = await _controller.Delete(0);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_IdExists_ReturnsOkResult()
        {
            var result = await _controller.Delete(1);

            Assert.IsType<OkResult>(result);
        }
    }
}
