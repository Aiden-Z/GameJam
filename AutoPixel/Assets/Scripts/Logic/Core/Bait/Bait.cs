using System;
using Logic.Core.Controller.PlatformController;
using UnityEngine;

namespace Logic.Core.Bait
{
    public class Bait : MonoBehaviour
    {
        public float Lifetime;
        public float FlyingTime;
        public SpriteRenderer SpriteRenderer;
        public SpriteRenderer ShowingSprite;

        public PlatformController Controller;

        private float m_dyingTimer;

        private BaitState State;
        private void Awake()
        {
            State = BaitState.Idle;
        }

        public void SetController(PlatformController controller)
        {
            Controller = controller;
        }

        private Transform m_relativeTransform;
        public void Aim()
        {
            m_dyingTimer = 0;
            State = BaitState.Aiming;
            ShowingSprite.gameObject.SetActive(false);
        }

        public void Grab(Transform parent)
        {
            m_relativeTransform = parent;
        }

        public void Drop()
        {
            State = BaitState.Idle;
        }

        private Vector3 m_from;
        public void Throw(PlayerController.PlayerController thrower)
        {
            m_from = thrower.transform.position;
            State = BaitState.Flying;
            ShowingSprite.gameObject.SetActive(true);
            ShowingSprite.transform.position = m_from;
            m_direction = transform.position - m_from;
        }

        private float m_flyingTimer;
        private Vector3 m_direction;
        private void FixedUpdate()
        {
            var color = SpriteRenderer.color;
            switch (State)
            {
                case BaitState.Idle:
                    if (m_relativeTransform != null)
                    {
                        transform.position = m_relativeTransform.position;
                    }
                    break;
                case BaitState.Aiming:
                    SpriteRenderer.color = new Color(color.r, color.g, color.b, 0.5f);
                    break;
                case BaitState.Flying:
                    m_flyingTimer += Time.fixedDeltaTime;
                    SpriteRenderer.color = new Color(color.r, color.g, color.b, 0.0f);
                    ShowingSprite.transform.position = m_from + m_direction * (m_flyingTimer / FlyingTime);
                    if (m_flyingTimer >= FlyingTime)
                    {
                        State = BaitState.Dying;
                        Controller.SetTarget(transform);
                    }
                    break;
                case BaitState.Dying:
                    m_dyingTimer += Time.fixedDeltaTime;
                    if (m_dyingTimer >= Lifetime)
                    {
                        Destroy(gameObject);
                    }
                    break;
            }
        }
    }

    public enum BaitState
    {
        Idle,
        Aiming,
        Flying,
        Dying
    }
}