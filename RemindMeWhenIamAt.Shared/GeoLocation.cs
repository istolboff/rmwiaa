using System;

namespace RemindMeWhenIamAt.SharedCode
{
    public readonly struct GeoLocation : IEquatable<GeoLocation>
    {
        public GeoLocation(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

#pragma warning disable CA1051 // It's a readonly struct
        public readonly double Latitude;

        public readonly double Longitude;
#pragma warning restore CA1051

        public override bool Equals(object obj) => obj is GeoLocation right && Equals(right);

        public bool Equals(GeoLocation other) => Latitude == other.Latitude && Longitude == other.Longitude;

        public override int GetHashCode() => Latitude.GetHashCode() ^ Longitude.GetHashCode();

        public static bool operator ==(GeoLocation left, GeoLocation right) => left.Equals(right);

        public static bool operator !=(GeoLocation left, GeoLocation right) => !left.Equals(right);
    }
}