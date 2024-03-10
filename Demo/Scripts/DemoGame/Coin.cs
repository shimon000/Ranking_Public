using UnityEngine;

namespace Ranking.Demo.Scripts.DemoGame 
{
    public class Coin : MonoBehaviour
    {
        void Update()
        {
            //コインらしく回す
            transform.Rotate(0,0.5f,0);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out IPlayer _player))
            {
                _player.AddScore(100);
                Destroy(gameObject);
            }
        }

    }
}
