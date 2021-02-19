using System;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JoyStickByTouchScript{
    [RequireComponent(typeof (ScreenTransformGesture), typeof ( LongPressGesture),typeof (TapGesture))]
    public class Joystick : MonoBehaviour
    {
        public float Horizontal { get { return (snapX) ? SnapFloat(input.x, AxisOptions.Horizontal) : input.x; } }
        public float Vertical { get { return (snapY) ? SnapFloat(input.y, AxisOptions.Vertical) : input.y; } }
        public Vector2 Direction { get { return new Vector2(Horizontal, Vertical); } }
        bool debug=true;
        public float HandleRange
        {
            get { return handleRange; }
            set { handleRange = Mathf.Abs(value); }
        }

        public float DeadZone
        {
            get { return deadZone; }
            set { deadZone = Mathf.Abs(value); }
        }

        public AxisOptions AxisOptions { get { return AxisOptions; } set { axisOptions = value; } }
        public bool SnapX { get { return snapX; } set { snapX = value; } }
        public bool SnapY { get { return snapY; } set { snapY = value; } }

        [SerializeField] private float handleRange = 1;
        [SerializeField] private float deadZone = 0;
        [SerializeField] private AxisOptions axisOptions = AxisOptions.Both;
        [SerializeField] private bool snapX = false;
        [SerializeField] private bool snapY = false;
        [SerializeField] private float TimeLimit=0.01f;
        [SerializeField] private float TimeLimitWhileDragging=0.4f;
        [SerializeField] protected RectTransform background = null;
        [SerializeField] private RectTransform handle = null;
        private RectTransform baseRect = null;

        private Canvas canvas;
        private Camera cam;
        protected bool joystickEnabled=false;
        private Vector2 input = Vector2.zero;
       
       
         LongPressGesture longPressGesture;
        ScreenTransformGesture screenTransformGesture;
        private void OnEnable()
        {
        screenTransformGesture.Transformed += OnDrag;
       screenTransformGesture.TransformCompleted += OnPointerUp;
        longPressGesture.LongPressed+=OnPointerDown;
      
        }
        /*public void AddFriendly(Gesture gesture){
            gesture.AddFriendlyGesture(longPressGesture);
            gesture.AddFriendlyGesture(screenTransformGesture);

        }*/

        public event EventHandler<EventArgs> OnTaped
        {
            add {   tapGesture.Tapped+= value; }
            remove {  tapGesture.Tapped -= value; }
        }

        private void OnDisable()
        {
          
             //tapGesture.Tapped-=OnPointerDown;
             //tapGesture.
        }
        TapGesture tapGesture;
        void Awake(){
            //pressGesture=GetComponent<PressGesture>();
            screenTransformGesture=GetComponent<ScreenTransformGesture>();
            longPressGesture=GetComponent<LongPressGesture>();
            longPressGesture.TimeToPress=TimeLimit;
            longPressGesture.AddFriendlyGesture(screenTransformGesture);
            tapGesture=GetComponent<TapGesture>();
            tapGesture.TimeLimit=TimeLimit;
            tapGesture.AddFriendlyGesture(screenTransformGesture);
            tapGesture.AddFriendlyGesture(longPressGesture);
            //tapGesture=GetComponent<TapGesture>();
            //tapGesture.TimeLimit=TimeLimit;
        }
        protected virtual void Start()
        {
            HandleRange = handleRange;
            DeadZone = deadZone;
            baseRect = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
                Debug.LogError("The Joystick is not placed inside a canvas");

            Vector2 center = new Vector2(0.5f, 0.5f);
            background.pivot = center;
            handle.anchorMin = center;
            handle.anchorMax = center;
            handle.pivot = center;
            handle.anchoredPosition = Vector2.zero;
            
        }

       
        public virtual void OnPointerDown( object sender, EventArgs e )
        {
            if(debug)
                Debug.Log("OnPointerDown");
            joystickEnabled=true;
            //OnDrag(sender,e);
        }

        public void OnDrag( object sender, EventArgs e )
        {
            if(debug)
                Debug.Log("OnDrag");
            if(!joystickEnabled)
                return;
            longPressGesture.TimeToPress=TimeLimitWhileDragging;
            tapGesture.TimeLimit=TimeLimitWhileDragging;
            cam = null;
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                cam = canvas.worldCamera;
            Gesture gesture=(Gesture)sender;
            Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
            Vector2 radius = background.sizeDelta / 2;
            input = (gesture.ScreenPosition - position) / (radius * canvas.scaleFactor);
            FormatInput();
            HandleInput(input.magnitude, input.normalized, radius, cam);
            handle.anchoredPosition = input * radius * handleRange;
        }

        protected virtual void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
        {
            if (magnitude > deadZone)
            {
                if (magnitude > 1)
                    input = normalised;
            }
            else
                input = Vector2.zero;
        }

        private void FormatInput()
        {
            if (axisOptions == AxisOptions.Horizontal)
                input = new Vector2(input.x, 0f);
            else if (axisOptions == AxisOptions.Vertical)
                input = new Vector2(0f, input.y);
        }

        private float SnapFloat(float value, AxisOptions snapAxis)
        {
            if (value == 0)
                return value;

            if (axisOptions == AxisOptions.Both)
            {
                float angle = Vector2.Angle(input, Vector2.up);
                if (snapAxis == AxisOptions.Horizontal)
                {
                    if (angle < 22.5f || angle > 157.5f)
                        return 0;
                    else
                        return (value > 0) ? 1 : -1;
                }
                else if (snapAxis == AxisOptions.Vertical)
                {
                    if (angle > 67.5f && angle < 112.5f)
                        return 0;
                    else
                        return (value > 0) ? 1 : -1;
                }
                return value;
            }
            else
            {
                if (value > 0)
                    return 1;
                if (value < 0)
                    return -1;
            }
            return 0;
        }

        public virtual void OnPointerUp( object sender, EventArgs e )
        {
             if(debug)
                Debug.Log("OnpointerUp");
            if( joystickEnabled==false)
                return;
            longPressGesture.TimeToPress=TimeLimit;
            tapGesture.TimeLimit=TimeLimit;
            joystickEnabled=false;
            input = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
           
        }

        protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
        {
            Vector2 localPoint = Vector2.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, screenPosition, cam, out localPoint))
            {
                Vector2 pivotOffset = baseRect.pivot * baseRect.sizeDelta;
                return localPoint - (background.anchorMax * baseRect.sizeDelta) + pivotOffset;
            }
            return Vector2.zero;
        }
    }

    public enum AxisOptions { Both, Horizontal, Vertical }
}