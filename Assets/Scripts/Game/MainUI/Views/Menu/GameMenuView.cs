
using strange.extensions.mediation.impl;
using UnityEngine;

namespace Game.MainUI.Views.Menu
{
    public class GameMenuView: View
    {
        [SerializeField] private GameObject GameMenu;
        
        public void ActivateMenu()
        {
            GameMenu.SetActive(true);
        }

        public void DeactivateMenu()
        {
            GameMenu.SetActive(false);
        }
    }
}