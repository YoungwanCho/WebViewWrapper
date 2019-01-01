using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MWV
{
	public class InputManager
	{
		private const float DEBUG_RAY_DURATION = 0.1f;

		private const float DEFAULT_SUBMIT_TIME = 1f;

		private const float DEFAULT_GAZE_SENSITIVITY = 0.1f;

		private static float START_TIME_OFFSET;

		private const float MOVE_SENSITIVITY = 15f;

		private MonoBehaviour _monoObject;

		private Camera _inputCamera;

		private Transform _inputCameraTransform;

		private bool _debugRay;

		private bool _touchController;

		private LayerMask _exclusionLayers;

		private AGaze _gazeObject;

		private float _gazeSubmitTime = 1f;

		private float _gazeSensitivity = 0.1f;

		private bool _multipleSubmits;

		private float _actionWaitTime;

		private float _swipePower;

		private InputManager.TargetInfo _lastInfoPosition;

		private InputManager.TargetInfo _lastInfoGaze;

		private InputManager.InputData _lastInputData;

		private InputTypes _inputType;

		private InputPhases _inputPhase;

		private MWV.InputSystem _inputSystem;

		private IEnumerator _inputUpdaterEnum;

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

		public MWV.InputSystem InputSystem
		{
			get
			{
				return this._inputSystem;
			}
		}

		public InputTypes InputType
		{
			get
			{
				return this._inputType;
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

		static InputManager()
		{
			InputManager.START_TIME_OFFSET = 1E-06f;
		}

		public InputManager(MonoBehaviour monoObject, MWV.InputSystem inputSystem, Camera inputCamera)
		{
			Transform transforms;
			this._monoObject = monoObject;
			this._inputCamera = inputCamera;
			if (this._inputCamera != null)
			{
				transforms = this._inputCamera.transform;
			}
			else
			{
				transforms = null;
			}
			this._inputCameraTransform = transforms;
			this._inputSystem = inputSystem;
		}

		public void AddInputListener(IInputManagerHandler listener)
		{
			IInputManagerHandler inputManagerHandler = listener;
			this.InputClickListener += new Action<GameObject, Vector2>(inputManagerHandler.OnInputClick);
			IInputManagerHandler inputManagerHandler1 = listener;
			this.InputMoveListener += new Action<GameObject, Vector2, float, InputPhases>(inputManagerHandler1.OnInputMove);
		}

		private Vector2? GetAreaCoord(Vector2 touchPos, object inputArea)
		{
			RaycastHit raycastHit;
			if (inputArea is Rect)
			{
				Rect rect = (Rect)inputArea;
				if (rect.Contains(touchPos))
				{
					return new Vector2?(new Vector2(touchPos.x / rect.width, 1f - touchPos.y / rect.height));
				}
			}
			else if (inputArea is MeshCollider)
			{
				MeshCollider meshCollider = (MeshCollider)inputArea;
				if (Physics.Raycast(this._inputCamera.ScreenPointToRay(touchPos), out raycastHit) && raycastHit.collider == meshCollider)
				{
					return new Vector2?(new Vector2(raycastHit.textureCoord.x, 1f - raycastHit.textureCoord.y));
				}
			}
			else if (inputArea is RectTransform)
			{
				Vector2 vector2 = Vector2.zero;
				RectTransform rectTransform = (RectTransform)inputArea;
				if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, touchPos, this._inputCamera, out vector2))
				{
					Rect rect1 = rectTransform.rect;
					float single = rect1.width * 0.5f + vector2.x;
					rect1 = rectTransform.rect;
					float single1 = single / rect1.width;
					rect1 = rectTransform.rect;
					float single2 = rect1.height * 0.5f - vector2.y;
					rect1 = rectTransform.rect;
					Vector2 vector21 = new Vector2(single1, single2 / rect1.height);
					if (vector21.x >= 0f && vector21.x <= 1f && vector21.y >= 0f && vector21.y <= 1f)
					{
						return new Vector2?(vector21);
					}
				}
			}
			return null;
		}

		private IEnumerator InputUpdater()
		{
			float single;
			float single1;
			while (true)
			{
				if (Application.isEditor && this._debugRay && this._inputCameraTransform != null)
				{
					UnityEngine.Debug.DrawRay(this._inputCameraTransform.position, this._inputCameraTransform.forward * this._inputCamera.farClipPlane, Color.blue, 0.1f);
				}
				if (this._inputSystem != MWV.InputSystem.Empty)
				{
					InputManager.InputData inputDatum = new InputManager.InputData(this._touchController);
					if (inputDatum.Phase != InputPhases.Empty || this._inputSystem == MWV.InputSystem.VR)
					{
						InputManager.TargetInfo targetInfo = new InputManager.TargetInfo(new Vector2?(inputDatum.Position), this._inputCamera, this._inputCameraTransform, this._lastInfoPosition);
						Vector2? nullable = null;
						InputManager.TargetInfo targetInfo1 = new InputManager.TargetInfo(nullable, this._inputCamera, this._inputCameraTransform, this._lastInfoGaze);
						if ((this._inputSystem != MWV.InputSystem.Touch || !(targetInfo.Object != null)) && (this._inputSystem != MWV.InputSystem.VR || !(targetInfo1.Object != null)))
						{
							if (this._inputSystem == MWV.InputSystem.VR)
							{
								targetInfo1.SendEventTrigger(1);
							}
							this._inputType = InputTypes.Empty;
							this._inputPhase = InputPhases.Empty;
							this._actionWaitTime = 0f;
							this._lastInfoGaze = null;
						}
						else
						{
							if (this._lastInfoPosition == null)
							{
								this._lastInfoPosition = targetInfo;
							}
							if (this._lastInfoGaze == null)
							{
								this._lastInfoGaze = targetInfo1;
							}
							if (this._lastInputData == null)
							{
								this._lastInputData = inputDatum;
							}
							if (this._inputSystem == MWV.InputSystem.VR)
							{
								targetInfo1.SendEventTrigger(0);
								if (this._gazeObject != null)
								{
									Vector3 distance = this._inputCameraTransform.position + (this._inputCameraTransform.forward * targetInfo1.Distance);
									this._gazeObject.Show(distance, targetInfo1.Normal);
								}
							}
							Vector2 vector2 = new Vector2(inputDatum.Position.x - this._lastInputData.Position.x, inputDatum.Position.y - this._lastInputData.Position.y);
							if (this._inputSystem == MWV.InputSystem.VR && !this._touchController)
							{
								vector2 = new Vector2(this._lastInfoGaze.Coords.x - targetInfo1.Coords.x, this._lastInfoGaze.Coords.y - targetInfo1.Coords.y);
							}
							if (Mathf.Abs(vector2.x) > Mathf.Abs(vector2.y))
							{
								vector2.Set(vector2.x / Mathf.Abs(vector2.x), vector2.y / Mathf.Abs(vector2.x));
							}
							else if (Mathf.Abs(vector2.x) < Mathf.Abs(vector2.y))
							{
								vector2.Set(vector2.x / Mathf.Abs(vector2.y), vector2.y / Mathf.Abs(vector2.y));
							}
							float single2 = Vector3.Distance(inputDatum.Position, this._lastInputData.Position);
							float single3 = 15f;
							if (this._inputSystem == MWV.InputSystem.VR && !this._touchController)
							{
								single2 = Vector3.Distance(targetInfo1.Coords, this._lastInfoGaze.Coords);
								single3 = this._gazeSensitivity;
							}
							single = (this._gazeObject != null ? this._gazeObject.SubmitTime : this._gazeSubmitTime);
							float single4 = single;
							if (single2 > single3)
							{
								this._inputType = InputTypes.Move;
								if (inputDatum.Phase == InputPhases.Pressed)
								{
									if (this._inputPhase == InputPhases.Empty)
									{
										this._inputPhase = InputPhases.Began;
									}
									InputManager.TargetInfo targetInfo2 = targetInfo1;
									if (this._inputSystem == MWV.InputSystem.Touch && !this._touchController)
									{
										targetInfo2 = targetInfo;
									}
									if (targetInfo2.InputManager != null)
									{
										targetInfo2.InputManager.OnInputMove(targetInfo2.Object, vector2, single2, this._inputPhase);
									}
									if (this._inputMoveListener != null)
									{
										this._inputMoveListener(targetInfo2.Object, vector2, single2, this._inputPhase);
									}
									this._inputPhase = InputPhases.Pressed;
									if (single2 < single3 * 2f)
									{
										this._swipePower = 0f;
									}
									else if (this._swipePower < single2)
									{
										this._swipePower = single2;
									}
								}
								if (this._inputSystem == MWV.InputSystem.VR)
								{
									this._actionWaitTime = Time.time + InputManager.START_TIME_OFFSET + single4;
								}
								this._lastInfoPosition = targetInfo;
								this._lastInfoGaze = targetInfo1;
								this._lastInputData = inputDatum;
							}
							if (this._actionWaitTime > 0f)
							{
								single1 = (single4 > 0f ? Mathf.Clamp(1f - (this._actionWaitTime - Time.time) / single4, 0f, 1f) : 0f);
								float single5 = single1;
								if (this._gazeObject != null)
								{
									this._gazeObject.SubmitAnimation(single5);
								}
								if (single5 >= 1f)
								{
									this._inputType = InputTypes.Click;
									targetInfo1.SendEventTrigger(15);
									if (targetInfo1.InputManager != null)
									{
										targetInfo1.InputManager.OnInputClick(targetInfo1.Object, targetInfo1.Coords);
									}
									if (this._inputClickListener != null)
									{
										this._inputClickListener(targetInfo1.Object, targetInfo1.Coords);
									}
									if (!this._multipleSubmits)
									{
										this._actionWaitTime = 0f;
									}
									else
									{
										this._actionWaitTime = Time.time + single4;
									}
								}
							}
							if (inputDatum.Phase == InputPhases.Ended)
							{
								if (this._inputType == InputTypes.Empty)
								{
									this._inputType = InputTypes.Click;
									InputManager.TargetInfo targetInfo3 = targetInfo1;
									targetInfo3.SendEventTrigger(15);
									if (this._inputSystem == MWV.InputSystem.Touch && !this._touchController)
									{
										targetInfo3 = targetInfo;
									}
									if (targetInfo3.InputManager != null)
									{
										targetInfo3.InputManager.OnInputClick(targetInfo3.Object, targetInfo3.Coords);
									}
									if (this._inputClickListener != null)
									{
										this._inputClickListener(targetInfo3.Object, targetInfo3.Coords);
									}
								}
								else if (this._inputType == InputTypes.Move)
								{
									this._inputPhase = InputPhases.Ended;
									InputManager.TargetInfo targetInfo4 = targetInfo1;
									if (this._inputSystem == MWV.InputSystem.Touch && !this._touchController)
									{
										targetInfo4 = this._lastInfoPosition;
									}
									if (targetInfo4.InputManager != null)
									{
										targetInfo4.InputManager.OnInputMove(targetInfo4.Object, vector2, this._swipePower, this._inputPhase);
									}
									if (this._inputMoveListener != null)
									{
										this._inputMoveListener(targetInfo4.Object, vector2, this._swipePower, this._inputPhase);
									}
								}
								this._swipePower = 0f;
								this._lastInfoPosition = null;
								this._lastInputData = null;
								this._inputType = InputTypes.Empty;
								this._inputPhase = InputPhases.Empty;
							}
						}
					}
				}
				yield return null;
			}
		}

		public void RemoveAllEvents()
		{
			Delegate[] invocationList;
			int i;
			if (this._inputClickListener != null)
			{
				invocationList = this._inputClickListener.GetInvocationList();
				for (i = 0; i < (int)invocationList.Length; i++)
				{
					this._inputClickListener -= (Action<GameObject, Vector2>)invocationList[i];
				}
			}
			if (this._inputMoveListener != null)
			{
				invocationList = this._inputMoveListener.GetInvocationList();
				for (i = 0; i < (int)invocationList.Length; i++)
				{
					this._inputMoveListener -= (Action<GameObject, Vector2, float, InputPhases>)invocationList[i];
				}
			}
		}

		public void RemoveInputListener(IInputManagerHandler listener)
		{
			IInputManagerHandler inputManagerHandler = listener;
			this.InputClickListener -= new Action<GameObject, Vector2>(inputManagerHandler.OnInputClick);
			IInputManagerHandler inputManagerHandler1 = listener;
			this.InputMoveListener -= new Action<GameObject, Vector2, float, InputPhases>(inputManagerHandler1.OnInputMove);
		}

		public void StartListener()
		{
			if (this._inputUpdaterEnum != null)
			{
				this._monoObject.StopCoroutine(this._inputUpdaterEnum);
			}
			this._inputUpdaterEnum = this.InputUpdater();
			this._monoObject.StartCoroutine(this._inputUpdaterEnum);
		}

		public void StopListener()
		{
			if (this._inputUpdaterEnum != null)
			{
				this._monoObject.StopCoroutine(this._inputUpdaterEnum);
			}
		}

		private event Action<GameObject, Vector2> _inputClickListener;

		private event Action<GameObject, Vector2, float, InputPhases> _inputMoveListener;

		public event Action<GameObject, Vector2> InputClickListener
		{
			add
			{
				this._inputClickListener += value;
			}
			remove
			{
				if (this._inputClickListener != null)
				{
					this._inputClickListener -= value;
				}
			}
		}

		public event Action<GameObject, Vector2, float, InputPhases> InputMoveListener
		{
			add
			{
				this._inputMoveListener += value;
			}
			remove
			{
				if (this._inputMoveListener != null)
				{
					this._inputMoveListener -= value;
				}
			}
		}

		private class InputData
		{
			private Vector2 _position;

			private InputPhases _phase;

			public InputPhases Phase
			{
				get
				{
					return this._phase;
				}
				set
				{
					this._phase = value;
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

			public InputData(bool touchController)
			{
				if (Application.isEditor | touchController)
				{
					this._position = Input.mousePosition;
					if (Input.GetMouseButtonDown(0))
					{
						this._phase = InputPhases.Began;
						return;
					}
					if (Input.GetMouseButton(0))
					{
						this._phase = InputPhases.Pressed;
						return;
					}
					if (Input.GetMouseButtonUp(0))
					{
						this._phase = InputPhases.Ended;
						return;
					}
				}
				else if (Input.touchCount > 0)
				{
					Touch touch = Input.GetTouch(0);
					this._position = touch.position;
					if (touch.phase == TouchPhase.Began)
					{
						this._phase = InputPhases.Began;
						return;
					}
					if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
					{
						this._phase = InputPhases.Pressed;
						return;
					}
					if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
					{
						this._phase = InputPhases.Ended;
					}
				}
			}
		}

		private class TargetInfo
		{
			private GameObject _object;

			private object _tranform;

			private IInputManagerHandler _inputManager;

			private IEventSystemHandler _eventSystem;

			private PointerEventData _pointerData;

			private Vector3 _coords;

			private Vector3 _normal;

			private float _distance;

			public Vector3 Coords
			{
				get
				{
					return this._coords;
				}
			}

			public float Distance
			{
				get
				{
					return this._distance;
				}
			}

			public IInputManagerHandler InputManager
			{
				get
				{
					return this._inputManager;
				}
			}

			public Vector3 Normal
			{
				get
				{
					return this._normal;
				}
			}

			public GameObject Object
			{
				get
				{
					return this._object;
				}
			}

			public object Tranform
			{
				get
				{
					return this._tranform;
				}
			}

			public TargetInfo(Vector2? position, Camera camera, Transform cameraTransform, InputManager.TargetInfo cachedData = null)
			{
				Ray ray;
				RaycastHit raycastHit;
				if (cachedData != null)
				{
					this._object = cachedData._object;
					this._tranform = cachedData._tranform;
					this._inputManager = cachedData._inputManager;
					this._eventSystem = cachedData._eventSystem;
				}
				this._pointerData = new PointerEventData(EventSystem.get_current());
				if (!position.HasValue)
				{
					this._pointerData.set_position(camera.WorldToScreenPoint(cameraTransform.position + (cameraTransform.forward * camera.farClipPlane)));
				}
				else
				{
					this._pointerData.set_position(position.Value);
				}
				List<RaycastResult> raycastResults = new List<RaycastResult>();
				EventSystem.get_current().RaycastAll(this._pointerData, raycastResults);
				if (raycastResults.Count <= 0)
				{
					ray = (!position.HasValue ? new Ray(cameraTransform.position, cameraTransform.forward) : camera.ScreenPointToRay(position.Value));
					if (Physics.Raycast(ray, out raycastHit, camera.farClipPlane))
					{
						if (this._object != raycastHit.transform.gameObject)
						{
							this._object = raycastHit.transform.gameObject;
							this._tranform = this._object.transform;
							this._inputManager = this._object.GetComponent<IInputManagerHandler>();
							this.SendEventTrigger(1);
							this._eventSystem = null;
						}
						this._normal = raycastHit.normal;
						this._distance = raycastHit.distance;
						this._coords = new Vector2(raycastHit.textureCoord.x, 1f - raycastHit.textureCoord.y);
						return;
					}
					this._object = null;
					this._tranform = null;
					this._inputManager = null;
					this._coords = Vector2.one * -1f;
					this._normal = Vector3.one * -1f;
					this._distance = -1f;
				}
				else
				{
					RaycastResult item = raycastResults[0];
					if (this._object != item.get_gameObject())
					{
						this._object = item.get_gameObject();
						this._tranform = this._object.GetComponent<RectTransform>();
						this._inputManager = this._object.GetComponent<IInputManagerHandler>();
						this.SendEventTrigger(1);
						for (GameObject i = this._object; i.transform.parent != null; i = i.transform.parent.gameObject)
						{
							this._eventSystem = i.GetComponent<IEventSystemHandler>();
							if (this._eventSystem != null)
							{
								break;
							}
						}
					}
					RectTransform rectTransform = (RectTransform)this._tranform;
					this._normal = rectTransform.rotation * Vector3.forward;
					this._distance = item.distance;
					if (!position.HasValue)
					{
						this._coords = cameraTransform.position + (cameraTransform.forward * this._distance);
						position = new Vector2?(camera.WorldToScreenPoint(this._coords));
					}
					Vector2 vector2 = Vector2.zero;
					if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, position.Value, camera, out vector2))
					{
						Rect rect = rectTransform.rect;
						float single = rect.width * 0.5f + vector2.x;
						rect = rectTransform.rect;
						float single1 = single / rect.width;
						rect = rectTransform.rect;
						float single2 = rect.height * 0.5f - vector2.y;
						rect = rectTransform.rect;
						this._coords = new Vector2(single1, single2 / rect.height);
						return;
					}
				}
			}

			public void SendEventTrigger(EventTriggerType e)
			{
				if (this._pointerData != null)
				{
					if (e == 1 && this._eventSystem is IPointerExitHandler)
					{
						(this._eventSystem as IPointerExitHandler).OnPointerExit(this._pointerData);
					}
					if (e == null && this._eventSystem is IPointerEnterHandler)
					{
						(this._eventSystem as IPointerEnterHandler).OnPointerEnter(this._pointerData);
					}
					if (e == 4 && this._eventSystem is IPointerClickHandler)
					{
						(this._eventSystem as IPointerClickHandler).OnPointerClick(this._pointerData);
					}
					if (e == 15 && this._eventSystem is ISubmitHandler)
					{
						(this._eventSystem as ISubmitHandler).OnSubmit(this._pointerData);
					}
				}
			}
		}
	}
}