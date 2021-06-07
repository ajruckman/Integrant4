using System.Collections.Generic;
using Integrant4.API;

namespace Integrant4.Fundament
{
    public class Validation : IValidation
    {
        public Validation(ValidationResultType resultType, Content message)
        {
            ResultType = resultType;
            Message    = message;
        }

        public ValidationResultType ResultType { get; }
        public IRenderable          Message    { get; }

        public static IReadOnlyList<Validation> One(ValidationResultType resultType, Content message) =>
            new[] {new Validation(resultType, message)};
    }

    public class ValidationSet : IValidationSet
    {
        public ValidationSet
        (
            IReadOnlyList<IValidation>                              overallValidations,
            IReadOnlyDictionary<string, IReadOnlyList<IValidation>> memberValidations
        )
        {
            OverallValidations = overallValidations;
            MemberValidations  = memberValidations;
        }

        public IReadOnlyList<IValidation>                              OverallValidations { get; }
        public IReadOnlyDictionary<string, IReadOnlyList<IValidation>> MemberValidations  { get; }
    }
}