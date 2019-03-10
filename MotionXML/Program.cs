using MotionList;
using System;
using System.Collections.Generic;
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
            string output = "output.xml";
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

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine(HelpText);
                return;
            }
            
            Xml = new XmlDocument();

            if (string.IsNullOrEmpty(labels))
                Labels = new Dictionary<ulong, string>();
            else
                Labels = GetLabels(labels);

            switch (mode)
            {
                case AsmMode.Invalid:
                    Console.WriteLine("Asm mode not set. See -h");
                    break;
                case AsmMode.Disasm:
                    MFile = new MotionFile(input);
                    Disasm();
                    Xml.Save(output);
                    break;
                case AsmMode.Asm:
                    break;
            }
        }

        static void Disasm()
        {
            Xml.AppendChild(Xml.CreateXmlDeclaration("1.0", "UTF-8", null));
            XmlNode root = NodeWithAttribute("MotionList", "ID", ConvertHash(MFile.IDHash));
            foreach (Motion motion in MFile.Entries)
            {
                XmlNode motionNode = NodeWithAttribute("Motion", "Hash", ConvertHash(motion.MotionKind));
                motionNode.AppendChild(NodeWithValue("GameHash", ConvertHash(motion.GameHash)));
                motionNode.AppendChild(NodeWithValue("Flags", "0x" + motion.Flags.ToString("x4")));
                motionNode.AppendChild(NodeWithValue("TransitionFrames", motion.Frames.ToString()));

                motionNode.AppendChild(NodeWithValue("AnimationCount", motion.AnimationCount.ToString()));
                for (int i = 0; i < motion.AnimationCount; i++)
                    motionNode.AppendChild(NodeWithAttributeValue("AnimationHash", ConvertHash(motion.AnimationHashes[i]), "ID", i.ToString()));
                for (int i = 0; i < motion.AnimationCount; i++)
                    motionNode.AppendChild(NodeWithAttributeValue("AnimationUnk", motion.AnimationUnks[i].ToString(), "ID", i.ToString()));

                foreach (var hash in motion.ExtraHashes)
                    motionNode.AppendChild(NodeWithAttributeValue("ExtraHash", ConvertHash(hash.Value), "Kind", hash.Key.ToString()));

                if (motion.HasExtended)
                {
                    motionNode.AppendChild(NodeWithValue("XluStart", motion.XluStart.ToString()));
                    motionNode.AppendChild(NodeWithValue("XluEnd", motion.XluEnd.ToString()));
                    motionNode.AppendChild(NodeWithValue("CancelFrame", motion.CancelFrame.ToString()));
                    motionNode.AppendChild(NodeWithValue("NoStopIntp", motion.NoStopIntp.ToString()));
                }
                root.AppendChild(motionNode);
            }
            Xml.AppendChild(root);
        }

        static void Asm()
        {

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
