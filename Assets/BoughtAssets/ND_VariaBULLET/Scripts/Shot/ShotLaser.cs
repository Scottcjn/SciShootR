#region Script Synopsis
    //A standard laser type shot which instantiates beginning, middle and end segments.
    //Learn more about laser type shots at: https://neondagger.com/variabullet2d-in-depth-shot-guide/#laser-shots
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class ShotLaser : ShotLaserBase
    {
        [Header("Laser Settings")]

        [Tooltip("Sets the base laser behavior as always attached to the emitter (continuous) or detachable (packet).")]
        public LaserExpression expressionType = LaserExpression.continuous;

        private BasicAnimation mainAnim;
        private int releaseDirection;

        public override void InitialSet()
        {
            base.InitialSet();

            if (expressionType == LaserExpression.continuous)
                FiringScript.OnStoppedFiring.AddListener(destroy);
            else
                FiringScript.OnStoppedFiring.AddListener(UnParent);
        }

        public override void Start()
        {
            base.Start();

            maxDistance = (CalcObject.getMaxScreenDistance() + AddedMaxDistance) * ppu * globalDirection;

            originGo = initLaserParts(new GameObject("Origin"), new Vector2(0, 0), OriginImg);
            mainGo = initLaserParts(new GameObject("Main"), new Vector2(originGo.GetComponent<SpriteRenderer>().sprite.bounds.size.x * globalDirection, 0), MainImg);
            tipGo = initLaserParts(new GameObject("Tip"), new Vector2(0, 0), TipImg);

            originAnim = initAnimations(originGo, OriginImg);
            mainAnim = initAnimations(mainGo, MainImg);
            blastAnim = initAnimations(tipGo, BlastImg);
            tipAnim = initAnimations(tipGo, TipImg);
        }

        protected override void UnParent()
        {
            FiringScript.OnStoppedFiring.RemoveListener(UnParent);
            transform.parent = null;

            ShotLaserPacket packetScript = gameObject.AddComponent<ShotLaserPacket>();
            packetScript.ShotSpeed = this.ShotSpeed;
            packetScript.ShotDirection = this.globalDirection;
            packetScript.ReleaseDirection = this.releaseDirection;

            packetScript.FrameSkip = this.FrameSkip;
            packetScript.originAnim = this.originAnim;
            packetScript.mainAnim = this.mainAnim;
            packetScript.tipAnim = this.tipAnim;

            Destroy(this);
        }

        protected override void animate()
        {
            base.animate();

            mainAnim.SyncAnimate(originAnim.FrameNum);
            releaseDirection = (transform.parent.lossyScale.x < 0) ? -1 : 1;              
        }

        protected override void movement()
        {
            float speed = scaledSpeed * ppu;

            RaycastHit2D ray = boxCast();
            //Utilities.DebugDrawLine(gameObject.transform.position, ray.point); //visualizes raycast in scene view for debug purposes

            if (ray.collider == null)
                laserExtension(speed);
            else
                laserCollision(ray, speed);

            tipGo.transform.localPosition = new Vector2(mainGo.transform.localScale.x / ppu + originGo.GetComponent<SpriteRenderer>().sprite.bounds.size.x * globalDirection, 0);

            if (!collided)
                return;

            if (expressionType == LaserExpression.packet)
                sendPacketCollData(ray);
            else
                sendLaserCollData(ray);
        }

        private void laserExtension(float speed)
        {
            float laserExtension = Mathf.MoveTowards(mainGo.transform.localScale.x, maxDistance, Time.deltaTime * speed);
            mainGo.transform.localScale = new Vector3(laserExtension, mainGo.transform.localScale.y, 1);

            tipAnim.SyncAnimate(originAnim.FrameNum);
        }

        private void laserCollision(RaycastHit2D ray, float speed)
        {
            //more accurate than ray.distance. Fixes discrepency between when fired on horizontal plane vs vertical plane when rotated
            float distance = Vector2.Distance(mainGo.transform.position, ray.point);
            float blastArea = BlastImg[0].bounds.size.x * ppu * CollisionTipZone;

            if (mainGo.transform.localScale.x * globalDirection >= distance * ppu - blastArea)
            {                                              //Fix for left facing
                mainGo.transform.localScale = new Vector3((distance * globalDirection * ppu) - (blastArea * globalDirection), mainGo.transform.localScale.y, 1);
                blastAnim.SyncAnimate(originAnim.FrameNum);
                collided = true;
            }
            else
            {                                                                           //Fix for left facing
                float laserExtension = Mathf.MoveTowards(mainGo.transform.localScale.x, (distance * globalDirection * ppu) - (blastArea * globalDirection), Time.deltaTime * speed);
                mainGo.transform.localScale = new Vector3(laserExtension, mainGo.transform.localScale.y, 1);

                float bufferZone = 0.002f;

                if (laserExtension * globalDirection >= (distance * ppu) - blastArea - bufferZone)
                { blastAnim.SyncAnimate(originAnim.FrameNum); collided = true; }
                else
                    tipAnim.SyncAnimate(originAnim.FrameNum);
            }
        }

        private GameObject initLaserParts(GameObject part, Vector3 initPosition, Sprite[] sprite)
        {
            part.transform.parent = this.transform;
            part.transform.localPosition = initPosition;
            part.transform.localScale = new Vector3(part.transform.localScale.x * globalDirection, part.transform.localScale.y);
            part.transform.rotation = new Quaternion();

            SpriteRenderer partSprite = part.AddComponent<SpriteRenderer>();
            partSprite.sprite = sprite[0];
            partSprite.sortingLayerName = sortLayer;
            partSprite.sortingOrder = sortOrder;
            partSprite.color = FiringScript.SpriteColor;

            return part;
        }

        public enum LaserExpression
        {
            continuous,
            packet
        }
    }
}