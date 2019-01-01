using System;
using System.Runtime.InteropServices;

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

        public WrapperInternal()
        {
        }

        [DllImport("__Internal", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern void MWVClickTo(IntPtr index, int x, int y);

        [DllImport("__Internal", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern int MWVContentHeight(IntPtr index);

        [DllImport("__Internal", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern int MWVGetState(IntPtr index);

        [DllImport("__Internal", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern float MWVGetStateFloatValue(IntPtr index);

        [DllImport("__Internal", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern long MWVGetStateLongValue(IntPtr index);

        [DllImport("__Internal", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern IntPtr MWVGetStateStringValue(IntPtr index);

        [DllImport("__Internal", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern string MWVGetUrl(IntPtr index);

        [DllImport("__Internal", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern bool MWVMoveBack(IntPtr index);

        [DllImport("__Internal", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern bool MWVMoveForward(IntPtr index);

        [DllImport("__Internal", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern IntPtr MWVNativeGetTexturePointer(IntPtr index);

        [DllImport("__Internal", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern IntPtr MWVNativeInit();

        [DllImport("__Internal", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern void MWVNativeInitWebView(IntPtr index, int width, int height);

        [DllImport("__Internal", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern void MWVNativeSetPixelsBuffer(IntPtr index, IntPtr buffer, int width, int height);

        [DllImport("__Internal", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern void MWVNativeUpdateFrameBuffer(IntPtr index);

        [DllImport("__Internal", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern void MWVNativeUpdateTexture(IntPtr index, IntPtr texture);

        [DllImport("__Internal", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern void MWVRelease(IntPtr index);

        [DllImport("__Internal", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern void MWVScrollBy(IntPtr index, int x, int y);

        [DllImport("__Internal", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern void MWVSetInputText(IntPtr index, string text);

        [DllImport("__Internal", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern void MWVSetUrl(IntPtr index, string url);

        [DllImport("__Internal", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern void MWVShowKeyboard(IntPtr index, bool state);

        [DllImport("__Internal", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern void MWVWebSetData(IntPtr index, string data);

        public IntPtr NativeHelperGetAudioSamples(IntPtr samples, int length)
        {
            return IntPtr.Zero;
        }

        public IntPtr NativeHelperGetTexture(IntPtr index)
        {
            return WrapperInternal.MWVNativeGetTexturePointer(index);
        }

        public IntPtr NativeHelperGetUnityRenderCallback()
        {
            return IntPtr.Zero;
        }

        public int NativeHelperInit()
        {
            return (int)WrapperInternal.MWVNativeInit();
        }

        public void NativeHelperInitWebView(IntPtr index, int width, int height)
        {
            WrapperInternal.MWVNativeInitWebView(index, width, height);
        }

        public void NativeHelperSetPixelsBuffer(IntPtr mpInstance, IntPtr buffer, int width, int height)
        {
            WrapperInternal.MWVNativeSetPixelsBuffer(mpInstance, buffer, width, height);
        }

        public void NativeHelperSetTexture(IntPtr index, IntPtr texture)
        {
        }

        public void NativeHelperSetUnityLogMessageCallback(IntPtr callback)
        {
        }

        public void NativeHelperUpdateIndex(IntPtr index)
        {
        }

        public void NativeHelperUpdatePixelsBuffer(IntPtr mpInstance)
        {
            WrapperInternal.MWVNativeUpdateFrameBuffer(mpInstance);
        }

        public void NativeHelperUpdateTexture(IntPtr index, IntPtr texture)
        {
            WrapperInternal.MWVNativeUpdateTexture(index, texture);
        }

        public void WebClickTo(IntPtr wObj, int x, int y)
        {
            WrapperInternal.MWVClickTo(wObj, x, y);
        }

        public int WebContentHeight(IntPtr wObj)
        {
            return WrapperInternal.MWVContentHeight(wObj);
        }

        public WebStates WebGetState(IntPtr mpObj)
        {
            return (WebStates)WrapperInternal.MWVGetState(mpObj);
        }

        public object WebGetStateValue(IntPtr wObj)
        {
            object stringAnsi = WrapperInternal.MWVGetStateFloatValue(wObj);
            if ((float)stringAnsi < 0f)
            {
                stringAnsi = WrapperInternal.MWVGetStateLongValue(wObj);
                if ((long)stringAnsi < (long)0)
                {
                    stringAnsi = Marshal.PtrToStringAnsi(WrapperInternal.MWVGetStateStringValue(wObj));
                }
            }
            return stringAnsi;
        }

        public string WebGetUrl(IntPtr wObj)
        {
            return WrapperInternal.MWVGetUrl(wObj);
        }

        public bool WebMoveBack(IntPtr wObj)
        {
            return WrapperInternal.MWVMoveBack(wObj);
        }

        public bool WebMoveForward(IntPtr wObj)
        {
            return WrapperInternal.MWVMoveForward(wObj);
        }

        public void WebRelease(IntPtr mpObj)
        {
            WrapperInternal.MWVRelease(mpObj);
        }

        public void WebScrollBy(IntPtr wObj, int x, int y)
        {
            WrapperInternal.MWVScrollBy(wObj, x, y);
        }

        public void WebSetData(IntPtr wObj, string data)
        {
            WrapperInternal.MWVWebSetData(wObj, data);
        }

        public void WebSetInputText(IntPtr wObj, string text)
        {
            WrapperInternal.MWVSetInputText(wObj, text);
        }

        public void WebSetUrl(IntPtr mpObj, string url)
        {
            WrapperInternal.MWVSetUrl(mpObj, url);
        }

        public void WebShowKeyboard(IntPtr wObj, bool state)
        {
            WrapperInternal.MWVShowKeyboard(wObj, state);
        }

        public void WebCallFunction(IntPtr wObj, string functionName)
        {
        }
    }
}