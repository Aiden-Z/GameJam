using System;
using Logic.Core.Ant;
using Logic.Core.Controller.PlatformController;
using UnityEngine;

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

        private void Awake()
        {
            Instance = this;
        }
    }
}