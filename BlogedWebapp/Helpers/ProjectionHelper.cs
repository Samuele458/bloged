using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
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
        ///  Property is ignored
        /// </summary>
        None,

        /// <summary>
        ///  Projection in Preview mode
        /// </summary>
        Preview,

        /// <summary>
        ///  Projection in Normal mode
        /// </summary>
        Normal,

        /// <summary>
        ///  Projection in Full mode
        /// </summary>
        Full
    }

    /// <summary>
    ///  Specifies behaviour to follow with related entities (nested objects)
    /// </summary>
    public enum NestedProjectionBehaviour
    {
        /// <summary>
        ///  Nested entities are not projected
        /// </summary>
        None,

        /// <summary>
        ///  ProjectionBehaviour is lowered on each nested entity (default)
        /// </summary>
        LowerProjection,

        /// <summary>
        ///  ProjectionBehaviour is always equal, also in nested entities
        /// </summary>
        Equal
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

        /// <summary>
        ///  Based on NestedProjectionBehaviour value, evaluates the new value 
        ///  of ProjectionBehaviour for children entities.
        /// </summary>
        /// <param name="nestedProjectionBehaviour">Nested Projection Behaviour</param>
        /// <param name="projectionBehaviour">Current Projection Behaviour</param>
        /// <returns>New ProjectionBehaviour for children</returns>
        public static ProjectionBehaviour EvaluateNewProjectionBehaviour(
            NestedProjectionBehaviour nestedProjectionBehaviour,
            ProjectionBehaviour projectionBehaviour
        )
        {
            ProjectionBehaviour returnValue = ProjectionBehaviour.None;

            if (nestedProjectionBehaviour == NestedProjectionBehaviour.LowerProjection)
            {
                // Behaviour il lowered by one step (full --> normal, normal --> preview, etc)
                switch (projectionBehaviour)
                {
                    case ProjectionBehaviour.Preview:
                        returnValue = ProjectionBehaviour.None;
                        break;

                    case ProjectionBehaviour.Normal:
                        returnValue = ProjectionBehaviour.Preview;
                        break;

                    case ProjectionBehaviour.Full:
                        returnValue = ProjectionBehaviour.Normal;
                        break;
                }
            }
            else if (nestedProjectionBehaviour == NestedProjectionBehaviour.Equal)
            {
                // The behaviour is the same
                returnValue = projectionBehaviour;
            }

            return returnValue;
        }
    }


    /// <summary>
    ///  Helper class for handling projections on Entity Framework
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ProjectionHelper<T> where T : class
    {


        /// <summary>
        ///  Build lambda representing projection of the specified entity
        /// </summary>
        /// <param name="type">Entity type to be projected</param>
        /// <param name="parameter">Parameter to be used in tha lambda function</param>
        /// <param name="projectionBehaviour">Projection Behaviour</param>
        /// <param name="nestedProjectionBehaviour">
        ///  Determines the projection behaviour with which the children
        ///  entitities should be projected. By default, for each child entity
        ///  the projection behaviour is lower by one step
        /// </param>
        /// <param name="alreadySelectedTypes">
        ///  List of types of entities already selected,
        ///  In order to avoid circular dependency problems 
        /// </param>
        /// <returns></returns>
        public static Expression BuildProjectionLambda(
            Type type,
            ParameterExpression parameter,
            ProjectionBehaviour projectionBehaviour = ProjectionBehaviour.Normal,
            NestedProjectionBehaviour nestedProjectionBehaviour = NestedProjectionBehaviour.LowerProjection,
            List<Type> alreadySelectedTypes = null
        )
        {
            alreadySelectedTypes ??= new List<Type>();

            // Lambda function
            var lambda = Expression.Lambda(
                MakeEntityObject(
                    type,
                    parameter,
                    projectionBehaviour,
                    nestedProjectionBehaviour,
                    alreadySelectedTypes
                ),
                parameter
            );

            return lambda;
        }

        /// <summary>
        ///  Build a projection
        /// </summary>
        /// <param name="dbset">DBSet on which make projection</param>
        /// <param name="projectionBehaviour">Projection behaviour</param>
        /// <param name="nestedProjectionBehaviour">
        ///  Determines the projection behaviour with which the children
        ///  entitities should be projected. By default, for each child entity
        ///  the projection behaviour is lower by one step
        /// </param>
        /// <param name="alreadySelectedTypes">
        ///  List of types of entities already selected,
        ///  In order to avoid circular dependency problems 
        /// </param>
        /// <returns>A Queryable object with applied projection filter</returns>
        public static IQueryable<T> BuildProjection(
            IQueryable<T> dbset,
            ProjectionBehaviour projectionBehaviour = ProjectionBehaviour.Normal,
            NestedProjectionBehaviour nestedProjectionBehaviour = NestedProjectionBehaviour.LowerProjection,
            List<Type> alreadySelectedTypes = null
        )
        {
            alreadySelectedTypes ??= new List<Type>();

            var entityType = typeof(T);

            // Parameter of lambda function
            var entityParameter = Expression.Parameter(typeof(T), "o");

            // Lambda function
            var lambda = Expression.Lambda<Func<T, T>>(
                MakeEntityObject(
                    entityType,
                    entityParameter,
                    projectionBehaviour,
                    nestedProjectionBehaviour,
                    alreadySelectedTypes
                ),
                entityParameter
            );

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
        /// <param name="nestedProjectionBehaviour">
        ///  Determines the projection behaviour with which the children
        ///  entitities should be projected. By default, for each child entity
        ///  the projection behaviour is lower by one step
        /// </param>
        /// <param name="alreadySelectedTypes">
        ///  List of types of entities already selected,
        ///  In order to avoid circular dependency problems 
        /// </param>
        /// <returns>The result expression</returns>
        public static Expression MakeEntityObject(
                Type entityType,
                Expression objectToAssign,
                ProjectionBehaviour projectionBehaviour,
                NestedProjectionBehaviour nestedProjectionBehaviour = NestedProjectionBehaviour.LowerProjection,
                List<Type> alreadySelectedTypes = null
            )
        {

            alreadySelectedTypes ??= new List<Type>();

            // Adding current type to already selected types
            alreadySelectedTypes.Add(entityType);

            var properties = entityType.GetProperties();
            Dictionary<PropertyInfo, RelatedEntity> propertiesWithRelatedEntity =
                new Dictionary<PropertyInfo, RelatedEntity>();

            Dictionary<PropertyInfo, Projection> propertiesWithProjection =
                new Dictionary<PropertyInfo, Projection>();


            // Constructor
            var constructor = Expression.New(entityType);

            // The list with all assignments for the current object
            List<MemberBinding> bindings = new List<MemberBinding>();

            // Sorting properties by name
            Array.Sort(properties, (a, b) => string.Compare(a.Name, b.Name));


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
                if (IsInProjection(projectionBehaviour, projectionAttribute.Behaviour) &&
                    projectionBehaviour > ProjectionBehaviour.None)
                {
                    // Property is in projection

                    var value = Expression.Property(objectToAssign, property);
                    MemberAssignment assignment = null;

                    // Checking if property is a normal value or has the RelatedEntity attribute. 
                    // Also, if nestedProjectionBehaviour is to none, also related entities must be
                    // projected as normal fields
                    if (relatedEntityAttribute == null ||
                        nestedProjectionBehaviour == NestedProjectionBehaviour.None ||
                        alreadySelectedTypes.Contains(value.Type))
                    {
                        // Property is a normal attribute.
                        // The assignment can be done just with the value.
                        assignment = Expression.Bind(property, value);
                    }
                    else
                    {

                        // Checking if entity is a list or not
                        if (value.Type != typeof(string) && value.Type.GetInterfaces().Contains(typeof(IEnumerable)))
                        {
                            // Current entity is a collection/list

                            // Getting generic type of enumerable
                            Type listType = value.Type.GenericTypeArguments[0];

                            // Getting "Select" method, to be called later
                            MethodInfo selectMethod = typeof(Enumerable)
                                                        .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
                                                        .Where(method => method.Name == "Select")
                                                        .First()
                                                        .MakeGenericMethod(listType, listType);


                            var parameter = Expression.Parameter(listType, "z");

                            // Building lambda
                            var lambda = BuildProjectionLambda(
                                listType,
                                parameter,
                                Projection.EvaluateNewProjectionBehaviour(
                                    nestedProjectionBehaviour,
                                    projectionBehaviour
                                ),
                                nestedProjectionBehaviour,
                                alreadySelectedTypes
                            );

                            // Select call e.g. o.Select(z => new ...)
                            var selectCall = Expression.Call(null, selectMethod, value, lambda);

                            // Getting ToList method
                            MethodInfo toListMethod = typeof(Enumerable)
                                                        .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
                                                        .Where(method => method.Name == "ToList")
                                                        .First()
                                                        .MakeGenericMethod(listType);

                            // .ToList() call
                            var toListCall = Expression.Call(null, toListMethod, selectCall);

                            assignment = Expression.Bind(property, toListCall);

                        }
                        else
                        {
                            // Current entity is not a collection

                            // Property is a sub entity.
                            // The assignment must be done by creating a new object
                            // of the specified entity type. This is done by calling 
                            // Recursively MakeEntityObject
                            assignment = Expression.Bind(
                                property,
                                MakeEntityObject(
                                    value.Type,
                                    value,
                                    Projection.EvaluateNewProjectionBehaviour(
                                        nestedProjectionBehaviour,
                                        projectionBehaviour
                                    ),
                                    nestedProjectionBehaviour,
                                    alreadySelectedTypes
                                )
                            );
                        }

                    }

                    // Add assignment to bindings (the list with all assignments for the current object)
                    if (assignment != null)
                    {
                        bindings.Add(assignment);
                    }
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
