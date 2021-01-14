using System.Threading.Tasks;
using Game.Core.Common;
using Infra.Controllers.Core;

namespace Game.MainUI
{
    public class MainUIController: ControllerWithResultBase
    {
        [Inject] public GameStartMenuSignal StartMenuSignal { get; set; }

        public MainUIController(
            IControllerFactory controllerFactory) : base(controllerFactory)
        {
        }

        protected override Task OnStartAsync()
        {
            StartMenuSignal.Dispatch();
            
            return Task.CompletedTask;
        }

        protected override Task OnStopAsync()
        {
            return Task.CompletedTask;
        }
    }
}