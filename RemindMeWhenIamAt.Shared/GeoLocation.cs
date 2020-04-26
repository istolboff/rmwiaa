using System;

namespace RemindMeWhenIamAt.SharedCode
{
    public readonly struct GeoLocation : IEquatable<GeoLocation>
    {
#pragma warning disable CA1051 // It's a readonly struct
        public readonly decimal X;

        public readonly decimal Y;
#pragma warning restore CA1051

        public override bool Equals(object obj) => obj is GeoLocation right && Equals(right);

        public bool Equals(GeoLocation other) => X == other.X && Y == other.Y;

        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();

        public static bool operator ==(GeoLocation left, GeoLocation right) => left.Equals(right);

        public static bool operator !=(GeoLocation left, GeoLocation right) => !left.Equals(right);
    }
}