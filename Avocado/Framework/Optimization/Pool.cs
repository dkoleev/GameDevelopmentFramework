using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Avocado.Framework.Optimization {
    public enum PoolType {
        WillGrow, // create new object if trying get from fulled pool
        ReplaceActive, //change one from active object with new if pool is full
        DontGrow // dont create new object if pool is full
    }

    public class Pool<T> where T : Component, IPoolable {
        private readonly Stack<T> _free;
        private readonly HashSet<T> _used;
        private readonly T _objSource;

        private PoolType _type;
        private Transform _objParent;
        
        public Pool(T objSource, int startSize, PoolType type = PoolType.WillGrow) : this (objSource, startSize, null, type) {

        }
        
        public Pool(T objSource, int startSize, Transform parent, PoolType type = PoolType.WillGrow) {
            _free = new Stack<T>(startSize);
            _used = new HashSet<T>();
            
            _type = type;
            _objSource = objSource;
            _objParent = parent;
            
            IncreasePool(startSize);
        }

        private void IncreasePool(int growAmount = 1) {
            for (int i = 0; i < growAmount; i++) {
                var obj = Object.Instantiate(_objSource);
                if (!(_objParent is null)) {
                    obj.transform.parent = _objParent;
                }

                obj.Release();
                _free.Push(obj);
            }
        }

        public T Get() {
            if (_free.Count > 0) {
                var obj = _free.Pop();
                _used.Add(obj);
                obj.Spawn();

                return obj;
            }

            IncreasePool();
            
            return Get();
        }

        public void Release(T obj) {
            obj.Release();

            if (_used.Contains(obj)) {
                _used.Remove(obj);
                _free.Push(obj);
            } else {
                Debug.Log("pool not contain object - " + obj.name);
            }
        }
    }
}