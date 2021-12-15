using BlogedWebapp.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnitTests.Helpers
{
    [TestClass]
    public class ProjectionHelperTests
    {

        // Classes used for doing tests

        public class TestEntity_WithProjectionAttributeOnProperty
        {
            [Projection(ProjectionBehaviour.Normal)]
            public string Name { get; set; }
        }

        public class TestEntity_WithoutProjectionAttributeOnProperty
        {
            public string Name { get; set; }
        }

        public class TestEntity_WithRelatedEntityAttributeOnProperty
        {
            [RelatedEntity]
            [Projection(ProjectionBehaviour.Normal)]
            public TestEntity_WithProjectionAttributeOnProperty Entity { get; set; }
        }

        public class TestEntity_WithoutRelatedEntityAttributeOnProperty
        {

            [Projection(ProjectionBehaviour.Normal)]
            public string Name { get; set; }
        }


        // --- Unit tests ---

        [TestMethod]
        public void ProjectionAttribute_PropertyWithProjectionAttribute_AttributeFound()
        {
            Type type = typeof(TestEntity_WithProjectionAttributeOnProperty);
            var attribute = type.GetProperties()[0].GetCustomAttribute<Projection>(true);

            Assert.IsNotNull(attribute);
        }

        [TestMethod]
        public void ProjectionAttribute_PropertyWithoutProjectionAttribute_AttributeNotFound()
        {
            Type type = typeof(TestEntity_WithoutProjectionAttributeOnProperty);
            var attribute = type.GetProperties()[0].GetCustomAttribute<Projection>(true);

            Assert.IsNull(attribute);
        }

        [TestMethod]
        public void RelatedEntityAttribute_PropertyWithRelatedEntityAttribute_AttributeFound()
        {
            Type type = typeof(TestEntity_WithRelatedEntityAttributeOnProperty);
            var attribute = type.GetProperties()[0].GetCustomAttribute<RelatedEntity>(true);

            Assert.IsNotNull(attribute);
        }

        [TestMethod]
        public void RelatedEntityAttribute_PropertyWithoutRelatedEntityAttribute_AttributeNotFound()
        {
            Type type = typeof(TestEntity_WithoutRelatedEntityAttributeOnProperty);
            var attribute = type.GetProperties()[0].GetCustomAttribute<RelatedEntity>(true);

            Assert.IsNull(attribute);
        }


        private static string StringifyProjectionLambda<T>() where T : class
        {
            List<T> list = new List<T>();
            IQueryable<T> queryable = list.AsQueryable();

            var result = ProjectionHelper<T>
                            .BuildProjectionLambda(queryable, ProjectionBehaviour.Normal)
                            .ToString();
            return result;
        }

        [TestMethod]
        public void BuildProjectionLambda()
        {
            var result = StringifyProjectionLambda<TestEntity_WithProjectionAttributeOnProperty>();
            var expectedResult = "o => new TestEntity_WithProjectionAttributeOnProperty() {Name = o.Name}";

            Assert.IsTrue(result == expectedResult);
        }

    }
}
