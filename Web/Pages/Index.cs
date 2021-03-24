using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Element;
using Integrant4.Element.Bits;
using Integrant4.Element.Constructs;
using Integrant4.Element.Inputs;
using Integrant4.Fundament;
using Integrant4.Structurant;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Web.Pages
{
    public partial class Index
    {
        private StructureInstance<Dog, DogState> _structureInstance = null!;

        private Header         _header                = null!;
        private ValidationView _overallValidationView = null!;
        private ValidationView _ageValidationView     = null!;

        [Inject] public IJSRuntime     JSRuntime      { get; set; } = null!;
        [Inject] public ElementService ElementService { get; set; } = null!;

        protected override void OnInitialized()
        {
            _structureInstance = Structure.Instantiate(new DogState()
            {
                NameFirst = "Annabell",
                NameLast  = "Annabell",
                Age       = -1,
                Breed     = "Rat Terrier",
            }, JSRuntime);

            _structureInstance.Construct();

            _structureInstance.OnMemberValueChange += (m, v) => Console.WriteLine($"{m.Definition.ID} -> {v}");

            _structureInstance.ValidationState.OnChange += () =>
            {
                // _overallValidationView.Update();
                // _ageValidationView.Update();
            };

            //

            _header = new Header(() => new IRenderable[]
            {
                new PageLink(() => "Secondary header".AsContent(),
                    new PageLink.Spec(() => "/elements") {IsTitle = Always.True}),
                new Filler(),
                new PageLink(() => "Normal link".AsContent(), new PageLink.Spec(() => "/elements")),
            }, Header.Style.Secondary);

            _overallValidationView = new ValidationView();
            _ageValidationView     = new ValidationView(nameof(DogState.Age));

            // _structureInstance.ValidationState.OnInvalidation += () => Console.WriteLine("Validation -> invalidation");
            // _structureInstance.ValidationState.OnBeginValidating += () => Console.WriteLine("Validation -> begin");
            // _structureInstance.ValidationState.OnFinishValidating += () => Console.WriteLine("Validation -> finished");

            // _structureInstance.ValidationState.OnFinishValidating += () => InvokeAsync(StateHasChanged);

            // _structureInstance.ValidationState.OnChange += () => InvokeAsync(StateHasChanged);

            _overallValidationView.SetState(_structureInstance.ValidationState);
            _ageValidationView.SetState(_structureInstance.ValidationState);
        }

        private async Task Reset()
        {
            await _structureInstance.ResetAllMemberInputValues();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await ElementService.ProcessJobs();
        }
    }

    public partial class Index
    {
        private static readonly Structure<Dog, DogState> Structure;

        private static Task<IReadOnlyList<IValidation>> DogValidations(StructureInstance<Dog, DogState> inst)
        {
            List<IValidation> result = new()
            {
                new Validation(ValidationResultType.Valid,   "Valid"),
                new Validation(ValidationResultType.Warning, "Warning"),
            };

            if (string.IsNullOrWhiteSpace(inst.State.NameFirst))
                result.Add(new Validation(ValidationResultType.Invalid, "First name is required"));

            if (string.IsNullOrWhiteSpace(inst.State.NameLast))
                result.Add(new Validation(ValidationResultType.Invalid, "Last name is required"));

            if (string.IsNullOrWhiteSpace(inst.State.Breed))
                result.Add(new Validation(ValidationResultType.Invalid, "Breed is required"));

            if (!string.IsNullOrWhiteSpace(inst.State.NameFirst) && !string.IsNullOrWhiteSpace(inst.State.NameLast))
                if (inst.State.NameFirst.Trim() == inst.State.NameLast.Trim())
                    result.Add(new Validation(ValidationResultType.Invalid, "First name cannot equal last name"));

            return Task.FromResult(result as IReadOnlyList<IValidation>);
        }

        static Index()
        {
            Structure = new Structure<Dog, DogState>
            (
                inst => Task.FromResult
                (
                    new Dog(inst.State.NameFirst!, inst.State.NameLast!, inst.State.Age!.Value, null)
                ),
                DogValidations);

            Structure.Register<string?>
            (
                nameof(Dog.NameFirst),
                inst => inst.StructureInstance.State.NameFirst,
                (inst, v) => inst.StructureInstance.State.NameFirst = v,
                inputGetter: inst => new TextInput
                (
                    inst.StructureInstance.JSRuntime!,
                    inst.Value()
                )
            );

            Structure.Register<string?>
            (
                nameof(Dog.NameLast),
                inst => inst.StructureInstance.State.NameLast,
                (inst, v) => inst.StructureInstance.State.NameLast = v,
                inputGetter: inst => new TextInput
                (
                    inst.StructureInstance.JSRuntime!,
                    inst.Value(),
                    new TextInput.Spec {IsDisabled = Always.True}
                )
            );

            Structure.Register<int?>
            (
                nameof(Dog.Age),
                inst => inst.StructureInstance.State.Age,
                (inst, v) => inst.StructureInstance.State.Age = v,
                inputGetter: inst => new IntegerInput
                (
                    inst.StructureInstance.JSRuntime!,
                    inst.Value(),
                    new IntegerInput.Spec {Consider0Null = Always.True, Width = () => 50}
                ),
                validationGetter: inst =>
                {
                    List<IValidation> result = new();

                    if (inst.StructureInstance.State.Age < 0)
                        result.Add(new Validation(ValidationResultType.Invalid, "Age must be at least 0"));

                    if (inst.StructureInstance.State.Age > 30)
                        result.Add(new Validation(ValidationResultType.Invalid, "Age must be at most 30"));

                    return Task.FromResult(result as IReadOnlyList<IValidation>);
                }
            );

            Structure.Register<string?>
            (
                nameof(Dog.Breed),
                inst => inst.StructureInstance.State.Breed,
                (inst, v) => inst.StructureInstance.State.Breed = v,
                inputGetter: inst => new SelectInput<string>
                (
                    inst.StructureInstance.JSRuntime!,
                    null,
                    new SelectInput<string>.Spec(() => new List<IOption<string>>
                        {
                            new Option<string>("Unknown",     "Unknown"),
                            new Option<string>("Rat Terrier", "Rat Terrier"),
                            new Option<string>("Boxer",       "Boxer"),
                            new Option<string>("Yorkie",      "Yorkie"),
                            new Option<string>("Chihuahua",   "Chihuahua"),
                            new Option<string>(null,          "Other"),
                        }
                    )
                )
            );
        }
    }

    public class Dog
    {
        public Dog(string nameFirst, string nameLast, int age, string? breed)
        {
            NameFirst = nameFirst;
            NameLast  = nameLast;
            Age       = age;
            Breed     = breed;
        }

        public string  NameFirst { get; }
        public string  NameLast  { get; }
        public int     Age       { get; }
        public string? Breed     { get; }
    }

    public class DogState
    {
        public string? NameFirst { get; set; }
        public string? NameLast  { get; set; }
        public int?    Age       { get; set; }
        public string? Breed     { get; set; }
    }
}