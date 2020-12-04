using System;
using System.Threading.Tasks;
using Game.Gameplay.Interfaces;
using Infra.Controllers.Core;
using Models.Interfaces;
using UnityAcademy.TreeOfControllersExample;
using UnityEngine;
using Random = UnityEngine.Random;

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
            IShapeLoader shapeLoader,
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
            return Task.CompletedTask;
        }
    }
}