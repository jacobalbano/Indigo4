using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indigo.Utility
{
    public class BinPacker
    {
        //  Stolen from Kyle Pulver (and then improved)
        public int Width { get; }
        public int Height { get; }

        List<Rectangle> nodes = new List<Rectangle>();

        public BinPacker(int width, int height)
        {
            Width = width;
            Height = height;
            nodes.Add(new Rectangle(0, 0, Width, Height));
        }

        public bool Pack(int w, int h, out Rectangle rect)
        {
            //  pad by one pixel on every side to prevent bleed
            w += 2;
            h += 2;

            for (int i = 0; i < nodes.Count; ++i)
            {
                if (w <= nodes[i].Width && h <= nodes[i].Height)
                {
                    var node = nodes[i];
                    nodes.RemoveAt(i);
                    rect = new Rectangle(node.X, node.Y, w, h);
                    nodes.Add(new Rectangle(rect.Right, rect.Y, node.Right - rect.Right, rect.Height));
                    nodes.Add(new Rectangle(rect.X, rect.Bottom, rect.Width, node.Bottom - rect.Bottom));
                    nodes.Add(new Rectangle(rect.Right, rect.Bottom, node.Right - rect.Right, node.Bottom - rect.Bottom));
                    
                    //  ignore padding for result
                    rect.X += 1;
                    rect.Y += 1;
                    rect.Width -= 1;
                    rect.Height -= 1;
                    return true;
                }
            }

            rect = Rectangle.Empty;
            return false;
        }
    }
}
