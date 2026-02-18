#region Script Synopsis
    //A non-physics type shot that homes in on a single direct target or the closest member of a group of targets, at "burst" intervals.
    //Action and description of fields can be found at https://neondagger.com/variabullet2d-in-depth-shot-guide/#default-shot-prefabs
#endregion

using UnityEngine;
using System;

namespace ND_VariaBULLET
{
    public class ShotHomingInertial : ShotBaseAnimatable, IRePoolable
    {   
        protected Transform objectToFollow;
        private HomingCalc calc;
        private Rigidbody2D body;

        [Header("Homing Settings")]

        [Tooltip("Directly sets the transform of the target to follow. [takes precedence over targetFromTag].")]
        public Transform targetDirect;

        [Tooltip("Sets target(s) to follow by its tag.")]
        public string targetFromTag;

        [Tooltip("Sets speed of rotation towards an object when tracking it.")]
        public float TrackRotationSpeed = 20;

        [Tooltip("Sets a radius in units wherein the shot will recognize a homing target.")]
        public float EngageRadius = 2;

        [Range(1, 10)]
        [Tooltip("Sets an FPS interval at which point the shot re-checks for the closest target to home in on. [Higher number = more frequent re-check].")]
        public int RecalculationFPS = 3; //used to recalc closest target every 6-to-60 frames

        [Tooltip("Scales the initial propelling burst of force.")]
        public float InitialPush;

        [Tooltip("Sets the timer interval between propelling bursts of force. [Higher number = less bursts].")]
        public int BurstFrequency = 45;

        private Timer burstTimer;
        internal bool burstFlag;

        public override void InitialSet()
        {
            calc = new HomingCalc();
            burstTimer = new Timer(0);

            body = GetComponent<Rigidbody2D>();
            body.AddForce(
                new Vector2(ShotSpeed / 10 * InitialPush * Trajectory.x, ShotSpeed / 10 * InitialPush * Trajectory.y),
                ForceMode2D.Impulse
            );
                        
            base.InitialSet();
        }

        public override void Start()
        {
            base.Start();

            if (targetDirect != null)
                objectToFollow = targetDirect;
            else
            {
                if (!String.IsNullOrEmpty(targetFromTag))
                    objectToFollow = calc.findClosestObject(this.transform, targetFromTag);
            }
                
            RecalculationFPS = 60 / RecalculationFPS;
        }

        public override void Update()
        {
            base.Update();
            movement();
        }

        private void movement()
        {
            follow();
            burst();
        }

        private void follow()
        {
            if (targetDirect == null)
                calc.recalcClosestObject(this.transform, ref objectToFollow, RecalculationFPS, targetFromTag);

            bool lockedOn = calc.isWithinRadius(this.transform, objectToFollow, EngageRadius);

            setRotation(objectToFollow, lockedOn);
        }

        private void burst()
        {
            burstTimer.Run(BurstFrequency);

            if (burstTimer.Flag)
                body.AddForce(CalcObject.RotationToShotVector(transform.rotation.eulerAngles.z) * ShotSpeed / 5, ForceMode2D.Impulse);

            burstFlag = burstTimer.Flag;
        }

        private void setRotation(Transform obj, bool trackingEngaged)
        {
            if (obj != null)
            {
                if (trackingEngaged)
                {
                    Vector3 vectorToTarget = obj.position - transform.position;
                    transform.rotation = CalcObject.VectorToRotationSlerp(transform.rotation, vectorToTarget, TrackRotationSpeed);
                }
            }
        }
    }
}