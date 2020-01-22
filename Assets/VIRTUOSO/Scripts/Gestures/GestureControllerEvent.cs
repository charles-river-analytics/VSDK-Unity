using VRTK;

namespace CharlesRiverAnalytics.Virtuoso.Gestures
{
    /// <summary>
    /// Allows the end user to designate gesture to VRTK button presses. For now, there is only
    /// a way to simulate pressing a button since a gesture is either done or not, so not all
    /// of VRTK's button mapping (like trigger hairline) have been defined yet.
    /// 
    /// Based on VRTK/Scripts/Interactions/VRTK_ControllerEvents
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2018
    /// Last Modified: Nicolas Herrera (nherrera@cra.com), December 2019
    /// </summary>
    public class GestureControllerEvent : VRTK_ControllerEvents
    {
        #region PublicVariables
        public SDK_BaseGestureLibrary.Hand controllerHandId;
        #endregion

        #region PublicAPI
        public void PressButton(ButtonAlias buttonToPress)
        {
            ControllerInteractionEventArgs controllerEventArgs = SetControllerEvent();

            switch (buttonToPress)
            {
                case ButtonAlias.Undefined:
                    break;
                case ButtonAlias.TriggerHairline:
                    OnTriggerHairlineStart(controllerEventArgs);
                    break;
                case ButtonAlias.TriggerTouch:
                    OnTriggerTouchStart(controllerEventArgs);
                    break;
                case ButtonAlias.TriggerPress:
                    OnTriggerPressed(controllerEventArgs);
                    break;
                case ButtonAlias.TriggerClick:
                    OnTriggerClicked(controllerEventArgs);
                    break;
                case ButtonAlias.GripHairline:
                    OnGripHairlineStart(controllerEventArgs);
                    break;
                case ButtonAlias.GripTouch:
                    OnGripTouchStart(controllerEventArgs);
                    break;
                case ButtonAlias.GripPress:
                    OnGripPressed(controllerEventArgs);
                    break;
                case ButtonAlias.GripClick:
                    OnGripClicked(controllerEventArgs);
                    break;
                case ButtonAlias.TouchpadTouch:
                    OnTouchpadTouchStart(controllerEventArgs);
                    break;
                case ButtonAlias.TouchpadPress:
                    OnTouchpadPressed(controllerEventArgs);
                    break;
                case ButtonAlias.TouchpadTwoTouch:
                    OnTouchpadTwoTouchStart(controllerEventArgs);
                    break;
                case ButtonAlias.TouchpadTwoPress:
                    OnTouchpadTwoPressed(controllerEventArgs);
                    break;
                case ButtonAlias.ButtonOneTouch:
                    OnButtonOneTouchStart(controllerEventArgs);
                    break;
                case ButtonAlias.ButtonOnePress:
                    OnButtonOnePressed(controllerEventArgs);
                    break;
                case ButtonAlias.ButtonTwoTouch:
                    OnButtonTwoTouchStart(controllerEventArgs);
                    break;
                case ButtonAlias.ButtonTwoPress:
                    OnButtonTwoPressed(controllerEventArgs);
                    break;
                case ButtonAlias.StartMenuPress:
                    OnStartMenuPressed(controllerEventArgs);
                    break;
                case ButtonAlias.TouchpadSense:
                    OnTouchpadSenseAxisChanged(controllerEventArgs);
                    break;
                case ButtonAlias.TriggerSense:
                    OnTriggerSenseAxisChanged(controllerEventArgs);
                    break;
                case ButtonAlias.MiddleFingerSense:
                    OnMiddleFingerSenseAxisChanged(controllerEventArgs);
                    break;
                case ButtonAlias.RingFingerSense:
                    OnRingFingerSenseAxisChanged(controllerEventArgs);
                    break;
                case ButtonAlias.PinkyFingerSense:
                    OnPinkyFingerSenseAxisChanged(controllerEventArgs);
                    break;
                case ButtonAlias.GripSense:
                    OnGripSenseAxisChanged(controllerEventArgs);
                    break;
                case ButtonAlias.GripSensePress:
                    OnGripSensePressed(controllerEventArgs);
                    break;
                default:
                    break;
            }
        }

        public void ReleaseButton(ButtonAlias buttonToPress)
        {
            ControllerInteractionEventArgs controllerEventArgs = SetControllerEvent();

            switch (buttonToPress)
            {
                case ButtonAlias.Undefined:
                    break;
                case ButtonAlias.TriggerHairline:
                    OnTriggerHairlineEnd(controllerEventArgs);
                    break;
                case ButtonAlias.TriggerTouch:
                    OnTriggerTouchEnd(controllerEventArgs);
                    break;
                case ButtonAlias.TriggerPress:
                    OnTriggerReleased(controllerEventArgs);
                    break;
                case ButtonAlias.TriggerClick:
                    OnTriggerUnclicked(controllerEventArgs);
                    break;
                case ButtonAlias.GripHairline:
                    OnGripHairlineEnd(controllerEventArgs);
                    break;
                case ButtonAlias.GripTouch:
                    OnGripTouchEnd(controllerEventArgs);
                    break;
                case ButtonAlias.GripPress:
                    OnGripReleased(controllerEventArgs);
                    break;
                case ButtonAlias.GripClick:
                    OnGripUnclicked(controllerEventArgs);
                    break;
                case ButtonAlias.TouchpadTouch:
                    OnTouchpadTouchEnd(controllerEventArgs);
                    break;
                case ButtonAlias.TouchpadPress:
                    OnTouchpadReleased(controllerEventArgs);
                    break;
                case ButtonAlias.TouchpadTwoTouch:
                    OnTouchpadTwoTouchEnd(controllerEventArgs);
                    break;
                case ButtonAlias.TouchpadTwoPress:
                    OnTouchpadTwoReleased(controllerEventArgs);
                    break;
                case ButtonAlias.ButtonOneTouch:
                    OnButtonOneTouchEnd(controllerEventArgs);
                    break;
                case ButtonAlias.ButtonOnePress:
                    OnButtonOneReleased(controllerEventArgs);
                    break;
                case ButtonAlias.ButtonTwoTouch:
                    OnButtonTwoTouchEnd(controllerEventArgs);
                    break;
                case ButtonAlias.ButtonTwoPress:
                    OnButtonTwoReleased(controllerEventArgs);
                    break;
                case ButtonAlias.StartMenuPress:
                    OnStartMenuReleased(controllerEventArgs);
                    break;
                case ButtonAlias.TouchpadSense:
                    OnTouchpadSenseAxisChanged(controllerEventArgs);
                    break;
                case ButtonAlias.TriggerSense:
                    OnTriggerSenseAxisChanged(controllerEventArgs);
                    break;
                case ButtonAlias.MiddleFingerSense:
                    OnMiddleFingerSenseAxisChanged(controllerEventArgs);
                    break;
                case ButtonAlias.RingFingerSense:
                    OnRingFingerSenseAxisChanged(controllerEventArgs);
                    break;
                case ButtonAlias.PinkyFingerSense:
                    OnPinkyFingerSenseAxisChanged(controllerEventArgs);
                    break;
                case ButtonAlias.GripSense:
                    OnGripSenseAxisChanged(controllerEventArgs);
                    break;
                case ButtonAlias.GripSensePress:
                    OnGripSenseReleased(controllerEventArgs);
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
