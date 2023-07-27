using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Configuration
{
    [CreateAssetMenu(menuName = "TestTask/SkillTreeCfg")]
    public class SkillTreeConfig : ScriptableObject
    {
        [SerializeField] private SkillNodeConfig[] nodes;

        public IEnumerable<SkillNodeConfig> Nodes => nodes;

        public IEnumerable<SkillConfig> Configs => nodes
            .Select(x => x.Config);

        public SkillNodeConfig GetNodeConfig(SkillConfig config) => nodes
            .FirstOrDefault(x => x.Config == config);

        public IEnumerable<SkillNodeConfig> GetRoots() => nodes
            .Where(x => x.Necessary.Count() == 0);

        public IEnumerable<SkillNodeConfig> GetLeaves() => nodes
            .Where(x => GetAvailable(x.Config).Count() == 0);

        public IEnumerable<SkillNodeConfig> GetNecessary(SkillConfig config)
        {
            var necessary = GetNodeConfig(config);

            if (necessary == null)
            {
                return Array.Empty<SkillNodeConfig>();
            }

            return nodes.Where(x => necessary.Necessary.Any(y => y == x.Config));
        }

        public IEnumerable<SkillNodeConfig> GetAvailable(SkillConfig config) => nodes
            .Where(x => x.Necessary.Contains(config));

        public bool Contains(SkillConfig config) => nodes
            .Any(x => x.Config == config);
    }
}
