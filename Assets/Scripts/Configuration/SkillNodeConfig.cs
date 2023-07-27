using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Configuration
{
    [Serializable]
    public class SkillNodeConfig
    {
        [SerializeField] private SkillConfig    config;
        [SerializeField] private int            cost;
        [SerializeField] private SkillConfig[]  necessaryToLearn;

        public SkillConfig Config => config;
        public int Cost => cost;
        public IEnumerable<SkillConfig> Necessary => necessaryToLearn;

        public override string ToString()
            => $"{Config.Name} C:{Cost} N:{necessaryToLearn.Length}";
    }
}
