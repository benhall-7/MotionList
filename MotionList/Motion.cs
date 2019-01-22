using System.IO;

namespace MotionList
{
    public class Motion
    {
        public ulong MotionKind { get; set; }
        public ulong GameHash { get; set; }
        public ushort Flags { get; set; }//replace this with individual flags later
        public byte Frames { get; set; }
        public bool HasAnimation { get; set; }
        public int Unk1 { get; set; }//always 0x1c?
        public ulong AnimationHash { get; set; }
        public int Unk2 { get; set; }
        public ulong ExpressionHash { get; set; }
        public ulong SoundHash { get; set; }
        public ulong EffectHash { get; set; }
        public byte XluStart { get; set; }
        public byte XluEnd { get; set; }
        public short CancelFrame { get; set; }

        internal Motion(BinaryReader reader)
        {
            MotionKind = reader.ReadUInt64();
            GameHash = reader.ReadUInt64();
            Flags = reader.ReadUInt16();
            Frames = reader.ReadByte();
            HasAnimation = reader.ReadBoolean();
            Unk1 = reader.ReadInt32();
            if (HasAnimation)
            {
                AnimationHash = reader.ReadUInt64();
                Unk2 = reader.ReadInt32();
            }
            ExpressionHash = reader.ReadUInt64();
            SoundHash = reader.ReadUInt64();
            EffectHash = reader.ReadUInt64();
            XluStart = reader.ReadByte();
            XluEnd = reader.ReadByte();
            CancelFrame = reader.ReadInt16();
        }

        internal void Write(BinaryWriter writer)
        {
            writer.Write(MotionKind);
            writer.Write(GameHash);
            writer.Write(Flags);
            writer.Write(Frames);
            writer.Write(HasAnimation);
            writer.Write(Unk1);
            if (HasAnimation)
            {
                writer.Write(AnimationHash);
                writer.Write(Unk2);
            }
            writer.Write(ExpressionHash);
            writer.Write(SoundHash);
            writer.Write(EffectHash);
            writer.Write(XluStart);
            writer.Write(XluEnd);
            writer.Write(CancelFrame);
        }
    }
}
