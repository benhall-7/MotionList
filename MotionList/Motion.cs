using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MotionList
{
    public abstract class Motion
    {
        public ulong MotionKind { get; set; }
        public ulong GameHash { get; set; }
        public uint Flags { get; set; }
        public int Unk14 { get; set; }//always 0x1c?
        public ulong AnimationHash { get; set; }

        public Motion(ulong motionKind, ulong gameHash, uint flags, int unk14, ulong animationHash)
        {
            MotionKind = motionKind;
            GameHash = gameHash;
            Flags = flags;
            Unk14 = unk14;
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
