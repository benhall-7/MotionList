using System.Collections.Generic;
using System.IO;

namespace MotionList
{
    public class MotionFile
    {
        public const ulong Magic = 0x06f5fea1e8;//motion

        public ulong IDHash { get; set; }
        public List<Motion> Entries { get; set; }

        public MotionFile()
        {

        }
        public MotionFile(string filename)
        {
            Entries = new List<Motion>();
            using (BinaryReader reader = new BinaryReader(File.OpenRead(filename)))
            {
                if (reader.ReadUInt64() != Magic)
                    throw new InvalidDataException("File contains an invalid header");
                IDHash = reader.ReadUInt64();
                int count = reader.ReadInt32();
                reader.BaseStream.Position = 0x18;//alignment
                for (int i = 0; i < count; i++)
                    Entries.Add(new Motion(reader));
            }
        }

        public void Save(string filename)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Create(filename)))
            {
                writer.Write(Magic);
                writer.Write(IDHash);
                writer.Write((long)Entries.Count);
                foreach (Motion motion in Entries)
                    motion.Write(writer);
            }
        }
    }
}
