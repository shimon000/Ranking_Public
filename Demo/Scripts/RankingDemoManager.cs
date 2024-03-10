using System;
using Cysharp.Threading.Tasks;
using Ranking.Scripts;
using Ranking.Scripts.Interface;
using Ranking.Demo;
using Ranking.Demo.Scripts.DemoGame;
using TMPro;
using UnityEngine;

/// <summary>
/// ランキングのデモシーンに使う処理を置いておくクラス
/// </summary>
public class RankingDemoManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField _playerNameInput, _scoreInput;
    [SerializeField] private PlayerNameHolder _playerNameHolder;
    [SerializeField] private PlayerScoreHolder _scoreHolder;
    private RankingStorage _storage;
    private bool isLogin = false;

    private async void Start()
    {
        //とりあえずログイン
        Login();
        
        //ストレージをセット
        SetStorage();
    }

    private void SetStorage()
    {
        _storage = RankingStorage.instance;
    }
    /// <summary>
    /// RegisterButtonで発火する関数
    /// </summary>
    public void Register()
    {
        _storage.Register();
    }
    
    public void OK_Score()
    {
        //ScoreのinputFieldに入力された値を変換しHolderに
        int value_int = int.Parse(_scoreInput.text);
        var score = new Score(value_int);
        _scoreHolder.UpdateScore(score);

        //inputFieldを空に
        _scoreInput.text = "";
    }

    public void OK_PlayerName()
    {
        //PlayerNameのinputFieldに入力された値を変換しストレージに登録
        string value_string = _playerNameInput.text;
        var playerName = new PlayerName(value_string);
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
}
