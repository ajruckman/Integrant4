using System;
using System.Collections.Generic;
using System.Threading;
using Integrant4.API;

namespace Integrant4.Fundament
{
    public class ValidationState : IValidationState
    {
        protected readonly object                   CacheLock = new();
        protected          CancellationTokenSource? TokenSource;

        public bool            IsBusy { get; protected set; }
        public IValidationSet? Result { get; protected set; }

        public bool IsValid()
        {
            if (IsBusy) return false;

            lock (CacheLock)
            {
                if (Result == null) return false;

                foreach (IValidation v in Result.OverallValidations)
                    if (v.ResultType is ValidationResultType.Undefined or ValidationResultType.Invalid)
                        return false;

                foreach ((_, IEnumerable<IValidation> l) in Result.MemberValidations)
                foreach (IValidation v in l)
                    if (v.ResultType is ValidationResultType.Undefined or ValidationResultType.Invalid)
                        return false;

                return true;
            }
        }

        public event Action?                 OnChange;
        public event Action?                 OnInvalidation;
        public event Action?                 OnBeginValidating;
        public event Action<IValidationSet>? OnFinishValidating;

        protected void InvokeOnChange()                           => OnChange?.Invoke();
        protected void InvokeOnInvalidation()                     => OnInvalidation?.Invoke();
        protected void InvokeOnBeginValidating()                  => OnBeginValidating?.Invoke();
        protected void InvokeOnFinishValidating(IValidationSet v) => OnFinishValidating?.Invoke(v);
    }
}