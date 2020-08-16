using UnityEngine;

namespace Logic.Core.Controller.StoneFinder
{
    public class StoneFinder : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            GameSceneManager.Instance.PlayerController.AddStone(other.GetComponent<Collectable>());
        }
    }
}