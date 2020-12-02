using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Infra.Controllers.Events;

namespace Infra.Controllers.Core
{
    public abstract class ControllerBase
    {
        private readonly List<ControllerBase> _childControllers = new List<ControllerBase>();
        private CancellationToken _cancelToken;

        private CancellationTokenSource _lifetimeTokenSource;
        private CancellationTokenSource _stopTokenSource;

        protected ControllerBase(IControllerFactory controllerFactory)
        {
            ControllerFactory = controllerFactory;
            State = ControllerState.Created;
            ControllerName = GetType().Name;
            _cancelToken = CancellationToken.None;
        }

        public ControllerState State { get; private set; }

        public IViewContext ViewContext { get; private set; }

        public CancellationToken CancellationToken
        {
            get
            {
                switch (State)
                {
                    case ControllerState.Created:
                    case ControllerState.Initialized:
                    case ControllerState.Stopped:
                    case ControllerState.ChildsStopped:
                    case ControllerState.Disposed:
                        return _cancelToken;
                    case ControllerState.Running:
                        return _stopTokenSource.Token;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(State));
                }
            }
        }

        protected IControllerFactory ControllerFactory { get; }
        protected string ControllerName { get; }
        protected ControllerBase Parent { get; set; }

        public Task CreateAndStartAsync<T>(CancellationToken token)
            where T : ControllerBase
        {
            return CreateAndStartAsyncInternal<T>(token: token);
        }

        public Task CreateAndStartAsync<T>(object arg, CancellationToken token)
            where T : ControllerBase
        {
            return CreateAndStartAsyncInternal<T>(arg, token: token);
        }

        public Task CreateAndStartAsync<T>(object arg,
            IControllerFactory factory,
            CancellationToken token)
            where T : ControllerBase
        {
            return CreateAndStartAsyncInternal<T>(arg, factory, token);
        }

        public Task CreateAndStartAsync<T>(Type type, CancellationToken token)
            where T : ControllerBase
        {
            return CreateAndStartAsyncInternal<T>(type, token: token);
        }

        public Task CreateAndStartAsync<T>(Type type,
            object arg,
            CancellationToken token)
            where T : ControllerBase
        {
            return CreateAndStartAsyncInternal<T>(type, arg, token: token);
        }

        public Task CreateAndStartAsync<T>(Type type,
            object arg,
            IControllerFactory factory,
            CancellationToken token)
            where T : ControllerBase
        {
            return CreateAndStartAsyncInternal<T>(type, arg, factory, token);
        }

        internal Task<T> CreateAndStartAsyncInternal<T>(object arg = null,
            IControllerFactory factory = null,
            CancellationToken token = default,
            IViewContext viewContext = null)
            where T : ControllerBase
        {
            return CreateAndStartAsyncInternal<T>(typeof(T), arg, factory, token, viewContext);
        }

        internal async Task<T> CreateAndStartAsyncInternal<T>(Type type,
            object arg = null,
            IControllerFactory factory = null,
            CancellationToken token = default,
            IViewContext viewContext = null)
            where T : ControllerBase
        {
            _cancelToken.ThrowIfCancellationRequested();

            factory = factory ?? ControllerFactory;
            var controller = factory.Create<T>(type);
            controller.WithViewContext(viewContext ?? ViewContext);
            controller.Initialize(arg, token);

            AddChild(controller);

            try
            {
                await controller.StartAsync();
            }
            catch
            {
                await StopAndRemoveController(controller);
                throw;
            }

            return controller;
        }

        public void Initialize(object arg, CancellationToken token)
        {
            switch (State)
            {
                case ControllerState.Created:
                {
                    SetArg(arg);
                    OnInitialize();

                    _cancelToken = token;
                    State = ControllerState.Initialized;
                    break;
                }
                default:
                    throw new InvalidOperationException(
                        $"Initialized called from non-'Created' state. Current state: {State}");
            }
        }

        public async Task StartAsync()
        {
            switch (State)
            {
                case ControllerState.Initialized:
                {
                    _lifetimeTokenSource = new CancellationTokenSource();
                    _stopTokenSource =
                        CancellationTokenSource.CreateLinkedTokenSource(_lifetimeTokenSource.Token, _cancelToken);

                    State = ControllerState.Running;

                    await OnStartAsync();
                    break;
                }
                default:
                    throw new InvalidOperationException(
                        $"Controller should be initialized before adding. Current state: {State}");
            }
        }

        public async Task StopAsync()
        {
            switch (State)
            {
                case ControllerState.Running:
                {
                    var childStopList = new List<Task>();
                    foreach (var child in _childControllers.ToArray())
                        if (child.State == ControllerState.Running)
                            childStopList.Add(child.StopAsync());

                    await Task.WhenAll(childStopList);

                    _lifetimeTokenSource.Cancel();

                    State = ControllerState.ChildsStopped;

                    await OnStopAsync();
                    State = ControllerState.Stopped;
                    break;
                }
                default:
                    throw new InvalidOperationException($"Controller can not be stopped from current state: {State}");
            }
        }

        public void Dispose()
        {
            switch (State)
            {
                case ControllerState.Initialized:
                case ControllerState.Running:
                case ControllerState.ChildsStopped:
                case ControllerState.Stopped:
                {
                    _lifetimeTokenSource.Cancel();

                    foreach (var child in _childControllers.ToArray())
                        if (child.State != ControllerState.Disposed)
                            child.Dispose();

                    OnDispose();

                    _lifetimeTokenSource.Dispose();
                    _stopTokenSource.Dispose();
                    State = ControllerState.Disposed;
                    break;
                }
                default:
                    throw new InvalidOperationException($"Controller can not be disposed from current state: {State}");
            }
        }

        protected void WithViewContext(IViewContext viewContext)
        {
            ViewContext = viewContext;
        }

        protected void DispatchBubblingEvent(IEvent e)
        {
            if (!HandleEvent(e)) BubbleEvent(e);
        }

        internal async Task StopAndRemoveController(ControllerBase controller)
        {
            try
            {
                await controller.StopAsync();
            }
            finally
            {
                DisposeAndRemoveController(controller);
            }
        }

        internal void DisposeAndRemoveController(ControllerBase controller)
        {
            try
            {
                controller.Dispose();
            }
            finally
            {
                RemoveChild(controller);
            }
        }

        protected virtual void SetArg(object arg)
        {
        }

        protected virtual void OnInitialize()
        {
        }

        protected abstract Task OnStartAsync();

        protected abstract Task OnStopAsync();

        protected virtual void OnDispose()
        {
        }

        protected virtual bool HandleEvent(IEvent e)
        {
            return false;
        }

        private void AddChild(ControllerBase controller)
        {
            controller.Parent = this;
            _childControllers.Add(controller);
        }

        private void RemoveChild(ControllerBase controller)
        {
            controller.Parent = null;
            _childControllers.Remove(controller);
        }

        private void BubbleEvent(IEvent e)
        {
            if (Parent != null)
                // Parent is null in root controller
                Parent.DispatchBubblingEvent(e);
        }

        public override string ToString()
        {
            var info = GetShortControllerInfo();
            return string.IsNullOrEmpty(info)
                ? ControllerName
                : $"{ControllerName}: {info}";
        }

        protected virtual string GetShortControllerInfo()
        {
            return "";
        }

        public virtual string GetFullControllerInfo()
        {
            return "";
        }
    }
}