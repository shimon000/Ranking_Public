using Ranking.Scripts.DataBase;

namespace Ranking.Scripts.Interface
{
    public interface IRankingStorage
    {
        public void InitializeData();
        public void UpdateData<T>(T data,RankingName name)
            where T : IRankingDataElement<T>;

        public T GetData<T>(RankingName name)
            where T : IRankingDataElement<T>;

        public void Register(RankingName name);
    }
}