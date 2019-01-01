using System;
using System.Runtime.InteropServices;

internal class WebViewBufferPage
{
	private const string CHROMA = "RV32";

	private const int PIXEL_SIZE_RGBA = 4;

	private readonly int _width;

	private readonly int _height;

	private readonly int _stride;

	private readonly int _lines;

	private readonly byte[] _framePixels;

	private GCHandle _gcHandle;

	public string Chroma
	{
		get
		{
			return "RV32";
		}
	}

	public byte[] FramePixels
	{
		get
		{
			return this._framePixels;
		}
	}

	internal IntPtr FramePixelsAddr
	{
		get
		{
			if (!this._gcHandle.IsAllocated)
			{
				this._gcHandle = GCHandle.Alloc(this._framePixels, GCHandleType.Pinned);
			}
			return this._gcHandle.AddrOfPinnedObject();
		}
	}

	public int Height
	{
		get
		{
			return this._height;
		}
	}

	public int Lines
	{
		get
		{
			return this._lines;
		}
	}

	public int Stride
	{
		get
		{
			return this._stride;
		}
	}

	public int Width
	{
		get
		{
			return this._width;
		}
	}

	public WebViewBufferPage(int width, int height)
	{
		this._width = width;
		this._height = height;
		this._stride = this._width * 4;
		this._lines = this._height;
		this._framePixels = new byte[this._stride * this._lines];
	}

	internal void ClearFramePixels()
	{
		if (this._gcHandle.IsAllocated)
		{
			this._gcHandle.Free();
		}
	}
}