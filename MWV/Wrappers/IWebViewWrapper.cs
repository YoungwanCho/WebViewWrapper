using System;

namespace MWV.Wrappers
{
    internal interface IWebViewWrapper
    {
        void WebSetUrl(IntPtr wObj, string url);

        string WebGetUrl(IntPtr wObj);

        void WebSetData(IntPtr wObj, string data);

        void WebRelease(IntPtr wObj);

        bool WebMoveForward(IntPtr wObj);

        bool WebMoveBack(IntPtr wObj);

        void WebSetInputText(IntPtr wObj, string text);

        void WebClickTo(IntPtr wObj, int x, int y);

        void WebScrollBy(IntPtr wObj, int x, int y);

        void WebShowKeyboard(IntPtr wObj, bool state);

        int WebContentHeight(IntPtr wObj);

        WebStates WebGetState(IntPtr wObj);

        object WebGetStateValue(IntPtr wObj);

        void WebCallFunction(IntPtr wObj, string functionName);
    }
}
