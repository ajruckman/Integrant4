using System.Collections.Generic;
using Integrant4.API;

namespace Integrant4.Structurant
{
    public class Validation : IValidation
    {
        public Validation(ValidationResultType resultType, Content message)
        {
            ResultType = resultType;
            Message    = message;
        }

        public ValidationResultType ResultType { get; }
        public Content              Message    { get; }
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

        public IReadOnlyList<IValidation> OverallValidations { get; }
        public IReadOnlyDictionary<string, IReadOnlyList<IValidation>> MemberValidations  { get; }
    }
}