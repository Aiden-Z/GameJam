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
        public Transform BaitRoot;
        public Bait.Bait BaitTemplate;
        public State State;
        public GameHud GameHud;
        public Transform BaitHolder;
        public BaitFinder BaitFinder;
        public GroundFinder GroundFinder;
        public RectTransform PositionPointer;
        public RectTransform Canvas;
        private Collider2D m_collider2D;
        private Rigidbody2D m_rigidbody2D;
        Animator anim;
        public AudioSource charaudio;
        public AudioClip m_pick;
        public AudioClip m_throw;
        public AudioClip m_walk;
        public AudioClip m_fix;
        public Bait.Bait[] InitBaits;
        
        private Dictionary<Type, Queue<Collectable>> m_collected = new Dictionary<Type, Queue<Collectable>>
        {
            {typeof(Bait.Bait), new Queue<Collectable>()},
            {typeof(Stone), new Queue<Collectable>()}
        };

        private void Awake()
        {
            m_collider2D = GetComponent<Collider2D>();
            m_rigidbody2D = GetComponent<Rigidbody2D>();
            State = State.Idle;
            GameHud.SetBaitsNum(0);
            anim = GetComponent<Animator>();
            charaudio = GetComponent<AudioSource>();
            foreach (var initBait in InitBaits)
            {
                m_collected[typeof(Bait.Bait)].Enqueue(initBait);
                initBait.gameObject.SetActive(false);
                GameHud.SetBaitsNum(m_collected[typeof(Bait.Bait)].Count);
            }
        }

        public float CoolDown;
        public float MaxPressingTime;
        public float MaxThrowDistance;
        public float MinThrowDistance;
        private float m_timer;
        private float m_pressTimer;
        private Bait.Bait m_holdingBait;
        [SerializeField] GameObject rotate;

        private void FixedUpdate()
        {
            rotate.transform.rotation = Quaternion.Euler(0, 0, m_angle - 90);
            Canvas.transform.rotation = Quaternion.Euler(0, 0, m_angle - 90);
            if (m_direction.Equals(Vector2.zero))
            {
                if (m_angle >= -45 && m_angle < 0 || m_angle < 45 && m_angle >= 0)
                {
                    anim.SetInteger(Direction, 4);
                    GroundFinder.transform.position = new Vector3(0.7f, 0f) + transform.position;
                }
                else if (m_angle >= 45 && m_angle < 135)
                {
                    anim.SetInteger(Direction, 1);
                    GroundFinder.transform.position = new Vector3(0f, 0.7f) + transform.position;
                }
                else if (m_angle >= 135 && m_angle <= 180 || m_angle < -135 && m_angle >= -180)
                {
                    anim.SetInteger(Direction, 2);
                    GroundFinder.transform.position = new Vector3(-0.7f, 0f) + transform.position;
                }
                else
                {
                    anim.SetInteger(Direction, 3);
                    GroundFinder.transform.position = new Vector3(0f, 0.7f) + transform.position;
                }
            }
            else
            {
                var absX = Mathf.Abs(m_direction.x);
                var absY = Mathf.Abs(m_direction.y);
                if (m_direction.x < 0 && absX > absY)
                {
                    anim.SetInteger(Direction, 2);
                }
                else if (m_direction.x > 0 && absX > absY)
                {
                    anim.SetInteger(Direction, 4);
                }
                else if (m_direction.y > 0 && absY > absX)
                {
                    anim.SetInteger(Direction, 1);
                }
                else
                {
                    anim.SetInteger(Direction, 3);
                }
            }
            m_rigidbody2D.velocity = m_direction * Velocity + GameSceneManager.Instance.PlatformController.Rigidbody2D.velocity;
            m_timer += Time.fixedDeltaTime;
            switch (m_fireTriggerPhase)
            {
                case InputActionPhase.Performed:
                    m_pressTimer += Time.fixedDeltaTime;
                    if (State == State.Throwing)
                    {
                        charaudio.clip = m_throw;
                        charaudio.time = 0.2f;
                        charaudio.Play();


                        GameHud.Press(m_pressTimer, MaxPressingTime);
                        PositionPointer.gameObject.SetActive(true);
                        var progress = Mathf.Clamp01(m_pressTimer / MaxPressingTime);
                        var rad = (m_angle) * Mathf.Deg2Rad;
                        var cosRad = Mathf.Cos(rad);
                        var sinRad = Mathf.Sin(rad);
                        var maxY = MaxThrowDistance * sinRad;
                        var maxX = MaxThrowDistance * cosRad;
                        var minY = MinThrowDistance * sinRad;
                        var minX = MinThrowDistance * cosRad;
                        m_holdingBait.transform.position =
                            transform.position + (new Vector3(maxX, maxY) - new Vector3(minX, minY)) * progress;
                        PositionPointer.transform.position = m_holdingBait.transform.position;
                    }
                    break;
            }

            switch (State)
            {
                case State.Holding:
                    m_holdingBait.transform.position = BaitHolder.position;
                    break;
            }

            foreach (var pair in m_collected)
            {
                foreach (var collectable in pair.Value)
                {

                    if (!collectable.isActiveAndEnabled) continue;
                    var curPos = collectable.transform.position;
                    var toPos = transform.position;
                    var x = Mathf.Lerp(curPos.x, toPos.x, 0.15f);
                    var y = Mathf.Lerp(curPos.y, toPos.y, 0.15f);
                    collectable.transform.position = new Vector3(x, y);
                    if (Vector3.Distance(collectable.transform.position, transform.position) < 0.15f)
                    {
                        collectable.gameObject.SetActive(false);
                        charaudio.clip = m_pick;
                        charaudio.time = 0.2f;
                        charaudio.Play();
                    }
                }
            }
        }

        private Vector2 m_direction;
        public void OnMove(InputAction.CallbackContext callbackContext)
        {
            m_direction = callbackContext.ReadValue<Vector2>();
            charaudio.clip = m_walk;
            charaudio.Play();
        }

        public void Collect(Collectable collectable)
        {
            m_collected[typeof(Bait.Bait)].Enqueue(collectable);
            
            GameHud.SetBaitsNum(m_collected[typeof(Bait.Bait)].Count);
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
                        if (m_collected[typeof(Bait.Bait)].Count == 0)
                        {
#if DEBUG && UNITY_EDITOR
                            bait = Instantiate(BaitTemplate, BaitRoot);
#else
                            break;
#endif
                        }
                        else
                        {
                            bait = (Bait.Bait) m_collected[typeof(Bait.Bait)].Dequeue();
                            GameHud.SetBaitsNum(m_collected[typeof(Bait.Bait)].Count);
                        }
                        
                        State = State.Holding;
                        bait.transform.position = BaitHolder.position;
                        bait.gameObject.layer = LayerMask.NameToLayer("Default");
                        bait.gameObject.SetActive(true);
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
                        PositionPointer.gameObject.SetActive(false);
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
        private static readonly int Direction = Animator.StringToHash("Direction");

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
  

        public void OnKnock(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.phase == InputActionPhase.Started)
            {
                var ground = GroundFinder.GetGround(GroundFinder.transform.position);
                ground.Knock();
                Debug.Log($"敲击{ground.name}");
                charaudio.clip = m_fix;
                charaudio.Play();
               

            }
        }

        public void OnHurt()
        {
            
        }

        public void AddStone(Collectable stone)
        {
            m_collected[typeof(Stone)].Enqueue(stone);
            GameHud.SetStoneNum(m_collected[typeof(Stone)].Count);
        }

        public bool ConsumeStone()
        {
            var queue = m_collected[typeof(Stone)];
            if (queue.Count != 0)
            {
                var stone = queue.Dequeue();
                GameHud.SetStoneNum(m_collected[typeof(Stone)].Count);
                Destroy(stone.gameObject);
                return true;
            }

            return false;
        }
    }

    public enum State
    {
        Idle = 0,
        Holding,
        Throwing
    }
}