using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MWV
{
    public class WebViewManagerEvents
    {
        private MonoBehaviour _monoObject;
        private IWebView _webView;
        private Queue<WebViewManagerEvents.WebEvent> _webViewEvents;
        private IEnumerator _stateListenerEnum;
        private WebStates _replaceState;
        private WebViewManagerEvents.WebEvent _replaceEvent;

        internal WebViewManagerEvents(MonoBehaviour monoObject, IWebView webView)
        {
            this._monoObject = monoObject;
            this._webView = webView;
            this._webViewEvents = new Queue<WebViewManagerEvents.WebEvent>();
        }

        private WebViewManagerEvents.WebEvent Event
        {
            get
            {
                return new WebViewManagerEvents.WebEvent(this._webView.State, this._webView.StateValue);
            }
        }

        private IEnumerator EventManager()
        {
            while (true)
            {
                WebViewManagerEvents.WebEvent webEvent = this.Event;
                if (webEvent != null && webEvent.State != WebStates.Empty)
                    this._webViewEvents.Enqueue(webEvent);
                if (this._webViewEvents.Count <= 0)
                    yield return (object)null;
                else
                    this.CallEvent();
            }
        }

        private void CallEvent()
        {
            WebViewManagerEvents.WebEvent webEvent = this._webViewEvents.Dequeue();
            if (this._replaceState == webEvent.State)
            {
                this._replaceState = WebStates.Empty;
                webEvent = this._replaceEvent;
            }
            switch (webEvent.State)
            {
                case WebStates.Prepared:
                    if (this._webPagePreparedListener == null)
                        break;
                    this._webPagePreparedListener((Texture2D)webEvent.Arg);
                    break;
                case WebStates.Started:
                    if (this._webPageStartedListener == null)
                        break;
                    this._webPageStartedListener(webEvent.GetStringArg);
                    break;
                case WebStates.Loading:
                    if (this._webPageLoadListener == null)
                        break;
                    this._webPageLoadListener((int)webEvent.GetFloatArg);
                    break;
                case WebStates.Finished:
                    if (this._webPageFinishedListener == null)
                        break;
                    this._webPageFinishedListener(webEvent.GetStringArg);
                    break;
                case WebStates.Error:
                    PageErrorCode pageErrorCode = PageErrorCode.ERROR_UNKNOWN;
                    try
                    {
                        pageErrorCode = (PageErrorCode)Enum.Parse(typeof(PageErrorCode), (-webEvent.GetFloatArg).ToString());
                    }
                    catch (Exception ex)
                    {
                    }
                    if (this._webPageErrorListener == null)
                        break;
                    this._webPageErrorListener(pageErrorCode);
                    break;
                case WebStates.HttpError:
                    if (this._webPageHttpErrorListener == null)
                        break;
                    this._webPageHttpErrorListener();
                    break;
                case WebStates.ElementReceived:
                    if (this._webPageElementReceivedListener == null)
                        break;
                    string getStringArg = webEvent.GetStringArg;
                    if (string.IsNullOrEmpty(getStringArg))
                        break;
                    string[] strArray = getStringArg.Split('@');
                    bool flag = false;
                    if (strArray.Length > 2 && !string.IsNullOrEmpty(strArray[2]) && !strArray[2].Equals("undefined"))
                        flag = true;
                    this._webPageElementReceivedListener(strArray[0], strArray[1], flag);
                    break;
            }
        }

        internal void SetEvent(WebStates state)
        {
            this._webViewEvents.Enqueue(new WebViewManagerEvents.WebEvent(state, (object)null));
        }

        internal void SetEvent(WebStates state, object arg)
        {
            this._webViewEvents.Enqueue(new WebViewManagerEvents.WebEvent(state, arg));
        }

        internal void ReplaceEvent(WebStates replaceState, WebStates newState, object arg)
        {
            this._replaceState = replaceState;
            this._replaceEvent = new WebViewManagerEvents.WebEvent(newState, arg);
        }

        public void StartListener()
        {
            this._webViewEvents.Clear();
            if (this._stateListenerEnum != null)
                this._monoObject.StopCoroutine(this._stateListenerEnum);
            this._stateListenerEnum = this.EventManager();
            this._monoObject.StartCoroutine(this._stateListenerEnum);
        }

        public void StopListener()
        {
            if (this._stateListenerEnum != null)
                this._monoObject.StopCoroutine(this._stateListenerEnum);
            while (this._webViewEvents.Count > 0)
                this.CallEvent();
        }

        public void RemoveAllEvents()
        {
            if (this._webPageStartedListener != null)
            {
                foreach (Action<string> invocation in this._webPageStartedListener.GetInvocationList())
                    this._webPageStartedListener -= invocation;
            }
            if (this._webPagePreparedListener != null)
            {
                foreach (Action<Texture2D> invocation in this._webPagePreparedListener.GetInvocationList())
                    this._webPagePreparedListener -= invocation;
            }
            if (this._webPageLoadListener != null)
            {
                foreach (Action<int> invocation in this._webPageLoadListener.GetInvocationList())
                    this._webPageLoadListener -= invocation;
            }
            if (this._webPageFinishedListener != null)
            {
                foreach (Action<string> invocation in this._webPageFinishedListener.GetInvocationList())
                    this._webPageFinishedListener -= invocation;
            }
            if (this._webPageErrorListener != null)
            {
                foreach (Action<PageErrorCode> invocation in this._webPageErrorListener.GetInvocationList())
                    this._webPageErrorListener -= invocation;
            }
            if (this._webPageHttpErrorListener != null)
            {
                foreach (Action invocation in this._webPageHttpErrorListener.GetInvocationList())
                    this._webPageHttpErrorListener -= invocation;
            }
            if (this._webPageElementReceivedListener == null)
                return;
            foreach (Action<string, string, bool> invocation in this._webPageElementReceivedListener.GetInvocationList())
                this._webPageElementReceivedListener -= invocation;
        }

        private event Action<string> _webPageStartedListener;

        public event Action<string> WebPageStartedListener
        {
            add
            {
                this._webPageStartedListener += value;
            }
            remove
            {
                if (this._webPageStartedListener == null)
                    return;
                this._webPageStartedListener -= value;
            }
        }

        private event Action<Texture2D> _webPagePreparedListener;

        public event Action<Texture2D> WebPagePreparedListener
        {
            add
            {
                this._webPagePreparedListener += value;
            }
            remove
            {
                if (this._webPagePreparedListener == null)
                    return;
                this._webPagePreparedListener -= value;
            }
        }

        private event Action<int> _webPageLoadListener;

        public event Action<int> WebPageLoadListener
        {
            add
            {
                this._webPageLoadListener += value;
            }
            remove
            {
                if (this._webPageLoadListener == null)
                    return;
                this._webPageLoadListener -= value;
            }
        }

        private event Action<string> _webPageFinishedListener;

        public event Action<string> WebPageFinishedListener
        {
            add
            {
                this._webPageFinishedListener += value;
            }
            remove
            {
                if (this._webPageFinishedListener == null)
                    return;
                this._webPageFinishedListener -= value;
            }
        }

        private event Action<PageErrorCode> _webPageErrorListener;

        public event Action<PageErrorCode> WebPageErrorListener
        {
            add
            {
                this._webPageErrorListener += value;
            }
            remove
            {
                if (this._webPageErrorListener == null)
                    return;
                this._webPageErrorListener -= value;
            }
        }

        private event Action _webPageHttpErrorListener;

        public event Action WebPageHttpErrorListener
        {
            add
            {
                this._webPageHttpErrorListener += value;
            }
            remove
            {
                if (this._webPageHttpErrorListener == null)
                    return;
                this._webPageHttpErrorListener -= value;
            }
        }

        private event Action<string, string, bool> _webPageElementReceivedListener;

        public event Action<string, string, bool> WebPageElementReceivedListener
        {
            add
            {
                this._webPageElementReceivedListener += value;
            }
            remove
            {
                if (this._webPageElementReceivedListener == null)
                    return;
                this._webPageElementReceivedListener -= value;
            }
        }

        internal class WebEvent
        {
            private WebStates _state;
            private object _arg;

            public WebEvent(WebStates state, object arg)
            {
                this._state = state;
                this._arg = arg;
            }

            public WebStates State
            {
                get
                {
                    return this._state;
                }
            }

            public object Arg
            {
                get
                {
                    return this._arg;
                }
                set
                {
                    this._arg = value;
                }
            }

            public int GetIntArg
            {
                get
                {
                    return this._arg == null || !(this._arg is int) ? 0 : (int)this._arg;
                }
            }

            public float GetFloatArg
            {
                get
                {
                    return this._arg == null || !(this._arg is float) ? 0.0f : (float)this._arg;
                }
            }

            public long GetLongArg
            {
                get
                {
                    return this._arg == null || !(this._arg is long) ? 0L : (long)this._arg;
                }
            }

            public string GetStringArg
            {
                get
                {
                    return this._arg == null || !(this._arg is string) ? string.Empty : (string)this._arg;
                }
            }
        }
    }
}
