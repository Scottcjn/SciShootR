#region Script Synopsis
    //A non-physics type shot that homes in on a single direct target or the closest member of a group of targets, at a constant speed.
    //Action and description of fields can be found at https://neondagger.com/variabullet2d-in-depth-shot-guide/#default-shot-prefabs
#endregion

using UnityEngine;
using System;

namespace ND_VariaBULLET
{
    public class ShotHoming : ShotBaseColorizable, IRePoolable
    {
        protected Transform objectToFollow;
        private HomingCalc calc;

        [Header("Homing Settings")]

        [Tooltip("Directly sets the transform of the target to follow. [Takes precedence over targetFromTag].")]
        public Transform targetDirect;

        [Tooltip("Sets target(s) to follow by tag.")]
        public string targetFromTag;

        [Tooltip("Sets the homing rotation behavior. [Tracking = rotates towards tracked object; Free = normal rotation setting apply].")]
        public HomingRotation RotationType;

        [Range(2, 40)]
        [Tooltip("Sets rotation speed when tracking type rotation is used for RotationType.")]
        public float TrackRotationSpeed = 20;

        [Range(2, 60)]
        [Tooltip("Sets the time in frames at which shots start homing on a target after being fired.")]
        public int homingEngageTime;
        private Timer homingEngage = new Timer(0);

        [Range(2, 100)]
        [Tooltip("Sets a radius in units wherein the shot will recognize a homing target.")]
        public float EngageRadius = 2;

        [Range(1, 10)]
        [Tooltip("Sets an FPS interval at which point the shot re-checks for the closest target to home in on. [Higher number = more frequent re-check].")]
        public int RecalculationFPS = 3; //used to recalc closest target every 6-to-60 frames.

        private Vector2 lastKnownPos;
        private bool resetSwitch;
        private bool assigned;

        private Vector2 prevShotPos;

        public override void InitialSet()
        {
            calc = new HomingCalc();
            lastKnownPos = new Vector2(0, 0);
            homingEngage.Reset();
            
            resetSwitch = false;
            assigned = false;

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
            calc = new HomingCalc();
        }

        public override void Update()
        {
            base.Update();
            movement();

            prevShotPos = transform.position;
        }

        private void movement()
        {
            homingEngage.RunOnce(homingEngageTime);

            if (!homingEngage.Flag)
                linear();
            else              
                follow();
        }

        private void linear()
        {
            transform.position += new Vector3(scaledSpeed * Time.deltaTime * Trajectory.x, scaledSpeed * Time.deltaTime * Trajectory.y, 0);
        }

        private void moveTowards()
        {
            transform.position = Vector3.MoveTowards(transform.position, lastKnownPos, Time.deltaTime * scaledSpeed);
        }

        private void follow()
        {
            if (targetDirect == null)
                calc.recalcClosestObject(this.transform, ref objectToFollow, RecalculationFPS, targetFromTag);

            bool lockedOn = calc.isWithinRadius(this.transform, objectToFollow, EngageRadius);

            if (objectToFollow != null && lockedOn)
            {
                lastKnownPos = new Vector2(objectToFollow.position.x, objectToFollow.position.y);
                assigned = true;

                if (resetSwitch)
                    resetSwitch = false;
            }
                
            if (objectToFollow == null || !lockedOn)
                {  resetLinearAngle(); linear(); }
            else
                moveTowards();

            setRotation(lockedOn);
        }

        private void resetLinearAngle()
        {
            if (!resetSwitch && assigned)
            {
                float angle = CalcObject.AngleBetweenVectors(transform.position, lastKnownPos);
                Trajectory = CalcObject.RotationToShotVector(angle);
                resetSwitch = true;
            }
        }

        private void setRotation(bool trackingEngaged)
        {
            if (RotationType == HomingRotation.Tracking)
            {
                if (trackingEngaged)
                {
                    Vector3 direction = (Vector2)transform.position - prevShotPos;
                    transform.rotation = CalcObject.VectorToRotationSlerp(transform.rotation, direction, TrackRotationSpeed);
                }
                else
                {
                    transform.rotation = CalcObject.VectorToRotationSlerp(transform.rotation, Trajectory, TrackRotationSpeed);
                }
            }
        }
    }

    public enum HomingRotation
    {
        Tracking,
        Free
    }
}