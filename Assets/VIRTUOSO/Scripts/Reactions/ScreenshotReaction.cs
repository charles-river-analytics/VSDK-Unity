using System;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Reaction
{
    /// <summary>
    /// Reaction that allows for captures of screenshots using Unity's SceenCapture class. Screenshots are saved as a
    /// .png with the filename correlating to the time of capture (yyyyMMddHHmmssfff). If no file path is specified, then
    /// the application's persistent data path is used.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// Updated: Dan Duggan (dduggan@cra.com) Jan 2020
    /// </summary>
    public class ScreenshotReaction : GenericReaction
    {
        #region PublicVariables
        [Tooltip("Where the screenshot will be saved on the local machine.")]
        public string filePath;
#if UNITY_2019_1_OR_NEWER
        // The enumeration is only in some versions of Unity
        [Tooltip("Which eye(s) should be included in the screenshot.")]
        public ScreenCapture.StereoScreenCaptureMode screenCaptureMode = ScreenCapture.StereoScreenCaptureMode.BothEyes;
#endif
#endregion

#region ReactionImplementation
        public override void StartReaction(object o, EventArgs e)
        {
#if UNITY_2019_1_OR_NEWER
            ScreenCapture.CaptureScreenshot(filePath + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png", screenCaptureMode);
#else
            ScreenCapture.CaptureScreenshot(filePath + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png");
#endif
        }
#endregion

#region UnityFunctions
        private void Awake()
        {
            if(string.IsNullOrEmpty(filePath))
            {
                filePath = Application.persistentDataPath;
            }

            // Make sure the given path is a folder directory
            if(!filePath.EndsWith("\\"))
            {
                filePath += "\\";
            }
        }
#endregion
    }
}