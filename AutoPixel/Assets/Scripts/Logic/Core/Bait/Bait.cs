using System;
using Logic.Core.Controller.PlatformController;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Logic.Core.Bait
{
    public class Bait : Collectable
    {
        public float Lifetime;
        public float FlyingTime;
        public float MaxRadius;
        public SpriteRenderer SpriteRenderer;
        public SpriteRenderer ShowingSprite;

        private float m_dyingTimer;

        private BaitState State;
        private void Awake()
        {
            State = BaitState.Idle;
        }

        public void Aim()
        {
            m_dyingTimer = 0;
            State = BaitState.Aiming;
            ShowingSprite.gameObject.SetActive(false);
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
                        GameSceneManager.Instance.PlatformController.SetTarget(transform);
                        GameSceneManager.Instance.AntManager.PushBait(this);
                    }
                    break;
                case BaitState.Dying:
                    m_dyingTimer += Time.fixedDeltaTime;
                    if (m_dyingTimer >= Lifetime)
                    {
                        GameSceneManager.Instance.PlatformController.RemoveTarget(transform);
                        Destroy(gameObject);
                    }
                    break;
            }
        }

        public float GetCurRadius()
        {
            return MaxRadius * m_dyingTimer / Lifetime;
        }

        public Vector3 GetRandomPoint()
        {
            var curRadius = GetCurRadius();
            var randomRad = Random.Range(0, Mathf.PI * 2);
            var x = Mathf.Cos(randomRad) * curRadius;
            var y = Mathf.Sin(randomRad) * curRadius;
            return new Vector3(x, y) + transform.position;
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