using System;
using System.Xml;
using MotionList;

namespace MotionXML
{
    class Program
    {
        static MotionFile list { get; set; }
        static XmlDocument xml { get; set; }

        static void Main(string[] args)
        {
            try
            {
                list = new MotionFile(args[0]);
                xml = new XmlDocument();
                xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
                XmlNode root = NodeWithAttribute("MotionList", "ID", "0x" + list.IDHash.ToString("x10"));
                foreach (Motion motion in list.Entries)
                {
                    XmlNode motionNode = NodeWithAttribute("Motion", "Kind", "0x" + motion.MotionKind.ToString("x10"));
                    motionNode.AppendChild(NodeWithValue("GameHash", "0x" + motion.GameHash.ToString("x10")));
                    motionNode.AppendChild(NodeWithValue("Flags", "0x" + motion.Flags.ToString("x8")));
                    motionNode.AppendChild(NodeWithValue("Unk14", motion.Unk14.ToString()));
                    motionNode.AppendChild(NodeWithValue("AnimationHash", "0x" + motion.AnimationHash.ToString("x10")));
                    if (motion is MotionA motionA)
                    {
                        motionNode.AppendChild(NodeWithValue("Unk20", motionA.Unk20.ToString()));
                        motionNode.AppendChild(NodeWithValue("ExpressionHash", "0x" + motionA.ExpressionHash.ToString("x10")));
                        motionNode.AppendChild(NodeWithValue("SoundHash", "0x" + motionA.SoundHash.ToString("x10")));
                        motionNode.AppendChild(NodeWithValue("EffectHash", "0x" + motionA.EffectHash.ToString("x10")));
                        motionNode.AppendChild(NodeWithValue("XluStart", motionA.XluStart.ToString()));
                        motionNode.AppendChild(NodeWithValue("XluEnd", motionA.XluEnd.ToString()));
                        motionNode.AppendChild(NodeWithValue("CancelFrame", motionA.CancelFrame.ToString()));
                    }
                    else
                    {
                        MotionB motionB = motion as MotionB;
                        motionNode.AppendChild(NodeWithValue("UnkHash20", "0x" + motionB.UnkHash20.ToString("x10")));
                        motionNode.AppendChild(NodeWithValue("UnkHash28", "0x" + motionB.UnkHash28.ToString("x10")));
                        motionNode.AppendChild(NodeWithValue("Unk30", motionB.Unk30.ToString()));
                    }
                    root.AppendChild(motionNode);
                }
                xml.AppendChild(root);
                xml.Save("output.xml");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Temporary arg: [filename]");
            }
        }

        static XmlNode NodeWithAttribute(string nodeName, string attrName, string attrValue)
        {
            XmlNode node = xml.CreateElement(nodeName);
            XmlAttribute attr = xml.CreateAttribute(attrName);
            attr.Value = attrValue;
            node.Attributes.Append(attr);
            return node;
        }

        static XmlNode NodeWithValue(string nodeName, string value)
        {
            XmlNode node = xml.CreateElement(nodeName);
            XmlNode inner = xml.CreateTextNode(value);
            node.AppendChild(inner);
            return node;
        }
    }
}
