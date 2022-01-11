using BlogedWebapp.Entities;
using BlogedWebapp.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

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

        public class TestEntity_WithoutFields
        {

        }

        public class TestEntity_WithNormalField
        {
            [Projection(ProjectionBehaviour.Normal)]
            public string Email { get; set; }
        }

        public class TestEntity_WithPreviewField
        {
            [Projection(ProjectionBehaviour.Preview)]
            public string Email { get; set; }
        }

        public class TestEntity_WithFullField
        {
            [Projection(ProjectionBehaviour.Full)]
            public string Email { get; set; }
        }

        public class TestEntity_WithCircularDependency_A
        {
            public string Name { get; set; }
            public string Surname { get; set; }

            [Projection(ProjectionBehaviour.Normal)]
            [RelatedEntity]
            public TestEntity_WithCircularDependency_B SubEntity_B { get; set; }
        }

        public class TestEntity_WithCircularDependency_B
        {
            public string AddressName { get; set; }

            public string AddressNum { get; set; }

            [Projection(ProjectionBehaviour.Normal)]
            [RelatedEntity]
            public TestEntity_WithCircularDependency_A SubEntity_A { get; set; }

        }

        public class TestEntity_WithCircularDependencyWithList_A : TestEntity_WithCircularDependency_A
        {

            public ICollection<TestEntity_WithCircularDependency_B> SubEntityList_B { get; set; }
        }

        public class TestEntity_WithCircularDependencyWithList_B : TestEntity_WithCircularDependency_A
        {
            [RelatedEntity]
            public ICollection<TestEntity_WithCircularDependency_B> _SubEntityList_B { get; set; }
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


        private static string StringifyProjectionLambda<T>(
            ProjectionBehaviour behaviour = ProjectionBehaviour.Normal,
            NestedProjectionBehaviour nestedProjectionBehaviour = NestedProjectionBehaviour.LowerProjection
        ) where T : class
        {
            var param = Expression.Parameter(typeof(T), "o");

            var result = ProjectionHelper<T>
                            .BuildProjectionLambda(
                                typeof(T),
                                param,
                                behaviour,
                                nestedProjectionBehaviour
                            ).ToString();
            return result;
        }


        private static bool LambdaResultsEqual(string lambda1, string lambda2)
        {
            Regex whitespacesRegex = new Regex(@"\s+");

            string lambda1Sanitized = whitespacesRegex.Replace(lambda1, "");
            string lambda2Sanitized = whitespacesRegex.Replace(lambda2, "");

            return lambda1Sanitized.Equals(lambda2Sanitized);
        }

        [TestMethod]
        public void BuildProjectionLambda_EntityWithoutFields_ResultCorrect()
        {
            var result = StringifyProjectionLambda<TestEntity_WithoutFields>();
            var expectedResult = "o => new TestEntity_WithoutFields() {}";

            Assert.IsTrue(result == expectedResult);
        }

        [TestMethod]
        public void BuildProjectionLambda_PreviewAccessInNormalField_ResultCorrect()
        {
            var result = StringifyProjectionLambda<TestEntity_WithNormalField>(ProjectionBehaviour.Preview);
            var expectedResult = "o => new TestEntity_WithNormalField() {}";

            Assert.IsTrue(result == expectedResult);
        }

        [TestMethod]
        public void BuildProjectionLambda_PreviewAccessInPreviewField_ResultCorrect()
        {
            var result = StringifyProjectionLambda<TestEntity_WithPreviewField>(ProjectionBehaviour.Preview);
            var expectedResult = "o => new TestEntity_WithPreviewField() {Email = o.Email}";

            Assert.IsTrue(result == expectedResult);
        }

        [TestMethod]
        public void BuildProjectionLambda_PreviewAccessInFullField_ResultCorrect()
        {
            var result = StringifyProjectionLambda<TestEntity_WithFullField>(ProjectionBehaviour.Preview);
            var expectedResult = "o => new TestEntity_WithFullField() {}";

            Assert.IsTrue(result == expectedResult);
        }

        [TestMethod]
        public void BuildProjectionLambda_NormalAccessInNormalField_ResultCorrect()
        {
            var result = StringifyProjectionLambda<TestEntity_WithNormalField>();
            var expectedResult = "o => new TestEntity_WithNormalField() {Email = o.Email}";

            Assert.IsTrue(result == expectedResult);
        }

        [TestMethod]
        public void BuildProjectionLambda_NormalAccessInPreviewField_ResultCorrect()
        {
            var result = StringifyProjectionLambda<TestEntity_WithPreviewField>();
            var expectedResult = "o => new TestEntity_WithPreviewField() {Email = o.Email}";

            Assert.IsTrue(result == expectedResult);
        }

        [TestMethod]
        public void BuildProjectionLambda_NormalAccessInFullField_ResultCorrect()
        {
            var result = StringifyProjectionLambda<TestEntity_WithFullField>();
            var expectedResult = "o => new TestEntity_WithFullField() {}";

            Assert.IsTrue(result == expectedResult);
        }



        [TestMethod]
        public void BuildProjectionLambda_FullAccessInNormalField_ResultCorrect()
        {
            var result = StringifyProjectionLambda<TestEntity_WithNormalField>(ProjectionBehaviour.Full);
            var expectedResult = "o => new TestEntity_WithNormalField() {Email = o.Email}";

            Assert.IsTrue(result == expectedResult);
        }

        [TestMethod]
        public void BuildProjectionLambda_FullAccessInPreviewField_ResultCorrect()
        {
            var result = StringifyProjectionLambda<TestEntity_WithPreviewField>(ProjectionBehaviour.Full);
            var expectedResult = "o => new TestEntity_WithPreviewField() {Email = o.Email}";

            Assert.IsTrue(result == expectedResult);
        }

        [TestMethod]
        public void BuildProjectionLambda_FullAccessInFullField_ResultCorrect()
        {
            var result = StringifyProjectionLambda<TestEntity_WithFullField>(ProjectionBehaviour.Full);
            var expectedResult =
                @"o => new TestEntity_WithFullField() {
                    Email = o.Email
                }";

            Assert.IsTrue(LambdaResultsEqual(result, expectedResult));
        }


        [TestMethod]
        public void BuildProjectionLambda_PreviewAccessBlogEntity_ResultCorrect()
        {
            var result = StringifyProjectionLambda<Blog>(ProjectionBehaviour.Preview);
            var expectedResult = @"
                o => new Blog() {
                    Id = o.Id,
                    Status = o.Status,
                    Title = o.Title,
                    UrlName = o.UrlName
                }";

            Assert.IsTrue(LambdaResultsEqual(result, expectedResult));
        }

        [TestMethod]
        public void BuildProjectionLambda_NormalAccessBlogEntity_ResultCorrect()
        {
            var result = StringifyProjectionLambda<Blog>(ProjectionBehaviour.Normal);
            var expectedResult = @"
                o => new Blog() {
		            Description = o.Description, 
		            Id = o.Id, 
		            Posts = o.Posts.Select(z => new Post() {
			            AuthorId = z.AuthorId, 
			            Id = z.Id, 
			            OwnerId = z.OwnerId, 
			            Status = z.Status, 
			            Title = z.Title, 
			            UrlName = z.UrlName
		            }).ToList(), 
		            Status = o.Status, 
		            Title = o.Title, 
		            UrlName = o.UrlName, 
		            UsersBlog = o.UsersBlog.Select(z => new UsersBlog() {
			            BlogId = z.BlogId, 
			            Id = z.Id, 
			            OwnerId = z.OwnerId, 
			            Role = z.Role, Status = z.Status}).ToList()
		        }";

            Assert.IsTrue(LambdaResultsEqual(result, expectedResult));
        }


        [TestMethod]
        public void BuildProjectionLambda_FullAccessBlogEntity_ResultCorrect()
        {
            var result = StringifyProjectionLambda<Blog>(ProjectionBehaviour.Full);
            var expectedResult = @"
                o => new Blog() {
	                CreatedOn = o.CreatedOn, 
	                Description = o.Description, 
	                Id = o.Id, 
	                Posts = o.Posts.Select(z => new Post() {
		                AuthorId = z.AuthorId, 
		                Content = z.Content, 
		                Id = z.Id, 
		                OwnerId = z.OwnerId, 
		                Status = z.Status, 
		                Title = z.Title, 
		                UrlName = z.UrlName
	                }).ToList(), 
	                Status = o.Status, 
	                Title = o.Title, 
	                UpdatedOn = o.UpdatedOn, 
	                UrlName = o.UrlName, 
	                UsersBlog = o.UsersBlog.Select(z => new UsersBlog() {
		                BlogId = z.BlogId, 
		                Id = z.Id, 
		                OwnerId = z.OwnerId, 
		                Role = z.Role, 
		                Status = z.Status
	                }).ToList()
                }";

            Assert.IsTrue(LambdaResultsEqual(result, expectedResult));
        }

        [TestMethod]
        public void BuildProjectionLambda_TestCircularDependency_InfinityLoopAvoided()
        {
            var result1 = StringifyProjectionLambda<TestEntity_WithCircularDependency_A>(
                ProjectionBehaviour.Full,
                NestedProjectionBehaviour.LowerProjection
            );

            var expectedResult1 = @"
            o => new TestEntity_WithCircularDependency_A() {
                Name = o.Name,
                SubEntity_B = new TestEntity_WithCircularDependency_B()
                {
                    AddressName = o.SubEntity_B.AddressName,
                    AddressNum = o.SubEntity_B.AddressNum,
                    SubEntity_A = o.SubEntity_B.SubEntity_A
                },
                Surname = o.Surname
            }";

            var result2 = StringifyProjectionLambda<TestEntity_WithCircularDependency_B>(
                ProjectionBehaviour.Full,
                NestedProjectionBehaviour.Equal
            );

            var expectedResult2 = @"
            o => new TestEntity_WithCircularDependency_B() {
	            AddressName = o.AddressName, 
	            AddressNum = o.AddressNum, 
	            SubEntity_A = new TestEntity_WithCircularDependency_A() {
		            Name = o.SubEntity_A.Name, 
		            SubEntity_B = o.SubEntity_A.SubEntity_B, 
		            Surname = o.SubEntity_A.Surname
	            }
            }";

            Assert.IsTrue(LambdaResultsEqual(result1, expectedResult1));
            Assert.IsTrue(LambdaResultsEqual(result2, expectedResult2));
        }


        [TestMethod]
        public void BuildProjectionLambda_TestCircularDependencyWithLists_InfinityLoopAvoided()
        {
            var result1 = StringifyProjectionLambda<TestEntity_WithCircularDependencyWithList_A>(
                ProjectionBehaviour.Full,
                NestedProjectionBehaviour.Equal
            );

            var expectedResult1 = @"
            o => new TestEntity_WithCircularDependencyWithList_A() {
	            Name = o.Name, 
	            SubEntity_B = new TestEntity_WithCircularDependency_B() {
		            AddressName = o.SubEntity_B.AddressName, 
		            AddressNum = o.SubEntity_B.AddressNum, 
		            SubEntity_A = new TestEntity_WithCircularDependency_A() {
			            Name = o.SubEntity_B.SubEntity_A.Name, 
			            SubEntity_B = o.SubEntity_B.SubEntity_A.SubEntity_B, 
			            Surname = o.SubEntity_B.SubEntity_A.Surname
		            }
	            }, 
	            SubEntityList_B = o.SubEntityList_B, 
	            Surname = o.Surname
            }";

            var result2 = StringifyProjectionLambda<TestEntity_WithCircularDependencyWithList_B>(
                ProjectionBehaviour.Full,
                NestedProjectionBehaviour.Equal
            );

            var expectedResult2 = @"
            o => new TestEntity_WithCircularDependencyWithList_B() {
	            _SubEntityList_B = o._SubEntityList_B.Select(
		            z => new TestEntity_WithCircularDependency_B() {
			            AddressName = z.AddressName, 
			            AddressNum = z.AddressNum, 
			            SubEntity_A = new TestEntity_WithCircularDependency_A() {
				            Name = z.SubEntity_A.Name, 
				            SubEntity_B = z.SubEntity_A.SubEntity_B, 
				            Surname = z.SubEntity_A.Surname
			            }
		            }
	            ).ToList(),
	            Name = o.Name, 
	            SubEntity_B = o.SubEntity_B, 
	            Surname = o.Surname
            }";

            Assert.IsTrue(LambdaResultsEqual(result1, expectedResult1));
            Assert.IsTrue(LambdaResultsEqual(result2, expectedResult2));
        }



    }
}
