using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logic.Core.Ground
{
    public class Ground : MonoBehaviour
    {
        public bool Life = true;
        
        public GroundType Type;      

        public int X, Y;

        public int Health;
        public Collider2D Collider2D;

        public bool IsAlive => Health >= 0;

        public bool Damage()
        {
            if (Life)
            {
                Life = false;
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
        public enum GroundType
        {
            Edge,
            Inner
        }
    }
}