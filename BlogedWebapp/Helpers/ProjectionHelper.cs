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
            get { return this.Behaviour; }
        }
    }

    public static class ProjectionHelper<T> where T : class
    {
        public static IQueryable<T> BuildProjection(IQueryable<T> dbset)
        {
            var entityType = typeof(T);

            var entityParameter = Expression.Parameter(typeof(T), "o");

            var lambda = Expression.Lambda<Func<T, T>>(MakeEntityObject(entityType, entityParameter), entityParameter);
            //System.Diagnostics.Debug.WriteLine(lambda.ToString());
            return dbset.Select(lambda);

        }

        // public static Expression<Func<T, T>> MakeProjectionLambda()
        //{

        //}

        public static Expression MakeEntityObject(Type entityType, Expression objectToAssign)
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

                if (relatedEntityAttribute == null)
                {

                    var value = Expression.Property(objectToAssign, property);
                    var assignment = Expression.Bind(property, value);

                    bindings.Add(assignment);
                }
                else
                {
                    var value = Expression.Property(objectToAssign, property);
                    var assignment = Expression.Bind(property, MakeEntityObject(value.Type, value));

                    bindings.Add(assignment);
                }
            }

            return Expression.MemberInit(constructor, bindings);
        }

    }
}
