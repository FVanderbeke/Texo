using System;
using Texo.Domain.Api.Entity;

namespace Texo.Domain.Default.SemVer
{
    public record SemVerVersion(ushort Major, ushort Minor = 0, ushort Patch = 0) : IVersion
    {
        public string Label => $"{Major}.{Minor}.{Patch}";

        public int CompareTo(IVersion other)
        {
            if (other is not SemVerVersion(var major, var minor, var patch))
            {
                return 1;
            }

            var majDiff = Major - major;

            if (majDiff != 0)
            {
                return majDiff;
            }


            var minDiff = Minor - minor;

            if (minDiff != 0)
            {
                return minDiff;
            }

            return Patch - patch;
        }

        public bool Equals(IVersion other)
        {
            return other is SemVerVersion(var major, var minor, var patch) && 
                   Tuple.Create(Major, Minor, Patch).Equals(Tuple.Create(major, minor, patch));
        }
    }
}
