using System.Collections.Generic;
using System.Linq;
using TestTask.Configuration;

namespace TestTask.Model
{
    public class SkillTree
    {
        public readonly IEnumerable<ISkillNode> Nodes;

        public SkillTree(IEnumerable<ISkillNode> nodes)
        {
            Nodes = nodes;
        }

        public IEnumerable<SkillConfig> CanAddTree(SkillTreeConfig treeConfig)
        {
            foreach (var config in treeConfig.Nodes
                .SelectMany(x => x.Necessary)
                .Distinct())
            {
                if (treeConfig.Contains(config))
                {
                    continue;
                }

                if (Contains(config))
                {
                    continue;
                }

                yield return config;
            }
        }

        public IEnumerable<(SkillConfig config, IEnumerable<SkillConfig> dependents)> CanRemoveTree(SkillTreeConfig treeConfig)
        {
            foreach (var node in treeConfig.Nodes
                .SelectMany(x => GetNode(x.Config).Available)
                .Distinct())
            {
                if (treeConfig.Contains(node.Config))
                {
                    continue;
                }

                yield return (node.Config, node.NecessaryConfigs.Intersect(treeConfig.Configs));
            }
        }

        public bool Contains(SkillConfig config) => Nodes
            .Any(x => x.Config == config);

        public ISkillNode GetNode(SkillConfig config) => Nodes
            .FirstOrDefault(x => x.Config == config);

        public IEnumerable<ISkillNode> GetRoots() => Nodes
            .Where(x => x.Necessary.Count() == 0);

        public IEnumerable<ISkillNode> GetLeaves() => Nodes
            .Where(x => x.Available.Count() == 0);
    }
}
