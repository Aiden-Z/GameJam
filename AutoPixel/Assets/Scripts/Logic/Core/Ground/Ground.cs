using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logic.Core.Ground
{
    public class Ground : MonoBehaviour
    {
        public GroundType Type;

        public int X, Y;

        public int Health
        {
            set => m_health = value;
            get => m_health;
        }

        private int m_health;
        public Collider2D Collider2D;

        public bool IsAlive => Health >= 0;

        public bool TimeDamage()
        {
            Health -= 50;
            if (Health > 0)
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

        public void Knock()
        {
            if (m_health <= 50)
            {
                if (GameSceneManager.Instance.PlayerController.ConsumeStone())
                {
                    Health += 50;
                }
            }
            else
            {
                Health -= 50;
                GameSceneManager.Instance.ThrowBait(transform.position);
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