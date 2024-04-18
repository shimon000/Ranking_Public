using System;
using Cysharp.Threading.Tasks;
using Ranking.Scripts;
using Ranking.Scripts.Interface;
using Ranking.Demo;
using Ranking.Demo.Scripts.DemoGame;
using Ranking.Scripts.DataBase;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ランキングDemoシーンのランキングパネルを管理するクラス
/// </summary>
public class RankingPanel : MonoBehaviour,IRankingViewer
{
    //ランキング関係のコンポーネント
    [SerializeField] private PlayerNameHolder _playerNameHolder;
    [SerializeField] private PlayerScoreHolder _scoreHolder;
    //UI関係のコンポーネント
    [SerializeField] private TMP_InputField _playerNameInput, _scoreInput;
    [SerializeField] private TextMeshProUGUI _nowScoreText, _nowNameText;
    [SerializeField] private Toggle _toggle;
    [SerializeField] private GameObject _nowDataPanel;
    [SerializeField] private GameObject _scoreUI;
    [SerializeField] private Canvas _leaderBoard, _setting;
    
    private RankingTable[] _rankingTables;
    private Score _score;
    private PlayerName _name;
    private RankingStorage _storage;
    private void Start()
    {
        //とりあえずログイン
        Login();
        
        //ストレージを取得
        SetStorage();
        
        //必要なコンポーネントを取得
        _rankingTables = gameObject.GetComponentsInChildren<RankingTable>();
        if(_rankingTables == null)
            Debug.LogError("RankingTableが取得できていません");
        
        _toggle = GetComponentInChildren<Toggle>();
        if(!_toggle)
            Debug.LogError("Toggleが取得できていません");
    }
    
    private void Update()
    {
        //inputFieldにカーソルがある場合
        if(_scoreInput.isFocused || _playerNameInput.isFocused)
            return;
        
        //Qキーを押したときにパネルの非表示を切り換える
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!_leaderBoard.isActiveAndEnabled)
            {
                ShowPanel();
            }
            else
            {
                HidePanel();
            }
        }
    }
    
    /// <summary>
    /// ランキングストレージを取得
    /// </summary>
    private void SetStorage()
    {
        _storage = RankingStorage.Instance;
    }
    /// <summary>
    /// ランキングストレージから値を取得する関数
    /// </summary>
    public void SetData()
    {
        if(!_storage)
            SetStorage();
            
        _score = _storage.GetData<Score>(RankingName.Ranking_Demo);
        _name = _storage.GetData<PlayerName>(RankingName.Ranking_Demo);
    }
    /// <summary>
    /// RegisterButtonで発火する関数
    /// </summary>
    public async void Register()
    {
        _storage.Register(RankingName.Ranking_Demo);
        //ログインしなおす
        await UniTask.Delay(1000);
        Login();
    }
    
    /// <summary>
    /// ScoreのOKボタンで発火する関数
    /// </summary>
    public void OK_Score()
    {
        //ScoreのinputFieldに入力された値を変換しHolderに
        int valueInt = int.Parse(_scoreInput.text);
        var score = new Score(valueInt);
        _scoreHolder.UpdateScore(score);

        //inputFieldを空に
        _scoreInput.text = "";
    }
    /// <summary>
    /// PlayerNameのOKボタンで発火する関数
    /// 
    /// </summary>
    public void OK_PlayerName()
    {
        //PlayerNameのinputFieldに入力された値を変換しストレージに登録
        string valueString = _playerNameInput.text;
        //nullか文字なしの時"NoName"として登録する
        if (string.IsNullOrEmpty(valueString))
        {
            valueString = "NoName";
        }
        var playerName = new PlayerName(valueString);
        _playerNameHolder.UpdatePlayerName(playerName);
        
        //inputFieldを空に
        _playerNameInput.text = "";
    }

    /// <summary>
    /// Loginボタンで発火する関数
    /// </summary>
    public void Login()
    {
        PlayFabManager.LogIn();
    }
    
    /// <summary>
    /// 現在のデータを表示する関数
    /// </summary>
    public async void ShowNowData()
    {
        SetData();

        _nowScoreText.text = _score.Value.ToString();
        _nowNameText.text = _name.Value;
        
        //2秒だけ表示
        _nowDataPanel.SetActive(true);
        await UniTask.Delay(2000);
        _nowDataPanel.SetActive(false);
    }
    
    /// <summary>
    /// UpdateButtonで発火する関数
    /// </summary>
    public async void UpdateRankingBoard()
    {
        "リーダーボードを更新します".RankingLog();
        var tableNum = _rankingTables.Length;
        
        //サーバーからランキングボードを取得
        var data = await PlayFabManager.GetLeaderboardAsync(tableNum,RankingName.Ranking_Demo);
        
        //その後、結果を表示
        for(int i = 0;i < tableNum;i++)
        {
            try
            {
                _rankingTables[i].Show(data[i],i);
            }
            catch (Exception e)
            {
                "ランキングが一定数以上登録されていませんでした".RankingLog();
                break;
            }
        }
        "リーダーボードを更新しました".RankingLog();
    }
    
    /// <summary>
    /// トグルが操作されたときに発火する関数
    /// </summary>
    public void OnChangeToggleValue()
    {
        //トグルの値によってスコアのUIの表示を切り替える
        _scoreUI.SetActive(_toggle.isOn);
    }
    /// <summary>
    /// パネルを表示する関数
    /// </summary>
    private void ShowPanel()
    {
        //ランキング表を更新
        UpdateRankingBoard();
        
        _leaderBoard.enabled = true;
        _setting.enabled = true;
    }
        
    /// <summary>
    /// パネルを隠す関数
    /// </summary>
    private void HidePanel()
    {
        _leaderBoard.enabled = false;
        _setting.enabled = false;
    }
}

