using strange.extensions.mediation.impl;
using TMPro;
using UnityEngine;

namespace Game.MainUI.Views.Views
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextViewBase : View
    {
        private TextMeshProUGUI _textMeshProUGUI;

        protected override void Awake()
        {
            base.Awake();
            _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        }
        
        protected void SetText(string text)
        {
            _textMeshProUGUI.text = text;
        }
    }
}