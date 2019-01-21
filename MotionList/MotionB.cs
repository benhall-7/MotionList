using System.IO;

namespace MotionList
{
    public class MotionB : Motion
    {
        public ulong UnkHash20 { get; set; }
        public ulong UnkHash28 { get; set; }
        public int Unk30 { get; set; }

        internal MotionB(BinaryReader reader, ulong motionKind, ulong gameHash, uint flags, int unk14, ulong animHash)
            : base(motionKind, gameHash, flags, unk14, animHash)
        {
            UnkHash20 = reader.ReadUInt64();
            UnkHash28 = reader.ReadUInt64();
            Unk30 = reader.ReadInt32();
        }

        internal override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write(UnkHash20);
            writer.Write(UnkHash28);
            writer.Write(Unk30);
        }
    }
}
