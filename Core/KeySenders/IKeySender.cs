namespace AlexNoddings.Infinit3.Core.KeySenders
{
    public interface IKeySender
    {
        bool IsReady();
        void SendChar(char character);
    }
}