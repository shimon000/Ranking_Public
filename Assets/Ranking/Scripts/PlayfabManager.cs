using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using Ranking.Scripts;
using Ranking.Scripts.DataBase;
using UnityEngine;


//参考　https://kan-kikuchi.hatenablog.com/entry/PlayFabLogin

/// <summary>
/// PlayFabに関する操作をまとめておくクラス
/// </summary>
public class PlayFabManager
{
    public static event Action onCompleteLogin;
    public static event Action onFailedLogin;
    public static event Action onCompleteRegister;
    public static event Action onFailedRegister;
    public static event Action onCompleteGetLeaderBoard;
    public static event Action onFailedGetLeaderBoard;
    
    /// <summary>
    /// ログインする関数
    /// </summary>
    public static void LogIn()
    {
        //カスタムＩＤを生成
        string _customID = GenerateCustomID();
        
        //ログイン
        PlayFabClientAPI.LoginWithCustomID(
            new LoginWithCustomIDRequest { CustomId = _customID, CreateAccount = true },
            OnLoginSuccess,
            error =>
            {
                onFailedLogin?.Invoke();
                "ログイン失敗".RankingLog();
            });
    }
    
    /// <summary>
    /// ログインが成功したとき
    /// </summary>
    private static void OnLoginSuccess(LoginResult result)
    {
        //IDが既に使われていた場合
        if (!result.NewlyCreated)
        {
            //ログインしなおし
            LogIn();
        }
        else
        {
            onCompleteLogin?.Invoke();
            $"ログインしました".RankingLog();
        }
    }

    public static void RegisterRankingData(RankingData data,RankingName name)
    {
        var scoreData = data.GetData<Score>();
        var nameData = data.GetData<PlayerName>();
        
        Register_Int(scoreData.Value,RankingEnumMethods.ConvertToEnumName(name));
        Register_String(nameData.Value);
    }

    /// <summary>
    /// Intデータを登録する関数
    /// </summary>
    private static void Register_Int(int value,string rankingName)
    {
        
        PlayFabClientAPI.UpdatePlayerStatistics
        (
            new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>()
                {
                    new StatisticUpdate
                    {
                        StatisticName = rankingName,
                        Value = value
                    }
                }
            },
            result =>
            {
                onCompleteRegister?.Invoke();
                $"{value}:スコア送信".RankingLog();
            },
            error =>
            {
                onFailedRegister?.Invoke();
                $"{value}:スコア送信に失敗\n{error.GenerateErrorReport()}".RankingLog();
            }
        );
    }
    
    /// <summary>
    /// Stringデータの登録
    /// </summary>
    private static void Register_String(string value)
    {
        //PlayFabは名前を３文字以上にする規定があるため空白で埋める
        if (value.Length < 3)
            value = "  " + value;
        
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = value
        };
        //ユーザ名の更新
        $"ユーザ名の更新開始".RankingLog();
        PlayFabClientAPI.UpdateUserTitleDisplayName
        (
            request,
            result =>
            {
                onCompleteRegister?.Invoke();
                $"ユーザ名の更新が成功しました : {result.DisplayName}".RankingLog();
            },
            error =>
            {
                onFailedRegister?.Invoke();
                Debug.LogError($"ユーザ名の更新に失敗しました\n{error.GenerateErrorReport()}");
            }
        );    
    }
    
    //=================================================================================
    //カスタムIDの生成
    //=================================================================================
    
    //IDに使用する文字
    private static readonly string ID_CHARACTERS = "0123456789abcdefghijklmnopqrstuvwxyz";

    //IDを生成する
    private static string GenerateCustomID() {
        int idLength = 32;//IDの長さ
        StringBuilder stringBuilder = new StringBuilder(idLength);
        var random = new System.Random();

        //ランダムにIDを生成
        for (int i = 0; i < idLength; i++){
            stringBuilder.Append(ID_CHARACTERS[random.Next(ID_CHARACTERS.Length)]);
        }

        return stringBuilder.ToString();
    }
    
    //=================================================================================
    //ランキングボードの取得
    //=================================================================================
    
    /// <summary>
    /// 非同期可能
    /// ランキング(リーダーボード)を取得
    /// </summary>
    public static async UniTask<RankingData[]> GetLeaderboardAsync(int _maxResultCount,RankingName name)
    {
        //ランキング名を取得
        string rankingName = RankingEnumMethods.ConvertToEnumName(name);
        //ランキングを表示できないのでfalse
        bool canShowLeaderBoard = false;
        RankingData[] rankArray = null;
        //GetLeaderboardRequestのインスタンスを生成
        var request = new GetLeaderboardRequest{
            StatisticName   = rankingName, //ランキング名(統計情報名)
            StartPosition   = 0,                 //何位以降のランキングを取得するか
            MaxResultsCount = _maxResultCount              //ランキングデータを何件取得するか(最大100)
        };

        //ランキング(リーダーボード)を取得
        $"ランキング(リーダーボード)の取得開始".RankingLog();
        PlayFabClientAPI.GetLeaderboard
            (
                request,
                //取得に成功したとき
                result =>
                {
                    // ランキングデータ配列の作成
                    rankArray = new RankingData[result.Leaderboard.Count];
                    
                    foreach (var r in result.Leaderboard)
                    {
                        RankingData dataForDataBase = RankingData.GenerateRankingDataWithoutDictionary();
                        
                        //順位、スコア、名前を取得
                        int i = r.Position;
                        int i_score = r.StatValue;
                        string i_name = r.DisplayName;
                        
                        if(i == null || i_score == null || i_name == null)
                            Debug.LogError("取得したランキングが欠落しています");
                        
                        Score score = new Score(i_score);
                        PlayerName playerName = new PlayerName(i_name);
                        
                        //データを更新
                        dataForDataBase.UpdateData(score);
                        dataForDataBase.UpdateData(playerName);
                        
                        //配列に格納
                        rankArray[i] = dataForDataBase;
                    }
                    onCompleteGetLeaderBoard?.Invoke();
                    "ランキングの取得に成功しました".RankingLog();
                    //rankingDataの生成が完了したのでtrue
                    canShowLeaderBoard = true;
                },
                //取得に失敗したとき
                error =>
                {
                    onFailedGetLeaderBoard?.Invoke();
                    Debug.LogError($"ランキング(リーダーボード)の取得に失敗しました\n{error.GenerateErrorReport()}");
                });

        //ランキング取得後戻り値として返す
        for (int i = 0; i < 100; i++)
        {
            if (canShowLeaderBoard)
            {
                return rankArray;
            }

            //0.1秒待機する
            await UniTask.Delay(TimeSpan.FromMilliseconds(100));
        }
        "ランキングが取得できません".RankingLog();
        return null;
    }
}
