namespace Presentation.Util
{
    public interface IBroadcastObserver
    {
        void BroadcastReceived(string action);
    }
}