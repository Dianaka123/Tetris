using System.Threading.Tasks;
using Infra.Controllers.Core;
using UnityAcademy.TreeOfControllersExample;
using UnityEngine;

namespace Controllers
{
    public class GameController: ControllerWithResultBase
    {
        private ISpawnManager _spawnManager;
        private IGridManager _gridManager;
        
        public GameController(ISpawnManager spawnManager,
            IGridManager gridManager,
            IControllerFactory controllerFactory) : base(controllerFactory)
        {
            _spawnManager = spawnManager;
            _gridManager = gridManager;
        }

        protected override Task OnStartAsync()
        {
            _spawnManager.GridManager = _gridManager;
            return Task.CompletedTask;
        }

        protected override Task OnStopAsync()
        {
            return Task.CompletedTask;
        }
    }
}