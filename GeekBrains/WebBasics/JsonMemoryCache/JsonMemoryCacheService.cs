using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text.Json;

namespace JsonMemoryCache
{
    public sealed class JsonMemoryCacheService
    {
        private readonly ConcurrentDictionary<Type, CacheHolder> _cache = new();

        public int Create<TType>(TType value)
        {
            var type = typeof(TType);

            var cache = new Cache(JsonSerializer.Serialize(value));

            if (_cache.ContainsKey(type))
            {
                return _cache[type].Add(cache);
            }

            if (_cache.TryAdd(type, new CacheHolder()))
            {
                return _cache[type].Add(cache);
            }

            return _cache[type].Add(cache);
        }

        public IReadOnlyCollection<(int, TType)> Get<TType>()
        {
            if (_cache.TryGetValue(typeof(TType), out var holder))
            {
                var cache = holder!.Get();
                if (cache.Count > 0) return cache.Select(c => (c.Id, JsonSerializer.Deserialize<TType>(c.EntryJson))).ToImmutableArray();
            }

            return Array.Empty<(int, TType)>();
        }

        private sealed class Cache
        {
            public Cache(string entryJson)
            {
                EntryJson = entryJson;
            }

            public string EntryJson { get; init; }
            public int Id { get; set; }
        }

        private sealed class CacheHolder
        {
            private int _indexCounter;
            private int IndexCounter => Interlocked.Increment(ref _indexCounter);

            private readonly List<Cache> _cache = new();

            public int Add(Cache cache)
            {
                var id = cache.Id = IndexCounter;
                _cache.Add(cache);
                return id;
            }

            public IReadOnlyCollection<Cache> Get()
            {
                return _cache.ToImmutableArray();
            }
        }
    }
}