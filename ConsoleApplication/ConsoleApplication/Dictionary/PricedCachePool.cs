//**************************************************************************************
//Create By fred on 2019/04/28
//
//@Description 按价格排序的缓存池
//
//  1、缓存池中每个Key下能缓存多个Value，以栈的方式存储，每次获取时拿到的是最后放入的Value
//  2、缓存池中每个Key对应一个价格Price，Price大的优先驻留缓存池，即超过上限时剔除Price最小的Key
//**************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication.Dictionary
{
    public class PricedCachePool<TKey, TValue>
    {
        #region protected 成员
        protected struct PricedKey
        {
            internal TKey key;
            internal ulong price;

            internal PricedKey(TKey key, ulong price)
            {
                this.key = key;
                this.price = price;
            }
        }

        protected sealed class Compare : IComparer<PricedKey>
        {
            int IComparer<PricedKey>.Compare(PricedKey x, PricedKey y)
            {
                if (x.price < y.price) return -1;
                if (x.price > y.price) return 1;

                return Comparer<TKey>.Default.Compare(x.key, y.key);
            }
        }

        protected int _poolCapacity;
        protected int _replicaLimit;
        protected SortedDictionary<PricedKey, Stack<TValue>> _pool;
        protected Dictionary<TKey, ulong> _prices;

        #endregion

        public event Action<TKey, IEnumerable<TValue>, ulong> OnEvict;

        public int Count => _pool.Count;

        public PricedCachePool(int capacity, int replicaLimit)
        {
            _poolCapacity = capacity;
            _replicaLimit = replicaLimit;
            _pool = new SortedDictionary<PricedKey, Stack<TValue>>(new Compare());
            _prices = new Dictionary<TKey, ulong>();
        }

        /// <summary>
        /// 尝试将要缓存的值放入缓存池中（会进行缓存数量、价格等检查）
        /// </summary>
        /// <param name="key">要缓存的键</param>
        /// <param name="value">要缓存的一个值</param>
        /// <param name="newPrice">键对应的最新价格</param>
        /// <returns>是否真正将值放入了缓冲池</returns>
        public bool TryPut(TKey key, TValue value, ulong newPrice)
        {

            if (_prices.TryGetValue(key, out ulong oldPrice))
            {
                PricedKey pricedKey = new PricedKey(key, oldPrice);
                var values = _pool[pricedKey];
                
                if (oldPrice != newPrice)
                {
                    _prices[key] = newPrice;
                    _pool.Remove(pricedKey);

                    pricedKey.price = newPrice;
                    _pool.Add(pricedKey, values);
                }

                if (values.Count < _replicaLimit)
                {
                    values.Push(value);
                    return true;
                }

                return false;
            }

            bool shouldAdd = false;
            bool shouldEvict = false;
            KeyValuePair<PricedKey, Stack<TValue>> min = default;

            if (_poolCapacity > _pool.Count)
            {
                // 缓存池未满时 => 添加
                shouldAdd = true;
            }
            else
            {
                min = _pool.First();

                if (newPrice > min.Key.price)
                {
                    // 缓存池已满&&当前值大于最小值 => 移除并添加
                    shouldEvict = true;
                    shouldAdd = true;
                }
            }

            if (shouldAdd)
            {
                var addKey = new PricedKey(key, newPrice);
                var addValue = new Stack<TValue>();
                addValue.Push(value);
                _pool.Add(addKey, addValue);
                _prices.Add(key, newPrice);
            }
            
            if (shouldEvict)
            {
                var evictKey = min.Key;
                var evictValue = min.Value;
                _pool.Remove(evictKey);
                _prices.Remove(evictKey.key);
                OnEvict?.Invoke(evictKey.key, evictValue, evictKey.price);
            }

            return shouldAdd;
        }

        /// <summary>
        /// 尝试从缓存池中拿走一个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryTake(TKey key, out TValue value)
        {
            if (_prices.TryGetValue(key, out ulong price))
            {
                PricedKey pricedKey = new PricedKey(key, price);
                var values = _pool[pricedKey];

                if (values.Count > 0)
                {
                    value = values.Pop();
                    return true;
                }
            }

            value = default;
            return false;
        }


    }
}
