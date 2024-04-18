using Ranking.Scripts.Interface;

namespace Ranking.Scripts
{
    public class Score : IRankingDataElement<Score>
    {
        private readonly int _value;
        public int Value => _value;
        
        /// <summary>
        /// デフォルトコンストラクタ
        /// ランキングデータの生成に必要
        /// </summary>
        public Score()
        {
            _value = 0;
        }
        public Score(int initialNum)
        {
            _value = initialNum;
        }
        
        /// <summary>
        /// スコアを加算する
        /// score = score.Add(new Score(100)) とする
        /// </summary>
        public Score Add(Score other)
        {
            var n = _value + other.Value;
            return new Score(n);
        }

        public Score Subtract(Score other)
        {
            var n = _value - other.Value;
            return new Score(n);
        }
    } 
}