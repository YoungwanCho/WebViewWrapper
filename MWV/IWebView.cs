using System;
using UnityEngine;

namespace MWV
{
	internal interface IWebView
	{
		int ContentHeight
		{
			get;
		}

		bool DeviceKeyboard
		{
			set;
		}

		WebViewManagerEvents EventManager
		{
			get;
		}

		byte[] FramePixels
		{
			get;
		}

		int Height
		{
			get;
		}

		bool IsReady
		{
			get;
		}

		GameObject OutputObject
		{
			get;
			set;
		}

		WebStates State
		{
			get;
		}

		object StateValue
		{
			get;
		}

		Uri Url
		{
			get;
		}

		int Width
		{
			get;
		}

		void AddWebListener(IWebListener listener);

		void ClickTo(int x, int y);

		void Load(Uri url);

		void Load(string data);

		bool MoveBack();

		bool MoveForward();

		void Release();

		void RemoveWebListener(IWebListener listener);

		void ScrollBy(int x, int y);

		void SetInputText(string text);

		void UnLoad(bool resetTexture);

		void UnLoad();
	}
}