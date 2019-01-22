using System.IO;

namespace MotionList
{
    public class MotionB : Motion
    {
        public int Unk30 { get; set; }

        internal MotionB(BinaryReader reader, ulong motionKind, ulong gameHash, ushort flags, byte frames, bool hasAnim, int unk14) : base(motionKind, gameHash, flags, frames, hasAnim, unk14)
        {
            ExpressionHash = reader.ReadUInt64();
            SoundHash = reader.ReadUInt64();
            EffectHash = reader.ReadUInt64();
            Unk30 = reader.ReadInt32();
        }

        internal override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write(ExpressionHash);
            writer.Write(SoundHash);
            writer.Write(EffectHash);
            writer.Write(Unk30);
        }
    }
}
