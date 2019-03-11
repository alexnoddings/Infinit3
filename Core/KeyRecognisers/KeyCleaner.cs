using System.Drawing;
using System.Drawing.Imaging;

namespace AlexNoddings.Infinit3.Core.KeyRecognisers
{
    public class KeyCleaner
    {
        private static readonly ImageAttributes NormalisedColoursAttributes;

        static KeyCleaner()
        {
            Color normalisedColour = Color.FromArgb(170, 170, 170);
            Color[] coloursToNormalise =
            {
                Color.FromArgb(111, 111, 111),
                Color.FromArgb(133, 133, 133),
                Color.FromArgb(131, 131, 131)
            };
            var colourReMaps = new ColorMap[coloursToNormalise.Length];

            for (var i = 0; i < coloursToNormalise.Length; i++)
                colourReMaps[i] = new ColorMap
                {
                    OldColor = coloursToNormalise[i],
                    NewColor = normalisedColour
                };

            NormalisedColoursAttributes = new ImageAttributes();
            NormalisedColoursAttributes.SetRemapTable(colourReMaps);
        }

        public static Bitmap Clean(Bitmap image)
        {
            var clean = new Bitmap(image.Width, image.Height);

            using (Graphics g = Graphics.FromImage(clean))
            {
                var destinationRectangle = new Rectangle(0, 0, clean.Width, clean.Height);
                g.DrawImage(image, destinationRectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel,
                    NormalisedColoursAttributes);
            }

            return clean;
        }
    }
}