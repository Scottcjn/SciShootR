#region Script Synopsis
    //A more dynamic laser type shot which uses sprite masking along with multiple animated sections
    //Learn more about laser type shots at: https://neondagger.com/variabullet2d-in-depth-shot-guide/#laser-shots
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class ShotLaserMask : ShotLaserBase
    {
        [Header("LaserMask Settings")]

        [Tooltip("The sprite used as mask.")]
        public Sprite MaskingSprite;

        [Tooltip("When two or more masked lasers interact, set this to a different value on each to de-conflict.")]
        public int UniqueMaskId;

        [Tooltip("Increments the start animation frame in each successive main chunk, for progressive seamless animations.")]
        public bool StaggerAnimations;

        private BasicAnimation[] mainAnims;

        private GameObject mainMask;
        private GameObject tipMask;

        [Tooltip("Adjusts the starting point for the main section.")]
        public float MainAdjust;

        [Tooltip("Adjusts main masking scale on Y-axis")]
        public float MainMaskYScale = 1;

        [Tooltip("Adjusts the starting point for the tip section.")]
        public float TipAdjust;

        [Tooltip("Adjusts tip masking scale on Y-axis")]
        public float TipMaskYScale = 1;

        public override void InitialSet()
        {
            base.InitialSet();

            FiringScript.OnStoppedFiring.AddListener(destroy);
        }

        public override void Start()
        {
            base.Start();

            UniqueMaskId = UniqueMaskId * 100;

            originGo = initLaserEnds(new GameObject("Origin"), new Vector2(0, 0), OriginImg, sortOrder + 1, false);
            mainGo = initLaserMain(new GameObject("Main"), new Vector2(MainAdjust * globalDirection, 0), MainImg, sortOrder);
            tipGo = initLaserEnds(new GameObject("Tip"), new Vector2((MainAdjust + TipAdjust) * globalDirection, 0), TipImg, sortOrder + 2, true);

            originAnim = initAnimations(originGo, OriginImg);
            blastAnim = initAnimations(tipGo, BlastImg);
            tipAnim = initAnimations(tipGo, TipImg);           
        }

        protected override void animate()
        {
            base.animate();

            foreach (BasicAnimation anim in mainAnims)
                anim.Animate(FrameSkip);
        }

        protected override void movement()
        {
            float speed = scaledSpeed * ppu;

            mainGo.transform.localPosition = new Vector2(MainAdjust * globalDirection, mainGo.transform.localPosition.y);

            RaycastHit2D ray = boxCast();
            //Utilities.DebugDrawLine(gameObject.transform.position, ray.point); //visualizes raycast in scene view for debug purposes

            if (ray.collider == null)
                laserExtension(speed);
            else
                laserCollision(ray, speed);

            tipGo.transform.localPosition = new Vector2((mainMask.transform.localScale.x / ppu * globalDirection) + ((MainAdjust + TipAdjust) * globalDirection), 0);
            tipMask.transform.localPosition = new Vector2(Mathf.Abs(TipAdjust), 0);

            if (collided)
                sendLaserCollData(ray);
        }

        private void laserExtension(float speed)
        {
            float laserExtension = Mathf.MoveTowards(mainMask.transform.localScale.x, maxDistance * globalDirection, Time.deltaTime * speed);
            mainMask.transform.localScale = new Vector3(laserExtension, mainMask.transform.localScale.y, 1);

            tipAnim.Animate(FrameSkip);
        }

        private void laserCollision(RaycastHit2D ray, float speed)
        {
            //more accurate than ray.distance. Fixes discrepency between when fired on horizontal plane vs vertical plane when rotated
            float distance = Vector2.Distance(mainGo.transform.position, ray.point);
            float blastArea = BlastImg[0].bounds.size.x * ppu * CollisionTipZone;

            if (mainMask.transform.localScale.x >= (distance * ppu) - blastArea)
            {
                mainMask.transform.localScale = new Vector3((distance * ppu) - (blastArea), mainMask.transform.localScale.y, 1);
                blastAnim.Animate(FrameSkip);
                collided = true;
            }
            else
            {
                float laserExtension = Mathf.MoveTowards(mainMask.transform.localScale.x, (distance * ppu) - blastArea, Time.deltaTime * speed);
                mainMask.transform.localScale = new Vector3(laserExtension, mainMask.transform.localScale.y, 1);

                float bufferZone = 0.002f;

                if (laserExtension >= (distance * ppu) - blastArea - bufferZone)
                { blastAnim.Animate(FrameSkip); collided = true; }
                else
                    tipAnim.Animate(FrameSkip);
            }
        }

        private GameObject initLaserEnds(GameObject part, Vector3 initPosition, Sprite[] sprite, int sort, bool isTip)
        {
            part.transform.parent = this.transform;
            part.transform.localPosition = initPosition;
            part.transform.localScale = new Vector3(part.transform.localScale.x * globalDirection, part.transform.localScale.y);
            part.transform.rotation = new Quaternion();

            SpriteRenderer partSprite = part.AddComponent<SpriteRenderer>();
            partSprite.sprite = sprite[0];
            partSprite.sortingOrder = sort + UniqueMaskId;
            partSprite.sortingLayerName = sortLayer;

            partSprite.color = FiringScript.SpriteColor;

            if (isTip)
                tipMask = initMask(part, "TipMask", new Vector2(Mathf.Abs(TipAdjust), 0), new Vector2(BlastImg[0].bounds.size.x * ppu, TipMaskYScale), sortOrder + 3, sortOrder);
            else
                partSprite.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

            return part;
        }

        private GameObject initLaserMain(GameObject main, Vector3 initPosition, Sprite[] sprite, int sort)
        {
            main.transform.parent = this.transform;
            main.transform.localPosition = initPosition;
            main.transform.localScale = new Vector3(main.transform.localScale.x * globalDirection, main.transform.localScale.y);
            main.transform.rotation = new Quaternion();

            int chunksNeeded = (int)((CalcObject.getMaxScreenDistance() / MainImg[0].bounds.size.x) + AddedMaxDistance);
            
            mainAnims = new BasicAnimation[chunksNeeded];

            int startFrame = 0;

            for (int i = 0; i < chunksNeeded; i++)
            {
                GameObject chunk = new GameObject("Chunk" + i);
                chunk.transform.parent = main.transform;
                chunk.transform.localPosition = new Vector2(sprite[0].bounds.size.x * i, 0);
                chunk.transform.localRotation = new Quaternion();
                chunk.transform.localScale = new Vector2(1, 1);

                SpriteRenderer chunkSprite = chunk.AddComponent<SpriteRenderer>();
                chunkSprite.sprite = sprite[startFrame];
                chunkSprite.sortingOrder = sort + UniqueMaskId;
                chunkSprite.sortingLayerName = sortLayer;
                chunkSprite.color = FiringScript.SpriteColor;
                chunkSprite.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

                mainAnims[i] = new BasicAnimation(ref chunkSprite, ref MainImg, startFrame);

                startFrame++;
                if (!StaggerAnimations || startFrame > sprite.Length - 1)
                    startFrame = 0;
            }

            maxDistance = chunksNeeded * MainImg[0].bounds.size.x * ppu * globalDirection;
            mainMask = initMask(main, "MainMask", new Vector2(0, 0), new Vector2(1, MainMaskYScale), sortOrder, sortOrder - 1);

            return main;
        }

        private GameObject initMask(GameObject parentGo, string maskName, Vector2 pos, Vector2 scale, int frontSortOrder, int backSortOrder)
        {
            GameObject mask = new GameObject(maskName);
            mask.transform.parent = parentGo.transform;
            mask.transform.localPosition = pos;
            mask.transform.localScale = scale;
            mask.transform.localRotation = new Quaternion();

            SpriteMask spriteMask = mask.AddComponent<SpriteMask>();
            spriteMask.isCustomRangeActive = true;
            spriteMask.frontSortingLayerID = SortingLayer.NameToID(sortLayer);
            spriteMask.frontSortingOrder = frontSortOrder + UniqueMaskId;
            spriteMask.backSortingLayerID = SortingLayer.NameToID(sortLayer);
            spriteMask.backSortingOrder = backSortOrder + UniqueMaskId;
            spriteMask.sprite = MaskingSprite;

            return mask;
        }
    }
}