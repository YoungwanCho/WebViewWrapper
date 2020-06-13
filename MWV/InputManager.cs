using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MWV
{
    public class InputManager
    {
        private static float START_TIME_OFFSET = 1E-06f;
        private float _gazeSubmitTime = 1f;
        private float _gazeSensitivity = 0.05f;
        private bool _prepareVrMode = true;
        private float _prepareVrModeTime = 2f;
        private Vector3 _startPointVr = Vector3.one * -1f;
        private const float DEBUG_RAY_DURATION = 0.1f;
        private const float DEFAULT_SUBMIT_TIME = 1f;
        private const float DEFAULT_GAZE_SENSITIVITY = 0.05f;
        private const float MOVE_SENSITIVITY = 15f;
        private MonoBehaviour _monoObject;
        private Camera _inputCamera;
        private Transform _inputCameraTransform;
        private bool _debugRay;
        private bool _touchController;
        private LayerMask _exclusionLayers;
        private AGaze _gazeObject;
        private bool _multipleSubmits;
        private float _actionWaitTime;
        private float _swipePower;
        private InputManager.TargetInfo _lastInfoPosition;
        private InputManager.TargetInfo _lastTargetInfo;
        private InputManager.MotionData _lastInputData;
        private InputSystem _inputSystem;
        private IEnumerator _inputHandlerEnum;

        private bool SendUnityEventTrigger(GameObject target, EventTriggerType trigger)
        {
            IEventSystemHandler eventSystemHandler = (IEventSystemHandler)null;
            if ((UnityEngine.Object)target == (UnityEngine.Object)null)
                return false;
            for (; (UnityEngine.Object)target.transform.parent != (UnityEngine.Object)null; target = target.transform.parent.gameObject)
            {
                eventSystemHandler = target.GetComponent<IEventSystemHandler>();
                if (eventSystemHandler != null)
                    break;
            }
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            switch (trigger)
            {
                case EventTriggerType.PointerEnter:
                    if (eventSystemHandler is IPointerEnterHandler)
                        (eventSystemHandler as IPointerEnterHandler).OnPointerEnter(eventData);
                    return true;
                case EventTriggerType.PointerExit:
                    if (eventSystemHandler is IPointerExitHandler)
                        (eventSystemHandler as IPointerExitHandler).OnPointerExit(eventData);
                    return true;
                case EventTriggerType.Submit:
                    if (eventSystemHandler is ISubmitHandler)
                        (eventSystemHandler as ISubmitHandler).OnSubmit((BaseEventData)eventData);
                    return true;
                default:
                    return false;
            }
        }

        public InputManager(MonoBehaviour monoObject, InputSystem inputSystem, Camera inputCamera)
        {
            this._monoObject = monoObject;
            this._inputCamera = inputCamera;
            this._inputCameraTransform = (UnityEngine.Object)this._inputCamera != (UnityEngine.Object)null ? this._inputCamera.transform : (Transform)null;
            this._inputSystem = inputSystem;
        }

        private IEnumerator InputHandler()
        {
            while (true)
            {
                if (Application.isEditor && this._debugRay && (UnityEngine.Object)this._inputCameraTransform != (UnityEngine.Object)null)
                    Debug.DrawRay(this._inputCameraTransform.position, this._inputCameraTransform.forward * this._inputCamera.farClipPlane, Color.blue, 0.1f);
                if (this._inputSystem != InputSystem.Empty)
                {
                    InputManager.MotionData motionData = new InputManager.MotionData(this._inputSystem, Application.isEditor || this._touchController);
                    InputManager.TargetInfo targetInfo = new InputManager.TargetInfo(motionData, this._inputCamera, this._inputCameraTransform);
                    if (targetInfo.IsEmpty)
                    {
                        yield return (object)null;
                        continue;
                    }
                    if (this._lastTargetInfo == null)
                        this._lastTargetInfo = targetInfo;
                    if ((UnityEngine.Object)this._lastTargetInfo.Object != (UnityEngine.Object)targetInfo.Object)
                    {
                        this.SendUnityEventTrigger(this._lastTargetInfo.Object, EventTriggerType.PointerExit);
                        this.SendUnityEventTrigger(targetInfo.Object, EventTriggerType.PointerEnter);
                    }
                    float num1 = this._gazeSubmitTime;
                    if ((UnityEngine.Object)this._gazeObject != (UnityEngine.Object)null)
                    {
                        this._gazeObject.Show(this._inputCameraTransform.position + this._inputCameraTransform.forward * targetInfo.Distance, targetInfo.Normal);
                        num1 = this._gazeObject.SubmitTime;
                    }
                    double num2 = (double)Vector3.Distance(targetInfo.Coords, this._lastTargetInfo.Coords);
                    if (this._inputSystem == InputSystem.VR && !this._touchController)
                    {
                        if (this._startPointVr == Vector3.one * -1f)
                            this._startPointVr = targetInfo.Coords;
                        bool flag = (double)Vector3.Distance(this._startPointVr, targetInfo.Coords) > (double)this._gazeSensitivity;
                        if (flag)
                        {
                            this._startPointVr = targetInfo.Coords;
                            this._actionWaitTime = Time.time;
                            if (!this._prepareVrMode)
                                this._actionWaitTime += InputManager.START_TIME_OFFSET + num1;
                        }
                        if (this._prepareVrMode && (double)this._actionWaitTime > 0.0 && (double)Time.time - (double)this._actionWaitTime > (double)this._prepareVrModeTime)
                        {
                            this._prepareVrMode = false;
                            motionData.Action = MotionActions.Began;
                            this._actionWaitTime = Time.time + InputManager.START_TIME_OFFSET + num1;
                        }
                        if (!this._prepareVrMode)
                        {
                            float progress = (double)num1 > 0.0 ? Mathf.Clamp01((float)(1.0 - ((double)this._actionWaitTime - (double)Time.time) / (double)num1)) : 0.0f;
                            if ((UnityEngine.Object)this._gazeObject != (UnityEngine.Object)null)
                                this._gazeObject.SubmitAnimation(progress);
                            if ((double)progress >= 1.0)
                            {
                                this.SendUnityEventTrigger(targetInfo.Object, EventTriggerType.Submit);
                                motionData.Action = MotionActions.Ended;
                                this._prepareVrMode = true;
                                this._actionWaitTime = !this._multipleSubmits ? 0.0f : Time.time + InputManager.START_TIME_OFFSET + num1;
                            }
                            else if (flag)
                                motionData.Action = MotionActions.Moved;
                        }
                    }
                    if (!motionData.IsEmpty)
                    {
                        if (this._motionEventsListener != null)
                            this._motionEventsListener(targetInfo.Object, motionData.Action, (Vector2)targetInfo.Coords);
                        if (targetInfo.InputManager != null)
                            targetInfo.InputManager.OnMotionEvents(targetInfo.Object, motionData.Action, (Vector2)targetInfo.Coords);
                    }
                    this._lastTargetInfo = targetInfo;
                    motionData = (InputManager.MotionData)null;
                    targetInfo = (InputManager.TargetInfo)null;
                }
                yield return (object)null;
            }
        }

        private Vector2? GetAreaCoord(Vector2 touchPos, object inputArea)
        {
            switch (inputArea)
            {
                case Rect rect2:
                    if (rect2.Contains(touchPos))
                        return new Vector2?(new Vector2(touchPos.x / rect2.width, (float)(1.0 - (double)touchPos.y / (double)rect2.height)));
                    break;
                case MeshCollider _:
                    MeshCollider meshCollider = (MeshCollider)inputArea;
                    RaycastHit hitInfo;
                    if (Physics.Raycast(this._inputCamera.ScreenPointToRay((Vector3)touchPos), out hitInfo) && (UnityEngine.Object)hitInfo.collider == (UnityEngine.Object)meshCollider)
                        return new Vector2?(new Vector2(hitInfo.textureCoord.x, 1f - hitInfo.textureCoord.y));
                    break;
                case RectTransform _:
                    Vector2 localPoint = Vector2.zero;
                    RectTransform rect1 = (RectTransform)inputArea;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rect1, touchPos, this._inputCamera, out localPoint))
                    {
                        Vector2 vector2 = Vector2.one;
                        ref Vector2 local = ref vector2;
                        double num1 = (double)rect1.rect.width * 0.5 + (double)localPoint.x;
                        Rect rect2 = rect1.rect;
                        double width = (double)rect2.width;
                        double num2 = num1 / width;
                        rect2 = rect1.rect;
                        double num3 = ((double)rect2.height * 0.5 - (double)localPoint.y) / (double)rect1.rect.height;
                        local = new Vector2((float)num2, (float)num3);
                        if ((double)vector2.x >= 0.0 && (double)vector2.x <= 1.0 && ((double)vector2.y >= 0.0 && (double)vector2.y <= 1.0))
                            return new Vector2?(vector2);
                        break;
                    }
                    break;
            }
            return new Vector2?();
        }

        public InputSystem InputSystem
        {
            get
            {
                return this._inputSystem;
            }
        }

        public bool DebugRay
        {
            get
            {
                return this._debugRay;
            }
            set
            {
                this._debugRay = value;
            }
        }

        public bool TouchController
        {
            get
            {
                return this._touchController;
            }
            set
            {
                this._touchController = value;
            }
        }

        public LayerMask ExclusionLayers
        {
            get
            {
                return this._exclusionLayers;
            }
            set
            {
                this._exclusionLayers = value;
            }
        }

        public float GazeSensitivity
        {
            get
            {
                return this._gazeSensitivity;
            }
            set
            {
                this._gazeSensitivity = value;
            }
        }

        public AGaze GazeObject
        {
            get
            {
                return this._gazeObject;
            }
            set
            {
                this._gazeObject = value;
            }
        }

        public float GazeSubmitTime
        {
            get
            {
                return this._gazeSubmitTime;
            }
            set
            {
                this._gazeSubmitTime = value;
            }
        }

        public float PrepareVrModeTime
        {
            get
            {
                return this._prepareVrModeTime;
            }
            set
            {
                this._prepareVrModeTime = value;
            }
        }

        public void StartListener()
        {
            if (this._inputHandlerEnum != null)
                this._monoObject.StopCoroutine(this._inputHandlerEnum);
            this._inputHandlerEnum = this.InputHandler();
            this._monoObject.StartCoroutine(this._inputHandlerEnum);
        }

        public void StopListener()
        {
            if (this._inputHandlerEnum == null)
                return;
            this._monoObject.StopCoroutine(this._inputHandlerEnum);
        }

        public void AddInputListener(IInputManagerListener listener)
        {
            this.MotionEventsListener += new Action<GameObject, MotionActions, Vector2>(listener.OnMotionEvents);
        }

        public void RemoveInputListener(IInputManagerListener listener)
        {
            this.MotionEventsListener -= new Action<GameObject, MotionActions, Vector2>(listener.OnMotionEvents);
        }

        public void RemoveAllEvents()
        {
            if (this._motionEventsListener == null)
                return;
            foreach (Action<GameObject, MotionActions, Vector2> invocation in this._motionEventsListener.GetInvocationList())
                this._motionEventsListener -= invocation;
        }

        private event Action<GameObject, MotionActions, Vector2> _motionEventsListener;

        public event Action<GameObject, MotionActions, Vector2> MotionEventsListener
        {
            add
            {
                this._motionEventsListener += value;
            }
            remove
            {
                if (this._motionEventsListener == null)
                    return;
                this._motionEventsListener -= value;
            }
        }

        private class MotionData
        {
            private Vector2 _position = Vector2.one * -1f;
            private MotionActions _action;

            public MotionData(InputSystem inputSystem, bool useTouchMotion)
            {
                if (!useTouchMotion && Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    this._position = touch.position;
                    if (touch.phase == TouchPhase.Began)
                        this._action = MotionActions.Began;
                    else if (touch.phase == TouchPhase.Moved)
                        this._action = MotionActions.Moved;
                    else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                        this._action = MotionActions.Ended;
                    if (inputSystem != InputSystem.VR)
                        return;
                    this._action = MotionActions.Empty;
                }
                else
                {
                    bool flag = (double)Math.Abs(Input.GetAxis("Mouse X")) > 0.0 || (double)Math.Abs(Input.GetAxis("Mouse Y")) > 0.0;
                    if (Input.GetMouseButtonDown(0))
                        this._action = MotionActions.Began;
                    else if (Input.GetMouseButton(0) & flag)
                        this._action = MotionActions.Moved;
                    else if (Input.GetMouseButtonUp(0))
                        this._action = MotionActions.Ended;
                    if (this._action == MotionActions.Empty)
                        return;
                    this._position = (Vector2)Input.mousePosition;
                }
            }

            public Vector2 Position
            {
                get
                {
                    return this._position;
                }
                set
                {
                    this._position = value;
                }
            }

            public MotionActions Action
            {
                get
                {
                    return this._action;
                }
                set
                {
                    this._action = value;
                }
            }

            public bool IsEmpty
            {
                get
                {
                    return this._action == MotionActions.Empty;
                }
            }

            public override string ToString()
            {
                return "Motion action: " + (object)this._action + " - (X=" + (object)this._position.x + ") : (Y=" + (object)this._position.y + ")";
            }
        }

        private class TargetInfo
        {
            private GameObject _object;
            private IInputManagerListener _inputManager;
            private PointerEventData _pointerData;
            private Vector2 _coords;
            private Vector3 _normal;
            private float _distance;

            public TargetInfo(InputManager.MotionData data, Camera camera, Transform cameraTransform)
            {
                object obj = (object)null;
                this._pointerData = new PointerEventData(EventSystem.current);
                this._pointerData.position = data.IsEmpty ? (Vector2)camera.WorldToScreenPoint(cameraTransform.position + cameraTransform.forward * camera.farClipPlane) : data.Position;
                List<RaycastResult> raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(this._pointerData, raycastResults);
                if (raycastResults.Count > 0)
                    obj = (object)raycastResults[0];
                RaycastHit hitInfo;
                if (Physics.Raycast(data.IsEmpty ? new Ray(cameraTransform.position, cameraTransform.forward) : camera.ScreenPointToRay((Vector3)data.Position), out hitInfo, camera.farClipPlane))
                {
                    if (obj != null)
                    {
                        if ((double)((RaycastResult)obj).distance > (double)hitInfo.distance)
                            obj = (object)hitInfo;
                    }
                    else
                        obj = (object)hitInfo;
                }
                switch (obj)
                {
                    case RaycastResult raycastResult:
                        this._object = raycastResult.gameObject;
                        this._inputManager = this._object.GetComponent<IInputManagerListener>();
                        RectTransform transform = (RectTransform)this._object.transform;
                        Vector2 screenPoint = data.Position;
                        Vector2 localPoint = Vector2.zero;
                        this._normal = transform.rotation * Vector3.forward;
                        this._distance = raycastResult.distance;
                        if (data.IsEmpty)
                        {
                            Vector3 position = cameraTransform.position + cameraTransform.forward * this._distance;
                            screenPoint = (Vector2)camera.WorldToScreenPoint(position);
                        }
                        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(transform, screenPoint, camera, out localPoint))
                            break;
                        this._coords = new Vector2((transform.rect.width * 0.5f + localPoint.x) / transform.rect.width, (transform.rect.height * 0.5f - localPoint.y) / transform.rect.height);
                        break;
                    case RaycastHit raycastHit:
                        this._object = raycastHit.transform.gameObject;
                        this._inputManager = this._object.GetComponent<IInputManagerListener>();
                        this._normal = raycastHit.normal;
                        this._distance = raycastHit.distance;
                        this._coords = new Vector2(1f - raycastHit.textureCoord.x, raycastHit.textureCoord.y);
                        break;
                    default:
                        this._object = (GameObject)null;
                        this._inputManager = (IInputManagerListener)null;
                        this._coords = Vector2.one * -1f;
                        this._normal = Vector3.one * -1f;
                        this._distance = -1f;
                        break;
                }
            }

            public GameObject Object
            {
                get
                {
                    return this._object;
                }
            }

            public Vector3 Coords
            {
                get
                {
                    return (Vector3)this._coords;
                }
            }

            public Vector3 Normal
            {
                get
                {
                    return this._normal;
                }
            }

            public float Distance
            {
                get
                {
                    return this._distance;
                }
            }

            public IInputManagerListener InputManager
            {
                get
                {
                    return this._inputManager;
                }
            }

            public bool IsEmpty
            {
                get
                {
                    return (UnityEngine.Object)this._object == (UnityEngine.Object)null;
                }
            }

            public override string ToString()
            {
                return "Target info: " + ((UnityEngine.Object)this._object != (UnityEngine.Object)null ? (object)this._object.name : (object)"null") + " - (X=" + (object)this._coords.x + ") : (Y=" + (object)this._coords.y + ")";
            }
        }
    }
}
