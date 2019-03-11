using System.Drawing;

namespace AlexNoddings.Infinit3.Core.KeyRecognisers
{
    public class KeyCropper
    {
        public static Bitmap Crop(Bitmap image)
        {
            Rectangle keyRectangle = GetKeyRectangle(image);
            if (keyRectangle.Width > 0)
            {
                var cropped = new Bitmap(keyRectangle.Width, keyRectangle.Height);
                using (Graphics g = Graphics.FromImage(cropped))
                {
                    var destRect = new Rectangle(0, 0, keyRectangle.Width, keyRectangle.Height);
                    g.DrawImage(image, destRect, keyRectangle, GraphicsUnit.Pixel);
                }

                return cropped;
            }

            return null;
        }

        private static Rectangle GetKeyRectangle(Bitmap image)
        {
            int keyWidth;
            int keyHeight;

            int centerPixelR;
            int upPixelR;
            int rightPixelR;
            int downPixelR;
            int leftPixelR;

            // Key image scales seemingly non-linearly based on resolution, meaning boundaries must be pre-set.
            // The colour of the top left pixels also change based on image scaling.
            if (image.Width == 2560 && image.Height == 1440 || image.Width == 2576 && image.Height == 1479)
            {
                keyWidth = 38;
                keyHeight = 48;

                centerPixelR = 218;
                upPixelR = 238;
                rightPixelR = 211;
                downPixelR = 178;
                leftPixelR = 210;
            }
            else if (image.Width == 1920 && image.Height == 1080)
            {
                keyWidth = 26;
                keyHeight = 33;

                centerPixelR = 241;
                upPixelR = 234;
                rightPixelR = 234;
                downPixelR = 184;
                leftPixelR = 196;
            }
            else
            {
                throw new UnableToCropImageException(
                    string.Format("Key width and height unknown for windows of size {0}x{1}", image.Width,
                        image.Height));
            }

            // Key image will only appear within a portion of the screen, don't bother searching outside of it
            var searchAreaX = (int) (image.Width * 0.25);
            var searchAreaY = (int) (image.Height * 0.4);
            var searchAreaW = (int) (image.Width * 0.12);
            var searchAreaH = (int) (image.Height * 0.2);

            for (int x = searchAreaX; x < searchAreaX + searchAreaW; x++)
            for (int y = searchAreaY; y < searchAreaY + searchAreaH; y++)
            {
                Color pixel = image.GetPixel(x, y);
                // Check if the pixel could be the center pixel
                if (pixel.R == centerPixelR)
                {
                    // If so, check the surrounding pixels as the background pulses colours
                    Color up = image.GetPixel(x, y - 1);
                    Color right = image.GetPixel(x + 1, y);
                    Color down = image.GetPixel(x, y + 1);
                    Color left = image.GetPixel(x - 1, y);
                    if (up.R == upPixelR &&
                        right.R == rightPixelR &&
                        down.R == downPixelR &&
                        left.R == leftPixelR)
                    {
                        var keyRectangle = new Rectangle(x + 1, y + 1, keyWidth, keyHeight);
                        return keyRectangle;
                    }
                }
            }

            return new Rectangle(0, 0, 0, 0);
        }
    }
}