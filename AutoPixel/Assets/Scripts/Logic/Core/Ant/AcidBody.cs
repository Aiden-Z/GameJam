using System;
using UnityEngine;

namespace Logic.Core.Ant
{
    public class AcidBody : MonoBehaviour
    {
        public float Velocity;
        public float CorrodeRadius;
        public Collider2D Collider2D;
        public Transform Target;

        private void FixedUpdate()
        {
            var distance = Vector3.Distance(Target.position, transform.position);
            if (distance < 0.5f)
            {
                var objects = Physics2D.OverlapCircleAll(transform.position, CorrodeRadius);
                foreach (var o in objects)
                {
                    if (o.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        var ground = o.GetComponent<Ground.Ground>();
                        ground.OnCorrode();
                    }
                    else if (o.gameObject.layer == LayerMask.NameToLayer("Player"))
                    {
                        GameSceneManager.Instance.PlayerController.OnHurt();
                    }
                }
                
                Destroy(gameObject);
            }
            else
            {
                var direction = (Target.position - transform.position).normalized;
                transform.position += direction * (Time.fixedDeltaTime * Velocity);
            }
        }
    }
}