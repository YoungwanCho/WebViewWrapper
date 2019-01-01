using System;

namespace MWV.Wrappers
{
    internal interface INativeWrapperHelper
    {
        string LibraryName
        {
            get;
        }

        IntPtr NativeHelperGetTexture(IntPtr mpInstance);

        IntPtr NativeHelperGetUnityRenderCallback();

        int NativeHelperInit();

        void NativeHelperSetPixelsBuffer(IntPtr mpInstance, IntPtr buffer, int width, int height);

        void NativeHelperSetTexture(IntPtr mpInstance, IntPtr texture);

        void NativeHelperUpdateIndex(IntPtr mpInstance);

        void NativeHelperUpdatePixelsBuffer(IntPtr mpInstance);

        void NativeHelperUpdateTexture(IntPtr mpInstance, IntPtr texture);
    }
}