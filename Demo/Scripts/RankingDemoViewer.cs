using System;
using Cysharp.Threading.Tasks;
using Ranking.Scripts;
using Ranking.Scripts.Interface;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ranking.Demo
{
    /// <summary>
    /// ランキングデモパネルのUIを管理するクラス
    /// </summary>
    public class RankingDemoViewer : MonoBehaviour,IRankingViewer
    {
        [SerializeField] private TextMeshProUGUI _nowScoreText, _nowNameText;
        [SerializeField] private Toggle _toggle;
        [SerializeField] private GameObject nowDataPanel;
        [SerializeField] private GameObject scoreUI;
        [SerializeField] private Canvas _board, _setting;
        private RankingTable[] _rankingTables;
        private Score _score;
        private PlayerName _name;
        private RankingStorage _storage;

        private void Awake()
        {
            SetStorage();
            
            _rankingTables = gameObject.GetComponentsInChildren<RankingTable>();
        
            if(_rankingTables == null)
                Debug.LogError("RankingTableが取得できていません");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (!_board.isActiveAndEnabled)
                {
                    ShowSettingPanel();
                }
                else
                {
                    HideSettingPanel();
                }
            }
        }

        /// <summary>
        /// 現在のデータを表示する関数
        /// </summary>
        public async void ShowNowData()
        {
            SetData();

            _nowScoreText.text = _score.IntValue.ToString();
            _nowNameText.text = _name.StringValue;
            
            //2秒だけ表示
            nowDataPanel.SetActive(true);
            await UniTask.Delay(2000);
            nowDataPanel.SetActive(false);
        }
        
        /// <summary>
        /// UpdateButtonで発火する関数
        /// </summary>
        public async void UpdateRankingBoard()
        {
            Debug.Log("リーダーボードを更新します");
            var tableNum = _rankingTables.Length;
        
            //サーバーからランキングボードを取得
            var data = await PlayFabManager.GetLeaderboardAsync(tableNum);
        
            //その後、結果を表示
            for(int i = 0;i < tableNum;i++)
            {
                try
                {
                    _rankingTables[i].Show(data[i],i);
                }
                catch (Exception e)
                {
                    Debug.Log("ランキングが一定数以上登録されていませんでした");
                    break;
                }
            }
            Debug.Log("リーダーボードを更新しました");
        }
        public void SetData()
        {
            if(!_storage)
                SetStorage();
            
            _score = _storage.GetData<Score>();
            _name = _storage.GetData<PlayerName>();
        }

        private void SetStorage()
        {
            _storage = RankingStorage.instance;
        }
        
        /// <summary>
        /// トグルが操作されたときに発火する関数
        /// </summary>
        public void OnChangeToggleValue()
        {
            //トグルの値によってスコアのUIの表示を切り替える
            scoreUI.SetActive(_toggle.isOn);
        }
        /// <summary>
        /// パネルを表示する関数
        /// </summary>
        private void ShowSettingPanel()
        {
            //ランキング表を更新
            UpdateRankingBoard();
            
            _board.enabled = true;
            _setting.enabled = true;
        }
        
        /// <summary>
        /// パネルを隠す関数
        /// </summary>
        private void HideSettingPanel()
        {
            _board.enabled = false;
            _setting.enabled = false;
        }
    }
}