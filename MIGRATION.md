Top migrate from ArcGIS.PCL to Anywhere.ArcGIS the following breaking changes need to be done / reviewed:

- All namespaces have changed from `ArcGIS.ServiceModel.*` to `Anywhere.ArcGIS.*`.

- `IGeometry` is now `IGeometry<T>`.

- You no longer need to call the static `ISerializer` `Init()` method as JSON.NET is now baked in.

- `SecurePortalGateway` has been renamed to just `PortalGateway`.

- A `Query` operation with an input geometry must now specify that geometry type as a constraint.

- No longer supports GeoJSON types or conversion to/from ArcGIS Features.

- Internally all requests now use an `ArcGISServerOperation` type, this allows before and after actions to be invoked for the HTTP request.