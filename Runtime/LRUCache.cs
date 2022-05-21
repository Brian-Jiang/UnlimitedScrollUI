using System;
using System.Collections.Generic;

namespace UnlimitedScrollUI {
    internal readonly struct LRUCacheItem<TK, TV> {
        public LRUCacheItem(TK k, TV v) {
            key = k;
            value = v;
        }

        public readonly TK key;
        public readonly TV value;
    }

    internal class LRUCache<TK, TV> {
        public int Count => cacheMap.Count;
        
        private readonly Action<TK, TV> onDestroy;
        private uint capacity;

        private readonly Dictionary<TK, LinkedListNode<LRUCacheItem<TK, TV>>> cacheMap =
            new Dictionary<TK, LinkedListNode<LRUCacheItem<TK, TV>>>();

        private readonly LinkedList<LRUCacheItem<TK, TV>> lruList = new LinkedList<LRUCacheItem<TK, TV>>();

        public LRUCache(Action<TK, TV> destroyAction, uint newCapacity) {
            onDestroy = destroyAction;
            capacity = newCapacity;
        }

        public TV Get(TK key) {
            if (!cacheMap.TryGetValue(key, out var node)) return default;
            
            var value = node.Value.value;
            lruList.Remove(node);
            lruList.AddLast(node);
            return value;
        }

        public bool TryGet(TK key, out TV value) {
            if (cacheMap.TryGetValue(key, out var node)) {
                value = node.Value.value;
                return true;
            }

            value = default;
            return false;
        }

        public void Add(TK key, TV value) {
            if (cacheMap.TryGetValue(key, out var existingNode)) {
                lruList.Remove(existingNode);
                lruList.AddLast(existingNode);
                return;
            }

            if (capacity <= 0) {
                onDestroy?.Invoke(key, value);
                return;
            }

            if (Count >= capacity) {
                RemoveFirst();
            }

            var cacheItem = new LRUCacheItem<TK, TV>(key, value);
            var node = new LinkedListNode<LRUCacheItem<TK, TV>>(cacheItem);
            lruList.AddLast(node);
            cacheMap.Add(key, node);
        }

        public TV Remove(TK key) {
            if (!cacheMap.TryGetValue(key, out var existingNode)) return default;
            
            lruList.Remove(existingNode);
            cacheMap.Remove(key);
            return existingNode.Value.value;
        }

        public void SetCapacity(uint newCapacity) {
            capacity = newCapacity;
            Trim();
        }

        public void Clear() {
            while (Count > 0) {
                RemoveFirst();
            }
        }

        private void Trim() {
            while (Count > capacity) {
                RemoveFirst();
            }
        }

        private void RemoveFirst() {
            var node = lruList.First;
            lruList.RemoveFirst();
            cacheMap.Remove(node.Value.key);
            
            onDestroy?.Invoke(node.Value.key, node.Value.value);
        }
    }
}
