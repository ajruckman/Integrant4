using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Integrant4.Fundament;

namespace Integrant4.Structurant
{
    public class ValidationState : IValidationState
    {
        private readonly object                   _cacheLock = new();
        private          CancellationTokenSource? _tokenSource;

        public bool            IsValidating { get; private set; }
        public IValidationSet? Result       { get; private set; }

        public bool IsValid()
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

                foreach ((_, IEnumerable<IValidation> l) in Result.MemberValidations)
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

        public event Action? OnChange;
        public event Action? OnInvalidation;
        public event Action? OnBeginValidating;
        public event Action? OnFinishValidating;

        internal void ValidateStructure<TStructure, TState>(StructureInstance<TStructure, TState> inst)
            where TStructure : class
            where TState : class
        {
            IsValidating = true;

            OnChange?.Invoke();
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
                OnChange?.Invoke();
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

            if (structure.Definition.OverallValidationGetter != null)
            {
                structureValidations = await structure.Definition.OverallValidationGetter.Invoke(structure);
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

        internal void Invalidate()
        {
            lock (_cacheLock)
            {
                Result = null;
                OnChange?.Invoke();
                OnInvalidation?.Invoke();
            }
        }
    }
}