Why the change? Well there are 2 main reasons, the first is that this is now a netstandard library rather than a portable class library (PCL), so the old naming didn't really apply. Secondly, NuGet now has package Id reservations and so I can't use ArcGIS as the suffix anymore.

To migrate from [ArcGIS.PCL](https://github.com/davetimmins/ArcGIS.PCL/) to [Anywhere.ArcGIS](https://github.com/davetimmins/Anywhere.ArcGIS/) the following __breaking__ :boom: changes need to be done / reviewed:

- All namespaces have changed from `ArcGIS.ServiceModel.*` to `Anywhere.ArcGIS.*`.

- You no longer need to call the static `ISerializer.Init()` method as JSON.NET is now baked in as the default.

- `SecurePortalGateway` has been renamed to just `PortalGateway`.

- Internally all requests now use an `ArcGISServerOperation` type, this allows before and after actions to be invoked for the HTTP request.

- Renamed `GdbVersion` to `GeodatabaseVersion`.

- Removed `Disabled` from `ICryptoProvider` and replaced it with `Enabled` which is `false` by default, so to opt in to trying to encrypt token requests (admin endpoint only) you now need to call `CryptoProviderFactory.Enabled = true;` once in your app code.