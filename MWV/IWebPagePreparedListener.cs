using UnityEngine;

namespace MWV
{
    public interface IWebPagePreparedListener
    {
        void OnWebPagePrepared(Texture2D videoTexture);
    }
}