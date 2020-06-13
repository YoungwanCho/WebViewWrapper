using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace MWV
{
    public class WebViewHelper
    {
        private static Regex _androidStorageRoot = new Regex("(^\\/.*)Android");

        public static void ApplyTextureToRenderingObject(Texture2D texture, GameObject renderingObject)
        {
            if ((Object)renderingObject == (Object)null)
                return;
            RawImage component1 = renderingObject.GetComponent<RawImage>();
            if ((Object)component1 != (Object)null)
            {
                component1.texture = (Texture)texture;
            }
            else
            {
                MeshRenderer component2 = renderingObject.GetComponent<MeshRenderer>();
                if ((Object)component2 != (Object)null && (Object)component2.material != (Object)null)
                    component2.material.mainTexture = (Texture)texture;
                else
                    Debug.LogError((object)(renderingObject.name + ": don't have 'RawImage' or 'MeshRenderer' component - ignored"));
            }
        }

        internal static Texture2D GenWebViewTexture(int width, int height)
        {
            return Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer ? new Texture2D(width, height, TextureFormat.BGRA32, false, true) : new Texture2D(width, height, TextureFormat.RGBA32, false);
        }

        public static bool HasMeshRenderer(GameObject gameObject)
        {
            return !((Object)gameObject == (Object)null) && (Object)gameObject.GetComponent<MeshRenderer>() != (Object)null;
        }

        public static Vector2 GetPixelSizeOfObject(GameObject obj, Camera camera)
        {
            if ((Object)obj == (Object)null)
                return Vector2.zero;
            if (WebViewHelper.HasMeshRenderer(obj))
                return (Object)camera == (Object)null ? Vector2.zero : WebViewHelper.GetPixelSizeOfMeshRenderer(obj.GetComponent<MeshRenderer>(), camera);
            RawImage component = obj.GetComponent<RawImage>();
            return (Object)component != (Object)null ? WebViewHelper.GetPixelSizeOfRawImage(component) : Vector2.zero;
        }

        public static Vector2 GetPixelSizeOfMeshRenderer(
          MeshRenderer meshRenderer,
          Camera camera)
        {
            if ((Object)meshRenderer == (Object)null)
                return Vector2.zero;
            Camera camera1 = camera;
            double x1 = (double)meshRenderer.bounds.min.x;
            Bounds bounds1 = meshRenderer.bounds;
            double y1 = (double)bounds1.min.y;
            bounds1 = meshRenderer.bounds;
            double z1 = (double)bounds1.min.z;
            Vector3 position1 = new Vector3((float)x1, (float)y1, (float)z1);
            Vector3 screenPoint1 = camera1.WorldToScreenPoint(position1);
            Camera camera2 = camera;
            bounds1 = meshRenderer.bounds;
            double x2 = (double)bounds1.max.x;
            Bounds bounds2 = meshRenderer.bounds;
            double y2 = (double)bounds2.max.y;
            bounds2 = meshRenderer.bounds;
            double z2 = (double)bounds2.min.z;
            Vector3 position2 = new Vector3((float)x2, (float)y2, (float)z2);
            Vector3 screenPoint2 = camera2.WorldToScreenPoint(position2);
            return new Vector2((float)(int)Mathf.Abs(screenPoint2.x - screenPoint1.x), (float)(int)Mathf.Abs(screenPoint2.y - screenPoint1.y));
        }

        public static Vector2 GetPixelSizeOfRawImage(RawImage rawImage)
        {
            return (Object)rawImage == (Object)null ? Vector2.zero : new Vector2(rawImage.rectTransform.rect.width, rawImage.rectTransform.rect.height);
        }

        public static Color GetAverageColor(byte[] frameBuffer)
        {
            if (frameBuffer == null)
                return Color.black;
            long num1 = 0;
            long num2 = 0;
            long num3 = 0;
            long num4 = 0;
            int num5 = frameBuffer.Length / 4;
            if (num5 <= 0 || num5 % 4 != 0)
                return Color.black;
            for (int index = 0; index < frameBuffer.Length; index += 4)
            {
                num1 += (long)frameBuffer[index];
                num2 += (long)frameBuffer[index + 1];
                num3 += (long)frameBuffer[index + 2];
                num4 += (long)frameBuffer[index + 3];
            }
            return new Color((float)(num1 / (long)num5), (float)(num2 / (long)num5), (float)(num3 / (long)num5), (float)(num4 / (long)num5));
        }

        public static Color32[] GetFrameColors(byte[] frameBuffer)
        {
            Color32[] color32Array = new Color32[frameBuffer.Length / 4];
            for (int index = 0; index < frameBuffer.Length; index += 4)
            {
                Color32 color32 = new Color32(frameBuffer[index], frameBuffer[index + 1], frameBuffer[index + 2], frameBuffer[index + 3]);
                color32Array[index / 4] = color32;
            }
            return color32Array;
        }

        public static string GetDeviceRootPath()
        {
            Match match = WebViewHelper._androidStorageRoot.Match(Application.persistentDataPath);
            return match.Length > 1 ? match.Groups[1].Value : Application.persistentDataPath;
        }

        public static bool IsSupportedPlatform
        {
            get
            {
                return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
            }
        }

        public static bool IsAndroidPlatform
        {
            get
            {
                return Application.platform == RuntimePlatform.Android;
            }
        }

        public static bool IsIPhonePlatform
        {
            get
            {
                return Application.platform == RuntimePlatform.IPhonePlayer;
            }
        }
    }
}
