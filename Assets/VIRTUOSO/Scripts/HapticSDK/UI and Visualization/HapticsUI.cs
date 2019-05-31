using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// In-Game UI overlay script for testing patterns. The developer must pass any haptic patterns they would like to
    /// play to this script. When the game is played, the UI provides a list of body parts and a list of patterns and allows
    /// for easy playing and testing of these patterns.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class HapticsUI : MonoBehaviour
    {
        #region PublicVariables
        public List<ScriptableHapticPattern> availablePatterns;
        public Dropdown patternDropdown;
        public Dropdown bodyPartDropdown;
        #endregion

        #region PrivateVariables
        private ScriptableHapticPattern selectedPattern;
        private List<HumanBodyBones> bodyPartList;
        private HumanBodyBones selectedBodyPart;
        private HapticManager hapticManager;
        #endregion

        #region PublicAPI
        /// <summary>
        /// Updates the selected pattern from a dropdown menu
        /// </summary>
        /// <param name="target">The dropdown menu that has the list of available patterns</param>
        public void DropdownPatternSelection(Dropdown target)
        {
            selectedPattern = availablePatterns[target.value];
        }

        /// <summary>
        /// Updates the selected body part from a downdown menu
        /// </summary>
        /// <param name="target">The dropdown menu that has a list of the body parts</param>
        public void DropdownBodyPartSelection(Dropdown target)
        {
            selectedBodyPart = bodyPartList[target.value];
        }

        /// <summary>
        /// Plays the selected pattern on the selected body part.
        /// </summary>
        public void PlaySelectedPattern()
        {
            if(selectedPattern != null)
            {
                hapticManager?.PlayPattern(selectedBodyPart, selectedPattern);
            }
        }

        /// <summary>
        /// Plays the selected pattern on all body parts that are registered with the haptic manager.
        /// </summary>
        public void PlayOnAllBodyParts()
        {
            if(selectedPattern != null)
            {
                foreach(HumanBodyBones bodyPart in (HumanBodyBones[])Enum.GetValues(typeof(HumanBodyBones)))
                {
                    hapticManager?.PlayPattern(bodyPart, selectedPattern);
                }
            }
        }
        #endregion

        #region UIFunctions
        private void SetDropdownOptions(Dropdown dropdown, List<string> optionNames, bool clearDropdownFirst = false)
        {
            if(clearDropdownFirst)
            {
                dropdown.ClearOptions();
            }

            dropdown.AddOptions(optionNames);
        }
        #endregion

        #region UnityFunctions
        void Start()
        {
            hapticManager = FindObjectOfType<HapticManager>();

            bodyPartList = Enum.GetValues(typeof(HumanBodyBones)).Cast<HumanBodyBones>().ToList();

            SetDropdownOptions(patternDropdown, availablePatterns.Select(P => P.name).ToList(), true);
            SetDropdownOptions(bodyPartDropdown, bodyPartList.Select(P => P.ToString()).ToList(), true);

            // Make sure the functions are invoked once so the pattern isnt' null
            DropdownPatternSelection(patternDropdown);
            DropdownBodyPartSelection(bodyPartDropdown);
        }
        #endregion
    }
}