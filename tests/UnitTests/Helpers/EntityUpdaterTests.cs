using BlogedWebapp.Entities;
using BlogedWebapp.Helpers;
using BlogedWebapp.Models.Dtos.Requests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BlogedWebbapp.UnitTests.Helpers
{
    [TestClass]
    public class EntityUpdaterTests
    {
        [TestMethod]
        public void Update_UpdateEntityWithRightObject_EntityUpdated()
        {
            // Entity object to be updated
            var entityObject = new ProfileData()
            {
                FirstName = "John",
                LastName = "Doe"
            };

            // Data with which update entity object
            var updateObject = new UpdateProfileRequestDto()
            {
                FirstName = "Jonathan",
                LastName = "Philips"
            };

            EntityUpdater.Update(entityObject, updateObject);

            Assert.AreEqual(entityObject.FirstName, "Jonathan");
            Assert.AreEqual(entityObject.LastName, "Philips");
        }

        [TestMethod]
        public void Update_UpdateEntityPartialFields_EntityUpdated()
        {
            // Entity object to be updated
            var entityObject = new ProfileData()
            {
                FirstName = "John",
                LastName = "Doe"
            };

            // Data with which update entity object
            var updateObject = new UpdateProfileRequestDto()
            {
                FirstName = "Jonathan"
            };

            EntityUpdater.Update(entityObject, updateObject);

            Assert.AreEqual(entityObject.FirstName, "Jonathan");
            Assert.AreEqual(entityObject.LastName, "Doe");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Update_PassingNullUpdateObject_ExceptionThrown()
        {
            // Entity object to be updated
            var entityObject = new ProfileData()
            {
                FirstName = "John",
                LastName = "Doe"
            };

            // Data with which update entity object
            object updateObject = null;

            EntityUpdater.Update(entityObject, updateObject);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Update_PassingNullEntityObject_ExceptionThrown()
        {
            // Entity object to be updated
            object entityObject = null;

            // Data with which update entity object
            var updateObject = new UpdateProfileRequestDto()
            {
                FirstName = "Jonathan"
            };

            EntityUpdater.Update(entityObject, updateObject);
        }
    }
}
