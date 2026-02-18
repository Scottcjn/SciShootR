#region Script Synopsis
    //Extended class for a SpreadPattern Controller script, mainly involved in setting up controller emitter positions
    //Attached to the Controller gameobject to form the core behavior for the controller/emitters.
    //Learn more about controllers/emitters at: http://neondagger.com/variabullet2d-quick-start-guide/#setting-emitters-controls
#endregion

using UnityEngine;
using System;

namespace ND_VariaBULLET
{
    [ExecuteInEditMode]
    public class SpreadPattern : BasePattern
    {
        [Range(-80, 80)]
        [Tooltip("Shifts emitters on Y-axis.")]
        public float SpreadYAxis;

        [Range(-80, 80)]
        [Tooltip("Shifts emitters on X-axis.")]
        public float SpreadXAxis;

        [Tooltip("Sets base algorithm for creating emitter placement.")]
        public PatternSelect patternSelect = PatternSelect.Radial;

        [SerializeField]
        private PresetName Preset;

        public override void LateUpdate()
        {
            if (!FreezeEdits)
            {
                presetCheck();
                base.LateUpdate();

                autoSetRadius();
                setAllPositions();
            }

            masterTriggerCheck();
        }

        private void masterTriggerCheck()
        {
            if (!Master || Utilities.IsEditorMode()) return;

            if (childControllers.Length > 1)
            {
                foreach (var child in childControllers)
                    child.TriggerAutoFire = TriggerAutoFire;
            }
        }

        private void setPositionsStack()
        {
            int mod = 0;

            if (Emitters.Count % 2 == 0)
                mod = Emitters.Count / 2;
            else
                mod = Emitters.Count / 2 + 1;

            int modAngle = mod;

            for (int i = 0; i < Emitters.Count; i++)
            {
                if (Emitters.Count % 2 == 0)
                {
                    if (i < Emitters.Count / 2)
                        { modAngle = (Math.Abs(modAngle) - 1) * -1; mod--; }
                    else if (i > Emitters.Count / 2)
                        { modAngle = modAngle + 1; mod++; }
                }
                else
                {
                    if (i <= Emitters.Count / 2)
                        { modAngle = (Math.Abs(modAngle) - 1) * -1; mod--; }
                    else
                        { modAngle = modAngle + 1; mod++; }
                }


                float xOffset = SpreadXAxis * mod;

                Emitters[i].transform.localPosition = new Vector2(xOffset, SpreadYAxis * i);   
                Emitters[i].transform.localRotation = new Quaternion(); //fixes issue of creating rotation issues on play
                Emitters[i].transform.localRotation = Quaternion.Euler(0, 0, SpreadDegrees * modAngle);
                Emitters[i].transform.localScale = new Vector3(1, 1, 1); //for maintaining ratio if using nested emitters/points

                foreach (Transform child in Emitters[i].transform)
                {
                    float tilt = 0;

                    if (i < Emitters.Count / 2 || Emitters.Count == 1 || UniDirectionPitch)
                        tilt = Pitch + FireScripts[i].LocalPitch;
                    else if (i >= (float)Emitters.Count / 2)
                        tilt = (Pitch + FireScripts[i].LocalPitch) * -1;

                    child.localPosition = new Vector2(SpreadRadius, 0);
                    child.localRotation = Quaternion.Euler(0, 0, tilt);
                    child.localScale = new Vector3(PointScale, PointScale, 1);
                }
            }

            float spreadRadiusAdjusted = (AutoCompRadius) ? 0 - SpreadRadius : 0;
            transform.localPosition = new Vector2(spreadRadiusAdjusted, 0);
        }

        private void setPositionsRadial()
        {
            int mod = 0;
            for (int i = 0; i < Emitters.Count; i++)
            {
                mod++;

                Emitters[i].transform.localPosition = new Vector2(SpreadXAxis, SpreadYAxis);
                Emitters[i].transform.localRotation = Quaternion.Euler(0, 0, SpreadDegrees * mod);      
                Emitters[i].transform.localScale = new Vector3(1, 1, 1);

                foreach (Transform child in Emitters[i].transform)
                {               
                    float tilt = 0;

                    if (i < Emitters.Count / 2 || Emitters.Count == 1 || UniDirectionPitch)
                        tilt = Pitch + FireScripts[i].LocalPitch;
                    else if (i >= (float)Emitters.Count / 2)
                        tilt = (Pitch + FireScripts[i].LocalPitch) * -1;

                    child.localPosition = new Vector2(SpreadRadius, 0);
                    child.localRotation = Quaternion.Euler(0, 0, tilt);
                    child.localScale = new Vector3(PointScale, PointScale, 1);
                }
            }

            float spreadRadiusAdjusted = (AutoCompRadius) ? 0 - SpreadRadius : 0;
            transform.localPosition = new Vector2(spreadRadiusAdjusted, 0);
        }

        private void autoCenterStack()
        {
            if (autoCenter)
            {
                float emittersYSum = 0;

                foreach (GameObject emitter in Emitters)
                    emittersYSum += emitter.transform.localPosition.y;

                foreach (GameObject emitter in Emitters)
                    emitter.transform.localPosition = new Vector2(emitter.transform.localPosition.x, emitter.transform.localPosition.y + emittersYSum / (float)EmitterAmount * -1);
            }

            transform.localRotation = Quaternion.Euler(0, 0, CenterRotation);
        }

        private void autoCenterRadial()
        {
            float cancelFix = 0.001f; //required for when a cancellation (zero) occurs between spread rotation and child rotation

            if (autoCenter)
            {
                float adjustment;

                if (Emitters.Count % 2 != 0)
                    adjustment = (float)Emitters.Count / 2 + 0.5f;
                else
                    adjustment = Emitters.Count / 2 + 0.5f;

                float rotation = (360 - (float)SpreadDegrees * adjustment) + cancelFix;
                transform.localRotation = Quaternion.Euler(0, 0, rotation);
                CenterRotation = rotation;
            }
            else
                transform.localRotation = Quaternion.Euler(0, 0, CenterRotation + cancelFix);
        }

        private void setParentRotation()
        {
            if (transform.parent.parent != null && parentPoint == null)
                transform.parent.parent.localRotation = Quaternion.Euler(0, 0, ParentRotation);
            else
            {
                if (ParentRotation != 0)
                    Utilities.Warn("Can't rotate parent rotation. It's controlled by a different controller or null.", this, transform.parent.parent);
            }
        }

        private void setAllPositions()
        {
            if (Emitters != null)
            {
                if (patternSelect == PatternSelect.Stack)
                {
                    setPositionsStack();
                    autoCenterStack();
                }
                else
                {
                    setPositionsRadial();
                    autoCenterRadial();
                }

                setParentRotation();
            }
        }

        private void presetCheck()
        {     
            if (Preset != PresetName.none) //load from preset if new state requested
            {
                BasicPresetState.ApplyPreset(this, Preset);
                Preset = PresetName.none;
            }
        }

        private void autoSetRadius()
        {
            if (AutoRadiusToSprite)
            {
                AutoRadiusToSprite = false;
                Sprite weapon = transform.parent.parent.GetComponent<SpriteRenderer>().sprite;

                if (weapon == null) { Utilities.Warn("Cant set radius to sprite as there is no parent sprite.", this, transform.parent.parent); return; }                  

                if (weapon.bounds.size.x > weapon.bounds.size.y)
                    SpreadRadius = weapon.bounds.size.x / 2;
                else
                    SpreadRadius = weapon.bounds.size.y / 2;            
            }
        }
    }
}