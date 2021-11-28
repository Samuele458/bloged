using System;
using System.Reflection;

namespace BlogedWebapp.Helpers
{
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
        public static void Update(object entityObject, object updateObject)
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

                //Update field only if different from default value (null if reference)
                if ((propertyType.IsValueType && updateValue != Activator.CreateInstance(propertyType)) ||
                   (!propertyType.IsValueType && updateValue != null))
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
