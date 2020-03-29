using System.Collections.Generic;

namespace Broadcast.Core.Infrastructure
{
    public class SingletonDictionary<TKey, TValue> : Singleton<IDictionary<TKey, TValue>>
    {
        static SingletonDictionary()
        {
            Singleton<Dictionary<TKey, TValue>>.Instance = new Dictionary<TKey, TValue>();
        }

        public new static IDictionary<TKey, TValue> Instance => Singleton<Dictionary<TKey, TValue>>.Instance;
    }
}