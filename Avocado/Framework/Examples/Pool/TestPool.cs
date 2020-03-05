using System.Collections;
using System.Collections.Generic;
using Avocado.Framework.Optimization;
using UnityEngine;

namespace Avocado.Framework.Examples.Pool {
    public class TestPool : MonoBehaviour {
        [SerializeField]private Enemy _enemyPrefab;
        [SerializeField]
        private int _startPoolSize = 10;
        [SerializeField]
        private float _spawnDelay = 2.0f;
        [SerializeField]
        private float _releaseDelay = 10.0f;

        [SerializeField]
        private bool _useFirstPool;
        
        private Pool<Enemy> _enemyPool;
        private Pool<Enemy> _enemyPool2;

        private Pool<Enemy> _currentPool;
        private List<Enemy> _enemies = new List<Enemy>();
        private bool _currentPoolBufferState = false;
        
        private void Start() {
            _currentPoolBufferState = _useFirstPool;
                
            _enemyPool = new Pool<Enemy>(_enemyPrefab, _startPoolSize, transform);
            _enemyPool2 = new Pool<Enemy>(_enemyPrefab, _startPoolSize);

            _currentPool = _useFirstPool ? _enemyPool : _enemyPool2;

            StartCoroutine(SpawnEnemy());
            StartCoroutine(DestroyEnemy());
        }

        private void Update() {
            if (_currentPoolBufferState != _useFirstPool) {
                _currentPoolBufferState = _useFirstPool;
                _currentPool = _useFirstPool ? _enemyPool : _enemyPool2;
            }
        }

        private IEnumerator SpawnEnemy() {
            while (true) {
                yield return new WaitForSeconds(_spawnDelay);

                var enemy = _currentPool.Get();
                _enemies.Add(enemy);
            }
        }

        private IEnumerator DestroyEnemy() {
            while (true) {
                yield return new WaitForSeconds(_releaseDelay);
                foreach (var enemy in _enemies) {
                    _enemyPool.Release(enemy);
                }
                
                _enemies.Clear();
            }
        }
    }
}