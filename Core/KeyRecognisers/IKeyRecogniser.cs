using System.Drawing;

namespace AlexNoddings.Infinit3.Core.KeyRecognisers
{
    public interface IKeyRecogniser
    {
        char GetChar(Bitmap keyImage);
    }
}