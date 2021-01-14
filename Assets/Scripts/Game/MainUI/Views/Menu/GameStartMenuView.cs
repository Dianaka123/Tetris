using Game.Core.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainUI.Views.Menu
{
    public class GameStartMenuView : GameMenuView
    {
        [SerializeField] private Button _startGame;

        [Inject] public GameStartSignal StartSignal { get; set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            _startGame.onClick.AddListener(DeactivateMenu);
        }

        protected override void OnDisable()
        {
            base.OnEnable();
            StartSignal.Dispatch();
            _startGame.onClick.RemoveListener(DeactivateMenu);
        }
    }
}