#region Script Synopsis
    //A demonstration custom shot prefab that converts to coins upon its emitter's destruction. Used in "BulletsToCoins" demo project.
#endregion

using UnityEngine;

namespace ND_VariaBULLET.Demo
{
    public class CoinOnDestroyed : ShotLinearNonPhysics
    {
        [Header("Coin Object")]
        public GameObject Coin;

        public override void Update()
        {
            base.Update();

            OnEmitterDestroyedDoOnce(
                shot => {
                    GameObject coin = GameObject.Instantiate(Coin);
                    coin.transform.position = shot.transform.position;
                    coin.transform.rotation = shot.transform.rotation;

                    RePoolOrDestroy();
                }
            );
        }
    }
}