using System;
using System.Reflection;

namespace BlogedWebapp.Helpers
{
    public static class EntityUpdater
    {
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

                if ((propertyType.IsValueType && updateValue != Activator.CreateInstance(propertyType)) ||
                   (!propertyType.IsValueType && updateValue != null))
                {
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
