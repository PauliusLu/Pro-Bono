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

namespace Karma.Controllers.Tests
{
    [TestClass()]
    public class ProfileControllerTests
    {


        [Fact]
        public async Task Index_ReturnsAViewResult_()
        {
            // Arrange
            var ProfileMockAgent = new Mock<KarmaContext>();
            var controller = new ProfileController(ProfileMockAgent.Object);
            controller.RouteData();
            // Act


            // Assert



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