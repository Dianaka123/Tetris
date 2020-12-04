
namespace Game.MainUI.Views.Views
{
    public class LinesHudView: TextViewBase
    {
        public void SetLinesCount(int count)
        {
            SetText($"{count}");
        }
    }
}