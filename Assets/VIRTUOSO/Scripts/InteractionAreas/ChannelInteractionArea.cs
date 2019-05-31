using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.InteractionAreas
{
    /// <summary>
    /// InteractionArea that requires the user to follow a path in 3D space. Like the multi-interaction area, 
    /// the children define the path that is needed to follow with the child ordering in the hierarchy determining 
    /// the real order. The children simply need to be InteractionAreas so that they are counted and can be 'checkpoints'
    /// in the path.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2018
    /// </summary>
    public class ChannelInteractionArea : MultiInteractionArea
    {
        #region PublicVariables
        [Header("Channel Settings")]
        [Tooltip("The ScriptableLineRenderer asset to define the visuals of the line. If left null, no line will be displayed.")]
        public Scriptable.ScriptableLineRenderer lineRendererInfo;
        [Tooltip("The radius of the path the user will have to follow.")]
        public float pathRadius = 0.05f;
        [Tooltip("The point on the game object that must remain in the channel interaction area at all times.")]
        public Transform pointToCheck;
        #endregion

        #region ProtectedVariables
        protected bool checkInsideCylinder;
        protected float lengthSq;
        protected float radiusSq;
        protected GameObject interactableObject;
        protected LineRenderer lineRender;
        #endregion

        #region UnityFunctions
        protected override void Awake()
        {
            base.Awake();

            enforceOrder = true;

            radiusSq = Mathf.Pow(pathRadius, 2);
        }

        protected void OnEnable()
        {
            if(lineRendererInfo != null)
            {
                lineRender = gameObject.AddComponent<LineRenderer>();
                lineRender.material = lineRendererInfo.lineMaterial;
                lineRender.startWidth = lineRendererInfo.startLineWidth;
                lineRender.endWidth = lineRendererInfo.endLineWidth;
                lineRender.positionCount = childrenAreas.Count;

                for(int n = 0; n < childrenAreas.Count; n++)
                {
                    lineRender.SetPosition(n, childrenAreas[n].transform.position);
                }
            }

            // Hook into enter events of the interaction areas
            for (int n = 0; n < childrenAreas.Count; n++)
            {
                childrenAreas[n].ObjectEnteredInteractionArea += ChildEnterInteractionArea;
            }

            StartState();
        }

        protected void OnDisable()
        {
            if(lineRender != null)
            {
                Destroy(lineRender);
            }

            for (int n = 0; n < childrenAreas.Count; n++)
            {
                childrenAreas[n].ObjectEnteredInteractionArea -= ChildEnterInteractionArea;
            }
        }

        protected override void Update()
        {
            base.Update();

            // The user has started using the interaction area so it needs to keep checking to see if they are inside the path
            if (checkInsideCylinder)
            {
                // If either of these statements are true, the user has left the channel interaction area, either they:
                // a) left the interaction area and the current interaction object is null or
                // b) the tracked point is outside of the cylinder
                if (childrenAreas[completionCount].CurrentInteractionObject == null
                    && !PointInsideCylinder(childrenAreas[completionCount].transform.position, 
                                            childrenAreas[completionCount + 1].transform.position, 
                                            lengthSq, 
                                            radiusSq, 
                                            pointToCheck.position))
                {
                    // No longer using the interaction area
                    checkInsideCylinder = false;

                    StartState();

                    OnObjectInterruptInteractionArea(SetInteractionAreaEvent(interactableObject));
                }
            }
        }
        #endregion

        #region EventCalls
        private void ChildEnterInteractionArea(object sender, InteractionAreaEventArgs interactionEvent)
        {
            checkInsideCylinder = true;

            InteractionArea area = sender as InteractionArea;
            interactableObject = interactionEvent.interactionObject;

            // Keep track of which child you're on
            completionCount = area.transform.GetSiblingIndex();
            pointToCheck = interactableObject.transform.Find("Point");

            // Start condition - Only want to send this once on the first visit
            if (completionCount == 0)
            {
                MoveToNextCylinder();

                OnObjectUsedInteractionArea(SetInteractionAreaEvent(interactableObject));
            }
            // End condition - Last IA in the children was reached
            else if (completionCount == childrenAreas.Count - 1)
            {
                checkInsideCylinder = false;

                StartState();

                OnObjectFinishedInteractionArea(SetInteractionAreaEvent(interactableObject));
            }
            else
            {
                MoveToNextCylinder();
            }
        }
        #endregion

        #region ChannelFunctions
        private void StartState()
        {
            SwitchChildren(0);
        }

        // Called when the user reaches the next interaction area in the list. Activates the next Interaction and gets the 
        // new distance between this interaction area and the next
        private void MoveToNextCylinder()
        {
            // Activate next object so its interaction area can be triggered
            SwitchChildren(completionCount + 1);

            // Get the length of the next cylindrical section
            lengthSq = Mathf.Pow(Vector3.Distance(childrenAreas[completionCount].transform.position, childrenAreas[completionCount + 1].transform.position), 2);
        }

        /// <summary>
        /// Given some parameters of a cylinder and a test point, will return true if that given
        /// point is inside the 3D space of the cylinder as defined by the two end points.
        /// 
        /// Adopted from http://www.flipcode.com/archives/Fast_Point-In-Cylinder_Test.shtml
        /// </summary>
        /// <param name="pt1">First end point of the cylinder</param>
        /// <param name="pt2">Other end point of the cylinder</param>
        /// <param name="lengthsq">The squared length of the cylinder</param>
        /// <param name="radius_sq">The squared radius of the cylinder</param>
        /// <param name="testpt">The point to check if its inside the cylinder</param>
        /// <returns></returns>
        private bool PointInsideCylinder(Vector3 pt1, Vector3 pt2, float lengthsq, float radius_sq, Vector3 testpt)
        {
            // translate so pt1 is origin.  Make vector from pt1 to pt2.  Need for this is easily eliminated
            Vector3 cylinderVector = pt2 - pt1;

            // vector from pt1 to test point.
            Vector3 vectorToPoint = testpt - pt1;

            // Dot the d and pd vectors to see if point lies behind the cylinder cap at pt1.x, pt1.y, pt1.z
            float dot = Vector3.Dot(cylinderVector, vectorToPoint);

            // If dot is less than zero the point is behind the pt1 cap.
            // If greater than the cylinder axis line segment length squared then the point is outside the other end cap at pt2.
            if (dot < 0.0f || dot > lengthsq)
            {
                return false;
            }
            else
            {
                // Point lies within the parallel caps, so find distance squared from point to line, using the fact that 
                // sin ^2 + cos^2 = 1 the dot = cos() * |d||pd|, and cross*cross = sin^2 * |d|^2 * |pd|^2
                // Careful: '*' means mult for scalars and dotproduct for vectors
                // In short, where dist is pt distance to cyl axis: 
                // dist = sin( pd to d ) * |pd|
                // distsq = dsq = (1 - cos^2( pd to d)) * |pd|^2
                // dsq = ( 1 - (pd * d)^2 / (|pd|^2 * |d|^2) ) * |pd|^2
                // dsq = pd * pd - dot * dot / lengthsq
                // where lengthsq is d*d or |d|^2 that is passed into this function distance squared to the cylinder axis:

                float dsq = Vector3.Dot(vectorToPoint, vectorToPoint) - dot * dot / lengthsq;

                if (dsq > radius_sq)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        #endregion
    }
}