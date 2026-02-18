#region Script Synopsis
    //A simple utility script for creating walls/platforms in order to test collisions. Used to set up non-damageable collision objects in several demo projects.
#endregion

using UnityEngine;
using System.Collections.Generic;

namespace ND_VariaBULLET.Demo
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class TerrainBuilder : MonoBehaviour
    {
        public bool FlipX;
        public bool FlipY;
        public bool ReverseSort;

        public float OffsetX;
        public float OffsetY;

        public Sprite Sprite;
        public Orientation Type;
        public Color Color = new Color(1,1,1,1);

        [Range(1, 99)]
        public int Amount = 0;

        private List<GameObject> items = new List<GameObject>();

        void Start()
        {
            this.enabled = false;
        }

        void Update()
        {
            foreach (GameObject item in items)
                DestroyImmediate(item);

            for (int i = 1; i <= Amount; i++)
                createNewBlock(i);

            this.gameObject.layer = LayerMask.NameToLayer("Terrain");

            Utilities.Warn("Disable or Remove Script Once Terrain is Set to Avoid Duplication", this, transform.parent);
        }

        private void createNewBlock(int iterator)
        {
            var item = new GameObject(Sprite.name);
            item.transform.parent = this.transform;

            var rend = item.AddComponent<SpriteRenderer>();
            rend.sprite = Sprite;
            rend.color = Color;
            rend.flipX = FlipX;
            rend.flipY = FlipY;
            rend.sortingLayerName = "Terrain";
            rend.sortingOrder = (!ReverseSort) ? iterator : Amount - iterator;

            var block = new Vector2(Sprite.bounds.size.x - OffsetX, Sprite.bounds.size.y - OffsetY);
            var mult = new Vector2(0,0);

            var body = GetComponent<Rigidbody2D>();
            body.gravityScale = 0;
            body.constraints = RigidbodyConstraints2D.FreezeAll;

            var coll = GetComponent<BoxCollider2D>();

            if (Type == Orientation.Platform)
            {
                mult.x = block.x * iterator;
                coll.size = new Vector2(mult.x + OffsetX, block.y);
                coll.offset = new Vector2((block.x / 2 * Amount) + block.x / 2, 0);
            }
            else
            {
                mult.y = block.y * iterator;
                coll.size = new Vector2(block.x, mult.y + OffsetY);
                coll.offset = new Vector2(0, (block.y / 2 * Amount) + block.y / 2);
            }

            item.transform.localPosition = new Vector2(mult.x, mult.y);
            items.Add(item);
        }

        public enum Orientation
        {
            Platform,
            Wall
        }
    }
}