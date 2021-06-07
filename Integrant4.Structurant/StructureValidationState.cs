using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Fundament;

namespace Integrant4.Structurant
{
    public class StructureValidationState : ValidationState
    {
        internal void ValidateStructure<TStructure, TState>(StructureInstance<TStructure, TState> inst)
            where TStructure : class
            where TState : class
        {
            IsBusy = true;
            InvokeOnChange();
            InvokeOnBeginValidating();

            TokenSource?.Cancel();
            TokenSource = new CancellationTokenSource();
            CancellationToken token = TokenSource.Token;

            Task.Run(() =>
            {
                token.ThrowIfCancellationRequested();

                ValidationSet v = DoValidation(inst, token);

                lock (CacheLock)
                {
                    Result = v;
                }

                IsBusy = false;
                InvokeOnChange();
                InvokeOnFinishValidating(v);
            }, token);
        }

        private static ValidationSet DoValidation<TStructure, TState>
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
                structureValidations = structure.Definition.OverallValidationGetter.Invoke(structure);
            }

            //

            token.ThrowIfCancellationRequested();

            Dictionary<string, IReadOnlyList<IValidation>> memberValidations = new();

            foreach (IMemberInstance<TStructure, TState> member in structure.MemberInstances)
            {
                IReadOnlyList<IValidation>? validations = member.Validations();
                if (validations == null) continue;
                memberValidations[member.Definition.ID] = validations;

                token.ThrowIfCancellationRequested();
            }

            //

            return new ValidationSet(structureValidations, memberValidations);
        }

        internal void Invalidate()
        {
            lock (CacheLock)
            {
                Result = null;
                InvokeOnChange();
                InvokeOnInvalidation();
            }
        }
    }
}