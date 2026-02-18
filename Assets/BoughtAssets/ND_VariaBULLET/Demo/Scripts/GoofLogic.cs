#region Script Synopsis
    //Runs game logic for Super Goof Bros. OnLaserCollision detects laser collision and converts impact to an angular force
#endregion

using System.Collections;
using UnityEngine;

namespace ND_VariaBULLET.Demo
{
    public class GoofLogic : MonoBehaviour
    {
        public GameObject Opponent;
        public GameObject endScreen;

        private BasePattern controller;
        private AutomateBase automator;

        private Rigidbody2D body;
        private Vector2 startPos;
        private AudioSource scream;

        private string[] fullName;
        private bool winner;

        void Start()
        {
            fullName = name.Split('_');
            body = GetComponent<Rigidbody2D>();
            scream = GetComponent<AudioSource>();

            startPos = transform.position;
            controller = GetComponentInChildren<BasePattern>();
            automator = GetComponentInChildren<AutomateBase>();

            int delay = Random.Range(60, 120);
            StartCoroutine(
                CoroutineExt.WaitForFramesDo(delay, () => { controller.TriggerAutoFire = true; automator.enabled = true; })
            );

            endScreen.SetActive(false);
        }

        public IEnumerator OnLaserCollision(CollisionArgs sender)
        {
            float force = 75 * sender.damage;

            Vector2 forceAngle = CalcObject.AngleToAddForce(sender.point, gameObject.transform.position, force
            );

            body.AddForce(forceAngle);

            if (!scream.isPlaying)
            {
                float maxPan = 33;
                scream.panStereo = transform.position.x / maxPan;
                scream.Play();
            }
            yield return null;
        }

        void Update()
        {

            killCheck();
            winCheck();   
        }

        private void killCheck()
        {
            if (transform.position.y < -28)
                Destroy(gameObject);
        }

        private void winCheck()
        {
            if (winner == false && Opponent == null)
            {
                controller.TriggerAutoFire = false;
                automator.enabled = false;
                winner = true;
            }

            if (winner)
                rollEndScreen();
        }

        private void rollEndScreen()
        {
            Vector3 camPos = Vector3.MoveTowards(GlobalShotManager.Instance.MainCam.transform.position, this.transform.position, Timer.deltaCounter / 20);
            GlobalShotManager.Instance.MainCam.transform.position = new Vector3(camPos.x, camPos.y, GlobalShotManager.Instance.MainCam.transform.position.z);
            endScreen.SetActive(true);
        }


        //Draw winner name
        public GUIStyle FontStyle = new GUIStyle();
        private int baseFontSize = 35;

        private int scaledFont
        {
            get { return (Screen.width + Screen.height) / baseFontSize; }
        }

        void OnGUI()
        {           
            if (winner)
                drawOSD();
        }

        void drawOSD()
        {
            FontStyle.fontSize = scaledFont;

            float offset = (startPos.x > 0) ? -1 : 1;
            GUIStyle altText = new GUIStyle(FontStyle);

            altText.normal.textColor = Color.yellow;

            GUI.Label(
                new Rect(Screen.width / 20 * offset, Screen.height / 13, Screen.width, Screen.height),
                string.Format("A Winner is... "), FontStyle
            );

            GUI.Label(
                new Rect(Screen.width / 20 * offset, Screen.height / 6, Screen.width, Screen.height),
                string.Format(fullName[0].ToUpper() + " " +fullName[1].ToUpper() + "!!!"), altText
            );
        }
    }
}
