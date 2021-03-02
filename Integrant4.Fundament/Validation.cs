using System.Collections.Generic;

namespace Integrant4.Fundament
{
    public class Validation
    {
        public readonly ValidationResultType ResultType;
        public readonly Content              Message;

        public Validation(ValidationResultType resultType, string message)
        {
            ResultType = resultType;
            Message    = message;
        }

        public static List<Validation> One(ValidationResultType resultType, string message) =>
            new() {new Validation(resultType, message)};
    }

    public interface IValidation
    {
        ValidationResultType ResultType { get; }
        Content              Message    { get; }
    }

    public interface IValidationSet
    {
        public IReadOnlyList<IValidation>                              OverallValidations { get; }
        public IReadOnlyDictionary<string, IReadOnlyList<IValidation>> MemberValidations  { get; }
    }

   
    public enum ValidationResultType
    {
        Undefined,
        Invalid,
        Warning,
        Valid,
    }
}