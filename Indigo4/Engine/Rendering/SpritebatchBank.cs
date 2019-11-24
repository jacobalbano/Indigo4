using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo.Engine.Rendering
{
    internal class SpritebatchBank
    {
        public SpritebatchBank(GraphicsDevice gd)
        {
            this.gd = gd;
            for (int i = 0; i < 10; i++)
                pool.Add(new SpriteBatch(gd));
        }

        public SpriteBatch Get(int depth)
        {
            if (!depthToIndex.TryGetValue(depth, out var index))
            {
                if (head >= pool.Count)
                    pool.Add(new SpriteBatch(gd));
                index = depthToIndex[depth] = head++;
            }

            return pool[index];
        }

        public void Reset()
        {
            head = 0;
            depthToIndex.Clear();
        }

        private readonly List<SpriteBatch> pool = new List<SpriteBatch>();
        private readonly Dictionary<int, int> depthToIndex = new Dictionary<int, int>();
        private int head = 0;

        private readonly GraphicsDevice gd;
    }
}
