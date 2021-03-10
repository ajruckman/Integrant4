using System;
using System.Collections.Generic;
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
using TextInput = Integrant4.Element.Inputs.TextInput;

namespace Web.Pages
{
    public partial class Index
    {
        private StructureInstance<Dog, DogState> _structureInstance = null!;

        private         Header     _header = null!;
        [Inject] public IJSRuntime JSRuntime { get; set; } = null!;

        protected override void OnInitialized()
        {
            _structureInstance = Structure.Instantiate(new DogState()
            {
                NameFirst = "Rosie",
                NameLast  = "Ruckman",
                Age       = 10,
            }, JSRuntime);

            _structureInstance.Construct();

            _structureInstance.OnMemberValueChange += (m, v) => Console.WriteLine($"{m.Definition.ID} -> {v}");

            //

            _header = new Header(() => new IRenderable[]
            {
                new PageLink(() => "Secondary header".AsContent(),
                    new PageLink.Spec(() => "/elements") {IsTitle = Always.True}),
                new Filler(),
                new PageLink(() => "Normal link".AsContent(), new PageLink.Spec(() => "/elements")),
            }, Header.Style.Secondary);
        }

        private async Task Reset()
        {
            _structureInstance.ResetAllMemberInputValues();
        }
    }

    public partial class Index
    {
        private static readonly Structure<Dog, DogState> Structure;

        static Index()
        {
            Structure = new Structure<Dog, DogState>(inst =>
                new Dog(inst.State.NameFirst!, inst.State.NameLast!, inst.State.Age!.Value, null));

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
                )
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
                            new Option<string>("Unknown", "Unknown"),
                            new Option<string>("Boxer", "Boxer"),
                            new Option<string>("Yorkie", "Yorkie"),
                            new Option<string>("Chihuahua", "Chihuahua"),
                            new Option<string>(null, "Other"),
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