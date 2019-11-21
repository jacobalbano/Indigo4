using Alexandria;
using FNT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Indigo.Content.Loaders
{
    public class FontLoader : Library.Loader<FontLibrary>
    {
        private GraphicsDevice graphicsDevice;

        public FontLoader(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
        }

        public override FontLibrary LoadFromStream(Stream dataStream, Library library)
        {
            return new FontLibrary(dataStream, graphicsDevice);
        }
    }
}
