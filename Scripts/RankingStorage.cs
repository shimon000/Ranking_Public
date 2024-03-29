﻿using System;
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
        public static RankingStorage instance;

        /// <summary>
        /// ランキングデータを更新するSceneの名前
        /// </summary>
        [SerializeField] 
        private string[] _initializeSceneNames;
        
        /// <summary>
        /// ランキングの型データを補完するデータベース
        /// </summary>
        [SerializeField] 
        private RankingElementTypeDataBase _database;
        
        /// <summary>
        /// ストレージが保持するランキングデータ
        /// </summary>
        private static RankingData _rankingData;

        private void Awake()
        {
            bool isExitScene = false;
            string[] sceneNames = new string[EditorBuildSettings.scenes.Length];
            
            //InitialSceneが設定されているかの確認
            if(_initializeSceneNames.Length < 1)
                Debug.LogError("InitialSceneNameを設定してください");

            //BuildSettingからscene名を取得
            for(int i = 0; i < EditorBuildSettings.scenes.Length; i++)
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
            if(!isExitScene)
                Debug.LogError("InitialSceneがBuildSettingに存在しません");
            
            //シングルトンパターン
            //現在のsceneがinitialSceneに設定されていなかったらスルー
            foreach (var sceneName in _initializeSceneNames)
            {
                if (SceneManager.GetActiveScene().name != sceneName)
                    continue;
                
                if (instance == null)
                {
                    instance = this;
                    DontDestroyOnLoad(this);
                }
                else
                {
                    Destroy(gameObject);
                }
                break;
            }
        }

        private void Start()
        {
            InitializeData();
        }
        
        /// <summary>
        /// ランキングデータを初期化する
        /// </summary>
        private void InitializeData()
        {
            Debug.Log("ランキングストレージを初期化します");
            _rankingData = _database.CreateRankingData();
            Debug.Log("ランキングストレージを初期化しました");
        }
        
        /// <summary>
        /// ランキングストレージのデータを更新
        /// </summary>
        /// <typeparam name="T">更新するデータ型</typeparam>
        public void UpdateData<T>(T data)
        where T : IRankingDataElement<T>
        {
            Debug.Log("ランキングストレージのデータを更新します");
            _rankingData.UpdateData<T>(data);
            Debug.Log("ランキングストレージのデータを更新しました");
        }
        
        /// <summary>
        /// ランキングストレージから指定した型のデータを取得する
        /// </summary>
        /// <typeparam name="T">取得するデータ型</typeparam>
        public T GetData<T>()
        where T : IRankingDataElement<T>
        {
            return _rankingData.GetData<T>();
        }
        
        //Storageのデータをランキングシステムに登録（アップロード）
        public void Register()
        {
            if (_rankingData == null)
                throw new Exception("ランキングデータがありません");
            
            PlayFabManager.RegisterRankingData(_rankingData);
        }
    }
}