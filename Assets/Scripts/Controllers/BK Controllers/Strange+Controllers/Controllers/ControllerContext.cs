using System;
using System.Threading;
using System.Threading.Tasks;
using Infra.Controllers.Core;
using strange.extensions.context.impl;
using strange.extensions.injector.api;

namespace BoardKings.Core.Controllers
{
    public class ControllerContext : CrossContext
    {
        private readonly Type _controllerType;

        public ControllerContext(Type controllerType)
            : base(null, false)
        {
            _controllerType = controllerType;
        }

        private IControllerFactory ControllerFactory { get; set; }

        protected override void addCoreComponents()
        {
            base.addCoreComponents();
            injectionBinder.Bind<IInjectionBinder>().ToValue(injectionBinder);
        }

        protected override void mapBindings()
        {
            base.mapBindings();

            injectionBinder.Bind<IControllerFactory>().To<ControllerFactory>().ToSingleton();
            injectionBinder.Bind(_controllerType).To(_controllerType);

            ControllerFactory = injectionBinder.GetInstance<IControllerFactory>();
        }

        public static Task<IOperationResult<TResult>> CreateAndStart<T, TArgs, TResult>(
            ControllerBase parent,
            TArgs bindingArgs,
            CancellationToken resultCancellationToken,
            CancellationToken controllerCancellationToken)
            where T : ControllerWithContext<TArgs, TResult>
        {
            var context = new ControllerContext(typeof(T));
            context.Start();
            var arg = new Tuple<ControllerContext, TArgs>(context, bindingArgs);
            return parent.CreateAndWaitResultAsync<T, TResult>(
                arg,
                context.ControllerFactory,
                resultCancellationToken,
                controllerCancellationToken);
        }

        public static Task<IOperationResult<TResult>> CreateAndStart<T, TArgs, TResult>(
            ControllerBase parent,
            TArgs bindingArgs,
            CancellationToken controllerCancellationToken)
            where T : ControllerWithContext<TArgs, TResult>
        {
            return CreateAndStart<T, TArgs, TResult>(
                parent,
                bindingArgs,
                CancellationToken.None,
                controllerCancellationToken);
        }

        public static Task<IOperationResult<ControllerResult>> CreateAndStart<T, TArgs>(
            ControllerBase parent,
            TArgs bindingArgs,
            CancellationToken controllerCancellationToken)
            where T : ControllerWithContext<TArgs>
        {
            return CreateAndStart<T, TArgs, ControllerResult>(
                parent,
                bindingArgs,
                controllerCancellationToken);
        }
    }
}