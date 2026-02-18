#region Script Synopsis
    //Controls the fixed angles used by the weapon in the "RunNGun" demo project.
#endregion

using UnityEngine;

namespace ND_VariaBULLET.Demo
{
    public class CantroGunAnim : MonoBehaviour
    {
        public SpreadPattern Controller;
        public Sprite[] Frames = new Sprite[3];
        private SpriteRenderer rend;
        
        void Start()
        {
            rend = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                rend.sprite = Frames[1];
                Controller.Pitch = 45;
                Controller.SpreadYAxis = 0.8f;
            }              
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                rend.sprite = Frames[2];
                Controller.Pitch = -45;
                Controller.SpreadYAxis = -1;
            }                
            else
            {
                rend.sprite = Frames[0];
                Controller.Pitch = 0;
                Controller.SpreadYAxis = 0;
            }
                
        }
    }
}