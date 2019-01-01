using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace MWV
{
    public class WebViewHelper
    {
        private static Regex _androidStorageRoot;

        public static bool IsAndroidPlatform
        {
            get
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    return true;
                }
                return false;
            }
        }

        public static bool IsIPhonePlatform
        {
            get
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    return true;
                }
                return false;
            }
        }

        public static bool IsSupportedPlatform
        {
            get
            {
                if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
                {
                    return false;
                }
                return true;
            }
        }

        static WebViewHelper()
        {
            WebViewHelper._androidStorageRoot = new Regex("(^\\/.*)Android");
        }

        public WebViewHelper()
        {
        }

        public static void ApplyTextureToRenderingObject(Texture2D texture, GameObject renderingObject)
        {
            if (renderingObject == null)
            {
                return;
            }
            RawImage component = renderingObject.GetComponent<RawImage>();
            if (component != null)
            {
                component.texture = (texture);
                return;
            }
            MeshRenderer meshRenderer = renderingObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null && meshRenderer.material != null)
            {
                meshRenderer.material.mainTexture = texture;
                return;
            }
            Debug.LogError(string.Concat(renderingObject.name, ": don't have 'RawImage' or 'MeshRenderer' component - ignored"));
        }

        internal static Texture2D GenWebViewTexture(int width, int height)
        {
            if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.WindowsPlayer)
            {
                return new Texture2D(width, height, TextureFormat.RGBA32, false);
            }
            return new Texture2D(width, height, TextureFormat.BGRA32, false, true);
        }

        public static Color GetAverageColor(byte[] frameBuffer)
        {
            if (frameBuffer == null)
            {
                return Color.black;
            }
            ulong num = (long)0;
            ulong num1 = (long)0;
            ulong num2 = (long)0;
            ulong num3 = (long)0;
            int length = (int)frameBuffer.Length / 4;
            if (length <= 0 || length % 4 != 0)
            {
                return Color.black;
            }
            for (int i = 0; i < (int)frameBuffer.Length; i += 4)
            {
                num += (ulong)frameBuffer[i];
                num1 += (ulong)frameBuffer[i + 1];
                num2 += (ulong)frameBuffer[i + 2];
                num3 += (ulong)frameBuffer[i + 3];
            }
            return new Color((float)(num / (ulong)length), (float)(num1 / (ulong)length), (float)(num2 / (ulong)length), (float)(num3 / (ulong)length));
        }

        public static string GetDeviceRootPath()
        {
            Match match = WebViewHelper._androidStorageRoot.Match(Application.persistentDataPath);
            if (match.Length <= 1)
            {
                return Application.persistentDataPath;
            }
            return match.Groups[1].Value;
        }

        public static Color32[] GetFrameColors(byte[] frameBuffer)
        {
            Color32[] color32Array = new Color32[(int)frameBuffer.Length / 4];
            for (int i = 0; i < (int)frameBuffer.Length; i += 4)
            {
                Color32 color32 = new Color32(frameBuffer[i], frameBuffer[i + 1], frameBuffer[i + 2], frameBuffer[i + 3]);
                color32Array[i / 4] = color32;
            }
            return color32Array;
        }

        public static Vector2 GetPixelSizeOfMeshRenderer(MeshRenderer meshRenderer, Camera camera)
        {
            if (meshRenderer == null)
            {
                return Vector2.zero;
            }
            float single = meshRenderer.bounds.min.x;
            float single1 = meshRenderer.bounds.min.y;
            Bounds bound = meshRenderer.bounds;
            Vector3 screenPoint = camera.WorldToScreenPoint(new Vector3(single, single1, bound.min.z));
            float single2 = meshRenderer.bounds.max.x;
            float single3 = meshRenderer.bounds.max.y;
            bound = meshRenderer.bounds;
            Vector3 vector3 = camera.WorldToScreenPoint(new Vector3(single2, single3, bound.min.z));
            float single4 = Mathf.Abs(vector3.x - screenPoint.x);
            float single5 = Mathf.Abs(vector3.y - screenPoint.y);
            return new Vector2((float)((int)single4), (float)((int)single5));
        }

        public static Vector2 GetPixelSizeOfObject(GameObject obj, Camera camera)
        {
            if (obj == null)
            {
                return Vector2.zero;
            }
            if (WebViewHelper.HasMeshRenderer(obj))
            {
                if (camera == null)
                {
                    return Vector2.zero;
                }
                return WebViewHelper.GetPixelSizeOfMeshRenderer(obj.GetComponent<MeshRenderer>(), camera);
            }
            RawImage component = obj.GetComponent<RawImage>();
            if (component == null)
            {
                return Vector2.zero;
            }
            return WebViewHelper.GetPixelSizeOfRawImage(component);
        }

        public static Vector2 GetPixelSizeOfRawImage(RawImage rawImage)
        {
            if (rawImage == null)
            {
                return Vector2.zero;
            }
            Rect _rectTransform = rawImage.rectTransform.rect;
            float single = _rectTransform.width;
            _rectTransform = rawImage.rectTransform.rect;
            return new Vector2(single, _rectTransform.height);
        }

        public static bool HasMeshRenderer(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return false;
            }
            return gameObject.GetComponent<MeshRenderer>() != null;
        }
    }
}