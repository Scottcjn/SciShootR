#region Script Synopsis
    //An rotatable & animatable bullet subclass. Allows for rotating the bullet and animating since it inherits ShotBaseAnimatable
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class ShotBaseRotatable : ShotBaseAnimatable
    {
        [Tooltip("Sets this shot's starting rotation to a random value.")]
        public bool RandomStartRotation;

        [Range(0, 60)]
        [Tooltip("Sets the base rotation speed for this shot.")]
        public float rotationSpeed;

        [Range(0, 10)]
        [Tooltip("Sets a random +/- range to base rotationSpeed.")]
        public float rotationSpeedRange;

        [Tooltip("Sets the direction that this shot rotates.")]
        public RotationDir RotationDirection;
        private float rotation;

        private Timer rotationTimer = new Timer(0);

        public override void InitialSet()
        {
            base.InitialSet();

            rotationTimer.Reset();

            if (RandomStartRotation)
                transform.Rotate(0, 0, UnityEngine.Random.Range(0, 360));
            else if (!InheritStartRotation)
                if (transform.parent == null)
                    transform.rotation = new Quaternion(); //fixes accidentally inheriting rotation upon repool.

            float dir;

            switch (RotationDirection)
            {
                case RotationDir.random :
                    dir = UnityEngine.Random.Range(0, 2) * 2 - 1;
                    break;
                case RotationDir.counter :
                    dir = 1;
                    break;
                default :
                    dir = -1;
                    break;
            }           
            rotation = Random.Range(rotationSpeed - rotationSpeedRange, rotationSpeed + rotationSpeedRange) * dir;
        }

        public override void Update()
        {
            base.Update();
            freeRotation();
        }

        private void freeRotation()
        {
            if (RotationDirection == RotationDir.directional)
                directionalCheck();

            transform.Rotate(0f, 0f, rotation * Timer.deltaCounter * scale);
        }

        private void directionalCheck()
        {
            if (!rotationTimer.Flag)
            {
                rotationTimer.Run(2);

                if (rotationTimer.Flag)
                    rotation = Mathf.Abs(rotation) * Direction.x * -1;
            }
        }

        public enum RotationDir
        {
            counter,
            clockwise,
            random,
            directional
        }
    }
}