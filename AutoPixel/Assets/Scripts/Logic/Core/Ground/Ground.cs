using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logic.Core.Ground
{
    public class Ground : MonoBehaviour
    {
        public int Life = 4;
        public GroundType Type;

        public int X, Y;

        public int Health;
        public Collider2D Collider2D;

        public bool IsAlive => Health >= 0;

        public bool TimeDamage()
        {
            Life -= 2;
            if (Life > 0)
            {
                return false;
            }
            else
            {
                gameObject.SetActive(false);
                return true;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Block"))
            {
                Health -= 25;
            }
        }

        public void OnCorrode()
        {
            Health -= 50;
            if (Health <= 0)
            {
                OnDead();
            }
        }
        
        public void OnDead()
        {
            Collider2D.gameObject.layer = LayerMask.NameToLayer("PlayerBlock");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {

        }

        public enum GroundType
        {
            Edge,
            Inner
        }
    }
}