using UnityEngine;

namespace MWV
{
    internal class WebArguments
    {
        private bool _useNativeWeb;

        private Vector2 _fixedPageSize;

        public Vector2 FixedPageSize
        {
            get
            {
                return this._fixedPageSize;
            }
            set
            {
                this._fixedPageSize = value;
            }
        }

        public bool UseNativePlayer
        {
            get
            {
                return this._useNativeWeb;
            }
            set
            {
                this._useNativeWeb = value;
            }
        }

        public WebArguments()
        {
        }
    }
}