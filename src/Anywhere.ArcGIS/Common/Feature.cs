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
        readonly string _idFieldName;

        public Feature()
        {
            Attributes = new Dictionary<string, object>();
        }

        /// <summary>
        /// Creates a feature that has the UniqueID field name set
        /// </summary>
        /// <param name="idFieldName">Name of the field to use to return the Id of the feature. Used when ObjectId is not on the layer e.g. query layer uses ESRI_OID</param>
        public Feature(string idFieldName)
            : this()
        {
            _idFieldName = idFieldName;
        }

        /// <summary>
        /// Create a new feature from an existing one using that has the UniqueID field name set
        /// </summary>
        /// <param name="featureToCopy">Feature that you want to copy.</param>
        /// <param name="idFieldName">Name of the field to use to return the Id of the feature. Used when ObjectId is not on the layer e.g. query layer uses ESRI_OID</param>
        public Feature(Feature<T> featureToCopy, string idFieldName)
        {
            if (featureToCopy == null)
            {
                throw new ArgumentNullException(nameof(featureToCopy));
            }

            _idFieldName = idFieldName;
            Attributes = new Dictionary<string, object>(featureToCopy.Attributes);
            Geometry = (T) featureToCopy.Geometry?.Clone();
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
                    if (copy.ContainsKey(ObjectIDName))
                    {
                        long.TryParse(copy[ObjectIDName].ToString(), out _oid);
                    }
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
                    if (copy.ContainsKey(GlobalIDName))
                    {
                        Guid.TryParse(copy[GlobalIDName].ToString(), out _globalID);
                    }
                }

                return _globalID;
            }
        }

        long _id = 0;
        /// <summary>
        /// Get the UniqueID for the feature when the layer doesn't contain an ObjectID. 
        /// Will return 0 if not found or if the id field name is not set
        /// </summary>
        public long UniqueID
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_idFieldName) && Attributes != null && Attributes.Any() && _id == 0)
                {
                    var copy = new Dictionary<string, object>(Attributes, StringComparer.OrdinalIgnoreCase);
                    if (copy.ContainsKey(_idFieldName))
                    {
                        long.TryParse(copy[_idFieldName].ToString(), out _id);
                    }
                }

                return _id;
            }
        }

        /// <summary>
        /// Get the value for an attribute
        /// </summary>
        /// <param name="attributeName">The attribute name that you want the value of for the feature</param>
        /// <returns>The value of the attribute or null if not found</returns>
        /// <remarks>Case insensitive</remarks>
        public object AttributeValue(string attributeName)
        {
            if (string.IsNullOrWhiteSpace(attributeName) || Attributes == null || !Attributes.Any())
            {
                return null;
            }

            object result = null;
            var copy = new Dictionary<string, object>(Attributes, StringComparer.OrdinalIgnoreCase);
            if (copy.ContainsKey(attributeName))
            {
                result = copy[attributeName];
            }

            return result;
        }

        public bool AttributesAreTheSame(Feature<T> other)
        {
            if (other == null || other.Attributes == null || Attributes == null || !Attributes.Any())
            {
                return false;
            }

            return (Attributes.Count == other.Attributes.Count) && !(Attributes.Except(other.Attributes)).Any();
        }

        public bool Equals(Feature<T> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return EqualityComparer<T>.Default.Equals(Geometry, other.Geometry) && AttributesAreTheSame(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T>.Default.GetHashCode(Geometry) * 397) 
                    ^ (Attributes != null ? Attributes.GetHashCode() : 0);
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var other = obj as Feature<T>;
            return other != null && Equals(other);
        }
    }
}
