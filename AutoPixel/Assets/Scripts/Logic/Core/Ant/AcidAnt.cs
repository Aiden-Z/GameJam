﻿using System;
using Logic.Core.Controller.PlatformController;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Logic.Core.Ant
{
    public class AcidAnt : MonoBehaviour
    {
        public float AttackInterval;
        public Animation AttackAnimation;

        private float m_attackTimer;
        private void FixedUpdate()
        {
            m_attackTimer += Time.fixedDeltaTime;
            if (m_attackTimer >= AttackInterval)
            {
                m_attackTimer = 0;
                
                var targetGround = GameSceneManager.Instance.PlatformController.GetRandomGround();
                var acidBody = Instantiate(GameSceneManager.Instance.AcidBodyTemplate, Vector3.zero,
                    Quaternion.identity, GameSceneManager.Instance.AcidBodyRoot);
                acidBody.Target = targetGround.transform;
                AttackAnimation.Play();
            }
        }
    }
}