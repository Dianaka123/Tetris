using System;
using Infra.Controllers.Core;
using strange.extensions.injector.api;

namespace BoardKings.Core.Controllers
{
    public abstract class ControllerWithContext<T, TR> : ControllerWithResultBase<TR>
    {
        private T _args;
        private ControllerContext _context;

        public ControllerWithContext(IControllerFactory controllerFactory)
            : base(controllerFactory)
        {
        }

        protected sealed override void SetArg(object arg)
        {
            var tuple = (Tuple<ControllerContext, T>) arg;
            _context = tuple.Item1;
            _args = tuple.Item2;
        }

        protected override void OnInitialize()
        {
            Bind(_context.injectionBinder, _args);
        }

        protected override void OnDispose()
        {
            Unbind(_context.injectionBinder);
            _context.Dispose();
        }

        protected abstract void Bind(ICrossContextInjectionBinder injectionBinder, T args);

        /// <summary>
        ///     To unbind cross context entities
        /// </summary>
        /// <param name="injectionBinder">StrangeIoC's binder</param>
        protected virtual void Unbind(ICrossContextInjectionBinder injectionBinder)
        {
        }
    }

    public abstract class ControllerWithContext<T> : ControllerWithContext<T, ControllerResult>
    {
        public ControllerWithContext(IControllerFactory controllerFactory)
            : base(controllerFactory)
        {
        }
    }
}