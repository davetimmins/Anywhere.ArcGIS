namespace Anywhere.ArcGIS.Common
{
    using System;

    /// <summary>
    /// Represents a REST endpoint
    /// </summary>
    public interface IEndpoint
    {
        /// <summary>
        /// Relative url of the resource
        /// </summary>
        string RelativeUrl { get; }

        /// <summary>
        /// Check the url is complete (ignore the scheme)
        /// </summary>
        /// <param name="rootUrl"></param>
        /// <returns></returns>
        string BuildAbsoluteUrl(string rootUrl);
    }

    /// <summary>
    /// Represents an ArcGIS Server REST endpoint
    /// </summary>
    public class ArcGISServerEndpoint : IEndpoint
    {
        /// <summary>
        /// Creates a new ArcGIS Server REST endpoint representation
        /// </summary>
        /// <param name="relativePath">Path of the endpoint relative to the root url of the ArcGIS Server</param>
        public ArcGISServerEndpoint(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                throw new ArgumentNullException(nameof(relativePath), "relativePath is null.");
            }

            if (!Uri.TryCreate(relativePath, UriKind.RelativeOrAbsolute, out Uri uri))
            {
                throw new InvalidOperationException("Not a valid relative url " + relativePath);
            }

            if (uri.IsAbsoluteUri)
            {
                RelativeUrl = uri.AbsolutePath.Trim('/') + "/";
            }
            else
            {
                RelativeUrl = uri.OriginalString.Trim('/') + "/";
            }

            if (RelativeUrl.IndexOf("rest/services/", StringComparison.OrdinalIgnoreCase) > -1)
            {
                RelativeUrl = RelativeUrl.Substring(RelativeUrl.LastIndexOf("rest/services/", StringComparison.OrdinalIgnoreCase));
            }

            RelativeUrl = RelativeUrl.Replace("rest/services/", "");
            RelativeUrl = "rest/services/" + RelativeUrl.Trim('/');
        }

        public string RelativeUrl { get; private set; }

        public string BuildAbsoluteUrl(string rootUrl)
        {
            if (string.IsNullOrWhiteSpace(rootUrl))
            {
                throw new ArgumentNullException(nameof(rootUrl), "rootUrl is null.");
            }

            return !RelativeUrl.Contains(rootUrl.Substring(6)) && !RelativeUrl.Contains(rootUrl.Substring(6))
                       ? rootUrl.Trim('/') + "/" + RelativeUrl
                       : RelativeUrl;
        }
    }

    /// <summary>
    /// Represents an ArcGIS Server Administration REST endpoint
    /// </summary>
    public class ArcGISServerAdminEndpoint : IEndpoint
    {
        /// <summary>
        /// Creates a new ArcGIS Server REST Administration endpoint representation
        /// </summary>
        /// <param name="relativePath">Path of the endpoint relative to the root url of the ArcGIS Server</param>
        public ArcGISServerAdminEndpoint(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                throw new ArgumentNullException(nameof(relativePath), "relativePath is null.");
            }

            if (!Uri.TryCreate(relativePath, UriKind.RelativeOrAbsolute, out Uri uri))
            {
                throw new InvalidOperationException("Not a valid relative url " + relativePath);
            }

            if (uri.IsAbsoluteUri)
            {
                RelativeUrl = uri.AbsolutePath.Trim('/') + "/";
            }
            else
            {
                RelativeUrl = uri.OriginalString.Trim('/') + "/";
            }

            if (RelativeUrl.IndexOf("admin/", StringComparison.OrdinalIgnoreCase) > -1)
            {
                RelativeUrl = RelativeUrl.Substring(RelativeUrl.LastIndexOf("admin/", StringComparison.OrdinalIgnoreCase));
            }

            RelativeUrl = RelativeUrl.Replace("admin/", "");
            RelativeUrl = "admin/" + RelativeUrl.Trim('/');
        }

        public string RelativeUrl { get; private set; }

        public string BuildAbsoluteUrl(string rootUrl)
        {
            if (string.IsNullOrWhiteSpace(rootUrl))
            {
                throw new ArgumentNullException(nameof(rootUrl), "rootUrl is null.");
            }

            return !RelativeUrl.Contains(rootUrl.Substring(6)) && !RelativeUrl.Contains(rootUrl.Substring(6))
                       ? rootUrl.Trim('/') + "/" + RelativeUrl
                       : RelativeUrl;
        }
    }

    public class ArcGISOnlineEndpoint : IEndpoint
    {
        /// <summary>
        /// Creates a new ArcGIS Online or Portal REST endpoint representation
        /// </summary>
        /// <param name="relativePath">Path of the endpoint relative to the root url of ArcGIS Online / Portal</param>
        public ArcGISOnlineEndpoint(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                throw new ArgumentNullException(nameof(relativePath), "relativePath is null.");
            }

            if (!Uri.TryCreate(relativePath, UriKind.RelativeOrAbsolute, out Uri uri))
            {
                throw new InvalidOperationException("Not a valid relative url " + relativePath);
            }

            if (uri.IsAbsoluteUri)
            {
                RelativeUrl = uri.AbsolutePath.Trim('/') + "/";
            }
            else
            {
                RelativeUrl = uri.OriginalString.Trim('/') + "/";
            }

            if (RelativeUrl.IndexOf("sharing/rest/", StringComparison.OrdinalIgnoreCase) > -1)
            {
                RelativeUrl = RelativeUrl.Substring(RelativeUrl.LastIndexOf("sharing/rest/", StringComparison.OrdinalIgnoreCase));
            }

            RelativeUrl = RelativeUrl.Replace("sharing/rest/", "");
            RelativeUrl = "sharing/rest/" + RelativeUrl.Trim('/');
        }

        public string RelativeUrl { get; private set; }

        public string BuildAbsoluteUrl(string rootUrl)
        {
            if (string.IsNullOrWhiteSpace(rootUrl))
            {
                throw new ArgumentNullException(nameof(rootUrl), "rootUrl is null.");
            }

            return !RelativeUrl.Contains(rootUrl.Substring(6)) && !RelativeUrl.Contains(rootUrl.Substring(6))
                       ? rootUrl.Trim('/') + "/" + RelativeUrl
                       : RelativeUrl;
        }
    }

    public class AbsoluteEndpoint : IEndpoint
    {
        /// <summary>
        /// Create an IEndpoint for the path
        /// </summary>
        /// <param name="path"></param>
        public AbsoluteEndpoint(string path)
        {
            RelativeUrl = path;
        }

        public string RelativeUrl { get; private set; }

        public string BuildAbsoluteUrl(string rootUrl)
        {
            return RelativeUrl;
        }
    }

    public class RootServerEndpoint : IEndpoint
    {
        /// <summary>
        /// Create an IEndpoint for the path
        /// </summary>
        /// <param name="path"></param>
        public RootServerEndpoint(string path)
        {
            RelativeUrl = path;
        }

        public string RelativeUrl { get; private set; }

        public string BuildAbsoluteUrl(string rootUrl)
        {
            if (string.IsNullOrWhiteSpace(rootUrl))
            {
                throw new ArgumentNullException(nameof(rootUrl), "rootUrl is null.");
            }

            return !RelativeUrl.Contains(rootUrl.Substring(6)) && !RelativeUrl.Contains(rootUrl.Substring(6))
                       ? rootUrl.Trim('/') + "/" + RelativeUrl
                       : RelativeUrl;
        }
    }
}
