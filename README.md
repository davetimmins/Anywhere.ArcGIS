<img align="right" height="120" src="https://raw.githubusercontent.com/davetimmins/Anywhere.ArcGIS/dev/logo.png">

### Anywhere.ArcGIS

[![Build status](https://ci.appveyor.com/api/projects/status/h45tr0g9eff9iv65?svg=true)](https://ci.appveyor.com/project/davetimmins/anywhere-arcgis) [![Build status](https://travis-ci.org/davetimmins/Anywhere.ArcGIS.svg)](https://travis-ci.org/davetimmins/Anywhere.ArcGIS)

[![NuGet Status](https://img.shields.io/nuget/vpre/Anywhere.ArcGIS.svg)](https://www.nuget.org/packages/Anywhere.ArcGIS/) [![GitHub Status](https://img.shields.io/github/release/davetimmins/Anywhere.ArcGIS/all.svg)](https://github.com/davetimmins/Anywhere.ArcGIS/releases)

MyGet feed [![MyGet Status](https://img.shields.io/myget/0b0e5c5f92fe4ccbba32eec8c0bb4c22/vpre/Anywhere.ArcGIS.svg)](https://www.myget.org/feed/0b0e5c5f92fe4ccbba32eec8c0bb4c22/package/nuget/Anywhere.ArcGIS)

Use ArcGIS Server REST resources without an official SDK. 
This is a netstandard 2.0 library, it was ported from [ArcGIS.PCL](https://github.com/davetimmins/ArcGIS.PCL/). It is not endorsed, affiliated or supported by Esri.

A typical use case would be the need to call some ArcGIS REST resource from server .NET code or maybe a console app. The features that this returns can be used directly as Esri JSON in JavaScript apps using the Esri JS API.

Works with secure and non-secure ArcGIS Server on premise / in the cloud, Portal for ArcGIS and ArcGIS Online. Also supports converting GeoJSON :left_right_arrow: ArcGIS Features.

### Quickstart

If you are calling a REST operation you will need to create a gateway to manage the request. There are a few different ones but the most basic is called `PortalGateway` and this can be used for connecting directly to services with ArcGIS Server.

Create an instance of that by specifying the root url of your server. The format of the root url is _scheme_://_host_:_port_/_instance_ so a typical default ArcGIS Server for your local machine would be _http://localhost:6080/arcgis_, note that you do not need to include `rest/services` in either the root url or your relative urls as it gets added automatically. One thing to look out for is that the url is case sensitive so make sure you enter it correctly.

```c#
var gateway = new PortalGateway("https://sampleserver3.arcgisonline.com/ArcGIS/");

// If you want to access secure resources then pass in a username / password
// this assumes the token service is in the default location for the ArcGIS Server
var secureGateway = new PortalGateway("https://sampleserver3.arcgisonline.com/ArcGIS/", "username", "password");

// Or use the static Create method which will discover the token service Url from the server Info endpoint
var autoTokenProviderLocationGateway = await PortalGateway.Create("https://sampleserver3.arcgisonline.com/ArcGIS/", "username", "password");
```

Now you have access to the various operations supported by it. For example to call a query against a service

```c#
var query = new Query("Earthquakes/EarthquakesFromLastSevenDays/MapServer/0".AsEndpoint())
{ 
    Where = "magnitude > 4.0" 
};
var result = await gateway.Query<Point>(query);
```

### Capabilities

Supports the following as typed operations:

 - `CheckGenerateToken` create a token automatically via an `ITokenProvider`
 - `Query` query a layer by attribute and / or spatial filters, also possible to do `BatchQuery`
 - `QueryForCount` only return the number of results for the query operation
 - `QueryForIds` only return the ObjectIds for the results of the query operation
 - `QueryForExtent` return the bounding extent for the result of the query operation
 - `QueryAttachments` return attachments grouped by the source feature object Ids and global id
 - `QueryDomains` returns full domain information for the domains referenced by the layers in the service
 - `Find` search across _n_ layers and fields in a service
 - `ApplyEdits` post adds, updates and deletes to a feature service layer
 - `DeleteFeatures` delete features in a feature layer or table
 - `Geocode` single line of input to perform a geocode using a custom locator or the Esri world locator
 - `CustomGeocode` single line of input to perform a geocode using a custom locator
 - `Suggest` lightweight geocode operation that only returns text results, commonly used for predictive searching
 - `ReverseGeocode` find location candidates for a input point location
 - `Simplify` alter geometries to be topologically consistent
 - `Project` convert geometries to a different spatial reference
 - `Buffer` buffers geometries by the distance requested
 - `DescribeSite` returns a url for every service discovered
 - `CreateReplica` create a replica for a layer
 - `UnregisterReplica` unregister a replica based on the Id
 - `DeleteAttachments` delete attachments that are associated with a feature
 - `Ping` verify that the server can be accessed
 - `Info` return the server information such as version and token authentication settings
 - `DescribeLegends` return legend information of layers
 - `DescribeServices` return services information (name, sublayers etc.)
 - `DescribeService` return service information (name, sublayers etc.)
 - `DescribeLayer` return layer information
 - `HealthCheck` verify that the server is accepting requests
 - `GetFeature` return a feature from a map/feature service
 - `ExportMap` get an image (or url to the image) of a service

REST admin operations:

 - `PublicKey` - admin operation to get public key used for encryption of token requests
 - `ServiceStatus` - admin operation to get the configured and actual status of a service
 - `ServiceReport` - admin operation to get the service report
 - `StartService` - admin operation to start a service
 - `StopService` - admin operation to stop a service
 - `ServiceStatistics` - admin operation to get the statistics of a service

There are also methods to add / update and download attachments for a feature and you can extend this library by writing your own operations.

Refer to the integration test project for more examples.

### Can I help to improve it and/or fix bugs?

Absolutely! Please feel free to raise issues, fork the source code, send pull requests, etc.

No pull request is too small. Even whitespace fixes are appreciated. Before you contribute anything make sure you read [CONTRIBUTING](https://github.com/davetimmins/Anywhere.ArcGIS/blob/master/CONTRIBUTING.md).

### Installation

#### NuGet Package Manager

    Install-Package Anywhere.ArcGIS

#### .NET CLI

    dotnet add package Anywhere.ArcGIS

#### Paket CLI

    paket add Anywhere.ArcGIS

You can also get the code from this site.

### What do the version numbers mean? ##

Anywhere.ArcGIS uses [Semantic Versioning](http://semver.org/).

### Icon

Icon made by [Freepik](http://www.freepik.com) from [www.flaticon.com](https://www.flaticon.com/free-icon/triangle-of-triangles_32915)
                
