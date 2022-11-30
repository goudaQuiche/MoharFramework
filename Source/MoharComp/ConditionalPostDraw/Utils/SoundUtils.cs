using Verse;
using Verse.Sound;

namespace ConPoDra
{
    public static class SoundUtils
    {
        public static void StopSound(this CompConditionalPostDraw comp)
        {
            if (comp.parent.Negligeable())
                return;

            if (comp.TriggersSoundActivityOnStop && comp.CurMaterialTracer.Displayed)
            {
                if (comp.CurPostDrawTask.soundMaterialPool.HasStopSound)
                    comp.CurPostDrawTask.soundMaterialPool.soundOnStop.PlayOneShot(new TargetInfo(comp.parent.Position, comp.parent.Map));

                if (comp.CurPostDrawTask.soundMaterialPool.HasSustainSound)
                {
                    if (comp.CurMaterialTracer.sustainer != null)
                    {
                        comp.CurMaterialTracer.sustainer.End();
                        comp.CurMaterialTracer.sustainer = null;
                    }
                }
            }
        }

        public static void StartSound(this CompConditionalPostDraw comp)
        {
            if (comp.parent.Negligeable())
                return;

            if (comp.TriggersSoundActivityOnStart && !comp.CurMaterialTracer.Displayed)
            {
                if (comp.CurPostDrawTask.soundMaterialPool.HasStartSound)
                    comp.CurPostDrawTask.soundMaterialPool.soundOnStart.PlayOneShot(new TargetInfo(comp.parent.Position, comp.parent.Map));

                if (comp.CurPostDrawTask.soundMaterialPool.HasSustainSound)
                {
                    comp.CurMaterialTracer.sustainer = comp.CurPostDrawTask.soundMaterialPool.soundSustain.TrySpawnSustainer(new TargetInfo(comp.parent.Position, comp.parent.Map));
                }
            }
        }
    }
}
