using System;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Logic.UI
{
    public class GameHud : MonoBehaviour
    {
        public Image ThrowProgress;
        public Text BaitsNum;
        public Transform Pointer;
        public Text StoneNum;

        public void Press(float curTime, float maxTime)
        {
            ThrowProgress.gameObject.SetActive(true);
            ThrowProgress.fillAmount = curTime / maxTime;
        }

        private void FixedUpdate()
        {
            var dir = (GameSceneManager.Instance.PlayerController.transform.position -
             GameSceneManager.Instance.FinishPoint.transform.position).normalized;
            Pointer.rotation = Quaternion.Euler(new Vector3(0, 0, (int) (Mathf.Rad2Deg * Mathf.Atan2(dir.y , dir.x)) - 90));
        }

        private void Awake()
        {
            BaitsNum.text = 0.ToString();
            StoneNum.text = 0.ToString();
        }

        public void Release()
        {
            ThrowProgress.gameObject.SetActive(false);
        }

        public void SetBaitsNum(int num)
        {
            BaitsNum.text = $"{num}";
        }

        public void SetStoneNum(int num)
        {
            StoneNum.text = $"{num}";
        }
    }
}