using System;
using System.Xml;
using MotionList;

namespace MotionXML
{
    class Program
    {
        static MotionFile MFile { get; set; }
        static XmlDocument Xml { get; set; }

        static void Main(string[] args)
        {
            try
            {
                MFile = new MotionFile(args[0]);
                Xml = new XmlDocument();
                Xml.AppendChild(Xml.CreateXmlDeclaration("1.0", "UTF-8", null));
                XmlNode root = NodeWithAttribute("MotionList", "ID", "0x" + MFile.IDHash.ToString("x10"));
                foreach (Motion motion in MFile.Entries)
                {
                    XmlNode motionNode = NodeWithAttribute("Motion", "Kind", "0x" + motion.MotionKind.ToString("x10"));
                    motionNode.AppendChild(NodeWithValue("GameHash", "0x" + motion.GameHash.ToString("x10")));
                    motionNode.AppendChild(NodeWithValue("Flags", "0x" + motion.Flags.ToString("x4")));
                    motionNode.AppendChild(NodeWithValue("TransitionFrames", motion.Frames.ToString()));
                    motionNode.AppendChild(NodeWithValue("HasAnimation", motion.HasAnimation.ToString()));
                    if (motion.HasAnimation)
                    {
                        motionNode.AppendChild(NodeWithValue("AnimationHash", "0x" + motion.AnimationHash.ToString("x10")));
                        motionNode.AppendChild(NodeWithValue("Unk", motion.Unk.ToString()));
                    }
                    if (motion.HasExpression)
                        motionNode.AppendChild(NodeWithValue("ExpressionHash", "0x" + motion.ExpressionHash.ToString("x10")));
                    if (motion.HasSound)
                        motionNode.AppendChild(NodeWithValue("SoundHash", "0x" + motion.SoundHash.ToString("x10")));
                    if (motion.HasEffect)
                        motionNode.AppendChild(NodeWithValue("EffectHash", "0x" + motion.EffectHash.ToString("x10")));
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
                Xml.Save("output.xml");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Temporary arg: [filename]");
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
    }
}
