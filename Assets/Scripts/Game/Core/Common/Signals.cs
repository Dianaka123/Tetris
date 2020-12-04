using strange.extensions.signal.impl;

namespace Game.Core.Common
{
    public class VerticalTickSignal : Signal
    {
    }

    public class HorizontalTickSignal : Signal
    {
    }

    public class ScoreChangedSignal : Signal<int>
    {
    }

    public class LevelChangedSignal : Signal<int>
    {
    }

    public class LinesCountChangedSignal : Signal<int>
    {
    }

    public class ShapeVerticalMoveSignal : Signal<int>
    {
    }

    public class VerticalIntervalChanged : Signal<int>
    {
    }

    public class HorizontalIntervalChanged : Signal<int>
    {
    }

    public class ShapeHorizontalMoveSignal : Signal<int>
    {
    }

    public class ShapeRotateSignal : Signal<int>
    {
    }

    public class LineFullSignal : Signal<int>
    {
    }
    
    public class GameStartSignal : Signal
    {
    }

    public class GameOverSignal : Signal
    {
    }
    
    public class GameStartMenuSignal : Signal
    {
    }
}