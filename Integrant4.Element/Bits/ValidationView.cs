using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Integrant4.API;
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
        public class Spec : IUnifiedSpec
        {
            internal static readonly Spec Default = new();

            public StyleGetter? Style { get; init; }

            public Callbacks.IsVisible? IsVisible { get; init; }

            public SpecSet ToSpec() => new()
            {
                BaseClasses = new ClassSet("I4E-Bit", "I4E-Bit-" + nameof(ValidationView)),
                IsVisible   = IsVisible,
            };
        }
    }

    public partial class ValidationView
    {
        private readonly string? _memberID;
        private readonly object  _validationLock = new();

        private Action?                     _stateHasChanged;
        private IValidationState?           _lastState;
        private bool                        _isInProgress;
        private IReadOnlyList<IValidation>? _validations;

        public ValidationView(Spec? spec = null) : base(spec ?? Spec.Default)
        {
            _styleGetter = spec?.Style ?? DefaultStyleGetter;
        }

        public ValidationView(IValidationState state, Spec? spec = null) : this(spec)
        {
            AttachState(state);
        }

        public ValidationView(string memberID, Spec? spec = null) : this(spec)
        {
            _memberID = memberID;
        }

        public ValidationView(IValidationState state, string memberID, Spec? spec = null) : this(state, spec)
        {
            _memberID = memberID;
        }

        //

        private void SetStateHasChanged(Action stateHasChanged)
        {
            _stateHasChanged = stateHasChanged;
        }

        public void AttachState(IValidationState state)
        {
            if (_lastState != null)
            {
                _lastState.OnInvalidation     -= HandleInvalidation;
                _lastState.OnBeginValidating  -= HandleBeginValidating;
                _lastState.OnFinishValidating -= HandleFinishValidating;
            }

            state.OnInvalidation     += HandleInvalidation;
            state.OnBeginValidating  += HandleBeginValidating;
            state.OnFinishValidating += HandleFinishValidating;

            _lastState = state;
        }

        private void HandleInvalidation()
        {
            lock (_validationLock)
            {
                _validations  = null;
                _isInProgress = false;
                _stateHasChanged?.Invoke();
            }
        }

        private void HandleBeginValidating()
        {
            lock (_validationLock)
            {
                _isInProgress = true;
                _stateHasChanged?.Invoke();
            }
        }

        private void HandleFinishValidating(IValidationSet result)
        {
            lock (_validationLock)
            {
                _validations = _memberID == null
                    ? result.OverallValidations
                    : result.MemberValidations.ContainsKey(_memberID)
                        ? result.MemberValidations[_memberID]
                        : new List<IValidation>();
                _isInProgress = false;
                _stateHasChanged?.Invoke();
            }
        }

        private (bool IsInProgress, IReadOnlyList<IValidation>? Validations) Read()
        {
            lock (_validationLock)
            {
                return (_isInProgress, _validations);
            }
        }
    }

    public partial class ValidationView
    {
        private static readonly BootstrapIcon IconInvalid = new("x-circle-fill", 16);
        private static readonly BootstrapIcon IconWarning = new("exclamation-triangle-fill", 16);
        private static readonly BootstrapIcon IconValid   = new("check-circle-fill", 16);

        public override RenderFragment Renderer()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                int seq = -1;

                builder.OpenComponent<Component>(++seq);
                builder.AddAttribute(++seq, nameof(Component.ValidationView), this);
                builder.CloseComponent();
            }

            return Fragment;
        }
    }

    public partial class ValidationView
    {
        private sealed class Component : ComponentBase
        {
            [Parameter] public ValidationView ValidationView { get; set; } = null!;

            protected override void OnParametersSet()
            {
                ValidationView.SetStateHasChanged(() => InvokeAsync(StateHasChanged));
            }

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                int seq = -1;
                builder.OpenElement(++seq, "div");

                BitBuilder.ApplyOuterAttributes(ValidationView, builder, ref seq, new[]
                {
                    "I4E-Bit-ValidationView--" + ValidationView._styleGetter.Invoke(),
                });

                //

                (bool isInProgress, IReadOnlyList<IValidation>? validations) = ValidationView.Read();

                if (isInProgress)
                {
                    builder.OpenElement(++seq, "span");
                    builder.AddAttribute(++seq, "class", "I4E-Bit-Validation I4E-Bit-Validation--Validating");
                    builder.AddContent(++seq, "Validating...");
                    builder.CloseElement();
                }
                else if (validations == null) { }
                else
                {
                    foreach (IValidation validation in validations)
                    {
                        builder.OpenElement(++seq, "span");

                        builder.AddAttribute(++seq, "class",
                            "I4E-Bit-Validation I4E-Bit-Validation--" + validation.ResultType);

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