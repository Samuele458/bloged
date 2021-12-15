using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BlogedWebapp.Helpers
{
    /// <summary>
    ///  Projection behaviour for querying database
    /// </summary>
    public enum ProjectionBehaviour
    {
        /// <summary>
        ///  Gets just the most important and lightweight information
        /// </summary>
        Minimal,

        /// <summary>
        ///  Default projection behaviour
        ///  It does not remove fields, but also does not include related entities
        /// </summary>
        Default,

        /// <summary>
        ///  Includes related entities to projection
        /// </summary>
        IncludeRelated,

        /// <summary>
        ///  Include related entities to projection and recersively add
        ///  their related entities.
        /// </summary>
        IncludeRecursively,

        //TODO set comments
        Preview,
        Normal,
        Full
    }

    /// <summary>
    ///  This attribute, if present on a property, specifies if that
    ///  property has to be handled as to be handled as a sub entity
    ///  (and so will be processed for projection of its fields), or
    ///  a normal field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class RelatedEntity : Attribute
    {

    }

    /// <summary>
    ///  Attribute for specifing the behaviour each
    ///  property has to follow.
    ///  If Projection attribute is not present, the behaviour for that
    ///  property will be equal to [Projection(ProjectionBehaviour.Normal)]
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class Projection : Attribute
    {
        private ProjectionBehaviour projectionBehaviour;

        public Projection(ProjectionBehaviour projectionBehaviour)
        {
            this.projectionBehaviour = projectionBehaviour;
        }

        public ProjectionBehaviour Behaviour
        {
            get { return this.projectionBehaviour; }
        }
    }


    /// <summary>
    ///  Helper class for handling projections on Entity Framework
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ProjectionHelper<T> where T : class
    {

        public static Expression<Func<T, T>> BuildProjectionLambda(
            IQueryable<T> dbset,
            ProjectionBehaviour projectionBehaviour
        )
        {
            var entityType = typeof(T);

            // Parameter of lambda function
            var entityParameter = Expression.Parameter(typeof(T), "o");

            // Lambda function
            var lambda = Expression.Lambda<Func<T, T>>(MakeEntityObject(entityType, entityParameter, projectionBehaviour), entityParameter);

            return lambda;
        }

        /// <summary>
        ///  Build a projection
        /// </summary>
        /// <param name="dbset">DBSet on which make projection</param>
        /// <param name="projectionBehaviour">Projection behaviour</param>
        /// <returns>A Queryable object with applied projection filter</returns>
        public static IQueryable<T> BuildProjection(
            IQueryable<T> dbset,
            ProjectionBehaviour projectionBehaviour
        )
        {
            var entityType = typeof(T);

            // Parameter of lambda function
            var entityParameter = Expression.Parameter(typeof(T), "o");

            // Lambda function
            var lambda = Expression.Lambda<Func<T, T>>(MakeEntityObject(entityType, entityParameter, projectionBehaviour), entityParameter);

            // Debugging
            System.Diagnostics.Debug.WriteLine(lambda.ToString());
            System.Diagnostics.Debug.WriteLine(dbset.Select(lambda).ToQueryString());

            // Applies projection and returns the result object (IQueryble)
            return dbset.Select(lambda);

        }

        /// <summary>
        ///  Creates an entity object dynamically.
        ///  Based on projectionBehaviour, MakeEntityObject can be applied recursively
        ///  on sub fields, in order to be able to make projection also in sub entities.
        ///  It is based on reflection.
        /// </summary>
        /// <param name="entityType">Entity type to be created</param>
        /// <param name="objectToAssign">
        ///  Expression object to be assigned to.
        ///  eg. the parameter of a lambda function, or another object
        ///  in case of a recursive call for filling sub entities
        /// </param>
        /// <param name="projectionBehaviour">
        ///  Behaviour to follow in order to make the projection.
        ///  It can allow to apply this method recursively to sub entities, in
        ///  order to do projection also in them
        /// </param>
        /// <returns>The result expression</returns>
        public static Expression MakeEntityObject(
                Type entityType,
                Expression objectToAssign,
                ProjectionBehaviour projectionBehaviour
            )
        {
            var properties = entityType.GetProperties();
            Dictionary<PropertyInfo, RelatedEntity> propertiesWithRelatedEntity =
                new Dictionary<PropertyInfo, RelatedEntity>();

            Dictionary<PropertyInfo, Projection> propertiesWithProjection =
                new Dictionary<PropertyInfo, Projection>();

            // Constructor
            var constructor = Expression.New(entityType);

            // The list with all assignments for the current object
            List<MemberBinding> bindings = new List<MemberBinding>();

            foreach (var property in properties)
            {
                var relatedEntityAttribute = property.GetCustomAttribute<RelatedEntity>(true);
                var projectionAttribute = property.GetCustomAttribute<Projection>(true);

                // Checking if property has Projection attribute.
                // If not, it should be handled as ProjectionBehaviour.Normal
                if (projectionAttribute == null)
                {
                    projectionAttribute = new Projection(ProjectionBehaviour.Normal);
                }

                // Checking if current property is in projection, based on the ProjectionBehaviour to follow
                if (IsInProjection(projectionBehaviour, projectionAttribute.Behaviour))
                {
                    // Property is in projection

                    var value = Expression.Property(objectToAssign, property);
                    MemberAssignment assignment;


                    // Checking if property is a normal value or has the RelatedEntity attribute. 
                    if (relatedEntityAttribute == null)
                    {
                        // Property is a normal attribute.
                        // The assignment can be done just with the value.
                        assignment = Expression.Bind(property, value);
                    }
                    else
                    {
                        // Property is a sub entity.
                        // The assignment must be done by creating a new object
                        // of the specified entity type. This is done by calling 
                        // Recursively MakeEntityObject
                        assignment = Expression.Bind(property, MakeEntityObject(value.Type, value, projectionBehaviour));
                    }

                    // Add assignment to bindings (the list with all assignments for the current object)
                    bindings.Add(assignment);
                }

            }

            // returns the object
            return Expression.MemberInit(constructor, bindings);
        }

        /// <summary>
        ///  Determines wether a property should be in projection or not
        /// </summary>
        /// <param name="projectionBehaviourToFollow">Projectionbehaviour to follow</param>
        /// <param name="propertyProjectionBehaviour">ProjectionBehaviour of the resource to be checked</param>
        /// <returns>True if property is in projection, false otherwise</returns>
        public static bool IsInProjection(
                ProjectionBehaviour projectionBehaviourToFollow,
                ProjectionBehaviour propertyProjectionBehaviour
            )
        {
            return projectionBehaviourToFollow >= propertyProjectionBehaviour;
        }

    }
}
