using Ranking.Scripts.Interface;

namespace Ranking.Scripts
{
    public class PlayerName : IRankingDataElement<PlayerName>
    {
        private readonly string _value;
        public string Value => _value;
        
        /// <summary>
        /// デフォルトコンストラクタ
        /// ランキングデータの生成に必要
        /// </summary>
        public PlayerName()
        {
            _value = "";
        }
        public PlayerName(string _initialChar)
        {
            _value = _initialChar;
        }

        public PlayerName(char _initial)
        {
            _value = _initial.ToString();
        }

        /// <summary>
        /// 文字を連結させ、新たなPlayerNameクラスを返す
        /// 実例 new PlayerName("a").Add(new PlayerName("bc")); はabcを返す
        /// </summary>
        public PlayerName Add(PlayerName addChar)
        {
            string newName = this._value + addChar.Value;
            return new PlayerName(newName);
        }
        
        /// <summary>
        /// 一文字消す
        /// </summary>
        public PlayerName DeleteLastChar()
        {
            var newName = _value.Remove(_value.Length-1);
            return new PlayerName(newName);
        }
    }
}