using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logic.Core.Controller.BaitFinder
{
    public class BaitFinder : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            GameSceneManager.Instance.PlayerController.Collect(other.GetComponent<Collectable>());
        }
    }
}