using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Integrant4.API;

namespace Integrant4.Structurant
{
    public class ValidationState
    {
        private readonly object                   _cacheLock = new();
        private          CancellationTokenSource? _tokenSource;

        public ValidationState()
        {
        }

        public ValidationSet? Result       { get; private set; }
        public bool           IsValidating { get; private set; }

        internal void ValidationStructure<TStructure, TState>(StructureInstance<TStructure, TState> inst)
            where TStructure : class
            where TState : class
        {
            IsValidating = true;

            OnBeginValidating?.Invoke();

            _tokenSource?.Cancel();
            _tokenSource = new CancellationTokenSource();
            CancellationToken token = _tokenSource.Token;

            Task.Run(async () =>
            {
                token.ThrowIfCancellationRequested();

                ValidationSet v = await DoValidation(inst, token);

                lock (_cacheLock)
                {
                    Result = v;
                }

                IsValidating = false;
                OnFinishValidating?.Invoke();
            }, token);
        }

        private static async Task<ValidationSet> DoValidation<TStructure, TState>
        (
            StructureInstance<TStructure, TState> structure,
            CancellationToken                     token
        )
            where TStructure : class
            where TState : class
        {
            token.ThrowIfCancellationRequested();

            IReadOnlyList<IValidation> structureValidations = new List<Validation>();

            if (structure.Definition.ValidationGetter != null)
            {
                structureValidations = await structure.Definition.ValidationGetter.Invoke(structure);
            }

            //

            token.ThrowIfCancellationRequested();

            Dictionary<string, IReadOnlyList<IValidation>> memberValidations = new();

            foreach (IMemberInstance<TStructure, TState> member in structure.MemberInstances)
            {
                IReadOnlyList<IValidation>? validations = await member.Validations();
                if (validations == null) continue;
                memberValidations[member.Definition.ID] = validations;

                token.ThrowIfCancellationRequested();
            }

            //

            return new ValidationSet(structureValidations, memberValidations);
        }

        public void Invalidate()
        {
            lock (_cacheLock)
            {
                Result = null;
                OnInvalidation?.Invoke();
            }
        }

        public bool Valid()
        {
            if (IsValidating) return false;

            lock (_cacheLock)
            {
                if (Result == null) return false;

                foreach (IValidation v in Result.OverallValidations)
                {
                    if (v.ResultType == ValidationResultType.Undefined ||
                        v.ResultType == ValidationResultType.Invalid)
                        return false;
                }

                foreach ((_, IReadOnlyList<IValidation> l) in Result.MemberValidations)
                {
                    foreach (IValidation v in l)
                    {
                        if (v.ResultType == ValidationResultType.Undefined ||
                            v.ResultType == ValidationResultType.Invalid)
                            return false;
                    }
                }

                return true;
            }
        }

        public event Action? OnInvalidation;
        public event Action? OnBeginValidating;
        public event Action? OnFinishValidating;
    }
}