using System;
using System.Collections;
using Game.Core.Common;
using Game.Core.Interfaces;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace Game.Core
{
    public class TickManager: View
    {        
        private const float VerticalInterval = 0.5f;
        private const float VerticalIntervalStep = 0.05f;
        private const float FastVerticalDivider = 5f;
        private const float HorizontalInterval = 0.25f;

        [Inject] 
        public IStatisticsManager StatisticsManager { get; set; }

        [Inject] 
        public ShapeRotateSignal ShapeRotateSignal { get; set; }

        [Inject] 
        public ShapeVerticalMoveSignal ShapeVerticalMoveSignal { get; set; }
        
        [Inject]
        public ShapeHorizontalMoveSignal ShapeHorizontalMoveSignal { get; set; }

        public bool IsFreezed { get; set; } = false;

        private Coroutine _horizontalMovingCoroutine;

        protected override void Start()
        {
            base.Start();
            StartCoroutine(MoveVertical());
        }

        public void DispatchRotate(int v) => ShapeRotateSignal.Dispatch(1);
        
        public void DispatchHorizontalMove(int v) => ShapeHorizontalMoveSignal.Dispatch(v);
        
        public void DispatchVerticalMove(int v) => ShapeVerticalMoveSignal.Dispatch(v);

        private void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                _horizontalMovingCoroutine = _horizontalMovingCoroutine ?? StartCoroutine(MoveHorizontal());
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!IsFreezed) DispatchRotate(1);
            }
        }

        private IEnumerator MoveHorizontal()
        {
            int GetStep() =>
                (Input.GetKey(KeyCode.LeftArrow) ? -1 : 0) + 
                (Input.GetKey(KeyCode.RightArrow) ? 1 : 0);

            var step = GetStep();
            while (step != 0)
            {
                if (!IsFreezed) DispatchHorizontalMove(step);
                yield return new WaitForSeconds(HorizontalInterval);
                step = GetStep();
            }
            _horizontalMovingCoroutine = null;
        }

        private IEnumerator MoveVertical()
        {
            while (true)
            {
                var defaultInterval = Math.Max(VerticalInterval - StatisticsManager.Level * VerticalIntervalStep, VerticalIntervalStep);
                var interval = Input.GetKey(KeyCode.DownArrow) ? defaultInterval / FastVerticalDivider : defaultInterval;
                yield return new WaitForSeconds(interval);
                if (!IsFreezed) DispatchVerticalMove(1);
            }
        }
        
    }
}