using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ranking.Demo.Scripts.DemoGame
{
    /// <summary>
    /// アラート処理を管理するクラス
    /// </summary>
    public class Alart : MonoBehaviour
    {
        [SerializeField]private float _charSize = 15;
        private TextMeshProUGUI _textMesh;
        private Image _image;
        private void Start()
        {
            if(!TryGetComponent(out _image))
                Debug.LogError("Imageが取得できません");
            _textMesh = GetComponentInChildren<TextMeshProUGUI>();
            if(!_textMesh)
                Debug.LogError("Textが取得できません");
            
            gameObject.SetActive(false);
            _image.gameObject.SetActive(false);
            
            PlayFabManager.onCompleteLogin += OnCompleteLogin;
            PlayFabManager.onFailedLogin += OnFailedLogin;
            PlayFabManager.onCompleteRegister += OnCompleteRegister;
            PlayFabManager.onFailedRegister += OnFailedRegister;
            PlayFabManager.onCompleteGetLeaderBoard += OnCompleteGetLeaderBoard;
            PlayFabManager.onFailedGetLeaderBoard += OnFailedGetLeaderBoard;
        }
    
        private async void ShowAlart(string message,Color messageColor)
        {
            _textMesh.text = message;
            _textMesh.color = messageColor;
            SetImageSize();
            //2秒間表示
            gameObject.SetActive(true);
            _image.gameObject.SetActive(true);
            await UniTask.Delay(2000);
            gameObject.SetActive(false);
            _image.gameObject.SetActive(false);
        }

        private void SetImageSize()
        {
            //これなぜかバグるので文字列の長さから取得する
            // var textSize = new Vector2(_textMesh.preferredWidth, _textMesh.preferredHeight);
            // _image.rectTransform.sizeDelta = textSize;
            // _textMesh.rectTransform.sizeDelta = textSize;

            var textWidth = _textMesh.text.Length * _charSize;
            var size = new Vector2(textWidth, _image.rectTransform.sizeDelta.y);
            _image.rectTransform.sizeDelta = size;
        }
        private async void OnCompleteLogin()
        {
            ShowAlart("Login Successful",Color.cyan);
        }
    
        private async void OnFailedLogin()
        {
            ShowAlart("Login Failed",Color.red);
        }
    
        private async void OnCompleteRegister()
        {
            ShowAlart("Register Successful",Color.cyan);
        }
    
        private async void OnFailedRegister()
        {
            ShowAlart("Register Failed",Color.red);
        }
    
        private async void OnCompleteGetLeaderBoard()
        {
            ShowAlart("Successfully Get Leader Board",Color.cyan);
        }
    
        private async void OnFailedGetLeaderBoard()
        {
            ShowAlart("Failed to Get Leader Board",Color.red);
        }
    }
}

