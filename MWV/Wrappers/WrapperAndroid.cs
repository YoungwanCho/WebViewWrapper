using System;
using System.Runtime.InteropServices;

namespace MWV.Wrappers
{
    internal class WrapperAndroid : INativeWrapperHelper
    {
        public const string LIBRARY_NAME = "MobileWebView";

        public string LibraryName
        {
            get
            {
                return "MobileWebView";
            }
        }

        public WrapperAndroid()
        {
        }

        [DllImport("MobileWebView", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern IntPtr MWVNativeGetStartRenderCallback();

        [DllImport("MobileWebView", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern IntPtr MWVNativeGetUnityRenderCallback();

        [DllImport("MobileWebView", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern IntPtr MWVNativeGetUpdateFrameBufferCallback();

        [DllImport("MobileWebView", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern IntPtr MWVNativeGetUpdateSurfaceTextureCallback();

        [DllImport("MobileWebView", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern int MWVNativeInit();

        [DllImport("MobileWebView", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern void MWVNativeSetPixelsBuffer(int mpInstance, IntPtr buffer, int width, int height);

        [DllImport("MobileWebView", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern void MWVNativeSetTexture(int mpInstance, IntPtr texture);

        [DllImport("MobileWebView", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern void MWVNativeUpdateFrameBuffer(int mpInstance);

        [DllImport("MobileWebView", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern void MWVNativeUpdateIndex(int mpInstance);

        public IntPtr NativeHelperGetStartRenderCallback()
        {
            return WrapperAndroid.MWVNativeGetStartRenderCallback();
        }

        public IntPtr NativeHelperGetTexture(IntPtr mpInstance)
        {
            return IntPtr.Zero;
        }

        public IntPtr NativeHelperGetUnityRenderCallback()
        {
            return WrapperAndroid.MWVNativeGetUnityRenderCallback();
        }

        public IntPtr NativeHelperGetUpdateFrameBufferCallback()
        {
            return WrapperAndroid.MWVNativeGetUpdateFrameBufferCallback();
        }

        public IntPtr NativeHelperGetUpdateSurfaceTextureCallback()
        {
            return WrapperAndroid.MWVNativeGetUpdateSurfaceTextureCallback();
        }

        public int NativeHelperInit()
        {
            return WrapperAndroid.MWVNativeInit();
        }

        public void NativeHelperSetPixelsBuffer(IntPtr mpInstance, IntPtr buffer, int width, int height)
        {
            WrapperAndroid.MWVNativeSetPixelsBuffer((int)mpInstance, buffer, width, height);
        }

        public void NativeHelperSetTexture(IntPtr mpInstance, IntPtr texture)
        {
            WrapperAndroid.MWVNativeSetTexture((int)mpInstance, texture);
        }

        public void NativeHelperUpdateIndex(IntPtr mpInstance)
        {
            WrapperAndroid.MWVNativeUpdateIndex((int)mpInstance);
        }

        public void NativeHelperUpdatePixelsBuffer(IntPtr mpInstance)
        {
            WrapperAndroid.MWVNativeUpdateFrameBuffer((int)mpInstance);
        }

        public void NativeHelperUpdateTexture(IntPtr mpInstance, IntPtr texture)
        {
        }
    }
}