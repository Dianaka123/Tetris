using System;

namespace Infra.Controllers.Core
{
    internal class OperationResult<T> : IOperationResult<T>
    {
        private readonly OperationCanceledException _canceledException;
        private readonly T _result;

        public OperationResult(OperationCanceledException canceledException)
        {
            _canceledException = canceledException;
        }

        public OperationResult(T result)
        {
            _result = result;
            IsCompleted = true;
        }

        public T Result
        {
            get
            {
                if (!IsCompleted)
                    throw new InvalidOperationException("[OperationResult] Result is undefined in Cancelled result");

                return _result;
            }
        }

        public bool IsCancelled => !IsCompleted;

        public bool IsCompleted { get; }

        public void ThrowIfCancelled()
        {
            if (IsCancelled)
                throw _canceledException;
        }
    }

    public interface IOperationResult<out T> : IOperationResult
    {
        T Result { get; }
    }

    public interface IOperationResult
    {
        bool IsCancelled { get; }
        bool IsCompleted { get; }

        void ThrowIfCancelled();
    }
}