using System;
using System.Collections;
using Models.Interfaces;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace Game
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

        private Coroutine _horizontalMovingCoroutine;

        protected override void Start()
        {
            base.Start();
            StartCoroutine(MoveVertical());
        }

        private void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                _horizontalMovingCoroutine = _horizontalMovingCoroutine ?? StartCoroutine(MoveHorizontal());
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ShapeRotateSignal.Dispatch(1);
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
                ShapeHorizontalMoveSignal.Dispatch(step);
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
                ShapeVerticalMoveSignal.Dispatch(1);
            }
        }
        
    }
}