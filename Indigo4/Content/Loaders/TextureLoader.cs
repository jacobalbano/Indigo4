using Alexandria;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Indigo.Content.Loaders
{
    internal class TextureLoader : Library.ReloadableLoader<Texture>
    {
        private GraphicsDevice graphicsDevice;

        public TextureLoader(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
        }

        public override Texture LoadFromStream(Stream dataStream, Library library)
        {
            return new Texture { XnaTexture = GetData(dataStream) };
        }

        public override void UpdateFromStream(Texture item, Stream dataStream)
        {
            item.XnaTexture.Dispose();
            item.XnaTexture = GetData(dataStream);
        }

        private Texture2D GetData(Stream stream)
        {
            Texture2D.TextureDataFromStreamEXT(stream, out int width, out int height, out var pixels);
            var texture = new Texture2D(graphicsDevice, width, height);

            texture.SetData(pixels);
            return texture;
        }
    }
}