using Game.Core.Common;
using Game.MainUI.Views.Views;
using strange.extensions.mediation.impl;

namespace Game.MainUI.Mediators
{
    public class LevelMediator: Mediator
    {
        [Inject] 
        public LevelHudView LevelView { get; set; }
        
        [Inject] 
        public LevelChangedSignal LevelChangedSignal { get; set; }
        
        public override void OnRegister()
        {
            base.OnRegister();
            LevelChangedSignal.AddListener(LevelChangedCallback);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            LevelChangedSignal.RemoveListener(LevelChangedCallback);
        }
        
        private void LevelChangedCallback(int obj)
        {
            LevelView.SetLevel(obj);
        }

    }
}