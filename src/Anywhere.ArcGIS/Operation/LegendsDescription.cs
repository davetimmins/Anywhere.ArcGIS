using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Anywhere.ArcGIS.Common;

namespace Anywhere.ArcGIS.Operation
{
	[DataContract]
	public class LegendsDescription : ArcGISServerOperation
	{
		public LegendsDescription(string relativeUrl,
			Action beforeRequest = null,
			Action afterRequest = null)
			: this(relativeUrl.AsEndpoint(), beforeRequest, afterRequest)
		{
		}

		/// <summary>
		///     The legend resource represents a map service's legend. It returns the legend
		///     information for all layers in the service. Each layer's legend information
		///     includes the symbol images and labels for each symbol. Each symbol is an
		///     image of size 20x20 pixels at 96 DPI. Additional information for each layer,
		///     such as the layer ID, name, and min and max scales, is also included.
		/// </summary>
		/// <param name="endpoint">Resource to apply the export against</param>
		public LegendsDescription(IEndpoint endpoint,
			Action beforeRequest = null,
			Action afterRequest = null)
			: base(endpoint.RelativeUrl.Trim('/') + "/" + Operations.Legend, beforeRequest, afterRequest)
		{
		}

		/// <summary>
		///     The device resolution of the exported image in dots per inch (DPI). If dpi is not
		///     specified, an image with a default DPI of 96 will be returned for each legend patch.
		/// </summary>
		[DataMember(Name = "dpi")]
		public int? Dpi { get; set; }

		[IgnoreDataMember]
		public List<string> DynamicLayers { get; set; }

		/// <summary>
		///     Dynamic layers definition. This parameter is required only for generating a legend for dynamic layers.
		/// </summary>
		/// <remarks>
		///     supportsDynamicLayers on the map service resource should be true to use the dynamicLayers property when requesting
		///     a legend.
		/// </remarks>
		[DataMember(Name = "dynamicLayers")]
		public string DynamicLayersValue =>
			DynamicLayers == null || !DynamicLayers.Any() ? null : string.Join(",", DynamicLayers);

		/// <summary>
		///     The size (width * height) of the exported legend symbols.
		///     If the size is not specified, an image with a default size of 15x15 will be exported.
		/// </summary>
		[IgnoreDataMember]
		public List<int> Size { get; set; }

		/// <summary>
		///     The size (width * height) of the exported image in device-independent units, for example,
		///     points (1 inch = 72 points). If size is not specified, an image with a default size of 15x15 points will be
		///     exported.
		/// </summary>
		/// <remarks>
		///     size has no effect on point/marker legend patches, since size point/marker legend
		///     patches depend on the actual marker size.
		/// </remarks>
		[DataMember(Name = "size")]
		public string SizeValue
		{
			get
			{
				if (Size?.Count > 0 && Size?.Count != 2)
				{
					throw new ArgumentOutOfRangeException(nameof(Size),
						"If you want to use the SizeRange parameter then you need to supply exactly 2 values: the lower and upper range");
				}

				return Size == null || !Size.Any() ? null : string.Join(",", Size);
			}
		}
	}

	[DataContract]
	public class LegendsDescriptionResponse : PortalResponse
	{
		[DataMember(Name = "layers")]
		public Layer[] Layers { get; set; }
	}

	[DataContract]
	public class Layer
	{
		[DataMember(Name = "layerId")]
		public int LayerId { get; set; }

		[DataMember(Name = "layerName")]
		public string LayerName { get; set; }

		[DataMember(Name = "layerType")]
		public string LayerType { get; set; }

		[DataMember(Name = "legend")]
		public Legend[] Legend { get; set; }

		[DataMember(Name = "maxScale")]
		public double MaxScale { get; set; }

		[DataMember(Name = "minScale")]
		public double MinScale { get; set; }
	}

	[DataContract]
	public class Legend
	{
		[DataMember(Name = "contentType")]
		public string ContentType { get; set; }

		[DataMember(Name = "height")]
		public int Height { get; set; }

		[IgnoreDataMember]
		public byte[] ImageData =>
			string.IsNullOrWhiteSpace(ImageDataValue) ? null : Convert.FromBase64String(ImageDataValue);

		[DataMember(Name = "imageData")]
		public string ImageDataValue { get; set; }
        
		[DataMember(Name = "label")]
		public string Label { get; set; }

		[DataMember(Name = "url")]
		public string Url { get; set; }

		[DataMember(Name = "values")]
		public string[] Values { get; set; }

		[DataMember(Name = "width")]
		public int Width { get; set; }
	}
}