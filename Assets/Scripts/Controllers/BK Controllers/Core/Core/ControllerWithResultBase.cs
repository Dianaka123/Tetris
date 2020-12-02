using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infra.Controllers.Core
{
    /// <summary>
    ///     Use extension method to create and await controller result.
    /// </summary>
    /// <typeparam name="T">Result type</typeparam>
    public abstract class ControllerWithResultBase<T> : ControllerBase
    {
        private readonly TaskCompletionSource<T> _resultSource = new TaskCompletionSource<T>();

        protected ControllerWithResultBase(IControllerFactory controllerFactory)
            : base(controllerFactory)
        {
        }

        internal async Task<T> GetResult(CancellationToken token)
        {
            using (token.Register(Cancel, true))
            {
                return await _resultSource.Task;
            }
        }

        protected void Complete(T result)
        {
            _resultSource.TrySetResult(result);
        }

        protected void Cancel()
        {
            _resultSource.TrySetCanceled();
        }

        protected void Fail(Exception e)
        {
            _resultSource.TrySetException(e);
        }
    }

    public abstract class ControllerWithResultBase : ControllerWithResultBase<ControllerResult>
    {
        protected ControllerWithResultBase(IControllerFactory controllerFactory)
            : base(controllerFactory)
        {
        }
    }
}