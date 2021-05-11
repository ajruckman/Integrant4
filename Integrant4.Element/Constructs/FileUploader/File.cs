using System.IO;

namespace Integrant4.Element.Constructs.FileUploader
{
    public class File
    {
        internal readonly ushort SerialID;

        public readonly string       Name;
        public readonly MemoryStream Data;
        public readonly string       Hash;

        internal File(ushort serialID, string name, MemoryStream data, string hash)
        {
            SerialID = serialID;
            Name     = name;
            Data     = data;
            Hash     = hash;
        }
    }
}