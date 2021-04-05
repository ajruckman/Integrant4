using System;
using System.Collections.Generic;

namespace Integrant4.API
{
    public interface IValidation
    {
        ValidationResultType ResultType { get; }
        IRenderable          Message    { get; }
    }

    public interface IValidationSet
    {
        IReadOnlyList<IValidation>                              OverallValidations { get; }
        IReadOnlyDictionary<string, IReadOnlyList<IValidation>> MemberValidations  { get; }
    }

    public interface IValidationState
    {
        bool            IsBusy { get; }
        IValidationSet? Result { get; }
        event Action?   OnChange;

        event Action?                 OnInvalidation;
        event Action?                 OnBeginValidating;
        event Action<IValidationSet>? OnFinishValidating;
        bool                          IsValid();
    }

    public enum ValidationResultType
    {
        Undefined,
        Invalid,
        Warning,
        Valid,
    }
}