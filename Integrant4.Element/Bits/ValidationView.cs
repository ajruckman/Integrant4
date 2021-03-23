using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    public partial class ValidationView : BitBase
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec
        {
            public StyleGetter? Style { get; init; }

            public Callbacks.IsVisible? IsVisible { get; init; }

            internal BaseSpec ToBaseSpec() => new()
            {
                IsVisible = IsVisible,
            };
        }
    }

    public partial class ValidationView
    {
        private readonly Callbacks.Callback<IValidationState> _validationState;
        private readonly string?                              _memberID;

        private event Action? OnChange;

        public ValidationView
        (
            Callbacks.Callback<IValidationState> validationState,
            Spec?                                spec = null
        )
            : base(spec?.ToBaseSpec(), new ClassSet("I4E-Bit", "I4E-Bit-" + nameof(ValidationView)))
        {
            _validationState = validationState;
            // IValidationState state = validationState.Invoke();
            // Subscribe(state);
            //
            // _validations = () => state.Result?.OverallValidations;
            _styleGetter = spec?.Style ?? DefaultStyleGetter;
        }

        public ValidationView
        (
            Callbacks.Callback<IValidationState> validationState,
            string                               memberID,
            Spec?                                spec = null
        )
            : base(spec?.ToBaseSpec(), new ClassSet("I4E-Bit", "I4E-Bit-" + nameof(ValidationView)))
        {
            _validationState = validationState;
            _memberID        = memberID;
            // IValidationState state = validationState.Invoke();
            // Subscribe(state);
            //
            // _validations = () => state.Result?.MemberValidations[memberID];
            _styleGetter = spec?.Style ?? DefaultStyleGetter;
        }

        private (bool IsValidating, IReadOnlyList<IValidation>? Validations) ReadState()
        {
            IValidationState state = _validationState.Invoke();

            return state.IsValidating || state.Result == null
                ? (true, null)
                : _memberID == null
                    ? (false, state.Result.OverallValidations)
                    : (false, state.Result.MemberValidations[_memberID]);
        }
    }

    public partial class ValidationView
    {
        private static readonly BootstrapIcon IconInvalid = new("x-circle-fill", 16);
        private static readonly BootstrapIcon IconWarning = new("exclamation-triangle-fill", 16);
        private static readonly BootstrapIcon IconValid   = new("check-circle-fill", 16);

        public override RenderFragment Renderer() => builder =>
        {
            builder.OpenComponent<Component>(0);
            builder.AddAttribute(1, "ValidationView", this);
            builder.CloseComponent();
        };
    }

    public partial class ValidationView
    {
        private class Component : ComponentBase
        {
            [Parameter] public ValidationView ValidationView { get; set; } = null!;

            protected override void OnParametersSet()
            {
                ValidationView.OnChange += () => InvokeAsync(StateHasChanged);
            }

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                int seq = -1;
                builder.OpenElement(++seq, "div");

                BitBuilder.ApplyAttributes(ValidationView, builder, ref seq, new string[]
                {
                    "I4E-Bit-ValidationView--" + ValidationView._styleGetter.Invoke(),
                }, null);

                //

                IReadOnlyList<IValidation> validations =
                    ValidationView._validations.Invoke() ?? Array.Empty<IValidation>();
                Console.WriteLine(validations.Count);
                foreach (IValidation validation in validations)
                {
                    builder.OpenElement(++seq, "span");

                    builder.AddAttribute(++seq, "class",
                        "I4E-Bit-ValidationView-Validation I4E-Bit-ValidationView-Validation--" +
                        validation.ResultType);

                    // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                    switch (validation.ResultType)
                    {
                        case ValidationResultType.Invalid:
                            builder.AddContent(++seq, IconInvalid.Renderer());
                            break;
                        case ValidationResultType.Warning:
                            builder.AddContent(++seq, IconWarning.Renderer());
                            break;
                        case ValidationResultType.Valid:
                            builder.AddContent(++seq, IconValid.Renderer());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    builder.AddContent(++seq, validation.Message.Renderer());

                    builder.CloseElement();
                }

                //

                builder.CloseElement();
            }
        }
    }

    public partial class ValidationView
    {
        public delegate Style StyleGetter();

        public enum Style
        {
            Inline,
            Block,
        }

        private readonly StyleGetter _styleGetter;

        private static StyleGetter DefaultStyleGetter => () => Style.Inline;
    }
}