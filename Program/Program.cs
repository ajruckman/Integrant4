using System.Threading.Tasks;
using Integrant4.Element.Bits;
using Integrant4.Resources.Icons;
using Integrant4.Structurant;

namespace Program
{
    internal static class Program
    {
        private static void Main()
        {
            new Button(() => new BootstrapIcon("arrow-right"));

            RunColorant.Run();
            return;

            // Expression<Func<Dog, int>> dogExpression;
            // Dog d = new();
            // dogExpression = _ => d.Age;
            // Console.WriteLine(dogExpression.Body);

            Structure<Dog, DogState> structure = new(inst =>
            {
                string nameFirst = inst.GetTyped<string>(nameof(Dog.NameFirst)).Value();
                string nameLast  = inst.GetTyped<string>(nameof(Dog.NameLast)).Value();
                int    age       = inst.GetTyped<int>(nameof(Dog.Age)).Value();

                return new Dog(nameFirst, nameLast, age);
            });

            // structure.Register<string>(new Member<Dog, DogState, string>
            //     (
            //     nameof(Dog.NameFirst),
            //     : inst => new TextInput
            //     (
            //         inst.StructureInstance.JSRuntime!,
            //         inst.Value,
            //         () => false,
            //         () => false
            //     ))
            // );
            // structure.Register<string>(nameof(Dog.NameLast));

            StructureInstance<Dog, DogState> structureInstance = structure.Instantiate(new DogState(), null!);

            structureInstance.Construct();
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