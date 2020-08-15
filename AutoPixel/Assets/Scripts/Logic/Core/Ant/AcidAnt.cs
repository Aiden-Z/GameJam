using System;
using Logic.Core.Controller.PlatformController;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Logic.Core.Ant
{
    public class AcidAnt : MonoBehaviour
    {
        public float AttackInterval;
        public Animator AcidAnimator;

        private float m_attackTimer;
        private static readonly int Attack = Animator.StringToHash("Attack");

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
                AcidAnimator.SetTrigger(Attack);
            }
        }
    }
}