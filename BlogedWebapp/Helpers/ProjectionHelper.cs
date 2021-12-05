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


    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class RelatedEntity : Attribute
    {

    }


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

    public static class ProjectionHelper<T> where T : class
    {
        public static IQueryable<T> BuildProjection(
            IQueryable<T> dbset,
            ProjectionBehaviour projectionBehaviour
        )
        {
            var entityType = typeof(T);

            var entityParameter = Expression.Parameter(typeof(T), "o");

            var lambda = Expression.Lambda<Func<T, T>>(MakeEntityObject(entityType, entityParameter, projectionBehaviour), entityParameter);

            System.Diagnostics.Debug.WriteLine(lambda.ToString());
            System.Diagnostics.Debug.WriteLine(dbset.Select(lambda).ToQueryString());

            return dbset.Select(lambda);

        }

        // public static Expression<Func<T, T>> MakeProjectionLambda()
        //{

        //}

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

            var constructor = Expression.New(entityType);

            List<MemberBinding> bindings = new List<MemberBinding>();

            foreach (var property in properties)
            {
                var relatedEntityAttribute = property.GetCustomAttribute<RelatedEntity>(true);
                var projectionAttribute = property.GetCustomAttribute<Projection>(true);

                if (projectionAttribute == null)
                {
                    projectionAttribute = new Projection(ProjectionBehaviour.Normal);
                }


                if (IsInProjection(projectionBehaviour, projectionAttribute.Behaviour))
                {
                    var value = Expression.Property(objectToAssign, property);
                    MemberAssignment assignment;

                    if (relatedEntityAttribute == null)
                    {
                        assignment = Expression.Bind(property, value);
                    }
                    else
                    {
                        assignment = Expression.Bind(property, MakeEntityObject(value.Type, value, projectionBehaviour));
                    }

                    bindings.Add(assignment);
                }

            }

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
