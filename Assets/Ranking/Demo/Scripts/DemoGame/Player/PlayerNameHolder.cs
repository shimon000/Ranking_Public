using System;
using Ranking.Scripts;
using Ranking.Scripts.DataBase;
using Ranking.Scripts.Interface;
using UniRx;
using UnityEngine;

namespace Ranking.Demo.Scripts.DemoGame
{
    //ランキングDemoシーンにおいてPlayerNameを保持するクラス
    public class PlayerNameHolder : MonoBehaviour,IRankingDataHolder<PlayerName>
    {
        private IRankingStorage _storage;
        private ReactiveProperty<PlayerName> _playerName = new ReactiveProperty<PlayerName>(new PlayerName("Player"));
        public IReadOnlyReactiveProperty<PlayerName> PlayerName => _playerName;
        
        private void Start()
        {
            SetStorage();
        }

        public void SetStorage()
        {
            _storage = RankingStorage.Instance;
        }
        
        public void SendData(PlayerName name)
        {
            _storage.UpdateData(name,RankingName.Ranking_Demo);
        }
        /// <summary>
        /// PlayerNameHolderのplayerNameを更新し、ストレージに保存する関数
        /// </summary>
        public void UpdatePlayerName(PlayerName playerName)
        {
            _playerName.Value = playerName;
            SendData(_playerName.Value);
        }
    }
}