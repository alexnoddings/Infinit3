using System.Drawing;
using System.Linq;
using AlexNoddings.Infinit3.Core.KeyRecognisers;
using IronOcr;
using IronOcr.Languages;

namespace AlexNoddings.Infinit3.Services.KeyRecognisers.IronOcr
{
    public class IronOcrKeyRecogniser : IKeyRecogniser
    {
        private static readonly Bitmap TrainingImage;
        private static readonly Rectangle TrainingImageSrc;

        private readonly AdvancedOcr _ocr;

        static IronOcrKeyRecogniser()
        {
            TrainingImage = (Bitmap) Image.FromFile("IronOcrKeyFrame.png");
            TrainingImageSrc = new Rectangle(0, 0, TrainingImage.Width, TrainingImage.Height);
        }

        public IronOcrKeyRecogniser()
        {
            _ocr = new AdvancedOcr
            {
                CleanBackgroundNoise = true,
                EnhanceContrast = true,
                EnhanceResolution = true,
                Language = English.OcrLanguagePack,
                Strategy = AdvancedOcr.OcrStrategy.Fast,
                ColorSpace = AdvancedOcr.OcrColorSpace.Color,
                DetectWhiteTextOnDarkBackgrounds = false,
                InputImageType = AdvancedOcr.InputTypes.AutoDetect,
                RotateAndStraighten = true,
                ReadBarCodes = false,
                ColorDepth = 8
            };
        }

        public char GetChar(Bitmap image)
        {
            char c;
            using (Bitmap croppedKey = KeyCropper.Crop(image))
            {
                if (croppedKey != null)
                    using (Bitmap cleanKey = KeyCleaner.Clean(croppedKey))
                    {
                        using (Bitmap trainedImage = GenerateTrainedImage(cleanKey))
                        {
                            c = _ocr.Read(trainedImage).Text.Last();
                        }
                    }
                else
                    c = (char) 0;
            }

            return c;
        }

        private static Bitmap GenerateTrainedImage(Bitmap key)
        {
            var trainedImage = new Bitmap(key.Width * 27, key.Height);

            var trainingImageDestination = new Rectangle(0, 0, key.Width * 26, key.Height);
            var keySource = new Rectangle(0, 0, key.Width, key.Height);
            var keyDestination = new Rectangle(key.Width * 26, 0, key.Width, key.Height);

            using (Graphics g = Graphics.FromImage(trainedImage))
            {
                g.DrawImage(TrainingImage, trainingImageDestination, TrainingImageSrc, GraphicsUnit.Pixel);
                g.DrawImage(key, keyDestination, keySource, GraphicsUnit.Pixel);
            }

            return trainedImage;
        }
    }
}