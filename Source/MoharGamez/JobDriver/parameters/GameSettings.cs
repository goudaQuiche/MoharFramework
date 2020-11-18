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
        public IntRange throwInterval;
        public List<ProjectileOption> projectileOptionList;

        public GameThoughts thoughtList;

        public bool debug = false;

        public bool HasThought => thoughtList != null && (!thoughtList.thoughtOptionPerShot.NullOrEmpty() || !thoughtList.thoughtOptionPlayingTogether.NullOrEmpty());
    }
}
