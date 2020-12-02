using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infra.Controllers.Core
{
    public class ViewContext : IViewContext
    {
        private static int _idCounter;
        private readonly CancellationTokenSource _disposeCts;
        private readonly SemaphoreSlim _semaphore;
        private Action _disposeCallback;

        public ViewContext(Action disposeCallback = null)
        {
            Id = GetNextId();
            _disposeCallback = disposeCallback;
            _disposeCts = new CancellationTokenSource();
            _semaphore = new SemaphoreSlim(1);
        }

        public int Id { get; }

        public async Task ShowModalAsync(Func<Task> action, CancellationToken cancellationToken = default)
        {
            if (_disposeCts.IsCancellationRequested) throw new ObjectDisposedException("ViewContext");

            var linkedTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(_disposeCts.Token, cancellationToken);
            try
            {
                await _semaphore.WaitAsync(linkedTokenSource.Token);
                await action();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<IViewContext> LockAsync(CancellationToken cancellationToken = default)
        {
            if (_disposeCts.IsCancellationRequested) throw new ObjectDisposedException("ViewContext");

            var linkedTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(_disposeCts.Token, cancellationToken);
            await _semaphore.WaitAsync(linkedTokenSource.Token);

            return new ViewContext(() => _semaphore.Release());
        }

        public IViewContext CreateChild()
        {
            if (_disposeCts.IsCancellationRequested) throw new ObjectDisposedException("ViewContext");

            return new ViewContext();
        }

        public void Dispose()
        {
            if (!_disposeCts.IsCancellationRequested)
            {
                if (_disposeCallback != null)
                {
                    _disposeCallback();
                    _disposeCallback = null;
                }

                _semaphore.Dispose();

                _disposeCts.Cancel();
                _disposeCts.Dispose();
            }
        }

        private static int GetNextId()
        {
            return Interlocked.Increment(ref _idCounter);
        }
    }
}