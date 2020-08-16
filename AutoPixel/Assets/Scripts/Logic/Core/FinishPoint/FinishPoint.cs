using System;
using Logic.Manager.SceneMgr;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Logic.Core.FinishPoint
{
    public class FinishPoint : MonoBehaviour
    {
        public string NextLevelName;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            SceneManager.LoadScene(NextLevelName);
        }
    }
}