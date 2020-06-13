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
        private IEnumerator _pageScrollEnum;

        internal WebViewAndroid(MonoBehaviour monoObject, GameObject outputObject, Vector2 size)
        {
            this._monoObject = monoObject;
            this._outputObject = outputObject;
            this._size = size;
            this._wrapper = Wrapper.Instance.PlatformWrapper as WrapperAndroid;
            this._playerIndex = this._wrapper.NativeHelperInit();
            this._pluginObj = new AndroidJavaObject("unitydirectionkit/mobilewebview/MobileWebView", new object[3]
            {
        (object) this._playerIndex,
        (object) (int) this._size.x,
        (object) (int) this._size.y
            });
            this._eventManager = new WebViewManagerEvents(this._monoObject, (IWebView)this);
        }

        private void UpdateSurfaceTexture()
        {
            if (this._pluginObj == null)
                return;
            if (SystemInfo.graphicsMultiThreaded)
                GL.IssuePluginEvent(this._wrapper.NativeHelperGetUpdateSurfaceTextureCallback(), (int)this._pluginObj.GetRawObject());
            else
                this._pluginObj.Call("exportUpdateSurfaceTexture");
        }

        private long FramesCounter
        {
            get
            {
                return this._pluginObj != null ? (long)this._pluginObj.Call<int>("exportFramesCounter") : 0L;
            }
        }

        private bool IsViewReady
        {
            get
            {
                return this._pluginObj != null && this._pluginObj.Call<bool>("exportIsViewReady");
            }
        }

        private Uri LoadUrl
        {
            set
            {
                if (this._pluginObj == null)
                    return;
                string str = WebViewHelper.GetDeviceRootPath() + value.AbsolutePath.TrimStart('/');
                if (File.Exists(str))
                    value = new Uri(str);
                this._pluginObj.Call("exportSetUrl", (object)value.AbsoluteUri);
            }
        }

        private string LoadData
        {
            set
            {
                if (this._pluginObj == null)
                    return;
                this._pluginObj.Call("exportSetData", (object)value);
            }
        }

        private float DisplayDensity
        {
            get
            {
                return this._pluginObj != null ? this._pluginObj.Call<float>("exportDisplayDensity") : 1f;
            }
        }

        private IEnumerator UpdatePageTexture()
        {
            while (true)
            {
                if (this.FramesCounter > 0L)
                {
                    if (!this._isTextureExist)
                    {
                        if ((UnityEngine.Object)this._viewTexture != (UnityEngine.Object)null)
                        {
                            UnityEngine.Object.Destroy((UnityEngine.Object)this._viewTexture);
                            this._viewTexture = (Texture2D)null;
                        }
                        int x = (int)this._size.x;
                        int y = (int)this._size.y;
                        if (this._pageBuffer == null || this._pageBuffer != null && this._pageBuffer.Width != x && this._pageBuffer.Height != y)
                        {
                            if (this._pageBuffer != null)
                                this._pageBuffer.ClearFramePixels();
                            this._pageBuffer = new WebViewBufferPage(x, y);
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
                        this._eventManager.SetEvent(WebStates.Prepared, (object)this._viewTexture);
                    }
                    this.UpdateSurfaceTexture();
                    GL.IssuePluginEvent(this._wrapper.NativeHelperGetUnityRenderCallback(), this._playerIndex);
                }
                yield return (object)null;
            }
        }

        private IEnumerator StartLoadProcess(string data, bool isUrl)
        {
            if (this._pluginObj != null)
            {
                while (!this.IsViewReady)
                    yield return (object)null;
                if (!this._isStarted && this._eventManager != null)
                    this._eventManager.StartListener();
                this.DeviceKeyboard = this._deviceKeyboard;
                if (isUrl)
                    this.LoadUrl = new Uri(data);
                else
                    this.LoadData = data;
                if (SystemInfo.graphicsMultiThreaded && !this._isReady)
                {
                    GL.IssuePluginEvent(this._wrapper.NativeHelperGetStartRenderCallback(), (int)this._pluginObj.GetRawObject());
                    this._isStarted = true;
                }
                else
                    this._isStarted = this._pluginObj.Call<bool>("exportStartRender");
                if (this._updatePageTextureEnum == null)
                {
                    this._updatePageTextureEnum = this.UpdatePageTexture();
                    this._monoObject.StartCoroutine(this._updatePageTextureEnum);
                }
                if (!this._isStarted)
                    this.UnLoad();
            }
        }

        public GameObject OutputObject
        {
            set
            {
                if ((UnityEngine.Object)this._outputObject != (UnityEngine.Object)null)
                    WebViewHelper.ApplyTextureToRenderingObject((Texture2D)null, this._outputObject);
                this._outputObject = value;
                if (!((UnityEngine.Object)this._viewTexture != (UnityEngine.Object)null))
                    return;
                WebViewHelper.ApplyTextureToRenderingObject(this._viewTexture, this._outputObject);
            }
            get
            {
                return this._outputObject;
            }
        }

        public WebViewManagerEvents EventManager
        {
            get
            {
                return this._eventManager;
            }
        }

        public WebStates State
        {
            get
            {
                int num = 0;
                if (this._pluginObj != null)
                    num = this._pluginObj.Call<int>("exportGetState");
                return (WebStates)num;
            }
        }

        public object StateValue
        {
            get
            {
                object obj = (object)this._pluginObj.Call<float>("exportGetStateFloatValue");
                if ((double)(float)obj < 0.0)
                {
                    obj = (object)this._pluginObj.Call<long>("exportGetStateLongValue");
                    if ((long)obj < 0L)
                        obj = (object)this._pluginObj.Call<string>("exportGetStateStringValue");
                }
                return obj;
            }
        }

        public void AddWebListener(IWebListener listener)
        {
            if (this._eventManager == null)
                return;
            this._eventManager.WebPageStartedListener += new Action<string>(((IWebPageStartedListener)listener).OnWebPageStarted);
            this._eventManager.WebPageLoadListener += new Action<int>(((IWebPageLoadingListener)listener).OnWebPageLoading);
            this._eventManager.WebPageFinishedListener += new Action<string>(((IWebPageFinishedListener)listener).OnWebPageFinished);
            this._eventManager.WebPageErrorListener += new Action<PageErrorCode>(((IWebPageErrorListener)listener).OnWebPageError);
            this._eventManager.WebPageHttpErrorListener += new Action(((IWebPageHttpErrorListener)listener).OnWebPageHttpError);
            this._eventManager.WebPageElementReceivedListener += new Action<string, string, bool>(((IWebPageElementReceivedListener)listener).OnWebPageElementReceived);
        }

        public void RemoveWebListener(IWebListener listener)
        {
            if (this._eventManager == null)
                return;
            this._eventManager.WebPageStartedListener -= new Action<string>(((IWebPageStartedListener)listener).OnWebPageStarted);
            this._eventManager.WebPageLoadListener -= new Action<int>(((IWebPageLoadingListener)listener).OnWebPageLoading);
            this._eventManager.WebPageFinishedListener -= new Action<string>(((IWebPageFinishedListener)listener).OnWebPageFinished);
            this._eventManager.WebPageErrorListener -= new Action<PageErrorCode>(((IWebPageErrorListener)listener).OnWebPageError);
            this._eventManager.WebPageHttpErrorListener -= new Action(((IWebPageHttpErrorListener)listener).OnWebPageHttpError);
            this._eventManager.WebPageElementReceivedListener -= new Action<string, string, bool>(((IWebPageElementReceivedListener)listener).OnWebPageElementReceived);
        }

        public void Load(Uri url)
        {
            if (this._startLoadProcessEnum != null)
                this._monoObject.StopCoroutine(this._startLoadProcessEnum);
            this._startLoadProcessEnum = this.StartLoadProcess(url.AbsoluteUri, true);
            this._monoObject.StartCoroutine(this._startLoadProcessEnum);
        }

        public void Load(string data)
        {
            if (this._startLoadProcessEnum != null)
                this._monoObject.StopCoroutine(this._startLoadProcessEnum);
            this._startLoadProcessEnum = this.StartLoadProcess(data, false);
            this._monoObject.StartCoroutine(this._startLoadProcessEnum);
        }

        public void UnLoad(bool resetTexture)
        {
            if (this._pluginObj != null && this._isStarted)
            {
                this._pluginObj.Call("exportStopRender");
                if (this._startLoadProcessEnum != null)
                {
                    this._monoObject.StopCoroutine(this._startLoadProcessEnum);
                    this._startLoadProcessEnum = (IEnumerator)null;
                }
                if (this._updatePageTextureEnum != null)
                {
                    this._monoObject.StopCoroutine(this._updatePageTextureEnum);
                    this._updatePageTextureEnum = (IEnumerator)null;
                }
                this._isStarted = false;
                this._isReady = false;
                this._isTextureExist = !resetTexture;
            }
            if (resetTexture && (UnityEngine.Object)this._viewTexture != (UnityEngine.Object)null)
            {
                UnityEngine.Object.Destroy((UnityEngine.Object)this._viewTexture);
                this._viewTexture = (Texture2D)null;
            }
            if (this._eventManager == null)
                return;
            this._eventManager.StopListener();
        }

        public void UnLoad()
        {
            this.UnLoad(true);
        }

        public void Release()
        {
            if (this._pluginObj != null && this._updatePageTextureEnum != null)
                this.UnLoad();
            if (this._updatePageTextureEnum != null)
            {
                this._monoObject.StopCoroutine(this._updatePageTextureEnum);
                this._updatePageTextureEnum = (IEnumerator)null;
            }
            if (this._eventManager != null)
            {
                this._eventManager.RemoveAllEvents();
                this._eventManager = (WebViewManagerEvents)null;
            }
            if (this._pluginObj == null)
                return;
            this._pluginObj.Call("exportRelease");
        }

        public Uri Url
        {
            get
            {
                return this._pluginObj != null ? new Uri(this._pluginObj.Call<string>("exportGetUrl")) : (Uri)null;
            }
        }

        public bool MoveForward()
        {
            return this._pluginObj != null && this._pluginObj.Call<bool>("exportMoveForward");
        }

        public bool MoveBack()
        {
            return this._pluginObj != null && this._pluginObj.Call<bool>("exportMoveBack");
        }

        public void SetInputText(string text)
        {
            if (this._pluginObj == null)
                return;
            this._pluginObj.Call("exportSetInputText", (object)text);
        }

        public void SetMotionEvent(MotionActions action, float x, float y)
        {
            if (this._pluginObj == null)
                return;
            this._pluginObj.Call("exportSetMotionEvent", (object)(int)action, (object)x, (object)y);
        }

        public void ClickTo(int x, int y)
        {
            this.SetMotionEvent(MotionActions.Began, (float)x, (float)y);
            this.SetMotionEvent(MotionActions.Ended, (float)x, (float)y);
        }

        public void ScrollBy(int x, int y, float scrollTime = 0.5f)
        {
            if (this._pluginObj == null)
                return;
            if (this._pageScrollEnum != null)
                this._monoObject.StopCoroutine(this._pageScrollEnum);
            this._pageScrollEnum = this.PageScrollHandler(x, y, scrollTime);
            this._monoObject.StartCoroutine(this._pageScrollEnum);
        }

        private IEnumerator PageScrollHandler(int x, int y, float scrollTime)
        {
            float progress = 0.0f;
            Vector2 pointStart = this._size / 2f;
            Vector2 pointEnd = new Vector2(pointStart.x + (float)x * this.DisplayDensity, pointStart.y + (float)y * this.DisplayDensity);
            this.SetMotionEvent(MotionActions.Began, pointStart.x, pointStart.y);
            while ((double)progress < 1.0)
            {
                progress = Mathf.Clamp01(progress + Time.deltaTime / scrollTime);
                Vector2 vector2 = Vector2.Lerp(pointStart, pointEnd, progress);
                this.SetMotionEvent(MotionActions.Moved, vector2.x, vector2.y);
                yield return (object)null;
            }
            this.SetMotionEvent(MotionActions.Ended, pointEnd.x, pointEnd.y);
        }

        public bool IsReady
        {
            get
            {
                return this._isReady;
            }
        }

        public byte[] FramePixels
        {
            get
            {
                if (SystemInfo.graphicsMultiThreaded)
                    GL.IssuePluginEvent(this._wrapper.NativeHelperGetUpdateFrameBufferCallback(), this._playerIndex);
                else
                    this._wrapper.NativeHelperUpdatePixelsBuffer((IntPtr)this._playerIndex);
                return this._pageBuffer.FramePixels;
            }
        }

        public bool DeviceKeyboard
        {
            set
            {
                this._deviceKeyboard = value;
                if (this._pluginObj == null || !this.IsViewReady)
                    return;
                this._pluginObj.Call("exportShowKeyboard", (object)value);
                this._pluginObj.Call("exportSetLongClickable", (object)value);
            }
        }

        public int Width
        {
            get
            {
                return (int)this._size.x;
            }
        }

        public int Height
        {
            get
            {
                return (int)this._size.y;
            }
        }

        public int ContentHeight
        {
            get
            {
                return this._pluginObj != null ? this._pluginObj.Call<int>("exportContentHeight") : 0;
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
