using System;
using System.Collections;
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
            set
            {
                if (m_health <= 0 && value > 0)
                {
                    SpriteRenderer.enabled = true;
                    gameObject.layer = LayerMask.NameToLayer("Ground");
                }
                else if (m_health > 0 && value <= 0)
                {
                    OnDead();
                }

                m_health = value;
            }
            get => m_health;
        }

        [SerializeField]
        private int m_health = 100;
        private float m_timer;
        private bool m_isAcidAffect = false;
        public Sprite[] Normal;
        public Sprite[] Toxic;
        public float AcidDuration;
        public SpriteRenderer SpriteRenderer;
        public Collider2D Collider2D;
        private static readonly int Health1 = Animator.StringToHash("Health");
        private static readonly int AcidAffect = Animator.StringToHash("AcidAffect");
        public float shaket;
        public float shakem;
        public int ThrowNum = 2;
        public bool IsAlive => Health > 0;


        private void FixedUpdate()
        {
            if (m_isAcidAffect)
            {
                m_timer += Time.fixedDeltaTime;
                if (m_timer >= AcidDuration)
                {
                    m_isAcidAffect = false;
                }
            }

            if (m_health > 0)
            {
                if (m_isAcidAffect)
                {
                    var index = m_health / 25 - 1;
                    SpriteRenderer.sprite = Toxic[index];
                }
                else
                {
                    var index = m_health / 25 - 1;
                    SpriteRenderer.sprite = Normal[index];
                }
            }
        }

        public bool TimeDamage()
        {
            Health -= 50;
            if (Health > 0)
            {
                return false;
            }
            else
            {   
                OnDead();
                return true;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Block"))
            {
                Health -= 25;
                StartCoroutine(GameSceneManager.Instance.Camerashake.Shake(0.2f, 0.5f));
               
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
                GameSceneManager.Instance.ThrowBait(transform.position, ThrowNum);
                if (Health <= 0)
                {
                    OnDead();
                }
            }
        }

        public void OnCorrode()
        {
            Health -= 50;
            m_isAcidAffect = true;
            if (Health <= 0)
            {
                OnDead();
            }
        }
        
        public void OnDead()
        {
            Collider2D.gameObject.layer = LayerMask.NameToLayer("PlayerBlock");
            SpriteRenderer.enabled = false;
            m_health = 0;
            GameSceneManager.Instance.CheckDeath();
        }

        public enum GroundType
        {
            Edge,
            Inner
        }
    }
}