using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MotionList
{
    class MotionA : Motion
    {
        public int Unk20 { get; set; }
        public ulong ExpressionHash { get; set; }
        public ulong SoundHash { get; set; }
        public ulong EffectHash { get; set; }
        public byte XluStart { get; set; }
        public byte XluEnd { get; set; }
        public short CancelFrame { get; set; }

        internal MotionA(BinaryReader reader, ulong motionKind, ulong gameHash, uint flags, int unk14, ulong animHash)
            : base(motionKind, gameHash, flags, unk14, animHash)
        {
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
