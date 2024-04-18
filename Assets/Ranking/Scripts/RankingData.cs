using System;
using System.Collections.Generic;
using Ranking.Scripts.DataBase;
using Ranking.Scripts.Interface;

namespace Ranking.Scripts
{
    /// <summary>
    /// ランキングに登録する要素の型データをキーとしてobject型のインスタンスを保持するクラス
    /// </summary>
    [Serializable]
    public class RankingData
    {
        /// <summary>
        /// 型データとobject型のインスタンスを保持するDictionary
        /// </summary>
        private Dictionary<Type, object> _dataDictionary;
        
        /// <summary>
        /// クラス内からしか呼べないコンストラクタ
        /// </summary>
        private RankingData(Dictionary<Type, object> dictionary)
        {
            _dataDictionary = dictionary;
        }
        
        /// <summary>
        /// 初期の辞書データを指定してRankingDataクラスを作成する
        /// </summary>
        public static RankingData GenerateRankingDataWithDictionary(Dictionary<Type,object> dictionary)
        {
            return new RankingData(dictionary);
        }
        
        /// <summary>
        /// 辞書データなしでRankingDataクラスを作成する
        /// </summary>
        public static RankingData GenerateRankingDataWithoutDictionary()
        {
            return new RankingData(new Dictionary<Type, object>());
        }

        /// <summary>
        /// 指定した型のランキングデータを取得する
        /// </summary>
        /// <typeparam name="T">指定する型</typeparam>
        public T GetData<T>()
        where T : IRankingDataElement<T>
        {
            Type assignedType = typeof(T);
            object obj = _dataDictionary[assignedType];
            if (obj == null)
                throw new KeyNotFoundException("指定された型オブジェクトが見つかりません");

            T data = (T)obj;
            if (data == null)
                throw new InvalidCastException("無効なキャストです");

            return data;
        }
        /// <summary>
        /// 指定した型のランキングデータを更新する
        /// Dictionaryになければ新たに登録する
        /// </summary>
        /// <typeparam name="T">指定する型</typeparam>
        public void UpdateData<T>(T newData)
        where T : IRankingDataElement<T>
        {
            Type assignedType = typeof(T);

            if (_dataDictionary.ContainsKey(assignedType))
            {
                //データを更新
                _dataDictionary[assignedType] = newData;
            }
            else
            {
                //データを追加
                _dataDictionary.Add(assignedType,newData);
            }
        }
    }
}