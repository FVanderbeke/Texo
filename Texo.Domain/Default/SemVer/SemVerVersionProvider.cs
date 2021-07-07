using Texo.Domain.Api.Provider;

namespace Texo.Domain.Default.SemVer
{
    public sealed class SemVerVersionProvider : IVersionProvider<SemVerVersion, SemVerVersionBuilder, SemVerStoredVersion, ISemVerVersionFactory, ISemVerVersionRepository>
    {
        public const string SemVerName = "SemVer";
        
        public SemVerVersionProvider(ISemVerVersionFactory factory, ISemVerVersionRepository repository)
        {
            Name = SemVerName;
            Factory = factory;
            Repository = repository;
        }

        public string Name { get; }

        public SemVerVersionBuilder Builder() => SemVerVersionBuilder.Create(); 
        
        public ISemVerVersionFactory Factory { get; }

        public ISemVerVersionRepository Repository { get; }
    }
}