using System;
using System.Collections.Generic;

namespace Integrant4.Fundament
{
    // public class Validation
    // {
    //     public readonly ValidationResultType ResultType;
    //     public readonly Content              Message;
    //
    //     public Validation(ValidationResultType resultType, string message)
    //     {
    //         ResultType = resultType;
    //         Message    = message;
    //     }
    //
    //     public static List<Validation> One(ValidationResultType resultType, string message) =>
    //         new() {new Validation(resultType, message)};
    // }

    public interface IValidation
    {
        ValidationResultType ResultType { get; }
        Content              Message    { get; }
    }

    public interface IValidationSet
    {
        IReadOnlyList<IValidation>                              OverallValidations { get; }
        IReadOnlyDictionary<string, IReadOnlyList<IValidation>> MemberValidations  { get; }
    }

    // public readonly struct ValidationUpdate
    // {
    //     public ValidationUpdate(bool isValidating, IValidationSet? result)
    //     {
    //         if (isValidating && result != null)
    //             throw new ArgumentException();
    //         if (!isValidating && result == null)
    //             throw new ArgumentException();
    //
    //         IsValidating = isValidating;
    //         Result       = result;
    //     }
    //
    //     public readonly bool            IsValidating;
    //     public readonly IValidationSet? Result;
    // }

    public interface IValidationState
    {
        event Action? OnChange;

        event Action?                 OnInvalidation;
        event Action?                 OnBeginValidating;
        event Action<IValidationSet>? OnFinishValidating;

        bool            IsValidating { get; }
        IValidationSet? Result       { get; }
        bool            IsValid();
    }

    public enum ValidationResultType
    {
        Undefined,
        Invalid,
        Warning,
        Valid,
    }
}