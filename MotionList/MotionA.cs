using System.IO;

namespace MotionList
{
    public class MotionA : Motion
    {
        public ulong AnimationHash { get; set; }
        public int Unk20 { get; set; }
        public byte XluStart { get; set; }
        public byte XluEnd { get; set; }
        public short CancelFrame { get; set; }

        internal MotionA(BinaryReader reader, ulong motionKind, ulong gameHash, ushort flags, byte frames, bool hasAnim, int unk14) : base(motionKind, gameHash, flags, frames, hasAnim, unk14)
        {
            AnimationHash = reader.ReadUInt64();
            Unk20 = reader.ReadInt32();
            ExpressionHash = reader.ReadUInt64();
            SoundHash = reader.ReadUInt64();
            EffectHash = reader.ReadUInt64();
            XluStart = reader.ReadByte();
            XluEnd = reader.ReadByte();
            CancelFrame = reader.ReadInt16();
        }

        internal override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write(AnimationHash);
            writer.Write(Unk20);
            writer.Write(ExpressionHash);
            writer.Write(SoundHash);
            writer.Write(EffectHash);
            writer.Write(XluStart);
            writer.Write(XluEnd);
            writer.Write(CancelFrame);
        }
    }
}
