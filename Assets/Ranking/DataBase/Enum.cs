using System;

namespace Ranking.Scripts.DataBase
{
    /// ランキングの名前
    public enum RankingName
    {
        Ranking_Demo,
        HardModeRanking,
        NormalModeRanking
    }

    public class RankingEnumMethods
    {
        //Enumの要素名をstringとして変換
        public static string ConvertToEnumName(RankingName name)
        {
            string result = Enum.GetName(typeof(RankingName),(int)name);
            return result;
        }
    }
}