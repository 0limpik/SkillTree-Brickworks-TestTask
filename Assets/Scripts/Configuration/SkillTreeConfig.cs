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

        public IEnumerable<SkillNodeConfig> GetRoots() => Nodes
            .Where(x => x.Necessary.Count() == 0);

        public IEnumerable<SkillNodeConfig> GetNecessary(SkillConfig cfg)
        {
            var necessary = Nodes.First(x => x.Config == cfg).Necessary;
            return Nodes.Where(x => necessary.Any(y => y == x.Config));
        }

        public IEnumerable<SkillNodeConfig> GetAvailable(SkillConfig cfg) => Nodes
            .Where(x => x.Necessary.Contains(cfg));
    }
}
