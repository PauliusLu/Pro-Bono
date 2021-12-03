using Microsoft.VisualStudio.TestTools.UnitTesting;
using Karma.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Karma.Data;
using Moq;
using Autofac.Extras.Moq;
using Karma.Models;
using Microsoft.AspNetCore.Hosting;

namespace Karma.Controllers.Tests
{
    [TestClass()]
    public class ProfileControllerTests
    {

        private readonly ProfileController _sut;
        private readonly Mock<KarmaContext> _contextMock = new Mock<KarmaContext>();
        private readonly Mock<IWebHostEnvironment> _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();

        public ProfileControllerTests()
            {
            _sut = new ProfileController(_contextMock.Object, _webHostEnvironmentMock.Object);
            }

        [Fact]
        public async Task Edit_()
        {
            

            // Arrange
            var id = Guid.NewGuid();
            var userDto = new User
            {
                UserName = "Labas",
                Name = "l",
                Surname = "aa"
            };
            
            var user = await _sut.Edit(id.ToString());
            _contextMock.Setup(x => x.Edit(id)).
                ReturnAsync(userDto);
            // Act
            var userTest = await _sut.Edit(id.ToString());
            // Assert

            Assert.Equal();

        }

        [Fact]
        public void LoadPost_ValidCall()
        {
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<KarmaContext>()
                    .Setup(x => x.Post);
            }
            throw new NotImplementedException();    
        }



        [TestMethod()]
        public void EditTest()
        {
            Assert.Fail();
        }
        private List<Post> GetTestSessions()
        {
            var sessions = new List<Post>();
            sessions.Add(new Post()
            {
                Id = 1,
                UserId = "test1",
                IsDonation = false,
                Date = new DateTime(2021, 12, 1),
                Title = "Kede",
                ItemType = 1,
                Description = "Nauja kede",
                IsVisible = true
            }) ;
            sessions.Add(new Post()
            {
                Id = 1,
                UserId = "test2",
                IsDonation = true,
                Date = new DateTime(2021, 12, 2),
                Title = "stalas",
                ItemType = 1,
                Description = "Naujas stalas",
                IsVisible = true
            });
            return sessions;
        }
    }
}