using System;
using System.Collections.Generic;
using Ranking.Scripts.Interface;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ranking.Scripts.DataBase;
using UnityEditor;

namespace Ranking.Scripts
{
    /// <summary>
    /// ランキングのストレージ
    /// 
    /// </summary>
    public class RankingStorage : MonoBehaviour,IRankingStorage
    {
        //シングルトンのインスタンス
        public static RankingStorage Instance;

        /// <summary>
        /// ランキングデータを更新するSceneの名前
        /// </summary>
        [SerializeField] private string[] _initializeSceneNames;
        
        /// <summary>
        /// ランキングの型データを補完するデータベース
        /// </summary>
        [SerializeField] private RankingConfig _config;
        
        /// <summary>
        ///　ランキングデータを保持するDictionary
        /// </summary>
        private Dictionary<RankingName,RankingData> _rankingDataDictionary;

        /// <summary>
        /// 初期化できているか
        /// </summary>
        private bool isSetuped = false;
        
        
        private void Awake()
        {
            if(_config == null)
                Debug.LogError("RankingConfigがnullです");
            
            //EditorでSceneの有無をチェック
#if UNITY_EDITOR
            //InitialSceneが設定されているとき
            if (_initializeSceneNames.Length > 1)
            {
                bool isExitScene = false;
                string[] sceneNames = new string[EditorBuildSettings.scenes.Length];
                
                //BuildSettingからscene名を取得
                for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
                {
                    var scenePath = EditorBuildSettings.scenes[i].path;
                    sceneNames[i] = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                }

                //initialに設定したsceneが存在するか確認
                foreach (var sceneName in sceneNames)
                {
                    foreach (var initialScene in _initializeSceneNames)
                    {
                        if (initialScene == sceneName)
                            isExitScene = true;
                    }
                }
                //sceneが存在しなかったら警告
                if (!isExitScene)
                    Debug.LogError("InitialSceneがBuildSettingに存在しません");
            }
#endif
            //シングルトンパターン
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                //Sceneが遷移するごとに呼び出される関数を設定
                SceneManager.sceneLoaded += InitializeOnPossible;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 初期化可能の時データを初期化する
        /// </summary>
        private void InitializeOnPossible(Scene nowScene,LoadSceneMode mode)
        {
            foreach (var sceneName in _initializeSceneNames)
            {
                if (nowScene.name == sceneName)
                {
                    InitializeData(); 
                    return;
                }
            }
        }
        
        /// <summary>
        /// ランキングデータを初期化する
        /// </summary>
        public void InitializeData()
        {
            "ランキングストレージを初期化します".RankingLog();
            //ランキングデータを取得
            _rankingDataDictionary = _config.CreateAllRankingDataDictionary();
            isSetuped = true;
            "ランキングストレージを初期化しました".RankingLog();
        }
        
        /// <summary>
        /// ランキングストレージのデータを更新
        /// </summary>
        /// <typeparam name="T">更新するデータ型</typeparam>
        public void UpdateData<T>(T data,RankingName name)
        where T : IRankingDataElement<T>
        {
            "ランキングストレージのデータを更新します".RankingLog();
            if (!isSetuped)
            {
                "ストレージが初期化されていません".RankingLog();
                return;
            }
            
            _rankingDataDictionary[name].UpdateData<T>(data);
            "ランキングストレージのデータを更新しました".RankingLog();
        }
        
        /// <summary>
        /// ランキングストレージから指定した型のデータを取得する
        /// </summary>
        /// <typeparam name="T">取得するデータ型</typeparam>
        public T GetData<T>(RankingName name)
        where T : IRankingDataElement<T>
        {
            if (isSetuped) 
                return _rankingDataDictionary[name].GetData<T>();
            
            "ストレージが初期化されていません".RankingLog();
            return default;
        }
        
        //Storageのデータをランキングシステムに登録（アップロード）
        public void Register(RankingName name)
        {
            if (_rankingDataDictionary[name] == null)
                throw new Exception("ランキングデータがありません");
            
            PlayFabManager.RegisterRankingData(_rankingDataDictionary[name],name);
        }

    }
}