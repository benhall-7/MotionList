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
            expression,
            sound,
            effect,
            game2,
            expression2,
            sound2,
            effect2
        }
        public enum ExtraHashGroup
        {
            none     = 0,
            f        = 8,
            sf       = 0x10,
            xsf      = 0x18,
            sfg2s2f2 = 0x28
        }

        public Motion()
        {

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
                case ExtraHashGroup.none:
                    break;
                case ExtraHashGroup.f:
                    ExtraHashes.Add(ExtraHashKind.effect, reader.ReadUInt64());
                    break;
                case ExtraHashGroup.sf:
                    ExtraHashes.Add(ExtraHashKind.sound, reader.ReadUInt64());
                    ExtraHashes.Add(ExtraHashKind.effect, reader.ReadUInt64());
                    break;
                case ExtraHashGroup.xsf:
                    ExtraHashes.Add(ExtraHashKind.expression, reader.ReadUInt64());
                    ExtraHashes.Add(ExtraHashKind.sound, reader.ReadUInt64());
                    ExtraHashes.Add(ExtraHashKind.effect, reader.ReadUInt64());
                    break;
                case ExtraHashGroup.sfg2s2f2:
                    ExtraHashes.Add(ExtraHashKind.sound, reader.ReadUInt64());
                    ExtraHashes.Add(ExtraHashKind.effect, reader.ReadUInt64());
                    ExtraHashes.Add(ExtraHashKind.game2, reader.ReadUInt64());
                    ExtraHashes.Add(ExtraHashKind.sound2, reader.ReadUInt64());
                    ExtraHashes.Add(ExtraHashKind.effect2, reader.ReadUInt64());
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

            writer.BaseStream.Position = (writer.BaseStream.Position + 3 >> 2) << 2;//alignment by 4

            int hashSize = ExtraHashes.Count * 8;
            if (!Enum.IsDefined(typeof(ExtraHashGroup), hashSize))
                throw new NotImplementedException($"No implemented hash group has the size = \'{Size}\'");

            switch ((ExtraHashGroup)hashSize)
            {
                case ExtraHashGroup.none:
                    break;
                case ExtraHashGroup.f:
                    writer.Write(ExtraHashes[ExtraHashKind.effect]);
                    break;
                case ExtraHashGroup.sf:
                    writer.Write(ExtraHashes[ExtraHashKind.sound]);
                    writer.Write(ExtraHashes[ExtraHashKind.effect]);
                    break;
                case ExtraHashGroup.xsf:
                    writer.Write(ExtraHashes[ExtraHashKind.expression]);
                    writer.Write(ExtraHashes[ExtraHashKind.sound]);
                    writer.Write(ExtraHashes[ExtraHashKind.effect]);
                    break;
                case ExtraHashGroup.sfg2s2f2:
                    writer.Write(ExtraHashes[ExtraHashKind.sound]);
                    writer.Write(ExtraHashes[ExtraHashKind.effect]);
                    writer.Write(ExtraHashes[ExtraHashKind.game2]);
                    writer.Write(ExtraHashes[ExtraHashKind.sound2]);
                    writer.Write(ExtraHashes[ExtraHashKind.effect2]);
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
