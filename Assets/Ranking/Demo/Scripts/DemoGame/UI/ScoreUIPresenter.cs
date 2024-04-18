using UniRx;
using UnityEngine;

namespace Ranking.Demo.Scripts.DemoGame
{
    public class ScoreUIPresenter : MonoBehaviour
    {
        [SerializeField] private PlayerScoreHolder _model;
        [SerializeField] private ScoreUIViewer _viewer;
        private void Start()
        {
            _model.Score.Subscribe(val =>
            {
                _viewer.UpdateText(val.Value);
            }).AddTo(this);
        }
    }
}