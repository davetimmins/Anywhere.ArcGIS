using Anywhere.ArcGIS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Anywhere.ArcGIS.Operation
{
    [DataContract]
    public class ExportMap : ArcGISServerOperation
    {
        public ExportMap(string relativeUrl, Action beforeRequest = null, Action<string> afterRequest = null)
            : this(relativeUrl.AsEndpoint(), beforeRequest, afterRequest)
        { }

        /// <summary>
        /// Requests an export of the map resources. Returns the image link in the response
        /// </summary>
        /// <param name="endpoint">Resource to apply the export against</param>
        public ExportMap(ArcGISServerEndpoint endpoint, Action beforeRequest = null, Action<string> afterRequest = null)
            : base(endpoint.RelativeUrl.Trim('/') + "/" + Operations.ExportMap, beforeRequest, afterRequest)
        {
            Size = new List<int> { 400, 400 };
            Dpi = 96;
            ImageFormat = "png";
        }

        /// <summary>
        /// (Required) The extent (bounding box) of the exported image. 
        /// Unless the bboxSR parameter has been specified, the bbox is assumed to be in the spatial reference of the map.
        /// </summary>
        [IgnoreDataMember]
        public Extent ExportExtent { get; set; }

        /// <summary>
        /// (Required) The extent (bounding box) of the exported image. 
        /// Unless the bboxSR parameter has been specified, the bbox is assumed to be in the spatial reference of the map.
        /// </summary>
        [DataMember(Name = "bbox")]
        public string ExportExtentBoundingBox
        {
            get
            {
                return ExportExtent == null ?
                    string.Empty :
                    string.Format("{0},{1},{2},{3}", ExportExtent.XMin, ExportExtent.YMin, ExportExtent.XMax, ExportExtent.YMax);
            }
        }

        /// <summary>
        /// The spatial reference of the ExportExtentBoundingBox.
        /// </summary>
        [DataMember(Name = "bboxSR")]
        public SpatialReference ExportExtentBoundingBoxSpatialReference
        {
            get { return ExportExtent == null ? null : ExportExtent.SpatialReference ?? null; }
        }

        /// <summary>
        /// The size (width * height) of the exported image in pixels. 
        /// If the size is not specified, an image with a default size of 400 * 400 will be exported.
        /// </summary>
        [IgnoreDataMember]
        public List<int> Size { get; set; }

        /// <summary>
        /// The size width *height) of the exported image in pixels. 
        /// If the size is not specified, an image with a default size of 400 * 400 will be exported.
        /// </summary>
        [DataMember(Name = "size")]
        public string SizeValue
        {
            get
            {
                if (Size?.Count > 0 && Size?.Count != 2)
                {
                    throw new ArgumentOutOfRangeException(nameof(Size), "If you want to use the SizeRange parameter then you need to supply exactly 2 values: the lower and upper range");
                }

                return Size == null || !Size.Any() ? null : string.Join(",", Size);
            }
        }

        /// <summary>
        /// The device resolution of the exported image (dots per inch). 
        /// If the dpi is not specified, an image with a default DPI of 96 will be exported.
        /// </summary>
        [DataMember(Name = "dpi")]
        public int Dpi { get; set; }

        /// <summary>
        /// The spatial reference of the exported image.
        /// </summary>
        [DataMember(Name = "imageSR")]
        public SpatialReference ImageSpatialReference { get; set; }

        /// <summary>
        /// The format of the exported image. The default format is png.
        /// Values: png | png8 | png24 | jpg | pdf | bmp | gif | svg | svgz | emf | ps | png32
        /// </summary>
        [DataMember(Name = "format")]
        public string ImageFormat { get; set; }

        /// <summary>
        /// Allows you to filter the features of individual layers in the exported map by specifying definition expressions for those layers. 
        /// Definition expression for a layer that is published with the service will be always honored.
        /// </summary>
        [DataMember(Name = "layerDefs")]
        public Dictionary<int, string> LayerDefinitions { get; set; }

        /// <summary>
        /// If true, the image will be exported with the background color of the map set as its transparent color. 
        /// The default is false. 
        /// Only the .png and .gif formats support transparency. 
        /// Internet Explorer 6 does not display transparency correctly for png24 image formats.
        /// </summary>
        [DataMember(Name = "transparent")]
        public bool TransparentBackground { get; set; }

        [IgnoreDataMember]
        public DateTime? From { get; set; }

        [IgnoreDataMember]
        public DateTime? To { get; set; }

        /// <summary>
        /// The time instant or the time extent to query.
        /// </summary>
        /// <remarks>If no To value is specified we will use the From value again, equivalent of using a time instant.</remarks>
        [DataMember(Name = "time")]
        public string Time
        {
            get
            {
                return (From == null) ? null : string.Format("{0},{1}",
                  From.Value.ToUnixTime(),
                  (To ?? From.Value).ToUnixTime());
            }
        }

        /// <summary>
        /// GeoDatabase version to export from.
        /// </summary>
        [DataMember(Name = "gdbVersion")]
        public string GeodatabaseVersion { get; set; }

        /// <summary>
        /// Use this parameter to export a map image at a specific scale, with the map centered around the center of the specified bounding box (bbox).
        /// </summary>
        [DataMember(Name = "mapScale")]
        public long MapScale { get; set; }

        /// <summary>
        /// Use this parameter to export a map image rotated at a specific angle, with the map centered around the center of the specified bounding box (bbox). 
        /// It could be positive or negative number.
        /// </summary>
        [DataMember(Name = "rotation")]
        public int Rotation { get; set; }
    }
    
    [DataContract]
    public class ExportMapResponse : PortalResponse
    {
        [DataMember(Name = "href")]
        public string ImageUrl { get; set; }

        [DataMember(Name = "width")]
        public int Width { get; set; }

        [DataMember(Name = "height")]
        public int Height { get; set; }

        [DataMember(Name = "extent")]
        public Extent Extent { get; set; }

        [DataMember(Name = "scale")]
        public double Scale { get; set; }

        public string ImageFormat { get { return string.IsNullOrEmpty(ImageUrl) ? string.Empty : ImageUrl.Substring(ImageUrl.LastIndexOf(".") + 1); } }

        public SpatialReference ImageSpatialReference { get { return Extent?.SpatialReference; } }
    }
}
