using System;

namespace MWV.Wrappers
{
    internal class WrapperInternal : INativeWrapperHelper, IWebViewWrapper
    {
        public const string LIBRARY_NAME = "MobileWebView";

        public string LibraryName
        {
            get
            {
                return "MobileWebView";
            }
        }

        public int NativeHelperInit()
        {
            return 0;
        }

        public void NativeHelperInitWebView(IntPtr index, int width, int height)
        {
        }

        public void NativeHelperUpdateIndex(IntPtr index)
        {
        }

        public void NativeHelperSetTexture(IntPtr index, IntPtr texture)
        {
        }

        public IntPtr NativeHelperGetTexture(IntPtr index)
        {
            return IntPtr.Zero;
        }

        public void NativeHelperUpdateTexture(IntPtr index, IntPtr texture)
        {
        }

        public void NativeHelperSetPixelsBuffer(
          IntPtr mpInstance,
          IntPtr buffer,
          int width,
          int height)
        {
        }

        public void NativeHelperUpdatePixelsBuffer(IntPtr mpInstance)
        {
        }

        public IntPtr NativeHelperGetAudioSamples(IntPtr samples, int length)
        {
            return IntPtr.Zero;
        }

        public IntPtr NativeHelperGetUnityRenderCallback()
        {
            return IntPtr.Zero;
        }

        public void NativeHelperSetUnityLogMessageCallback(IntPtr callback)
        {
        }

        public void WebSetUrl(IntPtr mpObj, string url)
        {
        }

        public string WebGetUrl(IntPtr wObj)
        {
            return string.Empty;
        }

        public void WebSetData(IntPtr wObj, string data)
        {
        }

        public void WebSetInputText(IntPtr wObj, string text)
        {
        }

        public void WebRelease(IntPtr mpObj)
        {
        }

        public bool WebMoveForward(IntPtr wObj)
        {
            return false;
        }

        public bool WebMoveBack(IntPtr wObj)
        {
            return false;
        }

        public void WebClickTo(IntPtr wObj, int x, int y)
        {
        }

        public void WebScrollBy(IntPtr wObj, int x, int y)
        {
        }

        public void WebShowKeyboard(IntPtr wObj, bool state)
        {
        }

        public int WebContentHeight(IntPtr wObj)
        {
            return 0;
        }

        public WebStates WebGetState(IntPtr mpObj)
        {
            return WebStates.Empty;
        }

        public object WebGetStateValue(IntPtr wObj)
        {
            return (object)null;
        }
    }
}
