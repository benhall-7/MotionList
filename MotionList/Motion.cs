using System.IO;

namespace MotionList
{
    public abstract class Motion
    {
        public ulong MotionKind { get; set; }
        public ulong GameHash { get; set; }
        public ushort Flags { get; set; }//replace this with individual flags later
        public byte Frames { get; set; }
        public bool HasAnimation { get; set; }
        public int Unk14 { get; set; }//always 0x1c?

        //these ones are in a different order depending on HasAnimation so they have to be set in derived classes
        public ulong ExpressionHash { get; set; }
        public ulong SoundHash { get; set; }
        public ulong EffectHash { get; set; }

        public Motion(ulong motionKind, ulong gameHash, ushort flags, byte frames, bool hasAnim, int unk14)
        {
            MotionKind = motionKind;
            GameHash = gameHash;
            Flags = flags;
            Frames = frames;
            HasAnimation = hasAnim;
            Unk14 = unk14;
        }

        internal virtual void Write(BinaryWriter writer)
        {
            writer.Write(MotionKind);
            writer.Write(GameHash);
            writer.Write(Flags);
            writer.Write(Unk14);
        }
    }
}
