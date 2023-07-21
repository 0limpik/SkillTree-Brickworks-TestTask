using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Configuration;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public class SkillTree
    {
        public IEnumerable<ISkillNode> Nodes => nodes.Values;

        private readonly Dictionary<SkillNodeConfig, SkillNode> nodes = new();

        public void Fill(SkillTreeConfig treeConfig)
        {
            nodes.Clear();

            foreach (var root in treeConfig.GetRoots())
            {
                nodes.Add(root, new SkillNode(root.Config));
            }

            var needNextPass = true;
            for (int i = 0; needNextPass; i++)
            {
                //Debug.Log($"Pass {i}\n{string.Join("\n", nodes)}");

                needNextPass = PassTree(treeConfig);
            }

            if (nodes.Keys.Count() != treeConfig.Nodes.Count())
            {
                foreach (var node in treeConfig.Nodes.Where(x => !nodes.Keys.Contains(x)))
                {
                    Debug.LogError($"{node.Config.Name} can't be added", node.Config);
                }
            }
        }

        public ISkillNode GetNode(SkillConfig config) => nodes.Values
            .First(x => x.Config == config);

        public IEnumerable<ISkillNode> GetRoots() => nodes
            .Where(x => x.Value.nethessary.Count() == 0)
            .Select(x => x.Value);

        public IEnumerable<ISkillNode> GetLeaves() => nodes
            .Where(x => x.Value.available.Count() == 0)
            .Select(x => x.Value);

        private bool PassTree(SkillTreeConfig treeConfig)
        {
            var newNodes = new Dictionary<SkillNodeConfig, SkillNode>();
            foreach (var (config, _) in nodes)
            {
                var available = treeConfig.GetAvailable(config.Config);
                PassNecessary(available, treeConfig, newNodes);
            }

            foreach (var (config, node) in newNodes)
            {
                nodes.Add(config, node);
            }

            foreach (var (config, node) in nodes)
            {
                var available = treeConfig.GetAvailable(config.Config);
                PassAvailable(available, node);
            }

            return newNodes.Count > 0;
        }

        private void PassNecessary(IEnumerable<SkillNodeConfig> available, SkillTreeConfig treeConfig, Dictionary<SkillNodeConfig, SkillNode> newNodes)
        {
            foreach (var availableCfg in available)
            {
                if (nodes.ContainsKey(availableCfg) || newNodes.ContainsKey(availableCfg))
                {
                    continue;
                }

                var necessary = treeConfig.GetNecessary(availableCfg.Config).ToArray();
                if (necessary.Any(x => !nodes.ContainsKey(x)))
                {
                    continue;
                }

                var node = new SkillNode(availableCfg.Config);
                node.nethessary.AddRange(necessary.Select(x => nodes[x]));
                newNodes.Add(availableCfg, node);
            }
        }

        private void PassAvailable(IEnumerable<SkillNodeConfig> available, SkillNode node)
        {
            if (node.available.Count() == available.Count())
            {
                return;
            }

            foreach (var availableCfg in available)
            {
                if (!nodes.TryGetValue(availableCfg, out var avalibleSkill))
                {
                    continue;
                }

                if (!node.available.Contains(avalibleSkill))
                {
                    node.available.Add(avalibleSkill);
                }
            }
        }
    }
}
