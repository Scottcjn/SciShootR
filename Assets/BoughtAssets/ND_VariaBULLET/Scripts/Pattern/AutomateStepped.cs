#region Script Synopsis
    //Stepped automator, which automates controller parameters in a series of stepped intervals.
    //Becomes attached to a controller via the attached spreadpattern (controller) script.
    //Learn more about automators at: https://neondagger.com/variabullet2d-in-depth-controller-guide/#automators
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class AutomateStepped : AutomateBase
    {
        [Tooltip("Sets time interval for moving from one step to the next. [higher number = longer delay].")]
        public float Interval;

        [Tooltip("Sets values as a series of steps for the control being modified.")]
        public float[] Steps;

        [Tooltip("Takes precedence over the controller's TriggerAutoFire & FireCommand settings, instead syncing shot triggering to each step change.")]
        public bool AutoSyncTrigger;

        [Tooltip("Sets the amount of trigger increments before AutoSyncTrigger no longer triggers shots. [0 = infinite].")]
        public int TriggerPasses;
        private int triggersPassed = 1;

        private int index;
        private bool isFwd = true;

        protected override void Awake()
        {
            accumulator = Interval + 1;
            base.Awake();

            if (AutoSyncTrigger && pattern.TriggerAutoFire)
            {               
                Utilities.Warn("BasePattern.TriggerAutoFire conflicts with Stepped Automator.AutoSyncTrigger. Disabling controller's BasePattern.TriggerAutoFire setting. If you want to use TriggerAutoFire as the main trigger instead, turn off the Stepped Automator trigger", this.transform.parent.parent);
                pattern.TriggerAutoFire = false;
            }
        }

        void Update()
        {
            delay.RunOnce(InitialDelay);
            if (!delay.Flag) return;

            if (Steps.Length == 0) { Utilities.Warn("No steps have been set for ", this, transform.parent.parent);  return; }

            accumulator += Time.deltaTime * 4;
            controlLink[Destination]((method(0, Steps.Length - 1, Interval)));
        }

        protected override float SinglePass(float start, float end, float interval)
        {
            if (accumulator > interval)
            {
                if (index < end)
                    index++;

                accumulator = 0;
                triggerCheck();
            } 

            return Steps[index];
        }

        protected override float Continuous(float start, float end, float interval)
        {
            if (accumulator > interval)
            {
                if (index < end)
                    index++;
                else
                    index = (int)start;

                accumulator = 0;
                triggerCheck();
            }
         
            return Steps[index];
        }

        protected override float PingPong(float start, float end, float interval)
        {
            if (accumulator > interval)
            {
                if (isFwd)
                {
                    if (index < end)
                        index++;
                    else { index = (int)end; isFwd = false; }
                }
                else
                {
                    if (index > start)
                        index--;
                    else { index = (int)start; isFwd = true; }
                }

                accumulator = 0;
                triggerCheck();
            }

            return Steps[index];
        }

        protected override float Randomized(float start, float end, float interval)
        {
            if (accumulator > interval)
            {
                int rand = index;

                if (Steps.Length > 1)
                    while (index == rand)
                        rand = Random.Range((int)start, (int)end + 1);

                index = rand;
                accumulator = 0;
                triggerCheck();
            }

            return Steps[index];
        }

        private void triggerCheck()
        {
            if (AutoSyncTrigger)
            {
                pattern.FireCommand = BasePattern.CommandType.AutomaticAutoHold;
                pattern.TriggerAutoFire = true;

                if (TriggerPasses == 0)
                    { pattern.TriggerAutoFire = true; return; }
                else if (triggersPassed > TriggerPasses)
                    { pattern.TriggerAutoFire = false; return; }

                triggersPassed++;
            }
        }
    }
}