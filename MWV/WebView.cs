using System;
using UnityEngine;

namespace MWV
{
    public class WebView : IWebView
    {
        private object _webViewObject;
        private IWebView _webView;

        public WebView(MonoBehaviour monoObject, GameObject outputObject)
          : this(monoObject, outputObject, new Vector2((float)Screen.width, (float)Screen.height))
        {
        }

        public WebView(MonoBehaviour monoObject, GameObject outputObject, Vector2 size)
        {
            if (WebViewHelper.IsSupportedPlatform)
            {
                if ((double)size.x <= 0.0 || (double)size.y <= 0.0)
                {
                    Debug.LogWarning((object)"Unsupported size (width <= 0, height <= 0): will be use device screen size by default");
                    size = new Vector2((float)Screen.width, (float)Screen.height);
                }
                if (WebViewHelper.IsAndroidPlatform)
                    this._webViewObject = (object)new WebViewAndroid(monoObject, outputObject, size);
                if (WebViewHelper.IsIPhonePlatform)
                    this._webViewObject = (object)new WebViewIPhone(monoObject, outputObject, size);
            }
            if (this._webViewObject == null)
            {
                Debug.LogWarning((object)"This platform is unsupported for MWV asset, all functionality will be ignored!");
            }
            else
            {
                if (!(this._webViewObject is IWebView))
                    return;
                this._webView = this._webViewObject as IWebView;
            }
        }

        public object PlatformWebView
        {
            get
            {
                return this._webViewObject;
            }
        }

        public GameObject OutputObject
        {
            get
            {
                return this._webView != null ? this._webView.OutputObject : (GameObject)null;
            }
            set
            {
                if (this._webView == null)
                    return;
                this._webView.OutputObject = value;
            }
        }

        public WebViewManagerEvents EventManager
        {
            get
            {
                return this._webView != null ? this._webView.EventManager : (WebViewManagerEvents)null;
            }
        }

        public WebStates State
        {
            get
            {
                return this._webView != null ? this._webView.State : WebStates.Empty;
            }
        }

        public object StateValue
        {
            get
            {
                return this._webView != null ? this._webView.StateValue : (object)null;
            }
        }

        public void AddWebListener(IWebListener listener)
        {
            if (this._webView == null)
                return;
            this._webView.AddWebListener(listener);
        }

        public void RemoveWebListener(IWebListener listener)
        {
            if (this._webView == null)
                return;
            this._webView.RemoveWebListener(listener);
        }

        public void Load(Uri url)
        {
            if (this._webView == null)
                return;
            this._webView.Load(url);
        }

        public void Load(string data)
        {
            if (this._webView == null)
                return;
            this._webView.Load(data);
        }

        public void UnLoad(bool resetTexture)
        {
            if (this._webView == null)
                return;
            this._webView.UnLoad(resetTexture);
        }

        public void UnLoad()
        {
            this.UnLoad(true);
        }

        public void Release()
        {
            if (this._webView == null)
                return;
            this._webView.Release();
        }

        public Uri Url
        {
            get
            {
                return this._webView != null ? this._webView.Url : (Uri)null;
            }
        }

        public bool MoveForward()
        {
            return this._webView != null && this._webView.MoveForward();
        }

        public bool MoveBack()
        {
            return this._webView != null && this._webView.MoveBack();
        }

        public void SetInputText(string text)
        {
            if (this._webView == null)
                return;
            this._webView.SetInputText(text);
        }

        public void SetMotionEvent(MotionActions action, float x, float y)
        {
            if (this._webView == null)
                return;
            this._webView.SetMotionEvent(action, x, y);
        }

        public void ClickTo(int x, int y)
        {
            if (this._webView == null)
                return;
            this._webView.ClickTo(x, y);
        }

        public void ScrollBy(int x, int y, float scrollTime = 0.5f)
        {
            if (this._webView == null)
                return;
            this._webView.ScrollBy(x, y, scrollTime);
        }

        public bool IsReady
        {
            get
            {
                return this._webView != null && this._webView.IsReady;
            }
        }

        public byte[] FramePixels
        {
            get
            {
                return this._webView != null ? this._webView.FramePixels : (byte[])null;
            }
        }

        public bool DeviceKeyboard
        {
            set
            {
                if (this._webView == null)
                    return;
                this._webView.DeviceKeyboard = value;
            }
        }

        public int Width
        {
            get
            {
                return this._webView != null ? this._webView.Width : 0;
            }
        }

        public int Height
        {
            get
            {
                return this._webView != null ? this._webView.Height : 0;
            }
        }

        public int ContentHeight
        {
            get
            {
                return this._webView != null ? this._webView.ContentHeight : 0;
            }
        }

        public void CallFunction(string functionName, params string[] args)
        {
            _webView.CallFunction(functionName); _webView.CallFunction(functionName, args);
        }
    }
}
