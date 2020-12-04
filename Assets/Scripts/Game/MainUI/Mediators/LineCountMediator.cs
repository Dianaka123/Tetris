using Game.Core.Common;
using Game.Core.Interfaces;
using Game.MainUI.Views.Views;
using strange.extensions.mediation.impl;

namespace Game.MainUI.Mediators
{
    public class LineCountMediator: Mediator
    {
        [Inject]
        public IStatisticsManager StatisticsManager { get; set; }

        [Inject] 
        public LinesHudView LinesView { get; set; }
        
        [Inject] 
        public LineFullSignal LineFullSignal { get; set; }

        [Inject] 
        public LinesCountChangedSignal LinesCountChangedSignal { get; set; }

        public override void OnRegister()
        {
            base.OnRegister();
            LineFullSignal.AddListener(LineFullCallback);
            LinesCountChangedSignal.AddListener(LineCountChangedCallback);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            LineFullSignal.AddListener(LineFullCallback);
            LinesCountChangedSignal.AddListener(LineCountChangedCallback);
        }
        
        private void LineCountChangedCallback(int obj)
        {
            LinesView.SetLinesCount(obj);
        }

        private void LineFullCallback(int obj)
        {
            StatisticsManager.LinesCount += obj;
        }

    }
}