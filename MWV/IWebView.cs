using System;
using UnityEngine;

namespace MWV
{
    internal interface IWebView
    {
        GameObject OutputObject { get; set; }

        WebViewManagerEvents EventManager { get; }

        WebStates State { get; }

        object StateValue { get; }

        void AddWebListener(IWebListener listener);

        void RemoveWebListener(IWebListener listener);

        void Load(Uri url);

        void Load(string data);

        void UnLoad(bool resetTexture);

        void UnLoad();

        void Release();

        bool MoveForward();

        bool MoveBack();

        void SetInputText(string text);

        void SetMotionEvent(MotionActions action, float x, float y);

        void ClickTo(int x, int y);

        void ScrollBy(int x, int y, float scrollTime = 0.5f);

        Uri Url { get; }

        bool IsReady { get; }

        byte[] FramePixels { get; }

        bool DeviceKeyboard { set; }

        int Width { get; }

        int Height { get; }

        int ContentHeight { get; }

        void CallFunction(string functionName);
    }
}
