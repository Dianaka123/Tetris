
using strange.extensions.mediation.impl;
using UnityEngine;

namespace Game.MainUI.Views.Menu
{
    public class GameMenuView: View
    {
        [SerializeField] private GameObject GameMenu;
        
        public virtual void ActivateMenu()
        {
            GameMenu.SetActive(true);
        }

        public virtual void DeactivateMenu()
        {
            GameMenu.SetActive(false);
        }
    }
}