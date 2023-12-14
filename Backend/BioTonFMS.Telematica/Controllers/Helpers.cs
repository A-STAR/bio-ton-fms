using BioTonFMS.Domain.Monitoring;
using BioTonFMS.Telematica.Dtos.Monitoring;

namespace BioTonFMS.Telematica.Controllers;

public class TelematicaHelpers
{
    public const double DefaultDifLon = 0.08;
    public const double DefaultDifLat = 0.05;

    internal static ViewBounds? CalculateViewBounds(ICollection<TrackPointInfo> points)
    {
        if (points.Count == 0)
        {
            return null;
        }

        List<double> lons = points.Select(x => x.Longitude).ToList();
        List<double> lats = points.Select(x => x.Latitude).ToList();

        return CalculateViewBounds(lons, lats);
    }

    internal static ViewBounds? CalculateViewBounds(List<LocationAndTrack> locationsAndTracks)
    {
        if (locationsAndTracks.Count == 0)
        {
            return null;
        }
        List<double> lons = locationsAndTracks.SelectMany(x => x.Track).Select(x => x.Longitude).ToList();
        lons.AddRange(locationsAndTracks.Where(x => x.Longitude != null).Select(x => x.Longitude!.Value));

        List<double> lats = locationsAndTracks.SelectMany(x => x.Track).Select(x => x.Latitude).ToList();
        lats.AddRange(locationsAndTracks.Where(x => x.Latitude != null).Select(x => x.Latitude!.Value));

        return CalculateViewBounds(lons, lats);
    }

    private static ViewBounds? CalculateViewBounds(List<double> lons, List<double> lats)
    {
        var difLat = (lats.Max() - lats.Min()) / 20;
        var difLon = (lons.Max() - lons.Min()) / 20;

        if (difLat < DefaultDifLat)
        {
            difLat = DefaultDifLat;
        }
        if (difLon < DefaultDifLon)
        {
            difLon = DefaultDifLon;
        }

        var viewBounds = new ViewBounds
        {
            UpperLeftLatitude = lats.Max() + difLat,
            UpperLeftLongitude = lons.Min() - difLon,
            BottomRightLatitude = lats.Min() - difLat,
            BottomRightLongitude = lons.Max() + difLon
        };
        return viewBounds;
    }
}
