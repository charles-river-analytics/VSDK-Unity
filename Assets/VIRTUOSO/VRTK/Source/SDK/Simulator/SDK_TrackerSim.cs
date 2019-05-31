namespace VRTK
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// This class simulates the motions of a Tracker Puck based on the position, velocity, etc of an object in Unity. Code is based off SDK_ControllerSim.cs
    /// 
    /// Author: Dan Duggan (dduggan@cra.com) August 2018
    /// </summary>
    public class SDK_TrackerSim : MonoBehaviour
    {
        #region VRTK Code Duplicated from SDK_ControllerSim.cs
        /* These variables hold past data from the simulated tracker so that it can simulate forces that occur as
         * a result of movement; otherwise since the simulated tracker is only ever moved in the editor it will
         * not produce any physics from movement.
         * */
        private Vector3 lastPos;
        private Quaternion lastRot;
        private List<Vector3> posList;
        private List<Vector3> rotList;
        private float magnitude;
        private Vector3 axis;
        private static int MAX_LIST_SIZE = 4; //value is from original code

        private void Awake()
        {
            posList = new List<Vector3>();
            rotList = new List<Vector3>();
            lastPos = transform.position;
            lastRot = transform.rotation;
        }

        private void Update()
        {
            posList.Add((transform.position - lastPos) / Time.deltaTime);
            if (posList.Count > MAX_LIST_SIZE)
            {
                posList.RemoveAt(0);
            }
            Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(lastRot);
            deltaRotation.ToAngleAxis(out magnitude, out axis);
            rotList.Add((axis * magnitude));
            if (rotList.Count > MAX_LIST_SIZE)
            {
                rotList.RemoveAt(0);
            }
            lastPos = transform.position;
            lastRot = transform.rotation;
        }

        public Vector3 GetVelocity()
        {
            Vector3 velocity = Vector3.zero;
            foreach (Vector3 vel in posList)
            {
                velocity += vel;
            }
            velocity /= posList.Count;
            return velocity;
        }

        public Vector3 GetAngularVelocity()
        {
            Vector3 angularVelocity = Vector3.zero;
            foreach (Vector3 vel in rotList)
            {
                angularVelocity += vel;
            }
            angularVelocity /= rotList.Count;
            return angularVelocity;
        }


    }
    #endregion
}
