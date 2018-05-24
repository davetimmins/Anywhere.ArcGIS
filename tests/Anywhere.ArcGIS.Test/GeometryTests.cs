using Anywhere.ArcGIS.Common;
using System;
using Xunit;

namespace ArcGIS.Test
{
    public class GeometryTests
    {
        [Fact]
        public void FeaturesAreTheSame()
        {
            var feature1 = new Feature<Point>
            {
                Geometry = new Point { X = 50.342, Y = -30.331, SpatialReference = SpatialReference.WGS84 }
            };
            feature1.Attributes.Add("random", "rtcxbvbx");
            feature1.Attributes.Add("something", 45445);

            var feature2 = new Feature<Point>
            {
                Geometry = new Point { X = 50.342, Y = -30.331, SpatialReference = SpatialReference.WGS84 }
            };
            feature2.Attributes.Add("random", "rtcxbvbx");
            feature2.Attributes.Add("something", 45445);

            Assert.Equal(feature1, feature2);
        }

        [Fact]
        public void FeatureCanUseUniqueID()
        {
            var feature1 = new Feature<Point>
            {
                Geometry = new Point { X = 50.342, Y = -30.331, SpatialReference = SpatialReference.WGS84 }
            };
            feature1.Attributes.Add("random", "rtcxbvbx");
            feature1.Attributes.Add("something", 45445);

            var feature2 = new Feature<Point>(feature1, "something");

            Assert.True(feature1.UniqueID == 0);
            Assert.True(feature2.UniqueID > 0);
            Assert.Equal(45445, feature2.UniqueID);
        }

        [Fact]
        public void FeaturesAreNotTheSame()
        {
            var feature1 = new Feature<Point>
            {
                Geometry = new Point { X = 50.342, Y = -2, SpatialReference = SpatialReference.WGS84 }
            };

            var feature2 = new Feature<Point>
            {
                Geometry = new Point { X = 50.342, Y = -30.331, SpatialReference = SpatialReference.WGS84 }
            };

            var feature3 = new Feature<Point>
            {
                Geometry = new Point { X = 50.342, Y = -30.331, SpatialReference = SpatialReference.WGS84 }
            };
            feature3.Attributes.Add("random", "rtcxbvbx");
            feature3.Attributes.Add("something", 45445);

            var feature4 = new Feature<Point>
            {
                Geometry = new Point { X = 50.342, Y = -30.331, SpatialReference = SpatialReference.WGS84 }
            };
            feature4.Attributes.Add("random", "rtcxbvbx");
            feature4.Attributes.Add("something", 4);

            var feature5 = new Feature<Point>
            {
                Geometry = new Point { X = 50.342, Y = -30.331, SpatialReference = SpatialReference.WGS84 }
            };
            feature5.Attributes.Add("random", "rtcxbvbx");
            feature5.Attributes.Add("something", 4);
            feature5.Attributes.Add("somethingelse", 4);

            var feature6 = new Feature<Point>
            {
                Geometry = new Point { X = 50.342, Y = -30.331, SpatialReference = SpatialReference.WGS84 }
            };
            feature6.Attributes.Add("random", "rtcxbvbx");
            feature6.Attributes.Add("something", 4);
            feature6.Attributes.Add("somethingelseagain", 4);

            Assert.NotEqual(feature1, feature2);
            Assert.NotEqual(feature3, feature4);
            Assert.NotEqual(feature1, feature3);
            Assert.NotEqual(feature1, feature4);
            Assert.NotEqual(feature2, feature3);
            Assert.NotEqual(feature2, feature4);
            Assert.NotEqual(feature1, feature5);
            Assert.NotEqual(feature2, feature5);
            Assert.NotEqual(feature3, feature5);
            Assert.NotEqual(feature4, feature5);
            Assert.NotEqual(feature1, feature6);
            Assert.NotEqual(feature2, feature6);
            Assert.NotEqual(feature3, feature6);
            Assert.NotEqual(feature4, feature6);
            Assert.NotEqual(feature5, feature6);
        }

        [Fact]
        public void SpatialReferencesAreTheSame()
        {
            var sr = new SpatialReference { Wkid = SpatialReference.WGS84.Wkid };
            Assert.Equal(sr, SpatialReference.WGS84);

            var sr2 = new SpatialReference { Wkid = SpatialReference.WGS84.LatestWkid };
            Assert.Equal(sr2, SpatialReference.WGS84);

            var sr3 = SpatialReference.WGS84;
            Assert.Equal(sr3, SpatialReference.WGS84);

            Assert.True(sr == sr2);
            Assert.True(sr == sr3);
            Assert.True(sr3 == sr2);
            Assert.False(SpatialReference.WGS84 == null);
            Assert.False(null == SpatialReference.WGS84);
            Assert.False(new SpatialReference { Wkid = 2193 } == SpatialReference.WGS84);
        }

        [Fact]
        public void SpatialReferencesAreNotTheSame()
        {
            var sr = new SpatialReference { Wkid = SpatialReference.WebMercator.Wkid };
            Assert.NotEqual(sr, SpatialReference.WGS84);

            var sr2 = SpatialReference.WebMercator;
            Assert.NotEqual(sr2, SpatialReference.WGS84);

            Assert.True(SpatialReference.WGS84 != SpatialReference.WebMercator);
            Assert.False(SpatialReference.WGS84 == SpatialReference.WebMercator);

            Assert.True(sr != SpatialReference.WGS84);
            Assert.True(SpatialReference.WebMercator != SpatialReference.WGS84);

            Assert.True(SpatialReference.WGS84 != null);
            Assert.True(null != SpatialReference.WGS84);
            Assert.True(new SpatialReference { Wkid = 2193 } != SpatialReference.WGS84);
            Assert.False(SpatialReference.WGS84 != new SpatialReference { Wkid = SpatialReference.WGS84.Wkid });
        }

        [Theory]
        [InlineData("ObjectId")]
        [InlineData("ObjectID")]
        [InlineData("Objectid")]
        [InlineData("objectid")]
        [InlineData("OBJECTID")]
        [InlineData("OBJECTid")]
        public void CanGetValidObjectID(string key)
        {
            long oid = 34223;
            var feature = new Feature<Point>();
            feature.Attributes.Add(key, oid);

            Assert.True(feature.ObjectID > 0);
            Assert.Equal(oid, feature.ObjectID);
        }

        [Theory]
        [InlineData("Object-Id")]
        [InlineData("Object ID")]
        [InlineData("Object_id")]
        [InlineData("object+id")]
        [InlineData("objevxvcvxcctid")]
        public void CannotGetInvalidObjectID(string key)
        {
            long oid = 34223;
            var feature = new Feature<Point>();
            feature.Attributes.Add(key, oid);

            Assert.Equal(0, feature.ObjectID);
        }

        [Fact]
        public void ObjectIDIsZeroOnInitialise()
        {
            var feature = new Feature<Point>();

            Assert.Equal(0, feature.ObjectID);
        }

        [Theory]
        [InlineData("GlobalId")]
        [InlineData("GlobalID")]
        [InlineData("Globalid")]
        [InlineData("globalid")]
        [InlineData("globalID")]
        [InlineData("GLOBALID")]
        [InlineData("GLOBALid")]
        public void CanGetValidGlobalID(string key)
        {
            var guid = Guid.NewGuid();
            var feature = new Feature<Point>();
            feature.Attributes.Add(key, guid);

            Assert.NotEqual(Guid.Empty, feature.GlobalID);
            Assert.Equal(guid, feature.GlobalID);
        }

        [Theory]
        [InlineData("Global-Id")]
        [InlineData("Global ID")]
        [InlineData("Global_id")]
        [InlineData("global+id")]
        [InlineData("globavcbbcblid")]
        public void CannotGetInvalidGlobalD(string key)
        {
            var guid = Guid.NewGuid();
            var feature = new Feature<Point>();
            feature.Attributes.Add(key, guid);

            Assert.Equal(Guid.Empty, feature.GlobalID);
        }

        [Theory]    
        [InlineData("dfds", 3443)]
        [InlineData("blahblah", 34.3)]
        [InlineData("Global_id", "ewe")]
        [InlineData("ESRI_OID", 's')]
        [InlineData("something something darkside", 453f)]
        public void CanGetAttributeValue(string key, object value)
        {
            var feature = new Feature<Point>();
            feature.Attributes.Add(key, value);

            var expected = feature.AttributeValue(key.ToLowerInvariant());

            Assert.Equal(expected, value);
        }

        [Fact]
        public void GlobalIDIsEmptyOnInitialise()
        {
            var feature = new Feature<Point>();

            Assert.Equal(Guid.Empty, feature.GlobalID);
        }
    }
}
