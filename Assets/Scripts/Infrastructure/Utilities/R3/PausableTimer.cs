using System;
using R3;
using UnityEngine;

namespace Infrastructure.Utilities
{
    public static class GenericExtensions
    {
        /// <summary>
        /// 一時停止と速度変更が可能なタイマーを生成します。
        /// </summary>
        public static Observable<long> PausableTimer(
            float duration,
            ReactiveProperty<bool> isPaused,
            ReactiveProperty<float> speedRate)
        {
            return Observable.EveryUpdate(UnityFrameProvider.Update)
                .Select(_ => Time.deltaTime) // 経過時間（秒）を選択

                // Scan: 過去の値(acc)と現在の値(dt)を使って累積値を計算
                .Scan(0f, (acc, dt) =>
                {
                    // isPausedがtrueなら時間を進めず、falseなら速度をかけて時間を進める
                    return isPaused.CurrentValue ? acc : acc + dt * speedRate.CurrentValue;
                })

                // TakeWhile: 累積値がduration未満の間だけストリームを継続
                .TakeWhile(elapsed => elapsed < duration)

                // Select: TakeWhileを通ったストリームの値を型合わせのため0Lに変換
                .Select(_ => 0L)
                // R3ではIObservable<T>の代わりにObservable<T>を返します
                .AsObservable();
        }

        /// <summary>
        /// 一時停止が可能なタイマーを生成します。（速度1.0f固定）
        /// </summary>
        public static Observable<long> PausableTimer(float duration, ReactiveProperty<bool> isPaused)
        {
            // R3では新しいReactivePropertyを作成する際にCurrentValueを設定します
            return PausableTimer(duration, isPaused, new ReactiveProperty<float>(1.0f));
        }
    }
}