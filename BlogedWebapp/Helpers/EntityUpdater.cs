using System;
using System.Linq;
using System.Reflection;

namespace BlogedWebapp.Helpers
{

    /// <summary>
    ///  Entity updated mode
    /// </summary>
    public enum EntityUpdaterMode
    {
        /// <summary>
        ///  Update fields that are not null
        /// </summary>
        UpdateWhereNotNull = 1,

        /// <summary>
        /// Update fields that are different
        /// </summary>
        UpdateWhereDifferent = 2
    }

    /// <summary>
    ///  Entity updater helper class
    /// </summary>
    public static class EntityUpdater
    {
        /// <summary>
        ///  Update entity object fields with new fields contained in update object
        /// </summary>
        /// <param name="entityObject">Object to be updated</param>
        /// <param name="updateObject">Object cointaining updated fields</param>
        public static void Update(object entityObject, object updateObject, EntityUpdaterMode mode = EntityUpdaterMode.UpdateWhereNotNull)
        {
            if (entityObject == null)
            {
                throw new ArgumentNullException("entityObject");
            }

            if (updateObject == null)
            {
                throw new ArgumentNullException("updateObject");
            }


            // Getting entity info
            var entityType = entityObject.GetType();
            var entityProperties = entityType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            if (entityProperties.Length == 0)
            {
                throw new ArgumentException("Entity object has not public properties.");
            }

            // Getting update object info
            var updateObjectType = updateObject.GetType();
            var updateObjectProperties = updateObjectType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            if (updateObjectProperties.Length == 0)
            {
                throw new ArgumentException("Update object has not public properties.");
            }

            // Updating fields
            foreach (var propertyToUpdate in updateObjectProperties)
            {
                var updateValue = propertyToUpdate.GetValue(updateObject);

                var propertyType = propertyToUpdate.PropertyType;
                var defaultValue = propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null;

                bool condition = false;

                if ((mode & EntityUpdaterMode.UpdateWhereNotNull) == EntityUpdaterMode.UpdateWhereNotNull)
                {
                    condition |= (propertyType.IsValueType && updateValue != Activator.CreateInstance(propertyType)) ||
                   (!propertyType.IsValueType && updateValue != null);
                }

                if ((mode & EntityUpdaterMode.UpdateWhereDifferent) == EntityUpdaterMode.UpdateWhereDifferent)
                {
                    var propertyToBeUpdated = entityProperties
                                                .Where(prop =>
                                                    prop.Name == propertyToUpdate.Name &&
                                                    prop.PropertyType == propertyToUpdate.PropertyType
                                                ).FirstOrDefault();

                    condition |= propertyToBeUpdated != null &&
                                 propertyToUpdate.GetType().IsValueType &&
                                 propertyToBeUpdated.GetValue(entityObject) == propertyToUpdate.GetValue(updateObject);

                }

                //Update field only if different from default value (null if reference)
                if (condition)
                {
                    //Updating field
                    foreach (var propertyToBeUpdated in entityProperties)
                    {
                        if (propertyToBeUpdated.Name == propertyToUpdate.Name)
                        {
                            propertyToBeUpdated.SetValue(entityObject, updateValue);
                        }
                    }
                }
            }
        }
    }
}
