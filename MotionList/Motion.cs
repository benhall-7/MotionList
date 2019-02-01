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
        private int Size { get; set; }
        public ulong AnimationHash { get; set; }
        public int Unk { get; set; }
        public ulong ExpressionHash { get; set; }
        public ulong SoundHash { get; set; }
        public ulong EffectHash { get; set; }
        public byte XluStart { get; set; }
        public byte XluEnd { get; set; }
        public byte CancelFrame { get; set; }
        public bool NoStopIntp { get; set; }

        public bool HasExpression { get { return Size / 8 >= 3; } }
        public bool HasSound { get { return Size / 8 >= 2; } }
        public bool HasEffect { get { return Size / 8 >= 1; } }
        public bool HasExtended { get { return Size % 8 == 4; } }

        internal Motion(BinaryReader reader)
        {
            MotionKind = reader.ReadUInt64();
            GameHash = reader.ReadUInt64();
            Flags = reader.ReadUInt16();
            Frames = reader.ReadByte();
            HasAnimation = reader.ReadBoolean();
            Size = reader.ReadInt32();
            if (HasAnimation)
            {
                AnimationHash = reader.ReadUInt64();
                Unk = reader.ReadInt32();
            }
            if (HasExpression)
                ExpressionHash = reader.ReadUInt64();
            if (HasSound)
                SoundHash = reader.ReadUInt64();
            if (HasEffect)
                EffectHash = reader.ReadUInt64();
            if (HasExtended)
            {
                XluStart = reader.ReadByte();
                XluEnd = reader.ReadByte();
                CancelFrame = reader.ReadByte();
                NoStopIntp = reader.ReadBoolean();
            }
        }

        internal void Write(BinaryWriter writer)
        {
            writer.Write(MotionKind);
            writer.Write(GameHash);
            writer.Write(Flags);
            writer.Write(Frames);
            writer.Write(HasAnimation);
            writer.Write(Size);
            if (HasAnimation)
            {
                writer.Write(AnimationHash);
                writer.Write(Unk);
            }
            if (HasExpression)
                writer.Write(ExpressionHash);
            if (HasSound)
                writer.Write(SoundHash);
            if (HasEffect)
                writer.Write(EffectHash);
            if (HasExtended)
            {
                writer.Write(XluStart);
                writer.Write(XluEnd);
                writer.Write(CancelFrame);
                writer.Write(NoStopIntp);
            }
        }
    }
}
