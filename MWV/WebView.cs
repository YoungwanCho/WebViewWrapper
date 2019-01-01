using System;
using UnityEngine;

namespace MWV
{
	public class WebView : IWebView
	{
		private object _webViewObject;

		private IWebView _webView;

		public int ContentHeight
		{
			get
			{
				if (this._webView == null)
				{
					return 0;
				}
				return this._webView.ContentHeight;
			}
		}

		public bool DeviceKeyboard
		{
			set
			{
				if (this._webView != null)
				{
					this._webView.DeviceKeyboard = value;
				}
			}
		}

		public WebViewManagerEvents EventManager
		{
			get
			{
				if (this._webView == null)
				{
					return null;
				}
				return this._webView.EventManager;
			}
		}

		public byte[] FramePixels
		{
			get
			{
				if (this._webView == null)
				{
					return null;
				}
				return this._webView.FramePixels;
			}
		}

		public int Height
		{
			get
			{
				if (this._webView == null)
				{
					return 0;
				}
				return this._webView.Height;
			}
		}

		public bool IsReady
		{
			get
			{
				if (this._webView == null)
				{
					return false;
				}
				return this._webView.IsReady;
			}
		}

		public GameObject OutputObject
		{
			get
			{
				if (this._webView == null)
				{
					return null;
				}
				return this._webView.OutputObject;
			}
			set
			{
				if (this._webView != null)
				{
					this._webView.OutputObject = value;
				}
			}
		}

		public object PlatformWebView
		{
			get
			{
				return this._webViewObject;
			}
		}

		public WebStates State
		{
			get
			{
				if (this._webView == null)
				{
					return WebStates.Empty;
				}
				return this._webView.State;
			}
		}

		public object StateValue
		{
			get
			{
				if (this._webView == null)
				{
					return null;
				}
				return this._webView.StateValue;
			}
		}

		public Uri Url
		{
			get
			{
				if (this._webView == null)
				{
					return null;
				}
				return this._webView.Url;
			}
		}

		public int Width
		{
			get
			{
				if (this._webView == null)
				{
					return 0;
				}
				return this._webView.Width;
			}
		}

		public WebView(MonoBehaviour monoObject, GameObject outputObject) : this(monoObject, outputObject, new Vector2((float)Screen.width, (float)Screen.height))
		{
		}

		public WebView(MonoBehaviour monoObject, GameObject outputObject, Vector2 size)
		{
			if (WebViewHelper.IsSupportedPlatform)
			{
				if (size.x <= 0f || size.y <= 0f)
				{
					Debug.LogWarning("Unsupported size (width <= 0, height <= 0): will be use device screen size by default");
					size = new Vector2((float)Screen.width, (float)Screen.height);
				}
				if (WebViewHelper.IsAndroidPlatform)
				{
					this._webViewObject = new WebViewAndroid(monoObject, outputObject, size);
				}
				if (WebViewHelper.IsIPhonePlatform)
				{
					this._webViewObject = new WebViewIPhone(monoObject, outputObject, size);
				}
			}
			if (this._webViewObject == null)
			{
				Debug.LogWarning("This platform is unsupported for MWV asset, all functionality will be ignored!");
				return;
			}
			if (this._webViewObject is IWebView)
			{
				this._webView = this._webViewObject as IWebView;
			}
		}

		public void AddWebListener(IWebListener listener)
		{
			if (this._webView != null)
			{
				this._webView.AddWebListener(listener);
			}
		}

		public void ClickTo(int x, int y)
		{
			if (this._webView != null)
			{
				this._webView.ClickTo(x, y);
			}
		}

		public void Load(Uri url)
		{
			if (this._webView != null)
			{
				this._webView.Load(url);
			}
		}

		public void Load(string data)
		{
			if (this._webView != null)
			{
				this._webView.Load(data);
			}
		}

		public bool MoveBack()
		{
			if (this._webView == null)
			{
				return false;
			}
			return this._webView.MoveBack();
		}

		public bool MoveForward()
		{
			if (this._webView == null)
			{
				return false;
			}
			return this._webView.MoveForward();
		}

		public void Release()
		{
			if (this._webView != null)
			{
				this._webView.Release();
			}
		}

		public void RemoveWebListener(IWebListener listener)
		{
			if (this._webView != null)
			{
				this._webView.RemoveWebListener(listener);
			}
		}

		public void ScrollBy(int x, int y)
		{
			if (this._webView != null)
			{
				this._webView.ScrollBy(x, y);
			}
		}

		public void SetInputText(string text)
		{
			if (this._webView != null)
			{
				this._webView.SetInputText(text);
			}
		}

		public void UnLoad(bool resetTexture)
		{
			if (this._webView != null)
			{
				this._webView.UnLoad(resetTexture);
			}
		}

		public void UnLoad()
		{
			this.UnLoad(true);
		}

        public void CallFunction(string functionName)
        {
            _webView.CallFunction(functionName);
        }
    }
}