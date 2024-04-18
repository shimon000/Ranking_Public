using UnityEngine;

namespace Ranking.Scripts
{
    public static class RankingLogExtension
    {
        /// <summary>
        /// Rankingタグをつけてコンソールに出力する
        /// 使用例 : "出力.RankingLog()"
        /// </summary>
        public static void RankingLog(this string message)
        {
            Debug.Log("[Ranking] " + message);
        }
    }
}