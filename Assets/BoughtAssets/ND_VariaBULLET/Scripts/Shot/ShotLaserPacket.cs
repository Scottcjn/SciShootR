#region Script Synopsis
    //A laser type shot which detaches into a bullet-type shot packet.
    //Learn more about laser type shots at: https://neondagger.com/variabullet2d-in-depth-shot-guide/#laser-shots
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class ShotLaserPacket : ShotBase
    {
        public int ReleaseDirection;
        public int ShotDirection;
        public BasicAnimation originAnim;
        public BasicAnimation mainAnim;
        public BasicAnimation tipAnim;
        public int FrameSkip;

        private float accumulatorX = 0;
        private GameObject packet;

        public override void Start()
        {
            packet = new GameObject("Packet");
            Quaternion storedRotation = gameObject.transform.rotation; //store rotation to apply after box collider is set (collider via sprite.bounds requires a non-rotated object)
            gameObject.transform.rotation = new Quaternion();

            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();

            float x = 0;
            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer renderer in renderers)
                x += renderer.bounds.size.x;

            collider.offset = new Vector2(x / 2 * ShotDirection, 0);
            collider.size = new Vector2(x, 1);

            gameObject.transform.parent = packet.transform;
            packet.transform.position = gameObject.transform.position;
            packet.transform.rotation = storedRotation;
            gameObject.transform.localPosition = new Vector2(0, 0);
        }

        public override void Update()
        {
            base.Update();
            movement();
        }

        private void movement()
        {
            accumulatorX = scaledSpeed * Time.deltaTime;
            transform.localPosition += new Vector3(accumulatorX * ReleaseDirection, 0, 0);

            originAnim.Animate(FrameSkip);
            mainAnim.SyncAnimate(originAnim.FrameNum);
            tipAnim.SyncAnimate(originAnim.FrameNum);
        }

        protected override void OnOutBounds()
        {
            if (CalcObject.IsOutBounds(transform))
                Destroy(packet);
        }

        protected override void RePoolOrDestroy()
        {
            Destroy(packet);
        }
    }
}