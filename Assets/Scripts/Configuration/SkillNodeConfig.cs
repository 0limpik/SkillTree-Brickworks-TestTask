using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Configuration
{
    [Serializable]
    public class SkillNodeConfig
    {
        [field: SerializeField] public SkillConfig Config { get; private set; }
        [field: SerializeField] public int Cost { get; private set; }
        [SerializeField] private SkillConfig[] necessaryToLearn;

        public IEnumerable<SkillConfig> Necessary => necessaryToLearn;

        public override string ToString()
            => $"{Config.Name} C:{Cost} N:{necessaryToLearn.Length}";
    }
}
