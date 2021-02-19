using System;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JoyStickByTouchScript{
    public class DynamicJoystick : Joystick
{
    public float MoveThreshold { get { return moveThreshold; } set { moveThreshold = Mathf.Abs(value); } }

    [SerializeField] private float moveThreshold = 1;

    protected override void Start()
    {
        MoveThreshold = moveThreshold;
        base.Start();
        background.gameObject.SetActive(false);
    }

    public override void OnPointerDown(object sender, EventArgs e)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(((Gesture)sender).ScreenPosition);
        background.gameObject.SetActive(true);
        base.OnPointerDown( sender,e);
    }

    public override void OnPointerUp(object sender, EventArgs e)
    {
        background.gameObject.SetActive(false);
        base.OnPointerUp(sender , e);
    }

    protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (magnitude > moveThreshold)
        {
            Vector2 difference = normalised * (magnitude - moveThreshold) * radius;
            background.anchoredPosition += difference;
        }
        base.HandleInput(magnitude, normalised, radius, cam);
    }
}}