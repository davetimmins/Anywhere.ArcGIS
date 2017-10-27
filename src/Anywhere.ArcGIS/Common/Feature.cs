using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Anywhere.ArcGIS.Common
{
    /// <summary>
    /// JSON feature object as returned by the REST API. Typically used to represent a client side graphic
    /// </summary>
    /// <typeparam name="T">Type of geometry that the feature represents.</typeparam>
    /// <remarks>All properties are optional.</remarks>
    [DataContract]
    public class Feature<T> : IEquatable<Feature<T>> where T : IGeometry
    {
        const string ObjectIDName = "objectid";
        const string GlobalIDName = "globalid";

        public Feature()
        {
            Attributes = new Dictionary<string, object>();
        }

        [DataMember(Name = "geometry")]
        public T Geometry { get; set; }

        /// <summary>
        /// A JSON object that contains a dictionary of name-value pairs.
        /// The names are the feature field names.
        /// The values are the field values and they can be any of the standard JSON types - string, number and boolean.
        /// </summary>
        /// <remarks>Date values are encoded as numbers. The number represents the number of milliseconds since epoch (January 1, 1970) in UTC.</remarks>
        [DataMember(Name = "attributes")]
        public Dictionary<string, object> Attributes { get; set; }

        long _oid = 0;
        /// <summary>
        /// Get the ObjectID for the feature. Will return 0 if not found
        /// </summary>
        public long ObjectID
        {
            get
            {
                if (Attributes != null && Attributes.Any() && _oid == 0)
                {
                    var copy = new Dictionary<string, object>(Attributes, StringComparer.OrdinalIgnoreCase);
                    if (copy.ContainsKey(ObjectIDName)) long.TryParse(copy[ObjectIDName].ToString(), out _oid);
                }

                return _oid;
            }
        }

        Guid _globalID;
        /// <summary>
        /// Get the GlobalID for the feature. Will return an empty Guid if not found
        /// </summary>
        public Guid GlobalID
        {
            get
            {
                if (Attributes != null && Attributes.Any() && _globalID == Guid.Empty)
                {
                    var copy = new Dictionary<string, object>(Attributes, StringComparer.OrdinalIgnoreCase);
                    if (copy.ContainsKey(GlobalIDName)) Guid.TryParse(copy[GlobalIDName].ToString(), out _globalID);
                }

                return _globalID;
            }
        }

        public bool AttributesAreTheSame(Feature<T> other)
        {
            if (other == null || other.Attributes == null || Attributes == null || !Attributes.Any()) return false;

            return (Attributes.Count == other.Attributes.Count) && !(Attributes.Except(other.Attributes)).Any();
        }

        public bool Equals(Feature<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<T>.Default.Equals(Geometry, other.Geometry) && AttributesAreTheSame(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T>.Default.GetHashCode(Geometry) * 397) ^ (Attributes != null ? Attributes.GetHashCode() : 0);
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as Feature<T>;
            return other != null && Equals(other);
        }
    }
}
