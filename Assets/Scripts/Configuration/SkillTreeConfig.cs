using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Configuration
{
    [CreateAssetMenu(menuName = "TestTask/SkillTreeCfg")]
    public class SkillTreeConfig : ScriptableObject
    {
        [field: SerializeField] public SkillNodeConfig[] Nodes { get; private set; }

        public IEnumerable<SkillConfig> Configs => Nodes.Select(x => x.Config);

        public SkillNodeConfig GetNodeConfig(SkillConfig config) => Nodes
            .FirstOrDefault(x => x.Config == config);

        public IEnumerable<SkillNodeConfig> GetRoots() => Nodes
            .Where(x => x.Necessary.Count() == 0);
        
        public IEnumerable<SkillNodeConfig> GetLeaves() => Nodes
            .Where(x => GetAvailable(x.Config).Count() == 0);

        public IEnumerable<SkillNodeConfig> GetNecessary(SkillConfig config)
        {
            var necessary = GetNodeConfig(config);

            if(necessary == null)
            {
                return Array.Empty<SkillNodeConfig>();
            }

            return Nodes.Where(x => necessary.Necessary.Any(y => y == x.Config));
        }

        public IEnumerable<SkillNodeConfig> GetAvailable(SkillConfig config) => Nodes
            .Where(x => x.Necessary.Contains(config));

        public bool Contains(SkillConfig config) => Nodes.Any(x => x.Config == config);
    }
}
