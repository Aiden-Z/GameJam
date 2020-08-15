using System;
using System.Collections.Generic;
using Logic.Core.Controller.BaitFinder;
using Logic.Core.Controller.GroundFinder;
using Logic.Core.Controller.PlatformController;
using Logic.UI;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Logic.Core.PlayerController
{
    public class PlayerController : MonoBehaviour
    {
        public float Velocity;
        public PlatformController PlatformController;
        public Transform BaitRoot;
        public Bait.Bait BaitTemplate;
        public State State;
        public GameHud GameHud;
        public Transform BaitHolder;
        public BaitFinder BaitFinder;
        public GroundFinder GroundFinder;
        private Collider2D m_collider2D;
        private Rigidbody2D m_rigidbody2D;
        
        private Queue<Bait.Bait> m_collectedBait = new Queue<Bait.Bait>();

        private void Awake()
        {
            m_collider2D = GetComponent<Collider2D>();
            m_rigidbody2D = GetComponent<Rigidbody2D>();
            State = State.Idle;
            GameHud.SetBaitsNum(0);
        }

        public float CoolDown;
        public float MaxPressingTime;
        public float MaxThrowDistance;
        public float MinThrowDistance;
        private float m_timer;
        private float m_pressTimer;
        private Bait.Bait m_holdingBait;
        private void FixedUpdate()
        {
            transform.rotation = Quaternion.Euler(0, 0, m_angle - 90);
            m_rigidbody2D.velocity = m_direction * Velocity;
            m_timer += Time.fixedDeltaTime;
            switch (m_fireTriggerPhase)
            {
                case InputActionPhase.Performed:
                    m_pressTimer += Time.fixedDeltaTime;
                    if (State == State.Throwing)
                    {
                        GameHud.Press(m_pressTimer, MaxPressingTime);
                        var progress = Mathf.Clamp01(m_pressTimer / MaxPressingTime);
                        var rad = (transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad;
                        var cosRad = Mathf.Cos(rad);
                        var sinRad = Mathf.Sin(rad);
                        var maxY = MaxThrowDistance * sinRad;
                        var maxX = MaxThrowDistance * cosRad;
                        var minY = MinThrowDistance * sinRad;
                        var minX = MinThrowDistance * cosRad;
                        m_holdingBait.transform.position =
                            transform.position + (new Vector3(maxX, maxY) - new Vector3(minX, minY)) * progress;
                    }
                    break;
            }

            switch (State)
            {
                case State.Holding:
                    m_holdingBait.transform.position = BaitHolder.position;
                    break;
            }
        }

        private Vector2 m_direction;
        public void OnMove(InputAction.CallbackContext callbackContext)
        {
            m_direction = callbackContext.ReadValue<Vector2>();
        }

        public void CollectBait(Bait.Bait bait)
        {
            m_collectedBait.Enqueue(bait);
            bait.gameObject.SetActive(false);
            GameHud.SetBaitsNum(m_collectedBait.Count);
        }

        private InputActionPhase m_fireTriggerPhase;
        public void OnFire(InputAction.CallbackContext callbackContext)
        {
            m_fireTriggerPhase = callbackContext.phase;
            switch (callbackContext.phase)
            {
                case InputActionPhase.Started:
                    if (State != State.Throwing && m_timer >= CoolDown)
                    {
                        Bait.Bait bait;
                        if (m_collectedBait.Count == 0)
                        {
#if DEBUG && UNITY_EDITOR
                            bait = Instantiate(BaitTemplate, BaitRoot);
#else
                            break;
#endif
                        }
                        else
                        {
                            bait = m_collectedBait.Dequeue();
                        }
                        
                        State = State.Holding;
                        bait.transform.position = BaitHolder.position;
                        bait.gameObject.layer = LayerMask.NameToLayer("Default");
                        bait.gameObject.SetActive(true);
                        bait.SetController(PlatformController);
                        m_holdingBait = bait;
                        State = State.Throwing;
                        m_holdingBait.Aim();
                        m_pressTimer = 0;
                    }
                    break;
                case InputActionPhase.Canceled:
                    if (State == State.Throwing)
                    {
                        State = State.Idle;
                        GameHud.Release();
                        m_holdingBait.Throw(this);
                        m_holdingBait = null;
                        m_timer = 0;
                    }
                    break;
                case InputActionPhase.Waiting:
                    break;
                case InputActionPhase.Disabled:
                    break;
            }
        }

        private int m_angle;
        public void OnLook(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.control.device == Pointer.current || callbackContext.control.device == Mouse.current)
            {
                var position = callbackContext.ReadValue<Vector2>();
                var curCamera = Camera.main;
                if (curCamera == null)
                {
                    Debug.LogWarning("场景相机获取为空");
                    return;
                }
                
                var playerCameraPos = (Vector2) curCamera.WorldToScreenPoint(transform.position);
                m_angle = (int) (Mathf.Rad2Deg * Mathf.Atan2(position.y - playerCameraPos.y, position.x - playerCameraPos.x));
                
            }
            else
            {
                var axis = callbackContext.ReadValue<Vector2>();
                m_angle = (int) (Mathf.Rad2Deg * Mathf.Atan2(axis.y , axis.x));
            }
        }
    }

    public enum State
    {
        Idle,
        Holding,
        Throwing
    }
}