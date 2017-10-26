using Anywhere.ArcGIS.Operation;
using System;
using System.Collections.Generic;

namespace Anywhere.ArcGIS
{
    /// <summary>
    /// Used for (de)serializtion of requests and responses. 
    /// </summary>
    /// <remarks>Split out as interface to allow injection. Also moves implementation out of this
    /// library so it can use whatever framework the developer wants.</remarks>
    public interface ISerializer
    {
        /// <summary>
        /// Convert an object into a dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToConvert"></param>
        /// <returns></returns>
        Dictionary<string, string> AsDictionary<T>(T objectToConvert) where T : CommonParameters;

        /// <summary>
        /// Deserialize string as a <see cref="IPortalResponse"/>
        /// </summary>
        /// <typeparam name="T">The type of the result from the call</typeparam>
        /// <param name="dataToConvert">Json string to deserialize</param>
        /// <returns></returns>
        T AsPortalResponse<T>(string dataToConvert) where T : IPortalResponse;
    }

    /// <summary>
    /// Simple factory for allowing the serializer to be accessed from gateway and token providers.
    /// </summary>
    public static class SerializerFactory
    {
        /// <summary>
        /// Used to return the default ISerializer used by gateway and token providers assuming that 
        /// none are passed to them in their constructors.
        /// This should be overridden in the platform specific implementations.
        /// </summary>
        public static Func<ISerializer> Get { get; set; }

        static SerializerFactory()
        {
            Get = (() => { return Serializers.JsonDotNetSerializer.Create(); });
        }
    }
}
