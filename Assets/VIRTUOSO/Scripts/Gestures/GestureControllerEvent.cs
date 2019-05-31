using CharlesRiverAnalytics.Virtuoso.Scriptable;
using System;
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
    /// Last Modified: Dan Duggan (dduggan@cra.com) January 2019
    /// </summary>
    /// <remarks>This class only provides calls to the Pressed/Released method calls. The classes are left
    /// virtual so anyone may override them and expand functionality to the other button states.
    /// </remarks>
    public class GestureControllerEvent : VRTK_ControllerEvents
    {
        #region PublicVariables
        public SDK_BaseGestureLibrary.Hand controllerHandId;
        public Gesture triggerGesture;
        public Gesture gripGesture;
        public Gesture touchpadGesture;
        public Gesture buttonOneGesture;
        public Gesture buttonTwoGesture;
        public Gesture startMenuGesture;
        #endregion

        #region ProtectedVariables
        protected struct GestureButtonStates
        {
            public GestureButtonStates(bool allValues)
            {
                isGestureButtonPressed = allValues;
                wasPressedLastFrame = allValues;
                wasPressedThisFrame = allValues;
                wasReleasedThisFrame = allValues;
                wasReleasedLastFrame = allValues;
            }

            public bool isGestureButtonPressed;
            public bool wasPressedLastFrame;
            public bool wasPressedThisFrame;
            public bool wasReleasedThisFrame;
            public bool wasReleasedLastFrame;
        }

        protected GestureButtonStates triggerGestureButton;
        protected GestureButtonStates gripGestureButton;
        protected GestureButtonStates touchPadGestureButton;
        protected GestureButtonStates buttonOneGestureButton;
        protected GestureButtonStates buttonTwoGestureButton;
        protected GestureButtonStates startMenuGestureButton;

        #endregion

        #region UnityFunctions
        protected override void Awake()
        {
            base.Awake();

            triggerGestureButton = new GestureButtonStates(false);
            gripGestureButton = new GestureButtonStates(false);
            touchPadGestureButton = new GestureButtonStates(false);
            buttonOneGestureButton = new GestureButtonStates(false);
            buttonTwoGestureButton = new GestureButtonStates(false);
            startMenuGestureButton = new GestureButtonStates(false);
        }
        protected override void Update()
        {
            CheckTriggerGesture();
            CheckGripGesture();
            CheckTouchpadGesture();
            CheckButtonOneGesture();
            CheckButtonTwoGesture();
            CheckStartMenuGesture();
        }
        #endregion

        #region GestureControllerChecks
        /// <summary>
        /// General call for checking if a gesture button has reached it's press/release state
        /// </summary>
        /// <param name="givenGestureButton">The parameter that holds the state of the gesture button</param>
        /// <param name="givenGesture">The Gesture that contains the gesture info of what needs to be checked</param>
        /// <param name="buttonPressAction">What actions to take when the gesture-button reaches the press state</param>
        /// <param name="buttonReleaseAction">What actions to take when the gesture-button reaches the release state</param>
        protected virtual void CheckGestureButton(ref GestureButtonStates givenGestureButton, Gesture givenGesture, Action buttonPressAction, Action buttonReleaseAction)
        {
            if (givenGesture == null)
                return;

            CheckGestureButtonPressed(ref givenGestureButton, givenGesture, buttonPressAction);
            CheckGestureButtonReleased(ref givenGestureButton, givenGesture, buttonReleaseAction);

            //Save frame information
            givenGestureButton.wasPressedLastFrame = givenGestureButton.wasPressedThisFrame;
            givenGestureButton.wasReleasedLastFrame = givenGestureButton.wasReleasedThisFrame;
        }

        protected virtual void CheckGestureButtonPressed(ref GestureButtonStates givenGestureButton, Gesture givenGesture, Action buttonPressAction)
        {
            // Pressed start
            givenGestureButton.wasPressedThisFrame = givenGesture.IsGestureOccuring(controllerHandId);

            if (givenGestureButton.wasPressedThisFrame && !givenGestureButton.wasPressedLastFrame && !givenGestureButton.isGestureButtonPressed)
            {
                givenGestureButton.isGestureButtonPressed = true;

                buttonPressAction();
            }
        }

        protected virtual void CheckGestureButtonReleased(ref GestureButtonStates givenGestureButton, Gesture givenGesture, Action buttonReleaseAction)
        {
            // Pressed end
            givenGestureButton.wasReleasedThisFrame = !givenGesture.IsGestureOccuring(controllerHandId);

            if (givenGestureButton.wasReleasedThisFrame && !givenGestureButton.wasReleasedLastFrame && givenGestureButton.isGestureButtonPressed)
            {
                givenGestureButton.isGestureButtonPressed = false;

                buttonReleaseAction();
            }
        }

        protected virtual void CheckTriggerGesture()
        {
            CheckGestureButton(ref triggerGestureButton, triggerGesture,
                                () =>
                                {
                                    OnTriggerPressed(SetControllerEvent(ref triggerPressed, true, 1.0f));
                                },
                                () =>
                                {
                                    OnTriggerReleased(SetControllerEvent(ref triggerPressed, false, 0f));
                                });
        }

        protected virtual void CheckGripGesture()
        {
            CheckGestureButton(ref gripGestureButton, gripGesture,
                                () =>
                                {
                                    OnGripPressed(SetControllerEvent(ref gripPressed, true, 1.0f));
                                },
                                () =>
                                {
                                    OnGripReleased(SetControllerEvent(ref gripPressed, false, 0f));
                                });
        }

        protected virtual void CheckTouchpadGesture()
        {
            CheckGestureButton(ref touchPadGestureButton, touchpadGesture,
                                () =>
                                {
                                    OnTouchpadPressed(SetControllerEvent(ref touchpadPressed, true, 1f));
                                },
                                () =>
                                {
                                    OnTouchpadReleased(SetControllerEvent(ref touchpadPressed, false, 0f));
                                });
        }

        protected virtual void CheckButtonOneGesture()
        {
            CheckGestureButton(ref buttonOneGestureButton, buttonOneGesture,
                                () =>
                                {
                                    OnButtonOnePressed(SetControllerEvent(ref buttonOnePressed, true, 1f));
                                },
                                () =>
                                {
                                    OnButtonOneReleased(SetControllerEvent(ref buttonOnePressed, false, 0f));
                                });
        }

        protected virtual void CheckButtonTwoGesture()
        {
            CheckGestureButton(ref buttonTwoGestureButton, buttonTwoGesture,
                                () =>
                                {
                                    OnButtonTwoPressed(SetControllerEvent(ref buttonTwoPressed, true, 1f));
                                },
                                () =>
                                {
                                    OnButtonTwoReleased(SetControllerEvent(ref buttonTwoPressed, false, 0f));
                                });
        }

        protected virtual void CheckStartMenuGesture()
        {
            CheckGestureButton(ref startMenuGestureButton, startMenuGesture,
                               () =>
                               {
                                   OnStartMenuPressed(SetControllerEvent(ref startMenuPressed, true, 1f));
                               },
                               () =>
                               {
                                   OnStartMenuReleased(SetControllerEvent(ref startMenuPressed, false, 0f));
                               });
        }
        #endregion
    }
}
