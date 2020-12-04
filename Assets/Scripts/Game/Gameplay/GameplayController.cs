using System.Threading.Tasks;
using Game.Core.Common;
using Game.Core.Interfaces;
using Game.Gameplay.Interfaces;
using Infra.Controllers.Core;
using UnityEngine;

namespace Game.Gameplay
{
    public class GameplayController: ControllerWithResultBase
    {
        private readonly ISpawnManager _spawnManager;
        private readonly IStatisticsManager _statisticsManager;
        private readonly ISoundManager _soundManager;

        public GameplayController(
            ISpawnManager spawnManager,
            IStatisticsManager statisticsManager,
            ISoundManager soundManager,
            IControllerFactory controllerFactory) : base(controllerFactory)
        {
            _spawnManager = spawnManager;
            _statisticsManager = statisticsManager;
            _soundManager = soundManager;
        }

        protected override Task OnStartAsync()
        {
            _soundManager.PlaybackSound(SoundType.Background);
            
            _statisticsManager.Level = 1;
            _statisticsManager.Score = 0;
            _statisticsManager.LinesCount = 0;

            _spawnManager.Initialize();
            _spawnManager.Spawn();

            return Task.CompletedTask;
        }

        protected override Task OnStopAsync()
        {
            _soundManager.Stop();
            return Task.CompletedTask;
        }
        

    }
}