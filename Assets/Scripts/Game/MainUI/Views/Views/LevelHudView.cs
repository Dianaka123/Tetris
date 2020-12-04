
namespace Game.MainUI.Views.Views
{
    public class LevelHudView: TextViewBase
    {
        public void SetLevel(int level)
        {
            SetText($"{level}");
        }
    }
}