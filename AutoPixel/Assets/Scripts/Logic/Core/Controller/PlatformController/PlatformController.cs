using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logic.Core.Controller.PlatformController
{
    public class PlatformController : MonoBehaviour
    {
        public HashSet<Transform> Targets = new HashSet<Transform>();
        public float ForceFactor;
        public float FloatingFactor;
        public Rigidbody2D Rigidbody2D;
        public List<Ground.Ground> Grounds = new List<Ground.Ground>();

        private void Awake()
        {
            var grounds = GetComponentsInChildren<Ground.Ground>();
            Grounds.AddRange(grounds);
        }

        public void SetTarget(Transform target)
        {
            Targets.Add(target);
            m_isMoving = true;
        }

        public void RemoveTarget(Transform target)
        {
            Targets.Remove(target);
        }

        private bool m_isMoving = false;
        private void FixedUpdate()
        {
            if (Targets.Count == 0)
            {
                if (m_isMoving)
                {
                    Rigidbody2D.velocity = Rigidbody2D.velocity.normalized * FloatingFactor;
                    m_isMoving = false;
                }
            }
            else
            {
                //Vector2 direction = Vector2.zero;
                foreach (var target in Targets)
                {
                    //direction += (Vector2)(target.position - transform.position);
                    Rigidbody2D.AddForceAtPosition((target.position - transform.position).normalized * ForceFactor, target.position);
                }

                //direction = direction.normalized * ForceFactor;
                
                
            }
        }
    }
}