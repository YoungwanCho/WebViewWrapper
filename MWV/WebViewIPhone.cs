using MWV.Wrappers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
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

		public int ContentHeight
		{
			get
			{
				if (this._pluginObj == IntPtr.Zero)
				{
					return 0;
				}
				return this._wrapper.WebContentHeight(this._pluginObj);
			}
		}

		public bool DeviceKeyboard
		{
			set
			{
				if (this._pluginObj != IntPtr.Zero)
				{
					this._wrapper.WebShowKeyboard(this._pluginObj, value);
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
				if (this._pluginObj != IntPtr.Zero)
				{
					this._wrapper.NativeHelperUpdatePixelsBuffer(this._pluginObj);
				}
				return this._pageBuffer.FramePixels;
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

		private string LoadData
		{
			set
			{
				if (this._pluginObj != IntPtr.Zero)
				{
					this._wrapper.WebSetData(this._pluginObj, value);
				}
			}
		}

		private Uri LoadUri
		{
			set
			{
				if (this._pluginObj != IntPtr.Zero)
				{
					string str = string.Concat(Application.streamingAssetsPath, value.AbsolutePath);
					if (File.Exists(str))
					{
						value = new Uri(str);
					}
					if (this._pluginObj != IntPtr.Zero)
					{
						this._wrapper.WebSetUrl(this._pluginObj, value.AbsoluteUri);
					}
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
				WebStates webState = WebStates.Empty;
				if (this._pluginObj != IntPtr.Zero)
				{
					webState = this._wrapper.WebGetState(this._pluginObj);
				}
				return webState;
			}
		}

		public object StateValue
		{
			get
			{
				if (this._pluginObj == IntPtr.Zero)
				{
					return null;
				}
				return this._wrapper.WebGetStateValue(this._pluginObj);
			}
		}

		public Uri Url
		{
			get
			{
				if (this._pluginObj == IntPtr.Zero)
				{
					return null;
				}
				return new Uri(this._wrapper.WebGetUrl(this._pluginObj));
			}
		}

		public int Width
		{
			get
			{
				return (int)this._size.x;
			}
		}

		internal WebViewIPhone(MonoBehaviour monoObject, GameObject outputObject, Vector2 size)
		{
			this._monoObject = monoObject;
			this._outputObject = outputObject;
			this._size = size;
			this._wrapper = Wrapper.Instance.PlatformWrapper as WrapperInternal;
			this._pluginObj = (IntPtr)this._wrapper.NativeHelperInit();
			this._wrapper.NativeHelperInitWebView(this._pluginObj, (int)this._size.x, (int)this._size.y);
			this._eventManager = new WebViewManagerEvents(this._monoObject, this);
			this._framesInSecond = 0.0222222228f;
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
			if (this._pluginObj != IntPtr.Zero)
			{
				this._wrapper.WebClickTo(this._pluginObj, x, y);
			}
		}

		public void Load(Uri url)
		{
			IntPtr intPtr = this._pluginObj;
			if (!this._isStarted && this._eventManager != null)
			{
				this._eventManager.StartListener();
			}
			this.LoadUri = url;
			if (this._updatePageTextureEnum == null)
			{
				this._updatePageTextureEnum = this.UpdatePageTexture();
				this._monoObject.StartCoroutine(this._updatePageTextureEnum);
			}
		}

		public void Load(string data)
		{
			IntPtr intPtr = this._pluginObj;
			if (!this._isStarted && this._eventManager != null)
			{
				this._eventManager.StartListener();
			}
			this.LoadData = data;
			if (this._updatePageTextureEnum == null)
			{
				this._updatePageTextureEnum = this.UpdatePageTexture();
				this._monoObject.StartCoroutine(this._updatePageTextureEnum);
			}
		}

		public bool MoveBack()
		{
			if (this._pluginObj == IntPtr.Zero)
			{
				return false;
			}
			return this._wrapper.WebMoveBack(this._pluginObj);
		}

		public bool MoveForward()
		{
			if (this._pluginObj == IntPtr.Zero)
			{
				return false;
			}
			return this._wrapper.WebMoveForward(this._pluginObj);
		}

		public void Release()
		{
			if (this._pluginObj != IntPtr.Zero && this._updatePageTextureEnum != null)
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
			if (this._pluginObj != IntPtr.Zero)
			{
				this._wrapper.WebRelease(this._pluginObj);
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
			if (this._pluginObj != IntPtr.Zero)
			{
				this._wrapper.WebScrollBy(this._pluginObj, x, y);
			}
		}

		public void SetInputText(string text)
		{
			if (this._pluginObj != IntPtr.Zero)
			{
				this._wrapper.WebSetInputText(this._pluginObj, text);
			}
		}

		public void UnLoad(bool resetTexture)
		{
			IntPtr intPtr = this._pluginObj;
			if (this._isStarted)
			{
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
				float single = Time.time;
				if (single >= this._updateFrameTime)
				{
					this._shareTexturedPtr = this._wrapper.NativeHelperGetTexture(this._pluginObj);
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
							this._wrapper.NativeHelperSetPixelsBuffer(this._pluginObj, this._pageBuffer.FramePixelsAddr, this._pageBuffer.Width, this._pageBuffer.Height);
						}
						this._viewTexture = Texture2D.CreateExternalTexture(num, num1, TextureFormat.BGRA32, false, false, this._shareTexturedPtr);
						WebViewHelper.ApplyTextureToRenderingObject(this._viewTexture, this._outputObject);
						this._isTextureExist = true;
					}
					if (!this._isReady)
					{
						this._isReady = true;
						this._eventManager.SetEvent(WebStates.Prepared, this._viewTexture);
					}
					this._viewTexture.UpdateExternalTexture(this._shareTexturedPtr);
					this._updateFrameTime = single + this._framesInSecond;
				}
				yield return null;
			}
		}

        public void CallFunction(string functionName)
        {
            //throw new NotImplementedException();
        }
    }
}