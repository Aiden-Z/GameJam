using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logic.Core.Controller.BaitFinder
{
    public class BaitFinder : MonoBehaviour
    {
        public PlayerController.PlayerController Controller;

        private void OnTriggerEnter2D(Collider2D other)
        {
            Controller.CollectBait(other.GetComponent<Bait.Bait>());
        }
    }
}