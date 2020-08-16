using System;
using UnityEngine;

namespace Logic.Core.Controller
{
    public class DeathChecker : MonoBehaviour
    {
        private void OnTriggerExit2D(Collider2D other)
        {
            GameSceneManager.Instance.RestartGame();
        }
    }
}