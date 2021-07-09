using System;
using NodaTime;
using Texo.Domain.Model.Service;

namespace Texo.Infrastructure.Db.Service
{
    

    public static class TexoUtils
    {
        private sealed class DummyIdGenerator : IIdGenerator
        {
            private DummyIdGenerator()
            {
            }

            internal static readonly IIdGenerator Instance = new DummyIdGenerator(); 
        
            public Guid NewGuid() => Guid.NewGuid();
        }
        
        public static IClock DefaultClock => NodaTime.SystemClock.Instance;
        public static IIdGenerator DefaultIdGenerator => DummyIdGenerator.Instance;
    }
}