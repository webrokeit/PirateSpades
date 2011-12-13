// <copyright file="Func.cs">
//      mche@itu.dk
// </copyright>
// <summary>
//      A helper class used for different functionalities
// </summary>
// <author>Morten Chabert Eskesen (mche@itu.dk)</author>

namespace PirateSpadesGame.Misc {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A helper class used for different functionalities
    /// </summary>
    public class Func {
        /// <summary>
        ///  Wrapper for HitAlpha taking Rectangle and Texture
        /// </summary>
        /// <param name="rectangle">The rectangle</param>
        /// <param name="texture">The texture</param>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <returns>True if HitAlpha returns true</returns>
        public static bool HitAlpha(Rectangle rectangle, Texture2D texture, int x, int y) {
            return HitAlpha(0, 0, texture, texture.Width * (x - rectangle.X) /
                rectangle.Width, texture.Height * (y - rectangle.Y) / rectangle.Height);
        }

        /// <summary>
        /// Wraps Hit then determines if hit a transparent part of image
        /// </summary>
        /// <param name="tx">The tx</param>
        /// <param name="ty">The ty</param>
        /// <param name="texture">The texture</param>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <returns>True if it hits the rectangle formed by the texture or if it hits a transpart of the image</returns>
        private static bool HitAlpha(float tx, float ty, Texture2D texture, int x, int y) {
            if(Hit(tx, ty, texture, x, y)) {
                var data = new uint[texture.Width * texture.Height];
                texture.GetData<uint>(data);
                if((x - (int)tx) + (y - (int)ty) * texture.Width < texture.Width * texture.Height) {
                    return ((data[(x - (int)tx) + (y - (int)ty) * texture.Width] & 0xFF000000) >> 24) > 20;
                }
            }
            return false;
        }

        /// <summary>
        /// Determine if x,y are within rectangle formed by texture located at tx,ty
        /// </summary>
        /// <param name="tx">The tx</param>
        /// <param name="ty">The ty</param>
        /// <param name="texture">The texture</param>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <returns>True if the coordinates are within the rectangle formed by the texture</returns>
        private static bool Hit(float tx, float ty, Texture2D texture, int x, int y) {
            return (x >= tx &&
                x <= tx + texture.Width &&
                y >= ty &&
                y <= ty + texture.Height);
        }
    }
}
