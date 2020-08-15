using System;
using Logic.Core.Ant;
using Logic.Core.Controller.PlatformController;
using UnityEngine;
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

        private void Awake()
        {
            Instance = this;
        }

        public void ThrowBait(Vector3 pos)
        {
            var randomRad = Random.Range(0, Mathf.PI * 2);
            var x = Mathf.Cos(randomRad) * radius;
            var y = Mathf.Sin(randomRad) * radius;
            var bait = Instantiate(BaitTemplate, Vector3.zero, Quaternion.identity, BaitRoot);
            bait.transform.position = pos + new Vector3(x, y);
        }
    }
}