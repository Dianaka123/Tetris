using System;
using System.Threading.Tasks;
using Infra.Controllers.Core;
using UnityEngine;

namespace Infra.Controllers
{
    public sealed class AppClearingController : ControllerWithResultBase
    {
        public AppClearingController(
            IControllerFactory controllerFactory)
            : base(controllerFactory)
        {
        }

        protected override void OnInitialize()
        {
        }

        protected override Task OnStartAsync()
        {
            StartClearing();
            return Task.CompletedTask;
        }

        protected override Task OnStopAsync()
        {
            return Task.CompletedTask;
        }

        protected override void OnDispose()
        {
        }

        private void StartClearing()
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Complete(ControllerResult.None);
        }
    }
}