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
    /// </summary>
    public class ScreenshotReaction : GenericReaction
    {
        #region PublicVariables
        [Tooltip("Where the screenshot will be saved on the local machine.")]
        public string filePath;
        [Tooltip("Which eye(s) should be included in the screenshot.")]
        public ScreenCapture.StereoScreenCaptureMode screenCaptureMode = ScreenCapture.StereoScreenCaptureMode.BothEyes;
        #endregion

        #region ReactionImplementation
        public override void StartReaction(object o, EventArgs e)
        {
            ScreenCapture.CaptureScreenshot(filePath + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png", screenCaptureMode);
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