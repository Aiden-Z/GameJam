using System;
using UnityEngine;

namespace Logic.Core.Controller.PlatformController
{
    public class PlatformController : MonoBehaviour
    {
        public Transform Target;
        public float Velocity;
        public float FloatingVelocity;
        public float Duration;
        public Rigidbody2D Rigidbody2D;
        public Collider2D Collider2D;

        private float m_movingTimer;
        private Vector2 m_direction;
        private bool m_isMoving = false;
        public void SetTarget(Transform target)
        {
            Target = target;
            m_direction = (target.position - transform.position).normalized;
            Rigidbody2D.velocity = m_direction * Velocity;
            m_movingTimer = 0;
            m_isMoving = true;
        }

        private void FixedUpdate()
        {
            if (m_isMoving)
            {
                m_movingTimer += Time.fixedDeltaTime;
                if (m_movingTimer >= Duration)
                {
                    Target = null;
                    Rigidbody2D.velocity = Rigidbody2D.velocity.normalized * FloatingVelocity;
                    m_isMoving = false;
                }
            }
        }
    }
}