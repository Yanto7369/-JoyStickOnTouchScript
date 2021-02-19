using System;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using UnityEngine;
using UnityEngine.EventSystems;
namespace JoyStickByTouchScript{
public class FloatingJoystick : Joystick
{
    protected override void Start()
    {
        base.Start();
        background.gameObject.SetActive(false);
    }

    public override void OnPointerDown(object sender, EventArgs e)
    {
        background.anchoredPosition =ScreenPointToAnchoredPosition(((Gesture)sender).ScreenPosition);
        background.gameObject.SetActive(true);
        base.OnPointerDown(sender,e);
    }

    public override void OnPointerUp(object sender, EventArgs e)
    {
        background.gameObject.SetActive(false);
        base.OnPointerUp(sender, e);
    }
}}