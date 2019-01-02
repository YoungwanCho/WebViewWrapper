using MWV.Wrappers;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace MWV
{
    public class WebViewAndroid : IWebView
    {
        private const string PLUGIN_CLASS_PATH = "unitydirectionkit/mobilewebview/MobileWebView";

        private WrapperAndroid _wrapper;

        private int _playerIndex;

        private AndroidJavaObject _pluginObj;

        private MonoBehaviour _monoObject;

        private GameObject _outputObject;

        private Vector2 _size;

        private Texture2D _viewTexture;

        private bool _isStarted;

        private bool _isReady;

        private bool _isTextureExist;

        private bool _deviceKeyboard;

        private WebViewManagerEvents _eventManager;

        private WebViewBufferPage _pageBuffer;

        private IEnumerator _startLoadProcessEnum;

        private IEnumerator _updatePageTextureEnum;

        public int ContentHeight
        {
            get
            {
                if (this._pluginObj == null)
                {
                    return 0;
                }
                return this._pluginObj.Call<int>("exportContentHeight", new object[0]);
            }
        }

        public bool DeviceKeyboard
        {
            set
            {
                this._deviceKeyboard = value;
                if (this._pluginObj != null && this.IsViewReady)
                {
                    this._pluginObj.Call("exportShowKeyboard", new object[] { value });
                }
            }
        }

        public WebViewManagerEvents EventManager
        {
            get
            {
                return this._eventManager;
            }
        }

        public byte[] FramePixels
        {
            get
            {
                if (!SystemInfo.graphicsMultiThreaded)
                {
                    this._wrapper.NativeHelperUpdatePixelsBuffer((IntPtr)this._playerIndex);
                }
                else
                {
                    GL.IssuePluginEvent(this._wrapper.NativeHelperGetUpdateFrameBufferCallback(), this._playerIndex);
                }
                return this._pageBuffer.FramePixels;
            }
        }

        private long FramesCounter
        {
            get
            {
                if (this._pluginObj == null)
                {
                    return (long)0;
                }
                return (long)this._pluginObj.Call<int>("exportFramesCounter", new object[0]);
            }
        }

        public int Height
        {
            get
            {
                return (int)this._size.y;
            }
        }

        public bool IsReady
        {
            get
            {
                return this._isReady;
            }
        }

        private bool IsViewReady
        {
            get
            {
                if (this._pluginObj == null)
                {
                    return false;
                }
                return this._pluginObj.Call<bool>("exportIsViewReady", new object[0]);
            }
        }

        private string LoadData
        {
            set
            {
                if (this._pluginObj != null)
                {
                    this._pluginObj.Call("exportSetData", new object[] { value });
                }
            }
        }

        private Uri LoadUrl
        {
            set
            {
                if (this._pluginObj != null)
                {
                    string str = string.Concat(WebViewHelper.GetDeviceRootPath(), value.AbsolutePath.TrimStart(new char[] { '/' }));
                    if (File.Exists(str))
                    {
                        value = new Uri(str);
                    }
                    this._pluginObj.Call("exportSetUrl", new object[] { value.AbsoluteUri });
                }
            }
        }

        public GameObject OutputObject
        {
            get
            {
                return this._outputObject;
            }
            set
            {
                if (this._outputObject != null)
                {
                    WebViewHelper.ApplyTextureToRenderingObject(null, this._outputObject);
                }
                this._outputObject = value;
                if (this._viewTexture != null)
                {
                    WebViewHelper.ApplyTextureToRenderingObject(this._viewTexture, this._outputObject);
                }
            }
        }

        public WebStates State
        {
            get
            {
                int num = 0;
                if (this._pluginObj != null)
                {
                    num = this._pluginObj.Call<int>("exportGetState", new object[0]);
                }
                return (WebStates)num;
            }
        }

        public object StateValue
        {
            get
            {
                object obj = this._pluginObj.Call<float>("exportGetStateFloatValue", new object[0]);
                if ((float)obj < 0f)
                {
                    obj = this._pluginObj.Call<long>("exportGetStateLongValue", new object[0]);
                    if ((long)obj < (long)0)
                    {
                        obj = this._pluginObj.Call<string>("exportGetStateStringValue", new object[0]);
                    }
                }
                return obj;
            }
        }

        public Uri Url
        {
            get
            {
                if (this._pluginObj == null)
                {
                    return null;
                }
                return new Uri(this._pluginObj.Call<string>("exportGetUrl", new object[0]));
            }
        }

        public int Width
        {
            get
            {
                return (int)this._size.x;
            }
        }

        internal WebViewAndroid(MonoBehaviour monoObject, GameObject outputObject, Vector2 size)
        {
            this._monoObject = monoObject;
            this._outputObject = outputObject;
            this._size = size;
            this._wrapper = Wrapper.Instance.PlatformWrapper as WrapperAndroid;
            this._playerIndex = this._wrapper.NativeHelperInit();
            this._pluginObj = new AndroidJavaObject("unitydirectionkit/mobilewebview/MobileWebView", new object[] { this._playerIndex, (int)this._size.x, (int)this._size.y });
            this._eventManager = new WebViewManagerEvents(this._monoObject, this);
        }

        public void AddWebListener(IWebListener listener)
        {
            if (this._eventManager != null)
            {
                IWebListener webListener = listener;
                this._eventManager.WebPageStartedListener += new Action<string>(webListener.OnWebPageStarted);
                IWebListener webListener1 = listener;
                this._eventManager.WebPageLoadListener += new Action<int>(webListener1.OnWebPageLoading);
                IWebListener webListener2 = listener;
                this._eventManager.WebPageFinishedListener += new Action<string>(webListener2.OnWebPageFinished);
                IWebListener webListener3 = listener;
                this._eventManager.WebPageErrorListener += new Action<PageErrorCode>(webListener3.OnWebPageError);
                IWebListener webListener4 = listener;
                this._eventManager.WebPageHttpErrorListener += new Action(webListener4.OnWebPageHttpError);
                IWebListener webListener5 = listener;
                this._eventManager.WebPageElementReceivedListener += new Action<string, string, bool>(webListener5.OnWebPageElementReceived);
            }
        }

        public void ClickTo(int x, int y)
        {
            if (this._pluginObj != null)
            {
                this._pluginObj.Call("exportPageClickTo", new object[] { x, y });
            }
        }

        public void Load(Uri url)
        {
            if (this._startLoadProcessEnum != null)
            {
                this._monoObject.StopCoroutine(this._startLoadProcessEnum);
            }
            this._startLoadProcessEnum = this.StartLoadProcess(url.AbsoluteUri, true);
            this._monoObject.StartCoroutine(this._startLoadProcessEnum);
        }

        public void Load(string data)
        {
            if (this._startLoadProcessEnum != null)
            {
                this._monoObject.StopCoroutine(this._startLoadProcessEnum);
            }
            this._startLoadProcessEnum = this.StartLoadProcess(data, false);
            this._monoObject.StartCoroutine(this._startLoadProcessEnum);
        }

        public bool MoveBack()
        {
            if (this._pluginObj == null)
            {
                return false;
            }
            return this._pluginObj.Call<bool>("exportMoveBack", new object[0]);
        }

        public bool MoveForward()
        {
            if (this._pluginObj == null)
            {
                return false;
            }
            return this._pluginObj.Call<bool>("exportMoveForward", new object[0]);
        }

        public void Release()
        {
            if (this._pluginObj != null && this._updatePageTextureEnum != null)
            {
                this.UnLoad();
            }
            if (this._updatePageTextureEnum != null)
            {
                this._monoObject.StopCoroutine(this._updatePageTextureEnum);
                this._updatePageTextureEnum = null;
            }
            if (this._eventManager != null)
            {
                this._eventManager.RemoveAllEvents();
                this._eventManager = null;
            }
            if (this._pluginObj != null)
            {
                this._pluginObj.Call("exportRelease", new object[0]);
            }
        }

        public void RemoveWebListener(IWebListener listener)
        {
            if (this._eventManager != null)
            {
                IWebListener webListener = listener;
                this._eventManager.WebPageStartedListener -= new Action<string>(webListener.OnWebPageStarted);
                IWebListener webListener1 = listener;
                this._eventManager.WebPageLoadListener -= new Action<int>(webListener1.OnWebPageLoading);
                IWebListener webListener2 = listener;
                this._eventManager.WebPageFinishedListener -= new Action<string>(webListener2.OnWebPageFinished);
                IWebListener webListener3 = listener;
                this._eventManager.WebPageErrorListener -= new Action<PageErrorCode>(webListener3.OnWebPageError);
                IWebListener webListener4 = listener;
                this._eventManager.WebPageHttpErrorListener -= new Action(webListener4.OnWebPageHttpError);
                IWebListener webListener5 = listener;
                this._eventManager.WebPageElementReceivedListener -= new Action<string, string, bool>(webListener5.OnWebPageElementReceived);
            }
        }

        public void ScrollBy(int x, int y)
        {
            if (this._pluginObj != null)
            {
                this._pluginObj.Call("exportPageScrollBy", new object[] { x, y });
            }
        }

        public void SetInputText(string text)
        {
            if (this._pluginObj != null)
            {
                this._pluginObj.Call("exportSetInputText", new object[] { text });
            }
        }

        private IEnumerator StartLoadProcess(string data, bool isUrl)
        {
            if (this._pluginObj != null)
            {
                while (!this.IsViewReady)
                {
                    yield return null;
                }
                if (!this._isStarted && this._eventManager != null)
                {
                    this._eventManager.StartListener();
                }
                this.DeviceKeyboard = this._deviceKeyboard;
                if (!isUrl)
                {
                    this.LoadData = data;
                }
                else
                {
                    this.LoadUrl = new Uri(data);
                }
                if (!SystemInfo.graphicsMultiThreaded || this._isReady)
                {
                    this._isStarted = this._pluginObj.Call<bool>("exportStartRender", new object[0]);
                }
                else
                {
                    GL.IssuePluginEvent(this._wrapper.NativeHelperGetStartRenderCallback(), (int)this._pluginObj.GetRawObject());
                    this._isStarted = true;
                }
                if (this._updatePageTextureEnum == null)
                {
                    this._updatePageTextureEnum = this.UpdatePageTexture();
                    this._monoObject.StartCoroutine(this._updatePageTextureEnum);
                }
                if (!this._isStarted)
                {
                    this.UnLoad();
                }
            }
        }

        public void UnLoad(bool resetTexture)
        {
            if (this._pluginObj != null && this._isStarted)
            {
                this._pluginObj.Call("exportStopRender", new object[0]);
                if (this._startLoadProcessEnum != null)
                {
                    this._monoObject.StopCoroutine(this._startLoadProcessEnum);
                    this._startLoadProcessEnum = null;
                }
                if (this._updatePageTextureEnum != null)
                {
                    this._monoObject.StopCoroutine(this._updatePageTextureEnum);
                    this._updatePageTextureEnum = null;
                }
                this._isStarted = false;
                this._isReady = false;
                this._isTextureExist = !resetTexture;
            }
            if (resetTexture && this._viewTexture != null)
            {
                UnityEngine.Object.Destroy(this._viewTexture);
                this._viewTexture = null;
            }
            if (this._eventManager != null)
            {
                this._eventManager.StopListener();
            }
        }

        public void UnLoad()
        {
            this.UnLoad(true);
        }

        private IEnumerator UpdatePageTexture()
        {
            while (true)
            {
                if (this.FramesCounter > (long)0)
                {
                    if (!this._isTextureExist)
                    {
                        if (this._viewTexture != null)
                        {
                            UnityEngine.Object.Destroy(this._viewTexture);
                            this._viewTexture = null;
                        }
                        int num = (int)this._size.x;
                        int num1 = (int)this._size.y;
                        if (this._pageBuffer == null || this._pageBuffer != null && this._pageBuffer.Width != num && this._pageBuffer.Height != num1)
                        {
                            if (this._pageBuffer != null)
                            {
                                this._pageBuffer.ClearFramePixels();
                            }
                            this._pageBuffer = new WebViewBufferPage(num, num1);
                            this._wrapper.NativeHelperSetPixelsBuffer((IntPtr)this._playerIndex, this._pageBuffer.FramePixelsAddr, this._pageBuffer.Width, this._pageBuffer.Height);
                        }
                        this._viewTexture = WebViewHelper.GenWebViewTexture(this._pageBuffer.Width, this._pageBuffer.Height);
                        WebViewHelper.ApplyTextureToRenderingObject(this._viewTexture, this._outputObject);
                        this._wrapper.NativeHelperSetTexture((IntPtr)this._playerIndex, this._viewTexture.GetNativeTexturePtr());
                        this._isTextureExist = true;
                    }
                    if (!this._isReady)
                    {
                        this._isReady = true;
                        this._eventManager.SetEvent(WebStates.Prepared, this._viewTexture);
                    }
                    this.UpdateSurfaceTexture();
                    GL.IssuePluginEvent(this._wrapper.NativeHelperGetUnityRenderCallback(), this._playerIndex);
                }
                yield return null;
            }
        }

        private void UpdateSurfaceTexture()
        {
            if (this._pluginObj != null)
            {
                if (SystemInfo.graphicsMultiThreaded)
                {
                    GL.IssuePluginEvent(this._wrapper.NativeHelperGetUpdateSurfaceTextureCallback(), (int)this._pluginObj.GetRawObject());
                    return;
                }
                this._pluginObj.Call("exportUpdateSurfaceTexture", new object[0]);
            }
        }

        public void CallFunction(string functionName, params string[] args)
        {
            if (this._pluginObj != null)
            {
                this._pluginObj.Call("exportCallFunction", new object[] { functionName, args });
            }
        }
    }
}