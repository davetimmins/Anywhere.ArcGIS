namespace Anywhere.ArcGIS.Common
{
    using System;

    public interface IHttpOperation
    {
        IEndpoint Endpoint { get; }

        Action BeforeRequest { get; }

        Action<string> AfterRequest { get; }
    }
}
