using System.IO;

namespace MotionList
{
    public abstract class Motion
    {
        public ulong MotionKind { get; set; }
        public ulong GameHash { get; set; }
        public ushort Flags { get; set; }//replace this with individual flags later
        public byte Frames { get; set; }
        public byte Type { get; set; }
        public int Unk14 { get; set; }//always 0x1c?
        public ulong AnimationHash { get; set; }

        public Motion(ulong motionKind, ulong gameHash, ushort flags, byte frames, byte type, int unk14, ulong animationHash)
        {
            MotionKind = motionKind;
            GameHash = gameHash;
            Flags = flags;
            Frames = frames;
            Type = type;
            Unk14 = unk14;
            AnimationHash = animationHash;
        }

        internal virtual void Write(BinaryWriter writer)
        {
            writer.Write(MotionKind);
            writer.Write(GameHash);
            writer.Write(Flags);
            writer.Write(Unk14);
            writer.Write(AnimationHash);
        }
    }
}
