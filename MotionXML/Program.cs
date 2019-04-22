using MotionList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace MotionXML
{
    class Program
    {
        static MotionFile MFile { get; set; }
        static XmlDocument Xml { get; set; }
        static Dictionary<ulong, string> Labels { get; set; }

        static string HelpText = $"Required args: -d / -a [disasm/asm] ; [file]{Environment.NewLine}" +
            "Optional args: -l [labels] ; -o [output]";

        static void Main(string[] args)
        {
            AsmMode mode = AsmMode.Invalid;
            string input = "";
            string output = "";
            string labels = "";
            if (args.Length == 0)
            {
                Console.WriteLine(HelpText);
                return;
            }
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-h":
                    case "help":
                        Console.WriteLine(HelpText);
                        break;
                    case "-d":
                        mode = AsmMode.Disasm;
                        break;
                    case "-a":
                        mode = AsmMode.Asm;
                        break;
                    case "-l":
                        labels = args[++i];
                        break;
                    case "-o":
                        output = args[++i];
                        break;
                    default:
                        input = args[i];
                        break;
                }
            }

            if (mode == AsmMode.Invalid)
            {
                Console.WriteLine("Asm mode not set. Use -d or -a, see -h for details");
                return;
            }

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("No input file set. See -h for details");
                return;
            }

            if (string.IsNullOrEmpty(output))
            {
                if (mode == AsmMode.Disasm)
                    output = "output.xml";
                else
                    output = "output_mlist.bin";
            }

            //labels are only used on disassembly. During assembly they are parsed/hashed automatically
            if (string.IsNullOrEmpty(labels) || mode == AsmMode.Asm)
                Labels = new Dictionary<ulong, string>();
            else
                Labels = GetLabels(labels);

            Xml = new XmlDocument();

            if (mode == AsmMode.Disasm)
            {
                MFile = new MotionFile(input);
                Disasm();
                Xml.Save(output);
            }
            else
            {
                Xml.Load(input);
                MFile = new MotionFile();
                Asm();
                MFile.Save(output);
            }
        }

        static void Disasm()
        {
            Xml.AppendChild(Xml.CreateXmlDeclaration("1.0", "UTF-8", null));
            XmlNode root = NodeWithAttribute("motion_list", "id", ConvertHash(MFile.IDHash));
            foreach (Motion motion in MFile.Entries)
            {
                XmlNode motionNode = NodeWithAttribute("motion", "hash", ConvertHash(motion.MotionKind));
                motionNode.AppendChild(NodeWithValue("game_hash", ConvertHash(motion.GameHash)));
                motionNode.AppendChild(NodeWithValue("flags", "0x" + motion.Flags.ToString("x4")));
                motionNode.AppendChild(NodeWithValue("transition_frames", motion.Frames.ToString()));

                motionNode.AppendChild(NodeWithValue("animation_count", motion.AnimationCount.ToString()));
                for (int i = 0; i < motion.AnimationCount; i++)
                    motionNode.AppendChild(NodeWithAttributeValue("animation_hash", ConvertHash(motion.AnimationHashes[i]), "id", i.ToString()));
                for (int i = 0; i < motion.AnimationCount; i++)
                    motionNode.AppendChild(NodeWithAttributeValue("animation_unk", motion.AnimationUnks[i].ToString(), "id", i.ToString()));

                foreach (var hash in motion.ExtraHashes)
                    motionNode.AppendChild(NodeWithAttributeValue("extra_hash", ConvertHash(hash.Value), "kind", hash.Key.ToString()));

                if (motion.HasExtended)
                {
                    motionNode.AppendChild(NodeWithValue("xlu_start", motion.XluStart.ToString()));
                    motionNode.AppendChild(NodeWithValue("xlu_end", motion.XluEnd.ToString()));
                    motionNode.AppendChild(NodeWithValue("cancel_frame", motion.CancelFrame.ToString()));
                    motionNode.AppendChild(NodeWithValue("no_stop_intp", motion.NoStopIntp.ToString()));
                }
                root.AppendChild(motionNode);
            }
            Xml.AppendChild(root);
        }

        static void Asm()
        {
            XmlElement root = Xml.DocumentElement;
            MFile.IDHash = ConvertToHash(root.Attributes["id"].Value);
            var entries = MFile.Entries = new List<Motion>();
            foreach (XmlElement elem in root.ChildNodes)
            {
                Motion motion = new Motion();

                string mkind = elem.Attributes["hash"].Value;
                motion.MotionKind = ConvertToHash(mkind);
                motion.GameHash = ConvertToHash(elem["game_hash"].InnerText);

                ushort flags;
                string flagText = elem["flags"].InnerText;
                if (!flagText.StartsWith("0x")
                    || !ushort.TryParse(flagText.Substring(2),
                        NumberStyles.HexNumber,
                        CultureInfo.InvariantCulture,
                        out flags))
                    throw new Exception($"Error in motion_kind \'{mkind}\': Flags not formatted to proper hexadecimal");
                motion.Flags = flags;
                motion.Frames = byte.Parse(elem["transition_frames"].InnerText);

                motion.AnimationCount = byte.Parse(elem["animation_count"].InnerText);
                motion.AnimationHashes = new List<ulong>(motion.AnimationCount);
                motion.AnimationUnks = new List<byte>(motion.AnimationCount);
                for (int i = 0; i < motion.AnimationCount; i++)
                {
                    motion.AnimationHashes.Add(ConvertToHash(GetXmlByTagAndID("animation_hash", i, elem).InnerText));
                    motion.AnimationUnks.Add(byte.Parse(GetXmlByTagAndID("animation_unk", i, elem).InnerText));
                }

                motion.ExtraHashes = new Dictionary<Motion.ExtraHashKind, ulong>();
                foreach (XmlElement extra in elem.GetElementsByTagName("extra_hash"))
                {
                    Motion.ExtraHashKind kind = (Motion.ExtraHashKind)Enum.Parse(
                        typeof(Motion.ExtraHashKind),
                        extra.Attributes["kind"].Value);
                    motion.ExtraHashes.Add(kind, ConvertToHash(extra.InnerText));
                }
                
                if (CheckContainsExtra(elem))
                {
                    motion.HasExtended = true;
                    motion.XluStart = byte.Parse(elem["xlu_start"].InnerText);
                    motion.XluEnd = byte.Parse(elem["xlu_end"].InnerText);
                    motion.CancelFrame = byte.Parse(elem["cancel_frame"].InnerText);
                    motion.NoStopIntp = bool.Parse(elem["no_stop_intp"].InnerText);
                }

                entries.Add(motion);
            }
        }

        static XmlNode NodeWithAttribute(string nodeName, string attrName, string attrValue)
        {
            XmlNode node = Xml.CreateElement(nodeName);

            XmlAttribute attr = Xml.CreateAttribute(attrName);
            attr.Value = attrValue;
            node.Attributes.Append(attr);

            return node;
        }

        static XmlNode NodeWithValue(string nodeName, string value)
        {
            XmlNode node = Xml.CreateElement(nodeName);

            XmlNode inner = Xml.CreateTextNode(value);
            node.AppendChild(inner);

            return node;
        }

        static XmlNode NodeWithAttributeValue(string nodeName, string value, string attrName, string attrValue)
        {
            XmlNode node = Xml.CreateElement(nodeName);

            XmlAttribute attr = Xml.CreateAttribute(attrName);
            attr.Value = attrValue;
            node.Attributes.Append(attr);

            XmlNode inner = Xml.CreateTextNode(value);
            node.AppendChild(inner);

            return node;
        }
        
        static string ConvertHash(ulong hash)
        {
            if (Labels.TryGetValue(hash, out string val))
                return val;
            return "0x" + hash.ToString("x10");
        }

        static ulong ConvertToHash(string val)
        {
            if (val.StartsWith("0x"))
                return ulong.Parse(val.Substring(2), NumberStyles.HexNumber);
            return (ulong)val.Length << 32 | CRC.CRC32(val);//hash40 generation
        }

        static XmlNode GetXmlByTagAndID(string tag, int id, XmlElement element)
        {
            XmlNodeList nodes = element.GetElementsByTagName(tag);
            foreach (XmlNode node in nodes)
            {
                if (id == byte.Parse(node.Attributes["id"].Value))
                    return node;
            }
            string mkind = element.Attributes["hash"].Value;
            throw new Exception($"Error in motion_kind \'{mkind}\': Animation ID mismatch");
        }

        static bool CheckContainsExtra(XmlElement element)
        {
            foreach (XmlElement elem in element.ChildNodes)
            {
                switch (elem.Name)
                {
                    case "xlu_start":
                    case "xlu_end":
                    case "cancel_frame":
                    case "no_stop_intp":
                        return true;
                }
            }
            return false;
        }

        static Dictionary<ulong, string> GetLabels(string filepath)
        {
            Dictionary<ulong, string> labels = new Dictionary<ulong, string>();
            foreach (var line in File.ReadAllLines(filepath))
                labels.Add((ulong)line.Length << 32 | CRC.CRC32(line), line);
            return labels;
        }

        enum AsmMode
        {
            Asm,
            Disasm,
            Invalid
        }
    }
}
