namespace Microsoft.WindowsAzure.ActiveDirectory.GraphClient
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class DirectorySchemaExtensionsHelper
    {
        /// <summary>
        /// Adds schema extensions to a specified object. The object passed in is usually a strongly typed version of an 
        /// object in the Azure Active Directory that supports schema extensions (such as AadUser, AadTenantDetails etc).
        /// </summary>
        /// <param name="o">The object to which schema extension values should be added.</param>
        /// <param name="extensionValues">The extension names and values to add to the object.</param>
        /// <returns>
        /// A <see cref="JToken"/> object which contains all the original properties present in the object passed in and the 
        /// additional schema extensions that were added.
        /// </returns>
        /// <remarks>
        /// Currently, only string and byte[] values are supported for schema extensions. Any other type of value will throw
        /// an <see cref="InvalidOperationException"/>.
        /// </remarks>
        public static JToken AddSchemaExtensionsToObject(object o, IDictionary<string, object> extensionValues)
        {
            // Create serializer settings that will ignore null values on the original object
            // before converting to JToken. This makes sure that when the JToken itself is serialized 
            // to JSON string, the null values don't materialize in the string representation.
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            JsonSerializer serializer = JsonSerializer.CreateDefault(jsonSettings);
            JToken extendedObject = JToken.FromObject(o, serializer);
            foreach (var kvpair in extensionValues)
            {
                if (kvpair.Value is string)
                {
                    extendedObject[kvpair.Key] = (string)kvpair.Value;
                }
                else if (kvpair.Value is byte[])
                {
                    extendedObject[kvpair.Key] = Convert.ToBase64String((byte[])kvpair.Value);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            return extendedObject;
        }
    }
}
