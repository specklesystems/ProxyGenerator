namespace ProxyInterfaceSourceGeneratorTests.Source.Disposable;

public interface IRevitLocationPoint : IRevitLocation { }

public interface IRevitLocationCurve : IRevitLocation
{
    IRevitCurve Curve { get; }
}

public class LocationPoint : Location
{
    public XYZ Point => throw new NotImplementedException();
}

public class Location : APIObject { }

public class APIObject { }

public class XYZ { }

public interface IRevitLocation { }

public interface IRevitCurve
{
    double Length { get; }
}
