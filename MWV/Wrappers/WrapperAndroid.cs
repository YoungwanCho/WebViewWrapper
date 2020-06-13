using System;
using System.Runtime.InteropServices;

namespace MWV.Wrappers
{
    internal class WrapperAndroid : INativeWrapperHelper
    {
        public const string LIBRARY_NAME = "MobileWebView";

        [DllImport("MobileWebView")]
        private static extern int MWVNativeInit();

        [DllImport("MobileWebView")]
        private static extern void MWVNativeUpdateIndex(int mpInstance);

        [DllImport("MobileWebView")]
        private static extern void MWVNativeSetTexture(int mpInstance, IntPtr texture);

        [DllImport("MobileWebView")]
        private static extern void MWVNativeSetPixelsBuffer(
          int mpInstance,
          IntPtr buffer,
          int width,
          int height);

        [DllImport("MobileWebView")]
        private static extern void MWVNativeUpdateFrameBuffer(int mpInstance);

        [DllImport("MobileWebView")]
        private static extern IntPtr MWVNativeGetUnityRenderCallback();

        [DllImport("MobileWebView")]
        private static extern IntPtr MWVNativeGetStartRenderCallback();

        [DllImport("MobileWebView")]
        private static extern IntPtr MWVNativeGetUpdateSurfaceTextureCallback();

        [DllImport("MobileWebView")]
        private static extern IntPtr MWVNativeGetUpdateFrameBufferCallback();

        public string LibraryName
        {
            get
            {
                return "MobileWebView";
            }
        }

        public int NativeHelperInit()
        {
            return WrapperAndroid.MWVNativeInit();
        }

        public void NativeHelperUpdateIndex(IntPtr mpInstance)
        {
            WrapperAndroid.MWVNativeUpdateIndex((int)mpInstance);
        }

        public void NativeHelperSetTexture(IntPtr mpInstance, IntPtr texture)
        {
            WrapperAndroid.MWVNativeSetTexture((int)mpInstance, texture);
        }

        public IntPtr NativeHelperGetTexture(IntPtr mpInstance)
        {
            return IntPtr.Zero;
        }

        public void NativeHelperUpdateTexture(IntPtr mpInstance, IntPtr texture)
        {
        }

        public void NativeHelperSetPixelsBuffer(
          IntPtr mpInstance,
          IntPtr buffer,
          int width,
          int height)
        {
            WrapperAndroid.MWVNativeSetPixelsBuffer((int)mpInstance, buffer, width, height);
        }

        public void NativeHelperUpdatePixelsBuffer(IntPtr mpInstance)
        {
            WrapperAndroid.MWVNativeUpdateFrameBuffer((int)mpInstance);
        }

        public IntPtr NativeHelperGetUnityRenderCallback()
        {
            return WrapperAndroid.MWVNativeGetUnityRenderCallback();
        }

        public IntPtr NativeHelperGetStartRenderCallback()
        {
            return WrapperAndroid.MWVNativeGetStartRenderCallback();
        }

        public IntPtr NativeHelperGetUpdateSurfaceTextureCallback()
        {
            return WrapperAndroid.MWVNativeGetUpdateSurfaceTextureCallback();
        }

        public IntPtr NativeHelperGetUpdateFrameBufferCallback()
        {
            return WrapperAndroid.MWVNativeGetUpdateFrameBufferCallback();
        }
    }
}
