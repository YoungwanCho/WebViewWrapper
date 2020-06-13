using MWV.Wrappers;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace MWV
{
    public class WebViewIPhone : IWebView
    {
        private const float UPDATE_FRAME_RATE = 45f;
        private WrapperInternal _wrapper;
        private IntPtr _pluginObj;
        private IntPtr _shareTexturedPtr;
        private MonoBehaviour _monoObject;
        private GameObject _outputObject;
        private Vector2 _size;
        private Texture2D _viewTexture;
        private bool _isStarted;
        private bool _isReady;
        private bool _isTextureExist;
        private float _framesInSecond;
        private float _updateFrameTime;
        private WebViewManagerEvents _eventManager;
        private WebViewBufferPage _pageBuffer;
        private IEnumerator _updatePageTextureEnum;

        internal WebViewIPhone(MonoBehaviour monoObject, GameObject outputObject, Vector2 size)
        {
            this._monoObject = monoObject;
            this._outputObject = outputObject;
            this._size = size;
            this._wrapper = Wrapper.Instance.PlatformWrapper as WrapperInternal;
            this._pluginObj = (IntPtr)this._wrapper.NativeHelperInit();
            this._wrapper.NativeHelperInitWebView(this._pluginObj, (int)this._size.x, (int)this._size.y);
            this._eventManager = new WebViewManagerEvents(this._monoObject, (IWebView)this);
            this._framesInSecond = 0.02222222f;
        }

        private Uri LoadUri
        {
            set
            {
                if (!(this._pluginObj != IntPtr.Zero))
                    return;
                string str = Application.streamingAssetsPath + value.AbsolutePath;
                if (File.Exists(str))
                    value = new Uri(str);
                if (!(this._pluginObj != IntPtr.Zero))
                    return;
                this._wrapper.WebSetUrl(this._pluginObj, value.AbsoluteUri);
            }
        }

        private string LoadData
        {
            set
            {
                if (!(this._pluginObj != IntPtr.Zero))
                    return;
                this._wrapper.WebSetData(this._pluginObj, value);
            }
        }

        private IEnumerator UpdatePageTexture()
        {
            while (true)
            {
                float time = Time.time;
                if ((double)time >= (double)this._updateFrameTime)
                {
                    this._shareTexturedPtr = this._wrapper.NativeHelperGetTexture(this._pluginObj);
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
                            this._wrapper.NativeHelperSetPixelsBuffer(this._pluginObj, this._pageBuffer.FramePixelsAddr, this._pageBuffer.Width, this._pageBuffer.Height);
                        }
                        this._viewTexture = Texture2D.CreateExternalTexture(x, y, TextureFormat.BGRA32, false, false, this._shareTexturedPtr);
                        WebViewHelper.ApplyTextureToRenderingObject(this._viewTexture, this._outputObject);
                        this._isTextureExist = true;
                    }
                    if (!this._isReady)
                    {
                        this._isReady = true;
                        this._eventManager.SetEvent(WebStates.Prepared, (object)this._viewTexture);
                    }
                    this._viewTexture.UpdateExternalTexture(this._shareTexturedPtr);
                    this._updateFrameTime = time + this._framesInSecond;
                }
                yield return (object)null;
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
                WebStates webStates = WebStates.Empty;
                if (this._pluginObj != IntPtr.Zero)
                    webStates = this._wrapper.WebGetState(this._pluginObj);
                return webStates;
            }
        }

        public object StateValue
        {
            get
            {
                return this._pluginObj != IntPtr.Zero ? this._wrapper.WebGetStateValue(this._pluginObj) : (object)null;
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
            IntPtr pluginObj = this._pluginObj;
            if (!this._isStarted && this._eventManager != null)
                this._eventManager.StartListener();
            this.LoadUri = url;
            if (this._updatePageTextureEnum != null)
                return;
            this._updatePageTextureEnum = this.UpdatePageTexture();
            this._monoObject.StartCoroutine(this._updatePageTextureEnum);
        }

        public void Load(string data)
        {
            IntPtr pluginObj = this._pluginObj;
            if (!this._isStarted && this._eventManager != null)
                this._eventManager.StartListener();
            this.LoadData = data;
            if (this._updatePageTextureEnum != null)
                return;
            this._updatePageTextureEnum = this.UpdatePageTexture();
            this._monoObject.StartCoroutine(this._updatePageTextureEnum);
        }

        public void UnLoad(bool resetTexture)
        {
            IntPtr pluginObj = this._pluginObj;
            if (this._isStarted)
            {
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
            if (this._pluginObj != IntPtr.Zero && this._updatePageTextureEnum != null)
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
            if (!(this._pluginObj != IntPtr.Zero))
                return;
            this._wrapper.WebRelease(this._pluginObj);
        }

        public Uri Url
        {
            get
            {
                return this._pluginObj != IntPtr.Zero ? new Uri(this._wrapper.WebGetUrl(this._pluginObj)) : (Uri)null;
            }
        }

        public bool MoveForward()
        {
            return this._pluginObj != IntPtr.Zero && this._wrapper.WebMoveForward(this._pluginObj);
        }

        public bool MoveBack()
        {
            return this._pluginObj != IntPtr.Zero && this._wrapper.WebMoveBack(this._pluginObj);
        }

        public void SetInputText(string text)
        {
            if (!(this._pluginObj != IntPtr.Zero))
                return;
            this._wrapper.WebSetInputText(this._pluginObj, text);
        }

        public void SetMotionEvent(MotionActions action, float x, float y)
        {
        }

        public void ClickTo(int x, int y)
        {
            if (!(this._pluginObj != IntPtr.Zero))
                return;
            this._wrapper.WebClickTo(this._pluginObj, x, y);
        }

        public void ScrollBy(int x, int y, float scrollTime = 0.5f)
        {
            if (!(this._pluginObj != IntPtr.Zero))
                return;
            this._wrapper.WebScrollBy(this._pluginObj, x, y);
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
                if (this._pluginObj != IntPtr.Zero)
                    this._wrapper.NativeHelperUpdatePixelsBuffer(this._pluginObj);
                return this._pageBuffer.FramePixels;
            }
        }

        public bool DeviceKeyboard
        {
            set
            {
                if (!(this._pluginObj != IntPtr.Zero))
                    return;
                this._wrapper.WebShowKeyboard(this._pluginObj, value);
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
                return this._pluginObj != IntPtr.Zero ? this._wrapper.WebContentHeight(this._pluginObj) : 0;
            }
        }

        public void CallFunction(string functionName)
        {
            //throw new NotImplementedException();
        }
    }
}
