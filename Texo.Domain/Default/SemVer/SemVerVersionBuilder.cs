using Texo.Domain.Api.Builder;

namespace Texo.Domain.Default.SemVer
{
    public sealed class SemVerVersionBuilder : IVersionBuilder<SemVerVersion>
    {
        public static SemVerVersionBuilder Create() {
            return new SemVerVersionBuilder();
        }

        public static SemVerVersionBuilder From(ushort major, ushort minor = default, ushort patch = default)
        {
            return Create().WithMajor(major).WithMinor(minor).WithPatch(patch);
        }

        private ushort _major;
        private ushort _minor;
        private ushort _patch;

        private SemVerVersionBuilder()
        {
        }

        public SemVerVersionBuilder WithMajor(ushort major)
        {
            this._major = major;
            return this;
        }

        public SemVerVersionBuilder WithMinor(ushort minor)
        {
            this._minor = minor;
            return this;
        }
        public SemVerVersionBuilder WithPatch(ushort patch)
        {
            this._patch = patch;
            return this;
        }

        public SemVerVersion Build()
        {
            return new(_major, _minor, _patch);
        }
    }
}
