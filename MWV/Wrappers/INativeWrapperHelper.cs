using System;

namespace MWV.Wrappers
{
    internal interface INativeWrapperHelper
    {
        string LibraryName { get; }

        int NativeHelperInit();

        void NativeHelperUpdateIndex(IntPtr mpInstance);

        void NativeHelperSetTexture(IntPtr mpInstance, IntPtr texture);

        IntPtr NativeHelperGetTexture(IntPtr mpInstance);

        void NativeHelperUpdateTexture(IntPtr mpInstance, IntPtr texture);

        void NativeHelperSetPixelsBuffer(IntPtr mpInstance, IntPtr buffer, int width, int height);

        void NativeHelperUpdatePixelsBuffer(IntPtr mpInstance);

        IntPtr NativeHelperGetUnityRenderCallback();
    }
}
