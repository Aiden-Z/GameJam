using System;
using Logic.Core.Controller.PlatformController;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Logic.Core.Ant
{
    public class AcidAnt : MonoBehaviour
    {
        public float AttackInterval;
        public float AttackRadius;
        public Animator AcidAnimator;
        public Transform From;

        private float m_attackTimer;
        private static readonly int Attack = Animator.StringToHash("Attack");

        private void FixedUpdate()
        {
            m_attackTimer += Time.fixedDeltaTime;
            if (m_attackTimer >= AttackInterval)
            {
                if (Vector3.Distance(GameSceneManager.Instance.PlatformController.transform.position, transform.position) > AttackRadius) return;
                m_attackTimer = 0;
                var targetGround = GameSceneManager.Instance.PlatformController.GetRandomGround();
                var acidBody = Instantiate(GameSceneManager.Instance.AcidBodyTemplate, From.position,
                    Quaternion.identity, GameSceneManager.Instance.AcidBodyRoot);
                acidBody.Target = targetGround.transform;
                AcidAnimator.SetTrigger(Attack);
            }
        }

        private void OnTriggerEnter2D(Collider2D other1)
        {
            Destroy(gameObject);
            GameSceneManager.Instance.ThrowBait(transform.position);
        }
    }
}