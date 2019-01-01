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

        private WebViewManagerEvents.WebEvent Event
        {
            get
            {
                return new WebViewManagerEvents.WebEvent(this._webView.State, this._webView.StateValue);
            }
        }

        internal WebViewManagerEvents(MonoBehaviour monoObject, IWebView webView)
        {
            this._monoObject = monoObject;
            this._webView = webView;
            this._webViewEvents = new Queue<WebViewManagerEvents.WebEvent>();
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
                    {
                        if (this._webPagePreparedListener == null)
                        {
                            break;
                        }
                        this._webPagePreparedListener((Texture2D)webEvent.Arg);
                        return;
                    }
                case WebStates.Started:
                    {
                        if (this._webPageStartedListener == null)
                        {
                            break;
                        }
                        this._webPageStartedListener(webEvent.GetStringArg);
                        return;
                    }
                case WebStates.Loading:
                    {
                        if (this._webPageLoadListener == null)
                        {
                            break;
                        }
                        this._webPageLoadListener((int)webEvent.GetFloatArg);
                        return;
                    }
                case WebStates.Finished:
                    {
                        if (this._webPageFinishedListener == null)
                        {
                            break;
                        }
                        this._webPageFinishedListener(webEvent.GetStringArg);
                        return;
                    }
                case WebStates.Error:
                    {
                        PageErrorCode pageErrorCode = PageErrorCode.ERROR_UNKNOWN;
                        try
                        {
                            Type type = typeof(PageErrorCode);
                            float getFloatArg = -webEvent.GetFloatArg;
                            pageErrorCode = (PageErrorCode)Enum.Parse(type, getFloatArg.ToString());
                        }
                        catch (Exception exception)
                        {
                        }
                        if (this._webPageErrorListener == null)
                        {
                            break;
                        }
                        this._webPageErrorListener(pageErrorCode);
                        return;
                    }
                case WebStates.HttpError:
                    {
                        if (this._webPageHttpErrorListener == null)
                        {
                            break;
                        }
                        this._webPageHttpErrorListener();
                        return;
                    }
                case WebStates.ElementReceived:
                    {
                        if (this._webPageElementReceivedListener == null)
                        {
                            break;
                        }
                        string getStringArg = webEvent.GetStringArg;
                        if (string.IsNullOrEmpty(getStringArg))
                        {
                            break;
                        }
                        string[] strArrays = getStringArg.Split(new char[] { '@' });
                        bool flag = false;
                        if ((int)strArrays.Length > 2 && !string.IsNullOrEmpty(strArrays[2]) && !strArrays[2].Equals("undefined"))
                        {
                            flag = true;
                        }
                        this._webPageElementReceivedListener(strArrays[0], strArrays[1], flag);
                        break;
                    }
                default:
                    {
                        return;
                    }
            }
        }

        private IEnumerator EventManager()
        {
            while (true)
            {
                WebViewManagerEvents.WebEvent @event = this.Event;
                if (@event != null && @event.State != WebStates.Empty)
                {
                    this._webViewEvents.Enqueue(@event);
                }
                if (this._webViewEvents.Count > 0)
                {
                    this.CallEvent();
                }
                else
                {
                    yield return null;
                }
            }
        }

        public void RemoveAllEvents()
        {
            Delegate[] invocationList;
            int i;
            if (this._webPageStartedListener != null)
            {
                invocationList = this._webPageStartedListener.GetInvocationList();
                for (i = 0; i < (int)invocationList.Length; i++)
                {
                    this._webPageStartedListener -= (Action<string>)invocationList[i];
                }
            }
            if (this._webPagePreparedListener != null)
            {
                invocationList = this._webPagePreparedListener.GetInvocationList();
                for (i = 0; i < (int)invocationList.Length; i++)
                {
                    this._webPagePreparedListener -= (Action<Texture2D>)invocationList[i];
                }
            }
            if (this._webPageLoadListener != null)
            {
                invocationList = this._webPageLoadListener.GetInvocationList();
                for (i = 0; i < (int)invocationList.Length; i++)
                {
                    this._webPageLoadListener -= (Action<int>)invocationList[i];
                }
            }
            if (this._webPageFinishedListener != null)
            {
                invocationList = this._webPageFinishedListener.GetInvocationList();
                for (i = 0; i < (int)invocationList.Length; i++)
                {
                    this._webPageFinishedListener -= (Action<string>)invocationList[i];
                }
            }
            if (this._webPageErrorListener != null)
            {
                invocationList = this._webPageErrorListener.GetInvocationList();
                for (i = 0; i < (int)invocationList.Length; i++)
                {
                    this._webPageErrorListener -= (Action<PageErrorCode>)invocationList[i];
                }
            }
            if (this._webPageHttpErrorListener != null)
            {
                invocationList = this._webPageHttpErrorListener.GetInvocationList();
                for (i = 0; i < (int)invocationList.Length; i++)
                {
                    this._webPageHttpErrorListener -= (Action)invocationList[i];
                }
            }
            if (this._webPageElementReceivedListener != null)
            {
                invocationList = this._webPageElementReceivedListener.GetInvocationList();
                for (i = 0; i < (int)invocationList.Length; i++)
                {
                    this._webPageElementReceivedListener -= (Action<string, string, bool>)invocationList[i];
                }
            }
        }

        internal void ReplaceEvent(WebStates replaceState, WebStates newState, object arg)
        {
            this._replaceState = replaceState;
            this._replaceEvent = new WebViewManagerEvents.WebEvent(newState, arg);
        }

        internal void SetEvent(WebStates state)
        {
            this._webViewEvents.Enqueue(new WebViewManagerEvents.WebEvent(state, null));
        }

        internal void SetEvent(WebStates state, object arg)
        {
            this._webViewEvents.Enqueue(new WebViewManagerEvents.WebEvent(state, arg));
        }

        public void StartListener()
        {
            this._webViewEvents.Clear();
            if (this._stateListenerEnum != null)
            {
                this._monoObject.StopCoroutine(this._stateListenerEnum);
            }
            this._stateListenerEnum = this.EventManager();
            this._monoObject.StartCoroutine(this._stateListenerEnum);
        }

        public void StopListener()
        {
            if (this._stateListenerEnum != null)
            {
                this._monoObject.StopCoroutine(this._stateListenerEnum);
            }
            while (this._webViewEvents.Count > 0)
            {
                this.CallEvent();
            }
        }

        private event Action<string, string, bool> _webPageElementReceivedListener;

        private event Action<PageErrorCode> _webPageErrorListener;

        private event Action<string> _webPageFinishedListener;

        private event Action _webPageHttpErrorListener;

        private event Action<int> _webPageLoadListener;

        private event Action<Texture2D> _webPagePreparedListener;

        private event Action<string> _webPageStartedListener;

        public event Action<string, string, bool> WebPageElementReceivedListener
        {
            add
            {
                this._webPageElementReceivedListener += value;
            }
            remove
            {
                if (this._webPageElementReceivedListener != null)
                {
                    this._webPageElementReceivedListener -= value;
                }
            }
        }

        public event Action<PageErrorCode> WebPageErrorListener
        {
            add
            {
                this._webPageErrorListener += value;
            }
            remove
            {
                if (this._webPageErrorListener != null)
                {
                    this._webPageErrorListener -= value;
                }
            }
        }

        public event Action<string> WebPageFinishedListener
        {
            add
            {
                this._webPageFinishedListener += value;
            }
            remove
            {
                if (this._webPageFinishedListener != null)
                {
                    this._webPageFinishedListener -= value;
                }
            }
        }

        public event Action WebPageHttpErrorListener
        {
            add
            {
                this._webPageHttpErrorListener += value;
            }
            remove
            {
                if (this._webPageHttpErrorListener != null)
                {
                    this._webPageHttpErrorListener -= value;
                }
            }
        }

        public event Action<int> WebPageLoadListener
        {
            add
            {
                this._webPageLoadListener += value;
            }
            remove
            {
                if (this._webPageLoadListener != null)
                {
                    this._webPageLoadListener -= value;
                }
            }
        }

        public event Action<Texture2D> WebPagePreparedListener
        {
            add
            {
                this._webPagePreparedListener += value;
            }
            remove
            {
                if (this._webPagePreparedListener != null)
                {
                    this._webPagePreparedListener -= value;
                }
            }
        }

        public event Action<string> WebPageStartedListener
        {
            add
            {
                this._webPageStartedListener += value;
            }
            remove
            {
                if (this._webPageStartedListener != null)
                {
                    this._webPageStartedListener -= value;
                }
            }
        }

        internal class WebEvent
        {
            private WebStates _state;

            private object _arg;

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

            public float GetFloatArg
            {
                get
                {
                    if (this._arg == null || !(this._arg is float))
                    {
                        return 0f;
                    }
                    return (float)this._arg;
                }
            }

            public int GetIntArg
            {
                get
                {
                    if (this._arg == null || !(this._arg is int))
                    {
                        return 0;
                    }
                    return (int)this._arg;
                }
            }

            public long GetLongArg
            {
                get
                {
                    if (this._arg == null || !(this._arg is long))
                    {
                        return (long)0;
                    }
                    return (long)this._arg;
                }
            }

            public string GetStringArg
            {
                get
                {
                    if (this._arg == null || !(this._arg is string))
                    {
                        return string.Empty;
                    }
                    return (string)this._arg;
                }
            }

            public WebStates State
            {
                get
                {
                    return this._state;
                }
            }

            public WebEvent(WebStates state, object arg)
            {
                this._state = state;
                this._arg = arg;
            }
        }
    }
}