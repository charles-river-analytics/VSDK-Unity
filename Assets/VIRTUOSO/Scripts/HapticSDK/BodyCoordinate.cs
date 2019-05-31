using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// This monobehavior is attached to the GameObject for the neccessary body and responsible for determining 
    /// where on that body part a hit has taken place in the body coordinate space. This object must also be
    /// set up to have the correct polar axis to get the expected behavior with haptic patterns.
    /// 
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    [RequireComponent(typeof(CapsuleCollider)), ExecuteInEditMode]
    public class BodyCoordinate : MonoBehaviour
    {
        #region PublicVariables
        [Tooltip("Automatically set if using on a rigged character. Otherwise, set what portion of the body this is suppose to be.")]
        public HumanBodyBones attachedBody;
        [Tooltip("The axis (local space) at which 0 degrees starts for calculating hit direction when looking down the height of the collider.")]
        public Vector3 polarAxis = Vector3.right;
        [Tooltip("When true, the body coordinate will traverse a pattern in a Counter-Clockwise direction.")]
        public bool invertAngleDirection = false;
        [Tooltip("Select this if you would like to see the Polar Axis on the GameObject without having it selected.")]
        public bool drawGizmoUnselected = false;
        #endregion

        #region PrivateVariables
        private CapsuleCollider capCollider;
        private HapticManager manager;
        private Vector3 distalColliderPosition;
        private Vector3 proximalColliderPosition;
        // If the collider's proximal end is actually the distal (due to the way the transform are set up), then mark this as true
        private bool isHeightInverted = false;
#if UNITY_EDITOR
        private const float ARROW_SIZE_MULTIPLIER = .75f;
        private const float GIZMO_SPHERE_SIZE = .005f;
#endif
        #endregion

        #region PublicAPI
        public Vector3 ProximalPosition
        {
            get
            {
                if (isHeightInverted)
                {
                    return distalColliderPosition;
                }
                else
                {
                    return proximalColliderPosition;
                }
            }
        }

        public Vector3 DistalPosition
        {
            get
            {
                if (isHeightInverted)
                {
                    return proximalColliderPosition;
                }
                else
                {
                    return distalColliderPosition;
                }
            }
        }

        /// <summary>
        /// Used to determine the normalized height and hit angle on the collider
        /// </summary>
        /// <param name="collisionPoint">The position in world space</param>
        /// <returns>The BodyCoordinateHit that contains the normalized height and angle on the capsule</returns>
        public BodyCoordinateHit CalculateBodyCoordinateHitFromPosition(Vector3 collisionPoint)
        {
            Vector3 localHitPosition = transform.InverseTransformPoint(collisionPoint);

            float hitHeight = CalculateNormalizedHitHeightFromLocalPosition(localHitPosition);
            float hitDegree = CalculateDegreeOfCollisionFromPosition(collisionPoint);

            return new BodyCoordinateHit(hitHeight, hitDegree);
        }

        /// <summary>
        /// Calculates the world position of a BodyCoordinateHit.
        /// </summary>
        /// <param name="bodyHit">The height and angle of the collider to find the position</param>
        /// <param name="accountForCapsule">If true, the position should account for the circle ends on the capsule</param>
        /// <returns>The world position based on the collider and provided body hit</returns>
        public Vector3 CalculatePositionFromBodyCoordinateHit(BodyCoordinateHit bodyHit, bool accountForCapsule = false)
        {
            float heightPosition = CalculatePositionFromNormalizedHeight(bodyHit.hitHeight);
            Vector3 radius = CalculatePositionFromAngle(bodyHit, heightPosition, accountForCapsule);

            switch (capCollider.direction)
            {
                // X Axis
                case 0: 
                    return new Vector3(heightPosition, radius.y, radius.z);
                // Y Axis
                case 1:
                    return new Vector3(radius.x, heightPosition, radius.z);
                // Z Axis
                case 2: 
                    return new Vector3(radius.x, radius.y, heightPosition);
                default:
                    return Vector3.zero;
            }
        }
        #endregion

        #region BodyCoordinateMethods
        private float CalculateNormalizedHeight(float value, float extremeValueOne, float extremeValueTwo)
        {
            // Comparisons with values less than 0 can screw things up, so shift everything up by the lowest value
            float minVal = Mathf.Abs(Mathf.Min(value, extremeValueOne, extremeValueTwo));
            value += minVal;
            extremeValueOne += minVal;
            extremeValueTwo += minVal;

            float normalizedValue;

            if (extremeValueOne < extremeValueTwo)
            {
                normalizedValue = Utilities.VirtMath.Normalize(value, extremeValueOne, extremeValueTwo);
            }
            else
            {
                normalizedValue = Utilities.VirtMath.Normalize(value, extremeValueTwo, extremeValueOne);
            }

            if (isHeightInverted)
            {
                return (1.0f - normalizedValue);
            }
            else
            {
                return normalizedValue;
            }
        }

        private float CalculateNormalizedHitHeightFromLocalPosition(Vector3 localHitPosition)
        {
            float hitHeight = 0.0f;

            // Use the direction of the collider to calculate the normalized height in local space
            switch (capCollider.direction)
            {
                // X Axis
                case 0: 
                    hitHeight = CalculateNormalizedHeight(localHitPosition.x, proximalColliderPosition.x, distalColliderPosition.x);

                    break;
                // Y Axis
                case 1: 
                    hitHeight = CalculateNormalizedHeight(localHitPosition.y, proximalColliderPosition.y, distalColliderPosition.y);

                    break;
                // Z Axis
                case 2: 
                    hitHeight = CalculateNormalizedHeight(localHitPosition.z, proximalColliderPosition.z, distalColliderPosition.z);

                    break;
            }

            return Mathf.Clamp01(hitHeight);
        }

        private float CalculatePositionFromNormalizedHeight(float normalizedHeight)
        {
            float colliderStartPoint = 0;

            switch (capCollider.direction)
            {
                // X Axis
                case 0: 
                    colliderStartPoint = transform.TransformPoint(ProximalPosition).x;
                    break;
                // Y Axis
                case 1: 
                    colliderStartPoint = transform.TransformPoint(ProximalPosition).y;
                    break;
                // Z Axis
                case 2: 
                    colliderStartPoint = transform.TransformPoint(ProximalPosition).z;
                    break;
            }

            if (isHeightInverted)
            {
                colliderStartPoint -= normalizedHeight * capCollider.height;
            }
            else
            {
                colliderStartPoint += normalizedHeight * capCollider.height;
            }

            return colliderStartPoint;
        }

        private Vector3 CalculatePositionFromAngle(BodyCoordinateHit bodyHit, float heightPosition = 0.0f, bool positionOnCapsule = false)
        {
            // Short circuit, if the height is at the extremes, then it will be at the poles
            if (positionOnCapsule)
            {
                if (bodyHit.hitHeight == 0.0f)
                {
                    return transform.TransformPoint(ProximalPosition);
                }
                else if (bodyHit.hitHeight == 1.0f)
                {
                    return transform.TransformPoint(DistalPosition);
                }
            }

            // Calculate the directional vector towards the hit point
            Vector3 rotationDirection = Vector3.zero;

            if (invertAngleDirection)
            {
                bodyHit.hitAngle = 360 - bodyHit.hitAngle;
            }

            switch (capCollider.direction)
            {
                // X Axis
                case 0: 
                    rotationDirection = Quaternion.AngleAxis(bodyHit.hitAngle, transform.right) * polarAxis;
                    break;
                // Y Axis
                case 1: 
                    rotationDirection = Quaternion.AngleAxis(bodyHit.hitAngle, transform.up) * polarAxis;
                    break;
                // Z Axis
                case 2: 
                    rotationDirection = Quaternion.AngleAxis(bodyHit.hitAngle, transform.forward) * polarAxis;
                    break;
            }

            // Account for the angle of the collider
            Vector3 positionByAngle = rotationDirection * capCollider.radius;

            // Since the capsule has two circles at the ends, make sure the position is along the curve on the circle if required
            if (positionOnCapsule)
            {
                // Don't bother doing this if new height is not at one of the circles extremes
                float radiusNormalizedPerHeight = capCollider.radius / capCollider.height;

                if (bodyHit.hitHeight >= radiusNormalizedPerHeight && bodyHit.hitHeight <= (1 - radiusNormalizedPerHeight))
                {
                    return transform.TransformPoint(positionByAngle);
                }

                // Get the transform that is aligned with this point along the axis
                Vector3 leveledTransform = transform.TransformPoint(capCollider.center);

                switch (capCollider.direction)
                {
                    case 0: // X Axis
                        leveledTransform = new Vector3(heightPosition, leveledTransform.y, leveledTransform.z);
                        positionByAngle = new Vector3(heightPosition, positionByAngle.y, positionByAngle.z);
                        break;
                    case 1: // Y Axis
                        leveledTransform = new Vector3(leveledTransform.x, heightPosition, leveledTransform.z);
                        positionByAngle = new Vector3(positionByAngle.x, heightPosition, positionByAngle.z);
                        break;
                    case 2: // Z Axis
                        leveledTransform = new Vector3(leveledTransform.x, leveledTransform.y, heightPosition);
                        positionByAngle = new Vector3(positionByAngle.x, positionByAngle.y, heightPosition);
                        break;
                }

                // Get the direction from the point towards the transform
                Vector3 directionTowardsCapsule = leveledTransform - positionByAngle;

                RaycastHit[] hitInfo;

                // Shoot a ray to find the edge of the capsule collider in the calculated direction
                hitInfo = Physics.RaycastAll(positionByAngle, directionTowardsCapsule.normalized, .5f);

                for (int n = 0; n < hitInfo.Length; n++)
                {
                    if (hitInfo[n].collider.gameObject.name == "Capsule")
                    {
                        // Move the positionByAngle in the direction towards the transform at the proper distance
                        positionByAngle += (directionTowardsCapsule.normalized * hitInfo[n].distance);
                    }
                }
            }

            return transform.TransformPoint(positionByAngle);
        }

        private float CalculateDegreeOfCollisionFromPosition(Vector3 hitPosition)
        {
            Vector3 hitDirection = hitPosition - transform.position;
            float signAngle = 0.0f;

            switch (capCollider.direction)
            {
                // X Axis
                case 0: 
                    signAngle = Utilities.VirtMath.AngleSigned(transform.TransformDirection(polarAxis), hitDirection, transform.right);

                    break;
                // Y Axis
                case 1: 
                    signAngle = Utilities.VirtMath.AngleSigned(transform.TransformDirection(polarAxis), hitDirection, transform.up);

                    break;
                // Z Axis
                case 2: 
                    signAngle = Utilities.VirtMath.AngleSigned(transform.TransformDirection(polarAxis), hitDirection, transform.forward);

                    break;
            }

            // Make sure a value between 0 and 360 is returned
            if (signAngle < 0)
            {
                signAngle += 360;
            }

            if (invertAngleDirection)
            {
                signAngle = 360 - signAngle;
            }

            return signAngle;
        }

        private void CalculateColliderEndPoints(Vector3 directionOfCollider)
        {
            // If the height is less than double the radius, then it acts as a sphere
            float halfLengthOfCollider = (capCollider.height >= 2 * capCollider.radius) ? capCollider.height / 2f : capCollider.radius;

            proximalColliderPosition = capCollider.center - (directionOfCollider * halfLengthOfCollider);
            distalColliderPosition = capCollider.center + (directionOfCollider * halfLengthOfCollider);

            // Depending on the transform that the collider is attached to, the proximal end may actually be closer to the transform and 
            // therefore be the distal end, so mark that as inverted so that the normalized height value will be correct
            float proximalDistanceToTransformOrigin = Vector3.Distance(proximalColliderPosition, Vector3.zero);
            float distalDistanceToTransformOrigin = Vector3.Distance(distalColliderPosition, Vector3.zero);

            isHeightInverted = (distalDistanceToTransformOrigin < proximalDistanceToTransformOrigin);
        }
        #endregion

        #region UnityFunctions
        void Awake()
        {
            capCollider = GetComponent<CapsuleCollider>();

            manager = FindObjectOfType<HapticManager>();

            // Calculate the local positions of the ends of the collider
            switch (capCollider.direction)
            {
                // X Axis
                case 0: 
                    CalculateColliderEndPoints(transform.InverseTransformDirection(transform.right));
                    break;
                // Y Axis
                case 1: 
                    CalculateColliderEndPoints(transform.InverseTransformDirection(transform.up));
                    break;
                // Z Axis
                case 2: 
                    CalculateColliderEndPoints(transform.InverseTransformDirection(transform.forward));
                    break;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            BodyCoordinateHit bodyHit = CalculateBodyCoordinateHitFromPosition(collision.contacts[0].point);
            HapticPatternCollisionHolder hitPattern = collision.gameObject.GetComponent<HapticPatternCollisionHolder>();

            if (hitPattern != null)
            {
                manager.BodyPartHit(attachedBody, bodyHit, hitPattern.GetHapticPattern());
            }
            else
            {
                manager.BodyPartHit(attachedBody, bodyHit);
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if(transform.hasChanged)
            {
                transform.localScale = Vector3.one;
            }
        }

        private void OnDrawGizmos()
        {
            if (drawGizmoUnselected)
            {
                DrawGizmo();
            }
        }

        private void OnDrawGizmosSelected()
        {
            DrawGizmo();
        }

        private void DrawGizmo()
        {
            Handles.color = Color.cyan;
            Handles.ArrowHandleCap(
                0,
                transform.TransformPoint(capCollider.center),
                transform.rotation * ((polarAxis != Vector3.zero) ? Quaternion.LookRotation(polarAxis) : Quaternion.identity),
                HandleUtility.GetHandleSize(transform.position) * ARROW_SIZE_MULTIPLIER,
                EventType.Repaint);

            if (isHeightInverted)
            {
                Gizmos.DrawSphere(transform.TransformPoint(capCollider.center), GIZMO_SPHERE_SIZE);

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.TransformPoint(distalColliderPosition), GIZMO_SPHERE_SIZE);

                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(transform.TransformPoint(proximalColliderPosition), GIZMO_SPHERE_SIZE);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.TransformPoint(proximalColliderPosition), GIZMO_SPHERE_SIZE);

                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(transform.TransformPoint(distalColliderPosition), GIZMO_SPHERE_SIZE);
            }
        }
#endif
        #endregion
    }
}