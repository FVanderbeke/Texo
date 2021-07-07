using System;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using Texo.Domain.Api.Provider;

namespace Texo.Domain.Api.Service
{
    public sealed class GlobalTransactionProvider : IDisposable
    {
        private readonly Seq<ITransactionProvider> _providers;
        private readonly Dictionary<string, object> _context;

        public GlobalTransactionProvider(IEnumerable<ITransactionProvider> providers)
        {
            _context = new Dictionary<string, object>();
            _providers = toList(providers).OrderBy(p => p.Priority).ToSeq();
            _providers.Do(t => t.OnLoad(_context));
        }

        public Option<R> GetContextProperty<R>(string propertyName) where R : class
        {
            return Optional(_context[propertyName])
                .Filter(o => o is R)
                .Map(o => o as R);
        }
        
        public void Begin()
        {
            _providers.Do(t => t.Begin(_context));
        }

        public void Commit()
        {
            _providers.Do(t => t.Commit(_context));
        }

        public void Rollback()
        {
            _providers.Do(t => t.Rollback(_context));
        }

        public void Dispose()
        {
            _providers.Do(t => t.OnDispose(_context));
        }
    }
}