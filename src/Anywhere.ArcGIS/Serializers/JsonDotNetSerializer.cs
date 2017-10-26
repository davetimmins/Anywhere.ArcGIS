namespace Anywhere.ArcGIS.Serializers
{
    using Anywhere.ArcGIS;
    using Anywhere.ArcGIS.Operation;
    using System.Collections.Generic;

    public class JsonDotNetSerializer : ISerializer
    {
        static ISerializer _serializer = null;

        public static ISerializer Create(Newtonsoft.Json.JsonSerializerSettings settings = null)
        {
            _serializer = new JsonDotNetSerializer(settings);
            return _serializer ?? new JsonDotNetSerializer(settings);
        }

        readonly Newtonsoft.Json.JsonSerializerSettings _settings;

        public JsonDotNetSerializer(Newtonsoft.Json.JsonSerializerSettings settings = null)
        {
            _settings = settings ?? new Newtonsoft.Json.JsonSerializerSettings
            {
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore,
                StringEscapeHandling = Newtonsoft.Json.StringEscapeHandling.EscapeHtml,
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None
            };
        }

        public Dictionary<string, string> AsDictionary<T>(T objectToConvert) where T : CommonParameters
        {
            var stringValue = Newtonsoft.Json.JsonConvert.SerializeObject(objectToConvert, _settings);

            var jobject = Newtonsoft.Json.Linq.JObject.Parse(stringValue);
            var dict = new Dictionary<string, string>();
            foreach (var item in jobject)
            {
                dict.Add(item.Key, item.Value.ToString());
            }
            return dict;
        }

        public T AsPortalResponse<T>(string dataToConvert) where T : IPortalResponse
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(dataToConvert, _settings);
        }
    }
}
