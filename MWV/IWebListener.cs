namespace MWV
{
    public interface IWebListener : IWebPageStartedListener, IWebPageLoadingListener, IWebPageFinishedListener, IWebPageErrorListener, IWebPageHttpErrorListener, IWebPageElementReceivedListener
    {

    }
}