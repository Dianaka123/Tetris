using System;
using Game;
using Models.Interfaces;

namespace UnityAcademy.TreeOfControllersExample
{
    public class StatisticsManager: IStatisticsManager
    {
        [Inject] 
        public ScoreChangedSignal ScoreChangedSignal { get; set; }

        [Inject] 
        public LinesCountChangedSignal LinesCountChangedSignal { get; set; }

        [Inject] 
        public LevelChangedSignal LevelChangedSignal { get; set; }

        private int score = 0;
        public int Score
        {
            get => score;
            set
            {
                score = Math.Max(value, 0);
                ScoreChangedSignal.Dispatch(score);
            }
        }
        
        private int linesCount = 0;
        public int LinesCount
        {
            get => linesCount;
            set
            {
                linesCount = Math.Max(value, 0);
                LinesCountChangedSignal.Dispatch(linesCount);
            }
        }

        private int level = 0;
        public int Level
        {
            get => level;
            set
            {
                level = Math.Max(value, 0);
                LevelChangedSignal.Dispatch(level);
            }
        }
    }
}