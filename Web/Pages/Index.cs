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
            _structure = new(inst =>
            {
                string nameFirst = inst.GetTyped<string>(nameof(Dog.NameFirst))!.Value;
                string nameLast  = inst.GetTyped<string>(nameof(Dog.NameLast))!.Value;
                int    age       = inst.GetTyped<int>(nameof(Dog.Age))!.Value;

                return new Dog(nameFirst, nameLast, age);
            });

            _structure.Register<string>(nameof(Dog.NameFirst),
                inputCallback: inst => new TextInput
                (
                    inst.StructureInstance.JSRuntime!,
                    inst.Value,
                    false,
                    false,
                    null
                ));
            _structure.Register<string>(nameof(Dog.NameLast));
            _structure.Register<int>(nameof(Dog.Age));

            _structureInstance = _structure.Instantiate(new DogState(), JSRuntime);

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
    }
}