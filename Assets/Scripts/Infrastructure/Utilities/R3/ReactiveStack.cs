using System;
using UnityEngine;
using R3;
using System.Linq;
using UnityEngine.Rendering;

namespace Infrastructure.Utilities
{
    public class ReactiveStack<T>
    {
        private ObservableList<T> _collection = new ObservableList<T>();
        private Subject<T> _pushSubject = new Subject<T>();
        private Subject<T> _popSubject = new Subject<T>();
        private Subject<Unit> _popOnEmptySubject = new Subject<Unit>();

        // 新しい Push イベントの Observable
        public R3.Observable<T> ObservePush() => _pushSubject;

        // 新しい Pop イベントの Observable
        public R3.Observable<T> ObservePop() => _popSubject;
        public R3.Observable<Unit> ObservePopOnEmpty() => _popOnEmptySubject;

        // 現在の要素数
        public int Count => _collection.Count;

        // 要素を「プッシュ」する (最後に追加)
        public void Push(T item)
        {
            _collection.Add(item);
            _pushSubject.OnNext(item); // Push イベントを発行
        }

        // 要素を「ポップ」する (最後の要素を取り出す)
        public T Pop()
        {
            if (_collection.Count > 0)
            {
                T lastItem = _collection.Last();
                _collection.RemoveAt(_collection.Count - 1);
                _popSubject.OnNext(lastItem); // Pop イベントを発行
                return lastItem;
            }
            else
            {
                _popOnEmptySubject.OnNext(Unit.Default); // 空のスタックからポップしようとしたときのイベントを発行
                throw new InvalidOperationException("Stack is empty.");
            }
        }

        // スタックの一番上の要素を削除せずに確認する
        public T Peek()
        {
            if (_collection.Count > 0)
            {
                return _collection.Last();
            }
            else
            {
                throw new InvalidOperationException("Stack is empty.");
            }
        }

        // スタックが空かどうか
        public bool IsEmpty => _collection.Count == 0;
    }
}