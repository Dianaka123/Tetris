using Game.Core.Common;
using Game.Core.Interfaces;
using Game.MainUI.Views.Views;
using strange.extensions.mediation.impl;

namespace Game.MainUI.Mediators
{
    public class HUDMediator: Mediator
    {
        [Inject] 
        public IStatisticsManager StatisticsManager { get; set; }

        [Inject] 
        public ScoreHudView ScoreView { get; set; }
        
        [Inject] 
        public ShapeVerticalMoveSignal ShapeVerticalMoveSignal { get; set; }

        [Inject] 
        public ScoreChangedSignal ScoreChangedSignal { get; set; }
        
        public override void OnRegister()
        {
            base.OnRegister();
            
            ShapeVerticalMoveSignal.AddListener(ShapeMoveCallback);
            ScoreChangedSignal.AddListener(ScoreChangedCallback);
        }
        private void ShapeMoveCallback(int obj)
        {
            StatisticsManager.Score += obj;
        }

        public override void OnRemove()
        {
            base.OnRemove();
            ScoreChangedSignal.RemoveListener(ScoreChangedCallback);
        }
        
        private void ScoreChangedCallback(int v)
        {
            ScoreView.SetScore(v);
        }

    }
}