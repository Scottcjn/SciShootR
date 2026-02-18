#region Script Synopsis
    //Simple script for determining if a button sprite has been clicked to then restart the scene
#endregion

using UnityEngine;
using UnityEngine.SceneManagement;

namespace ND_VariaBULLET.Demo
{
    public class RestartButton : MonoBehaviour
    {
        private Vector2 center;
        private Vector2 edge;
        private Vector2 mouseInput;

        void Start()
        {
            edge = GetComponent<SpriteRenderer>().bounds.size / 2;
        }

        void Update()
        {
            if (!Input.GetMouseButtonDown(0))
                return;

            mouseInput = GlobalShotManager.Instance.MainCam.ScreenToWorldPoint(Input.mousePosition);
            center = transform.position;

            bool buttonClicked = (mouseInput.x >= center.x - edge.x && mouseInput.x <= center.x + edge.x && mouseInput.y >= center.y - edge.y && mouseInput.y <= center.y + edge.y);

            if (!buttonClicked)
                return;

            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
    }
}
