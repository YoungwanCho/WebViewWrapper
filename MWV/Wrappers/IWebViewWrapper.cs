using System;

namespace MWV.Wrappers
{
    internal interface IWebViewWrapper
    {
        void WebClickTo(IntPtr wObj, int x, int y);

        int WebContentHeight(IntPtr wObj);

        WebStates WebGetState(IntPtr wObj);

        object WebGetStateValue(IntPtr wObj);

        string WebGetUrl(IntPtr wObj);

        bool WebMoveBack(IntPtr wObj);

        bool WebMoveForward(IntPtr wObj);

        void WebRelease(IntPtr wObj);

        void WebScrollBy(IntPtr wObj, int x, int y);

        void WebSetData(IntPtr wObj, string data);

        void WebSetInputText(IntPtr wObj, string text);

        void WebCallFunction(IntPtr wObj, string functionName);

        void WebSetUrl(IntPtr wObj, string url);

        void WebShowKeyboard(IntPtr wObj, bool state);
    }
}