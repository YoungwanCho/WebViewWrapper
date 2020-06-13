using System;
using UnityEngine;

namespace MWV.Wrappers
{
    internal class Wrapper : INativeWrapperHelper, IWebViewWrapper
    {
        private static Wrapper _instance;
        private object _wrapperObject;
        private INativeWrapperHelper _nativeWrapper;
        private IWebViewWrapper _webViewWrapper;

        private Wrapper()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    this._wrapperObject = (object)new WrapperInternal();
                    break;
                case RuntimePlatform.Android:
                    this._wrapperObject = (object)new WrapperAndroid();
                    break;
            }
            if (this._wrapperObject is INativeWrapperHelper)
                this._nativeWrapper = this._wrapperObject as INativeWrapperHelper;
            if (!(this._wrapperObject is IWebViewWrapper))
                return;
            this._webViewWrapper = this._wrapperObject as IWebViewWrapper;
        }

        public static Wrapper Instance
        {
            get
            {
                if (Wrapper._instance == null)
                    Wrapper._instance = new Wrapper();
                return Wrapper._instance;
            }
        }

        public object PlatformWrapper
        {
            get
            {
                return this._wrapperObject;
            }
        }

        public string LibraryName
        {
            get
            {
                return this._nativeWrapper.LibraryName;
            }
        }

        public int NativeHelperInit()
        {
            return this._nativeWrapper.NativeHelperInit();
        }

        public void NativeHelperUpdateIndex(IntPtr mpInstance)
        {
            this._nativeWrapper.NativeHelperUpdateIndex(mpInstance);
        }

        public void NativeHelperSetTexture(IntPtr mpInstance, IntPtr texture)
        {
            this._nativeWrapper.NativeHelperSetTexture(mpInstance, texture);
        }

        public IntPtr NativeHelperGetTexture(IntPtr mpInstance)
        {
            return this._nativeWrapper.NativeHelperGetTexture(mpInstance);
        }

        public void NativeHelperUpdateTexture(IntPtr mpInstance, IntPtr texture)
        {
            this._nativeWrapper.NativeHelperUpdateTexture(mpInstance, texture);
        }

        public void NativeHelperSetPixelsBuffer(
          IntPtr mpInstance,
          IntPtr buffer,
          int width,
          int height)
        {
            this._nativeWrapper.NativeHelperSetPixelsBuffer(mpInstance, buffer, width, height);
        }

        public void NativeHelperUpdatePixelsBuffer(IntPtr mpInstance)
        {
            this._nativeWrapper.NativeHelperUpdatePixelsBuffer(mpInstance);
        }

        public IntPtr NativeHelperGetUnityRenderCallback()
        {
            return this._nativeWrapper.NativeHelperGetUnityRenderCallback();
        }

        public void WebSetUrl(IntPtr mpObj, string path)
        {
            this._webViewWrapper.WebSetUrl(mpObj, path);
        }

        public string WebGetUrl(IntPtr wObj)
        {
            return this._webViewWrapper.WebGetUrl(wObj);
        }

        public void WebSetData(IntPtr wObj, string data)
        {
            this._webViewWrapper.WebSetData(wObj, data);
        }

        public void WebRelease(IntPtr mpObj)
        {
            this._webViewWrapper.WebRelease(mpObj);
        }

        public bool WebMoveForward(IntPtr wObj)
        {
            return this._webViewWrapper.WebMoveForward(wObj);
        }

        public bool WebMoveBack(IntPtr wObj)
        {
            return this._webViewWrapper.WebMoveBack(wObj);
        }

        public void WebSetInputText(IntPtr wObj, string text)
        {
            this._webViewWrapper.WebSetInputText(wObj, text);
        }

        public void WebClickTo(IntPtr wObj, int x, int y)
        {
            this._webViewWrapper.WebClickTo(wObj, x, y);
        }

        public void WebScrollBy(IntPtr wObj, int x, int y)
        {
            this._webViewWrapper.WebScrollBy(wObj, x, y);
        }

        public void WebShowKeyboard(IntPtr wObj, bool state)
        {
            this._webViewWrapper.WebShowKeyboard(wObj, state);
        }

        public int WebContentHeight(IntPtr wObj)
        {
            return this._webViewWrapper.WebContentHeight(wObj);
        }

        public WebStates WebGetState(IntPtr mpObj)
        {
            return this._webViewWrapper.WebGetState(mpObj);
        }

        public object WebGetStateValue(IntPtr wObj)
        {
            return this._webViewWrapper.WebGetStateValue(wObj);
        }

        public void WebCallFunction(IntPtr wObj, string functionName)
        {
            this._webViewWrapper.WebCallFunction(wObj, functionName);
        }
    }
}
