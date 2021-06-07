using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Integrant4.API;

namespace Integrant4.Fundament
{
    public class FieldValidationState : ValidationState
    {
        private readonly List<Func<IReadOnlyList<IValidation>>>           _overallValidators = new();
        private readonly List<(string, Func<IReadOnlyList<IValidation>>)> _memberValidators  = new();

        public void AddOverall(Func<IReadOnlyList<IValidation>> validator)
        {
            _overallValidators.Add(validator);
        }

        public void AddField(string id, Func<IReadOnlyList<IValidation>> validator)
        {
            _memberValidators.Add((id, validator.Invoke));
        }

        public void Validate()
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

                IReadOnlyList<IValidation> overallValidations = _overallValidators.SelectMany(v => v.Invoke()).ToList();

                //

                token.ThrowIfCancellationRequested();

                Dictionary<string, IReadOnlyList<IValidation>> memberValidations = new();

                foreach ((string id, Func<IReadOnlyList<IValidation>> validator) in _memberValidators)
                {
                    IReadOnlyList<IValidation> validations = validator.Invoke();

                    memberValidations[id] = validations;
                }

                //

                Result = new ValidationSet(overallValidations, memberValidations);
                IsBusy = false;
                InvokeOnChange();
                InvokeOnFinishValidating(Result);
            }, token);
        }
    }
}