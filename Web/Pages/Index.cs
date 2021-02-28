using System;
using Integrant4.Element.Inputs;
using Integrant4.Structurant;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Web.Pages
{
    public partial class Index
    {
        [Inject] public IJSRuntime JSRuntime { get; set; } = null!;

        private Structure<Dog, DogState>         _structure         = null!;
        private StructureInstance<Dog, DogState> _structureInstance = null!;

        protected override void OnInitialized()
        {
            _structure = new Structure<Dog, DogState>(inst =>
                new Dog(inst.State.NameFirst!, inst.State.NameLast!, inst.State.Age!.Value));

            _structure.Register<string?>
            (
                nameof(Dog.NameFirst),
                inst => inst.StructureInstance.State.NameFirst,
                (inst, v) => inst.StructureInstance.State.NameFirst = v,
                inputGetter: inst => new TextInput
                (
                    inst.StructureInstance.JSRuntime!,
                    inst.Value(),
                    false,
                    false,
                    null
                )
            );

            _structure.Register<string?>
            (
                nameof(Dog.NameLast),
                inst => inst.StructureInstance.State.NameLast,
                (inst, v) => inst.StructureInstance.State.NameLast = v,
                inputGetter: inst => new TextInput
                (
                    inst.StructureInstance.JSRuntime!,
                    inst.Value(),
                    false,
                    false,
                    null
                )
            );

            _structure.Register<int?>
            (
                nameof(Dog.Age),
                inst => inst.StructureInstance.State.Age,
                (inst, v) => inst.StructureInstance.State.Age = v,
                inputGetter: inst => new IntegerInput
                (
                    inst.StructureInstance.JSRuntime!, inst.Value(), false, false, true
                )
            );

            _structureInstance = _structure.Instantiate(new DogState()
            {
                NameFirst = "Rosie",
                NameLast  = "Ruckman",
                Age       = 10,
            }, JSRuntime);

            _structureInstance.Construct();

            _structureInstance.OnMemberValueChange += (m, v) => Console.WriteLine($"{m.Definition.ID} -> {v}");
        }
    }

    public class Dog
    {
        public Dog(string nameFirst, string nameLast, int age)
        {
            NameFirst = nameFirst;
            NameLast  = nameLast;
            Age       = age;
        }

        public string NameFirst { get; }
        public string NameLast  { get; }
        public int    Age       { get; }
    }

    public class DogState
    {
        public string? NameFirst { get; set; }
        public string? NameLast  { get; set; }
        public int?    Age       { get; set; }
    }
}