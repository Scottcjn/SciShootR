#region Script Synopsis
    //A bullet type shot which changes it's trajectory once, resulting in an "elbow" re-angled path.
    //Action and description of fields can be found at https://neondagger.com/variabullet2d-in-depth-shot-guide/#default-shot-prefabs
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class ShotReAngle : ShotNonPhysics, IRePoolable
    {
        [Header("ReAngle Settings")]

        [Tooltip("Enables proper shot behavior for vertical orientation. [Disabled = horizontal rotation].")]
        public bool VerticalOrientation;
        private int vertMod;

        [Tooltip("Sets the degree of the angling effect.")]
        public int Embellish;

        [Tooltip("Enables an auto-spray effect, changing the embellish angle according to AutoEmbellishRange and AutoEmbellishSpeed.")]
        public bool AutoEmbellish;

        [Tooltip("Sets the range of the AutoEmbellish spray effect on X/Y.")]
        public Vector2 AutoEmbellishRange;

        [Range(1,10)]
        [Tooltip("Sets the speed of the AutoEmbellish spray effect.")]
        public int AutoEmbellishSpeed;

        [Tooltip("Sets the time in frames at which point the shot changes angle.")]
        public int ReAngleTime;
        private Timer reAngle = new Timer(0);
        private bool reAngleTriggered;

        public override void InitialSet()
        {
            base.InitialSet();

            reAngle.Reset();
            reAngleTriggered = false;

            vertMod = (VerticalOrientation) ? 90 : 0;
        }

        public override void Update()
        {
            base.Update();
            
            if (Emitter == null) //if weapon is switched out, emitter becomes null so can no longer can process re-angle
                return;

            if (AutoEmbellish)
                spray();

            movement();
        }

        private void movement()
        {
            reAngle.RunOnce(ReAngleTime);

            if (reAngle.Flag)
            {
                if (reAngleTriggered)
                    return;

                float angle = Emitter.transform.rotation.eulerAngles.z - vertMod;
                angle += (angle < 0) ? 360 : 0;

                float embellish = Embellish;
                embellish = (angle < 180) ? embellish * -1 : embellish;

                if (Emitter.transform.lossyScale.x < 0)
                    embellish -= 180;

                Trajectory = CalcObject.RotationToShotVector(embellish + vertMod);
                transform.rotation = Quaternion.AngleAxis(embellish + vertMod, Vector3.forward);

                reAngleTriggered = true;
            }
        }

        private void spray()
        {
            if (!AutoEmbellish)
                return;

            Embellish = (int)Mathf.Lerp(AutoEmbellishRange.x, AutoEmbellishRange.y,
                Mathf.PingPong(Time.time * AutoEmbellishSpeed * Timer.deltaCounter, 1)
            );
        }
    }
}