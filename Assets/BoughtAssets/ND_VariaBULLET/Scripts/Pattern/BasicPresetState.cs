#region Script Synopsis
    //Simple state-based object that holds spreadpattern (controller) parameters and returns a new preset upon request
    //Example: SpreadPattern.presetCheck()
    //Learn more about presets at: https://neondagger.com/variabullet2d-in-depth-controller-guide/#presets
#endregion

namespace ND_VariaBULLET
{
    public class BasicPresetState
    {
        public int emitterAmount;
        public float spreadDegrees;
        public float pitch;
        public bool uniDirectionPitch;
        public float spreadRadius;
        public bool autoCompRadius;
        public float centerRotation;
        public bool autoCenter;
        public float spreadYAxis;
        public float spreadXAxis;
        public BasePattern.PatternSelect patternSelect;
        public float exitPointOffset;
        public float parentRotation;

        private BasicPresetState(int emitterAmount, float spreadDegrees, float pitch, bool uniDirectionPitch, float spreadRadius, bool autoCompRadius, float centerRotation, bool autoCenter, float spreadYAxis, float spreadXAxis, BasePattern.PatternSelect patternSelect, float exitPointOffset, float parentRotation)
        {
            this.emitterAmount = emitterAmount;
            this.spreadDegrees = spreadDegrees;
            this.pitch = pitch;
            this.uniDirectionPitch = uniDirectionPitch;
            this.spreadRadius = spreadRadius;
            this.autoCompRadius = autoCompRadius;
            this.centerRotation = centerRotation;
            this.autoCenter = autoCenter;
            this.spreadYAxis = spreadYAxis;
            this.spreadXAxis = spreadXAxis;
            this.patternSelect = patternSelect;
            this.exitPointOffset = exitPointOffset;
            this.parentRotation = parentRotation;
        }

        private static BasicPresetState RequestNewDefault(PresetName selection)
        {
            switch (selection)
            {
                case PresetName.reset:
                    return new BasicPresetState(1, 0, 0, false, 0, false, 0, true, 0, 0, BasePattern.PatternSelect.Radial, 0, 0);

                case PresetName.reAngle:
                    return new BasicPresetState(6, 64, 98, false, 3.5f, true, 0, true, -0.55f, 1.25f, BasePattern.PatternSelect.Stack, 6, 0);

                case PresetName.threeWay:
                    return new BasicPresetState(3, 13, -31, false, 1, true, 0, true, 0, 0, BasePattern.PatternSelect.Radial, 6, 0);

                case PresetName.verticalize:
                    return new BasicPresetState(1, 0, 0, false, 0, true, 0, true, 0, 0, BasePattern.PatternSelect.Radial, 0, 90);

                case PresetName.hellRadial:
                    return new BasicPresetState(9, 40, 0, false, 3.2f, false, 0, true, 0, 0, BasePattern.PatternSelect.Radial, 6, 0);

                case PresetName.tripleTriple:
                    return new BasicPresetState(9, 128, 0, false, 3.2f, false, 0, true, 0, 0, BasePattern.PatternSelect.Radial, 6, 0);

                case PresetName.vFormation:
                    return new BasicPresetState(5, 0, 0, false, 2.6f, true, 0, true, 0.7f, -1, BasePattern.PatternSelect.Stack, 6, 0);

                case PresetName.frontNBack:
                    return new BasicPresetState(3, 0, -170, false, 2.6f, true, 0, true, 1.6f, -1, BasePattern.PatternSelect.Stack, 6, 0);

                case PresetName.multiBomber:
                    return new BasicPresetState(3, 0, -54, false, 3.2f, false, -90, true, 1.3f, 0, BasePattern.PatternSelect.Stack, 0, 0);

                case PresetName.randomSpread:
                    return new BasicPresetState(15, 203, -40, false, 4.4f, false, 0, true, 0.3f, 0, BasePattern.PatternSelect.Stack, 4, 0);

                case PresetName.pentaWall:
                    return new BasicPresetState(10, 140, -100, false, 5, false, 0, true, 0, 0, BasePattern.PatternSelect.Radial, 4, 0);
                default:
                    return null;
            }
        }

        public static void ApplyPreset(SpreadPattern pattern, PresetName selection)
        {
            BasicPresetState preset = RequestNewDefault(selection);

            if (preset != null)
            {
                pattern.EmitterAmount = preset.emitterAmount;
                pattern.SpreadDegrees = preset.spreadDegrees;
                pattern.Pitch = preset.pitch;
                pattern.UniDirectionPitch = preset.uniDirectionPitch;
                pattern.SpreadRadius = preset.spreadRadius;
                pattern.AutoCompRadius = preset.autoCompRadius;
                pattern.CenterRotation = preset.centerRotation;
                pattern.autoCenter = preset.autoCenter;
                pattern.SpreadYAxis = preset.spreadYAxis;
                pattern.SpreadXAxis = preset.spreadXAxis;
                pattern.patternSelect = preset.patternSelect;
                pattern.ExitPointOffset = preset.exitPointOffset;
                pattern.ParentRotation = preset.parentRotation;
            }
        }
    }

    public enum PresetName
    {
        none,
        reset,
        reAngle,
        threeWay,
        verticalize,
        hellRadial,
        tripleTriple,
        vFormation,
        frontNBack,
        multiBomber,
        randomSpread,
        pentaWall
    }
}