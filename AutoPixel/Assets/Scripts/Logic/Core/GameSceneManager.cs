using System;
using Logic.Core.Ant;
using Logic.Core.Controller.PlatformController;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Logic.Core
{
    public class GameSceneManager : MonoBehaviour
    {
        public static GameSceneManager Instance;

        public PlatformController PlatformController;
        public PlayerController.PlayerController PlayerController;
        public Transform AcidBodyRoot;
        public AcidBody AcidBodyTemplate;
        public Ant.Ant AntTemplate;
        public AntManager AntManager;
        public float radius;
        public Bait.Bait BaitTemplate;
        public Transform BaitRoot;
        public FinishPoint.FinishPoint FinishPoint;
        public int MaxThrowBaitsNum;
        public int MinThrowBaitsNum;
        public AcidAnt AcidAntTemplate;
        public camerashake Camerashake;

        private void Awake()
        {
            Instance = this;
            Application.targetFrameRate = 120;
        }

        public void CheckDeath()
        {
            var noGround = true;
            foreach (var ground in PlatformController.GetGrounds())
            {
                foreach (var ground1 in ground)
                {
                    if (ground1.IsAlive)
                    {
                        noGround = false;
                    }
                }
            }

            if (noGround)
            {
                RestartGame();
            }
        }
        
        public void ThrowBait(Vector3 pos, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var randomRad = Random.Range(0, Mathf.PI * 2);
                var x = Mathf.Cos(randomRad) * radius;
                var y = Mathf.Sin(randomRad) * radius;
                var bait = Instantiate(BaitTemplate, Vector3.zero, Quaternion.identity, BaitRoot);
                bait.transform.position = pos + new Vector3(x, y);
            }
        }

        public void ThrowBait(Vector3 pos)
        {
            var num = Random.Range(MinThrowBaitsNum, MaxThrowBaitsNum);
            for (int i = 0; i < num; i++)
            {
                var randomRad = Random.Range(0, Mathf.PI * 2);
                var x = Mathf.Cos(randomRad) * radius;
                var y = Mathf.Sin(randomRad) * radius;
                var bait = Instantiate(BaitTemplate, Vector3.zero, Quaternion.identity, BaitRoot);
                bait.transform.position = pos + new Vector3(x, y);
            }
        }

        public void RestartGame()
        {
            var sceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(sceneName);
        }
    }
}