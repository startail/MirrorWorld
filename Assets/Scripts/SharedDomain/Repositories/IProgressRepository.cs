namespace SharedDomain.Repositories
{
    public interface IProgressRepository
    {
        /// <summary>
        /// 指定された GameProgressData を外部ストレージに保存する契約。
        /// </summary>
        /// <param name="data">保存するデータエンティティ。</param>
        void Save(GameProgressData data);

        /// <summary>
        /// 外部ストレージから GameProgressData を読み込む契約。
        /// </summary>
        /// <returns>読み込んだデータエンティティ。ファイルが存在しない場合はnullまたは初期値を返す。</returns>
        GameProgressData Load();
    }
}