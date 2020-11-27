using Verse;
using System;
using RimWorld;
using System.Collections.Generic;

namespace MoharGamez
{
    //[StaticConstructorOnStartup]

    public class TargetingGameSetting : Thing
    {
        public GameSettingsDef Def => def as GameSettingsDef;
    }

    public class GameSettingsDef : ThingDef
    {
        public List<GameSettings> gameSettingsList;
    }
    
    public class GameSettings
    {
        public Type driverClass;
        public JobDef jobDef;

        public SkillDef skillDefScaling;
        public int throwInterval;
        public float playedTogetherRatio;

        public List<ProjectileOption> projectileOptionList;

        public GameThoughts thoughtList;

        public BubbleInteraction bubbleInteraction;

        public bool debug = false;
        public bool debugThrowThought = false;
        public bool debugTogetherThought = false;

        public bool HasThought => thoughtList != null;
        public bool HasThrowThought => HasThought && !thoughtList.thoughtOptionPerShot.NullOrEmpty();
        public bool HasPlayingTogetherThought => HasThought && !thoughtList.thoughtOptionPlayingTogether.NullOrEmpty();

        public bool HasBubbleInteraction => bubbleInteraction != null;

        public bool HasDestroyingMotes => HasBubbleInteraction && bubbleInteraction.HasDestroyingMotes;
        public bool HasResistantMotes => HasBubbleInteraction && bubbleInteraction.HasResistantMotes;
    }
    public class BubbleInteraction
    {
        public List<ThingDef> destroyingMotes;
        public List<ThingDef> resistantMotes;

        public bool HasDestroyingMotes => !destroyingMotes.NullOrEmpty();
        public bool HasResistantMotes => !resistantMotes.NullOrEmpty();

        //public bool HasAtLeastOnItem => HasDestroyingMotes || HasResistantMotes;
    }
}
