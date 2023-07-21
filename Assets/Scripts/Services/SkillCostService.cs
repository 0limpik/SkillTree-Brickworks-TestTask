using System.Collections.Generic;
using Assets.Scripts.Configuration;

namespace Assets.Scripts.Services
{
    internal class SkillCostService
    {
        private readonly Dictionary<SkillConfig, SkillNodeConfig> nodes = new();

        public void AddTree(SkillTreeConfig treeConfig)
        {
            foreach (var node in treeConfig.Nodes)
            {
                nodes.Add(node.Config, node);
            }
        }

        public SkillNodeConfig GetTreeConfig(SkillConfig config) => nodes[config];
    }
}
