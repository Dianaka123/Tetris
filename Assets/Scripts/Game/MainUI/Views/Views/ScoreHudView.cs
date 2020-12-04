
namespace Game.MainUI.Views.Views
{
    public class ScoreHudView: TextViewBase
    {
        public void SetScore(int score)
        {
            SetText($"{score}");
        }
    }
}