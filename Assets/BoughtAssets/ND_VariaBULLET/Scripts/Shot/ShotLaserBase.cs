#region Script Synopsis
    //Abstract base class for Laser type shots
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public abstract class ShotLaserBase : ShotBase
    {
        [Range(1, 30)]
        [Tooltip("Sets the amount of hits produced when collides with object that has ShotCollisionDamage script attached. [higher number = more collisions].")]
        public int HitsPerSecond;

        [Header("Animation Settings")]

        [Tooltip("Animation frames for the start origin laser segment.")]
        public Sprite[] OriginImg;

        [Tooltip("Animation frames for the middle main laser segment.")]
        public Sprite[] MainImg;

        [Tooltip("Animation frames for the end tip laser segment.")]
        public Sprite[] TipImg;

        [Tooltip("Animation frames for the end blast laser segment when colliding.")]
        public Sprite[] BlastImg;

        protected BasicAnimation originAnim;
        protected BasicAnimation tipAnim;
        protected BasicAnimation blastAnim;

        protected GameObject originGo;
        protected GameObject mainGo;
        protected GameObject tipGo;

        [Range(0, 10)]
        [Tooltip("Sets interval for changing frames. Lower number = faster animation.")]
        public int FrameSkip;

        [Header("Collision Settings")]

        [Range(.1f, .9f)]
        [Tooltip("Decreases collision thickness of the laser.")]
        public float CollisionThickness = 0.5f;

        [Range(0.1f, 1)]
        [Tooltip("Adjusts the range around the tip of the laser where collision occurs.")]
        public float CollisionTipZone = 0.5f;

        protected float ppu;
        protected float maxDistance;

        [Tooltip("Adds to how far the laser extends at maximum extension. Fixes truncated lasers.")]
        public int AddedMaxDistance;

        protected int globalDirection;

        protected int collidesWith;
        protected bool collided;

        private float _collTiming;
        private float collTiming
        {
            get { _collTiming = (_collTiming > 60 / HitsPerSecond) ? 0 : _collTiming; return _collTiming; }
            set { _collTiming = value; }
        }

        public override void InitialSet()
        {
            transform.parent = Emitter;
            transform.localPosition = new Vector2(ExitPoint, 0);
            transform.rotation = Emitter.rotation;
        }

        public override void Start()
        {
            ppu = MainImg[0].pixelsPerUnit;
            collidesWith = Physics2D.GetLayerCollisionMask(gameObject.layer);
            globalDirection = (transform.parent.lossyScale.x < 0) ? -1 : 1; //needed to get the parent transform lossyScale to determine firing side
        }

        protected void destroy()
        {
            FiringScript.OnStoppedFiring.RemoveListener(destroy);
            Destroy(gameObject);
        }

        public override void Update()
        {
            base.Update();
            animate();
        }

        public void LateUpdate() //required to check hit detection before next Update runs
        {
            movement();
        }

        protected abstract void movement();

        protected virtual void animate()
        {
            collided = false;

            originGo.SetActive(true);
            mainGo.SetActive(true);
            tipGo.SetActive(true);

            originAnim.Animate(FrameSkip);
        }

        protected BasicAnimation initAnimations(GameObject part, Sprite[] frames)
        {
            SpriteRenderer rend = part.GetComponent<SpriteRenderer>();
            return new BasicAnimation(ref rend, ref frames);
        }

        protected RaycastHit2D boxCast()
        {
            Vector2 directionToCast = getDirectionVector2D(transform.rotation.eulerAngles.z);
            float girth = MainImg[0].bounds.size.y * CollisionThickness;
            return Physics2D.BoxCast(mainGo.transform.position, new Vector2(girth, girth), 0, directionToCast, maxDistance, collidesWith);
        }

        protected RaycastHit2D lineCast()
        {
            Vector2 directionToCast = getDirectionVector2D(transform.rotation.eulerAngles.z);
            return Physics2D.Raycast(mainGo.transform.position, directionToCast, maxDistance, collidesWith);
        }

        //Gets the rotation angle of the gameobject, factors in lossyscale if parent is flipped and uses the returned vector to cast the ray in the appropriate direction
        protected Vector2 getDirectionVector2D(float angle)
        {
            Vector2 castAngle = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
            float flip = (transform.lossyScale.x >= 0) ? 1 : -1; //factors in the global scale in case a parent has been flipped.
            return castAngle * flip;
        }

        protected void sendLaserCollData(RaycastHit2D ray)
        {
            if (collTiming == 0)
                ray.collider.SendMessage("OnLaserCollision", new CollisionArgs(gameObject, ray.point, DamagePerHit), SendMessageOptions.DontRequireReceiver);

            collTiming += Timer.deltaCounter;
        }

        protected void sendPacketCollData(RaycastHit2D ray)
        {
            ray.collider.SendMessage("OnLaserCollision", new CollisionArgs(gameObject, ray.point, DamagePerHit), SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject);
        }

        protected override void OnOutBounds()
        {
            //No Implementation. maxDistance determines distance while OnStoppedFiring destroys object
            return;
        }
    }
}