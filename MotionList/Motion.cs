using System;
using System.Collections.Generic;
using System.IO;

namespace MotionList
{
    public class Motion
    {
        public ulong MotionKind { get; set; }
        public ulong GameHash { get; set; }
        public ushort Flags { get; set; }//replace this with individual flags later
        public byte Frames { get; set; }
        public byte AnimationCount { get; set; }
        private int Size { get; set; }
        public List<ulong> AnimationHashes { get; set; }
        public List<byte> AnimationUnks { get; set; }
        public Dictionary<ExtraHashKind, ulong> ExtraHashes { get; set; }
        public bool HasExtended { get; set; }
        public byte XluStart { get; set; }
        public byte XluEnd { get; set; }
        public byte CancelFrame { get; set; }
        public bool NoStopIntp { get; set; }

        public enum ExtraHashKind
        {
            Expression,
            Sound,
            Effect,
            Game2,
            Expression2,
            Sound2,
            Effect2
        }
        public enum ExtraHashGroup
        {
            None    = 0,
            F       = 8,
            SF      = 0x10,
            XSF     = 0x18,
            SFG2S2F2 = 0x28
        }

        internal Motion(BinaryReader reader)
        {
            MotionKind = reader.ReadUInt64();
            GameHash = reader.ReadUInt64();
            Flags = reader.ReadUInt16();
            Frames = reader.ReadByte();
            AnimationCount = reader.ReadByte();
            if (AnimationCount > 3)
                throw new InvalidDataException("Filetype entry cannot have > 3 animations. Are there unknown flags?");
            Size = reader.ReadInt32();
            HasExtended = Size % 8 == 4;

            AnimationHashes = new List<ulong>(AnimationCount);
            AnimationUnks = new List<byte>(AnimationCount);

            for (int i = 0; i < AnimationCount; i++)
                AnimationHashes.Add(reader.ReadUInt64());
            for (int i = 0; i < AnimationCount; i++)
                AnimationUnks.Add(reader.ReadByte());

            reader.BaseStream.Position = (reader.BaseStream.Position + 3 >> 2) << 2;//alignment by 4

            int hashSize = Size / 8 * 8;
            if (!Enum.IsDefined(typeof(ExtraHashGroup), hashSize))
                throw new NotImplementedException($"No implemented hash group has the size = \'{Size}\'");

            ExtraHashes = new Dictionary<ExtraHashKind, ulong>();
            switch ((ExtraHashGroup)hashSize)
            {
                case ExtraHashGroup.None:
                    break;
                case ExtraHashGroup.F:
                    ExtraHashes.Add(ExtraHashKind.Effect, reader.ReadUInt64());
                    break;
                case ExtraHashGroup.SF:
                    ExtraHashes.Add(ExtraHashKind.Sound, reader.ReadUInt64());
                    ExtraHashes.Add(ExtraHashKind.Effect, reader.ReadUInt64());
                    break;
                case ExtraHashGroup.XSF:
                    ExtraHashes.Add(ExtraHashKind.Expression, reader.ReadUInt64());
                    ExtraHashes.Add(ExtraHashKind.Sound, reader.ReadUInt64());
                    ExtraHashes.Add(ExtraHashKind.Effect, reader.ReadUInt64());
                    break;
                case ExtraHashGroup.SFG2S2F2:
                    ExtraHashes.Add(ExtraHashKind.Sound, reader.ReadUInt64());
                    ExtraHashes.Add(ExtraHashKind.Effect, reader.ReadUInt64());
                    ExtraHashes.Add(ExtraHashKind.Game2, reader.ReadUInt64());
                    ExtraHashes.Add(ExtraHashKind.Sound2, reader.ReadUInt64());
                    ExtraHashes.Add(ExtraHashKind.Effect2, reader.ReadUInt64());
                    break;
            }
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

            if (AnimationCount > 3)
                throw new InvalidDataException("Filetype entry cannot have > 3 animations. Are there unknown flags?");
            writer.Write(AnimationCount);
            
            Size = 8 * ExtraHashes.Count + (HasExtended ? 4 : 0);
            writer.Write(Size);

            for (int i = 0; i < AnimationCount; i++)
                writer.Write(AnimationHashes[i]);
            for (int i = 0; i < AnimationCount; i++)
                writer.Write(AnimationUnks[i]);
            
            int hashSize = ExtraHashes.Count;
            if (!Enum.IsDefined(typeof(ExtraHashGroup), hashSize))
                throw new NotImplementedException($"No implemented hash group has the size = \'{Size}\'");

            switch ((ExtraHashGroup)hashSize)
            {
                case ExtraHashGroup.None:
                    break;
                case ExtraHashGroup.F:
                    writer.Write(ExtraHashes[ExtraHashKind.Effect]);
                    break;
                case ExtraHashGroup.SF:
                    writer.Write(ExtraHashes[ExtraHashKind.Sound]);
                    writer.Write(ExtraHashes[ExtraHashKind.Effect]);
                    break;
                case ExtraHashGroup.XSF:
                    writer.Write(ExtraHashes[ExtraHashKind.Expression]);
                    writer.Write(ExtraHashes[ExtraHashKind.Sound]);
                    writer.Write(ExtraHashes[ExtraHashKind.Effect]);
                    break;
                case ExtraHashGroup.SFG2S2F2:
                    writer.Write(ExtraHashes[ExtraHashKind.Sound]);
                    writer.Write(ExtraHashes[ExtraHashKind.Effect]);
                    writer.Write(ExtraHashes[ExtraHashKind.Game2]);
                    writer.Write(ExtraHashes[ExtraHashKind.Sound2]);
                    writer.Write(ExtraHashes[ExtraHashKind.Effect2]);
                    break;
            }

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
