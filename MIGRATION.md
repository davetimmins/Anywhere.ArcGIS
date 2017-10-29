Why the change? Well there are 2 main reasons, the first is that this is now a netstandard library rather than a portable class library (PCL), so the old naming didn't really apply. Secondly, NuGet now has package Id reservations and so I can't use ArcGIS as the suffix anymore.

To migrate from ArcGIS.PCL to Anywhere.ArcGIS the following breaking changes need to be done / reviewed:

- All namespaces have changed from `ArcGIS.ServiceModel.*` to `Anywhere.ArcGIS.*`.

- You no longer need to call the static `ISerializer` `Init()` method as JSON.NET is now baked in.

- `SecurePortalGateway` has been renamed to just `PortalGateway`.

- Internally all requests now use an `ArcGISServerOperation` type, this allows before and after actions to be invoked for the HTTP request.

- Renamed `GdbVersion` to `GeodatabaseVersion`.