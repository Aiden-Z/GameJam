using System.Collections;
using System.Collections.Generic;
using Logic.Core;
using Logic.Core.Scenes;
using Logic.FSM;
using Logic.FSM.Player;
using Logic.Manager.AudioMgr;
using Logic.Manager.DataTableMgr;
using Logic.Manager.EventMgr;
using Logic.Manager.InputManager;
using Logic.Manager.PlayerManager;
using Logic.Manager.ProgressManager;
using Logic.Manager.SceneMgr;
using Logic.Map.LevelMap;
using Logic.Map.LevelMap.MapItemCommon.Component;
using Logic.Map.LevelMap.MapItem.MapItem;
using Render.LightEffector;
using ScriptableObjects.DataTable;
using UI.CommonUI;
using UnityEngine;

namespace Logic.Temp
{
    public class TempPlayerController : MonoBehaviour
    {
        public float MoveSpeed;
        //public float ClimbingSpeed;
        public float JumpHeight;

        public LightDepresser lightDepresser;
        
        public CapsuleCollider2D Collider2D;
        public Rigidbody2D RigidBody2D;
        //public Rigidbody2D CurContactingRig;

        public PlayerStateMachine FSM;
        public Animator Animator;
        public GameObject LightGameObject;
        
        public bool IsOnGround;
        public bool CanClimb;

        protected int m_interactiveCoolDown;
        private HashSet<Collider2D> m_contactingLadders;
        private MapItemCombinerComponent m_currMapItemCombinerComp;

        private void Awake()
        {
            DataTableMgr.Instance.TryGetDataTableById<LevelDataTable, LevelData>(GameRoot.m_instance.m_levelId,
                out var levelDataTable);
            var posStrs = levelDataTable.BornPos.Split(',');
            var pos = new Vector2(float.Parse(posStrs[0]) * 0.001f, float.Parse(posStrs[1]) * 0.001f);
            
            lightDepresser.InitData(levelDataTable);
            
            transform.position = new Vector3(pos.x, pos.y, 0);
            PlayerManager.Instance.m_player = transform;
            
            Collider2D = GetComponentInChildren<CapsuleCollider2D>();
            RigidBody2D = GetComponent<Rigidbody2D>();

            m_interactiveCoolDown = 0;
                       
            m_contactingLadders = new HashSet<Collider2D>();
            m_currMapItemCombinerComp = null;

            if (!DataTableMgr.Instance.TryGetDataTableById<LevelDataTable, LevelData>(GameRoot.m_instance.m_levelId, out var outValue))
            {
                m_interactiveCoolDown = 1000;
            }
            else
            {
                m_interactiveCoolDown = outValue.InteractiveCoolDown;
            }

            // Init state machine.
            FSM = new PlayerStateMachine();
            FSM.PreInit(this, new StateMachineLayerInitializer());
            FSM.Reset();

            // Init input bindings.
            InputManager.Instance.AddCallback(KeyCode.Space, OnJump);
            InputManager.Instance.AddCallback(KeyCode.C, OnControl);
            InputManager.Instance.AddCallback(KeyCode.Q, test);
        }

        private void test()
        {
            MapLogicSharedDelegate.Instance.Invoke(1000, null, null);
        }
        
        private void Update()
        {
            FSM.Update();

            Animator.SetBool("IsWalking", FSM.GetCurrentStateID((int)EPlayerStateLayer.Action) == (int)EPlayerActionState.Walk);
            Animator.SetBool("IsOnGround", IsOnGround);
            Animator.SetBool("IsClimbing", FSM.GetCurrentStateID((int)EPlayerStateLayer.Action) == (int)EPlayerActionState.Climb);
            Animator.SetBool("IsControlling", FSM.GetCurrentStateID((int)EPlayerStateLayer.Action) == (int)EPlayerActionState.Control);
            Animator.SetBool("IsDead", FSM.GetCurrentStateID((int)EPlayerStateLayer.Action) == (int)EPlayerActionState.Dead);

            Vector3 localScale = Animator.transform.localScale;
            Animator.transform.localScale = new Vector3(FSM.m_isHeadingLeft ? 1.0f : -1.0f, localScale.y, localScale.z);
        }

        private void OnDestroy()
        {
            // Clear input bindings.
            InputManager.Instance.RemoveCallback(KeyCode.Space, OnJump);
            InputManager.Instance.RemoveCallback(KeyCode.C, OnControl);
            InputManager.Instance.RemoveCallback(KeyCode.Q, test);
        }

        private void OnGUI()
        {
#if UNITY_EDITOR
            GUI.Label(new Rect(200, 0, 220, 20), FSM.GetCurrentState((int)EPlayerStateLayer.Action).ToString());
#endif
        }

        public void OnLadderTriggerEnter(Collider2D other)
        {
            m_contactingLadders.Add(other);
            CanClimb = true;

            this.OnCombinerElemEnter(other);
        }

        public void OnLadderTriggerExit(Collider2D other)
        {
            m_contactingLadders.Remove(other);
            if (m_contactingLadders.Count == 0)
            {
                CanClimb = false;
            }

            this.OnCombinerElemExit(other);
        }

        public void OnGroundTriggerEnter(Collider2D other)
        {
            this.OnCombinerElemEnter(other);

            IsOnGround = true;
        }

        public void OnGroundTriggerStay(Collider2D other)
        {
            IsOnGround = true;
        }

        public void OnGroundTriggerExit(Collider2D other)
        {
            this.OnCombinerElemExit(other);

            if (m_currMapItemCombinerComp == null || m_contactingLadders.Count != 0)
            {
                IsOnGround = false;
            }
        }

        public void OnCombinerElemEnter(Collider2D other)
        {
            if (!other.attachedRigidbody)
            {
                return;
            }
            MapItemCombinerComponent newMapItemCombinerComp = other.attachedRigidbody.gameObject.GetComponent<MapItemCombinerComponent>();
            MapItemCombiner newMapItemCombiner = newMapItemCombinerComp.HostedItem as MapItemCombiner;
            newMapItemCombiner.IncElemsCountUnderPlayerFeet();
            Debug.Log("inc " + newMapItemCombiner.HashCode);

            if (m_currMapItemCombinerComp != newMapItemCombinerComp)
            {
                if (m_currMapItemCombinerComp != null)
                {
                    MapItemCombiner currMapItemCombiner = m_currMapItemCombinerComp.HostedItem as MapItemCombiner;
                    currMapItemCombiner.SetPower(false);
                }

                newMapItemCombiner.SetPower(true);
                m_currMapItemCombinerComp = newMapItemCombinerComp;
            }
        }

        public void OnCombinerElemExit(Collider2D other)
        {
            if (!other.attachedRigidbody)
            {
                return;
            }
            MapItemCombinerComponent mapItemCombinerComp = other.attachedRigidbody.gameObject.GetComponent<MapItemCombinerComponent>();
            MapItemCombiner mapItemCombiner = mapItemCombinerComp.HostedItem as MapItemCombiner;
            mapItemCombiner.DecElemsCountUnderPlayerFeet();
            Debug.Log("dec " + mapItemCombiner.HashCode);

            if (m_currMapItemCombinerComp == mapItemCombinerComp && mapItemCombiner.GetElemsCountUnderPlayerFeet() == 0)
            {
                mapItemCombiner.SetPower(false);
                m_currMapItemCombinerComp = null;
            }
        }

        public GameObject TempGetLadder()
        {
            foreach (var ladder in m_contactingLadders)
            {
                return ladder.gameObject;
            }
            return null;
        }

        /// <summary>
        /// 头部死亡触发器触发
        /// </summary>
        /// <param name="other"></param>
        public void OnDeathTriggerEnter(Collider2D other)
        {
            Debug.Log("DeathTrigger" + LayerMask.LayerToName(other.gameObject.layer));
#if __DEBUG__
            UnityEditor.EditorApplication.isPaused = true;
            return;
#endif
            OnDeath();
        }

        /// <summary>
        /// 掉出屏幕外
        /// </summary>
        /// <param name="other"></param>
        public void OnDeathTriggerExit(Collider2D other)
        {

#if __DEBUG__
	            if (!other.bounds.Intersects(Collider2D.bounds))
	            {
		            EditorApplication.isPaused = true;
	            }
            return;
#else
            OnDeath();
#endif
            
            
        }

        private void OnDeath()
        {
            GameRoot.m_instance.StartCoroutine(DepressBgm());

            FSM.TriggerEvent((int)EPlayerEvent.Die);

            if (MapLogic.m_instance.isOver) // 如果已经结束
            {
                return;
            }
            List<UiOptionPair> optionPairs = new List<UiOptionPair>();
            if (ProgressMgr.Instance.HasSavePoint())
            {
                optionPairs.Add(new UiOptionPair
                {
                    Callback = ProgressMgr.Instance.LoadProgress,
                    Title = "返回存档点"
                });
            }
            optionPairs.Add(new UiOptionPair
            {
                Callback = RestartLevel,
                Title = "重新开始本关"
            });
                
            optionPairs.Add(new UiOptionPair
            {
                Callback = ReturnToMainScene,
                Title = "主页面"
            });
            SceneUiBase.Instance.ShowOptionPanel(optionPairs, "...Oops");
            MapLogic.m_instance.SetAllowScroll(false);
            MapLogic.m_instance.isOver = true;
            
            AudioMgr.Instance.Play(AudioDefine.Death);
        }

        private void RestartLevel()
        {
            ProgressMgr.Instance.RestartLevel();
            GameRoot.m_instance.StartCoroutine(SceneMgr.Instance.SwitchScene(typeof(GameScene)));
        }

        private void ReturnToMainScene()
        {
            ProgressMgr.Instance.RestartLevel();
            GameRoot.m_instance.StartCoroutine(SceneMgr.Instance.SwitchScene(typeof(MainScene)));
        }

        private IEnumerator DepressBgm()
        {
            var start = 1f;
            while(start > 0)
            {
                AudioMgr.Instance.SetBgVolume(start);
                start -= Time.deltaTime;
                yield return null;
            }
            AudioMgr.Instance.SetBgVolume(0);

            yield return null;
        }

        public Rigidbody2D GetCurrMapItemCombinerRigidbody()
        {
            return m_currMapItemCombinerComp.Rigidbody2D;
        }

        public MapItemCombinerComponent GetCurrMapItemCombiner()
        {
            return m_currMapItemCombinerComp;
        }

        public bool GetCanInteractive()
        {
            if (m_currMapItemCombinerComp != null && m_currMapItemCombinerComp.HostedItem != null)
            {
                return m_currMapItemCombinerComp.HostedItem.IsInteractive;
            }
            return false;
        }

        public int GetInteractiveCoolDown()
        {
            return m_interactiveCoolDown;
        }

        // Event handlers
        public void OnJump()
        {
            FSM.TriggerEvent((int)EPlayerStateLayer.Action, (int)EPlayerEvent.Jump);
        }

        public void OnControl()
        {
            FSM.TriggerEvent((int)EPlayerStateLayer.Action, (int)EPlayerEvent.Control);
        }

        public bool IsControlling()
        {
            return FSM.GetCurrentState((int) EPlayerStateLayer.Action).GetID() == (int)EPlayerActionState.Control;
        }

        public void AddLight(float lightDelta)
        {
            lightDepresser.LightLevel += lightDelta;
        }

        /// <summary>
        /// 将光亮补充到
        /// </summary>
        /// <param name="lightDelta"></param>
        public void AddLightTo(float lightDelta)
        {
            if(lightDepresser.LightLevel < lightDelta)
            {
                lightDepresser.LightLevel = lightDelta;
            }
        }

        public float GetLightRatio()
        {
            return Mathf.Clamp01(lightDepresser.LightLevel / 100f);
        }
    }
}