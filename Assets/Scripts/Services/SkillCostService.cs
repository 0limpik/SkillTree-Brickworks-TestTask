using System.Collections.Generic;
using TestTask.Configuration;

namespace TestTask.Services
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

        public void RemoveTree(SkillTreeConfig treeConfig)
        {
            foreach (var node in treeConfig.Nodes)
            {
                nodes.Remove(node.Config);
            }
        }

        public SkillNodeConfig GetTreeConfig(SkillConfig config) => nodes[config];
    }
}
