using CharlesRiverAnalytics.Virtuoso.Haptic;
using UnityEditor;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Utilities
{
    /// <summary>
    /// Utility to setup the needed body coordinates on a character that uses an Animator.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class ConfigureHapticCharacter : EditorWindow
    {
        #region PrivateVariables
        // Since this utility follows the body parts on the Ragdoll wizard (plus hands)
        // If the value is set to true, will set up the needed scripts for the HapticSDK on that 
        private bool[] configureThisBodyPart;

        private static HumanBodyBones[] bodyParts =
        {
            HumanBodyBones.Hips,
            HumanBodyBones.LeftUpperLeg,
            HumanBodyBones.LeftLowerLeg,
            HumanBodyBones.LeftFoot,
            HumanBodyBones.RightUpperLeg,
            HumanBodyBones.RightLowerLeg,
            HumanBodyBones.RightFoot,
            HumanBodyBones.LeftUpperArm,
            HumanBodyBones.LeftLowerArm,
            HumanBodyBones.LeftHand,
            HumanBodyBones.RightUpperArm,
            HumanBodyBones.RightLowerArm,
            HumanBodyBones.RightHand,
            HumanBodyBones.Spine,
            HumanBodyBones.Head
        };
        #endregion

        #region ConfigurationMethods
        [MenuItem("VIRTUOSO/Haptics/Setup Haptic Character")]
        private static void SetupHaptics()
        {
            EditorWindow.GetWindow(typeof(ConfigureHapticCharacter));
        }

        private static void SetupBodyPart(GameObject bodyPartObject, HumanBodyBones bodyPart)
        {
            Collider objCollider = bodyPartObject.GetComponent<Collider>();

            if (objCollider == null)
            {
                objCollider = bodyPartObject.AddComponent<CapsuleCollider>();

                Debug.LogWarning("No collider was found on " + bodyPartObject.name + " and a CapsuleCollider " +
                                 "was added. Please ensure it is set up properly.", bodyPartObject);
            }
            else if(!(objCollider is CapsuleCollider))
            {
                GameObject.DestroyImmediate(objCollider);

                objCollider = bodyPartObject.AddComponent<CapsuleCollider>();

                Debug.LogWarning("Non-CapsuleCollider was found on " + bodyPartObject.name + " and a CapsuleCollider " +
                                 "was added. Please ensure it is set up properly.", bodyPartObject);
            }

            // Triggers do not recieve collision info like point of collision, so make sure that is off
            objCollider.isTrigger = false;
            objCollider.gameObject.layer = LayerMask.NameToLayer("Body");
            BodyCoordinate bodyCoordinate = objCollider.gameObject.AddComponent<BodyCoordinate>();
            bodyCoordinate.attachedBody = bodyPart;

            Debug.Log("Set up BodyCoordinate on " + bodyPartObject.name, bodyPartObject);
        }

        private void SetupCharacter(GameObject rootObject)
        {
            rootObject.AddComponent<HapticManager>();

            Animator animator = rootObject.GetComponent<Animator>();

            for (int n = 0; n < configureThisBodyPart.Length; n++)
            {
                if (configureThisBodyPart[n])
                {
                    SetupBodyPart(animator.GetBoneTransform(bodyParts[n]).gameObject, bodyParts[n]);
                }
            }
        }
        #endregion

        #region UnityFunctions
        private void Awake()
        {
            configureThisBodyPart = new bool[bodyParts.Length];
        }

        void OnGUI()
        {
            GameObject obj = Selection.activeGameObject;

            EditorGUILayout.BeginVertical();

            EditorGUILayout.HelpBox("This Wizard will help you set up the needed body coordinate systems on a rigged character. " +
                "Select a rigged character with an Animator component in order to use this Wizard.", MessageType.None);

            if (obj != null && obj.GetComponent<Animator>())
            {
                // Prompt for each body part
                for (int n = 0; n < bodyParts.Length; n++)
                {
                    configureThisBodyPart[n] = EditorGUILayout.Toggle(bodyParts[n].ToString(), configureThisBodyPart[n]);
                }

                if (GUILayout.Button("Configure Character"))
                {
                    SetupCharacter(obj);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Please select a GameObject with an Animator to use this wizard.", MessageType.Error);
            }

            EditorGUILayout.EndVertical();
        }
        #endregion
    }
}