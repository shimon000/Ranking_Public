using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR 
using UnityEditor;
#endif

namespace Ranking.Scripts.DataBase
{
    /// <summary>
    /// ランキングに登録する要素の型データを格納するデータベース
    /// </summary>
    [CreateAssetMenu(menuName = "Ranking/Config")]
    public class RankingConfig : ScriptableObject
    {
        [Serializable]
        private class RankingDataInfo
        {
            public RankingName _name;
#if UNITY_EDITOR
            public MonoScript[] _monoScript;
#endif
            [PersistentAmongPlayMode] public List<string> _elementName;
        }
        [SerializeField] private List<RankingDataInfo> _rankingData;
        
#if UNITY_EDITOR
        /// <summary>
        /// Inspectorに変数がアタッチされたときに呼ばれるイベント関数
        /// </summary>
        private void OnValidate()
        {
            //Inspectorにスクリプトがアタッチされたときにクラス名を取得し、stringに変換して対応する_elementNameに格納する
            for (int i_data = 0; i_data < _rankingData.Count; i_data++)
            {
                for (int i_script = 0; i_script < _rankingData[i_data]._monoScript.Length; i_script++)
                {
                    if (_rankingData[i_data]._monoScript.Length > _rankingData[i_data]._elementName.Count)
                        _rankingData[i_data]._elementName.Add(new string(""));

                    var script = _rankingData[i_data]._monoScript[i_script];

                    if (script != null)
                        _rankingData[i_data]._elementName[i_script] = script.GetClass().ToString();
                }
            }
        }
# endif

        /// <summary>
        /// ランキングの種類を指定しRankingDataを作成
        /// </summary>
        public RankingData CreateRankingData(RankingName rankingName)
        {
            Dictionary<Type, object> dictionary = new Dictionary<Type, object>();

            foreach (var data in _rankingData)
            {
                //指定されたRankingNameを_rankingDataから探索
                if (data._name != rankingName)
                    continue;

                //指定されたRankingNameに紐づけされているElementをすべてdictionaryに格納
                foreach (var name in data._elementName)
                {
                    Type type = Type.GetType(name);
                    if (type == null)
                        Debug.LogError("キャストできない型クラスです");

                    //もっともらしい型にリフレクション
                    object obj = Activator.CreateInstance(type);

                    if (dictionary.ContainsKey(type))
                        throw new Exception("指定する型が重複しています");

                    dictionary.Add(type, obj);
                }

                break;
            }

            return RankingData.GenerateRankingDataWithDictionary(dictionary);
        }

        /// <summary>
        /// すべてのランキングデータを保持するDictionaryを作成する
        /// </summary>
        public Dictionary<RankingName, RankingData> CreateAllRankingDataDictionary()
        {
            Dictionary<RankingName, RankingData> result = new Dictionary<RankingName, RankingData>();

            //RankingNameに紐づけられていたすべてのelementをひとつのRankingDataに格納
            foreach (var data in _rankingData)
            {
                Dictionary<Type, object> dictionary = new Dictionary<Type, object>();

                foreach (var name in data._elementName)
                {
                    Type type = Type.GetType(name);
                    if (type == null)
                        Debug.LogError("キャストできない型クラスです");

                    //もっともらしい型にリフレクション
                    object obj = Activator.CreateInstance(type);

                    if (dictionary.ContainsKey(type))
                        throw new Exception("指定する型が重複しています");

                    dictionary.Add(type, obj);
                }

                RankingData rankingData = RankingData.GenerateRankingDataWithDictionary(dictionary);

                if (result.ContainsKey(data._name))
                    Debug.LogError("ランキングの種類が重複しています");
                result.Add(data._name, rankingData);
            }

            return result;
        }
    }
}