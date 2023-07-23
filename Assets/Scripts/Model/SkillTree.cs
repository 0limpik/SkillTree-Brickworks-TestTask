using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Configuration;
using Unity.VisualScripting;

namespace Assets.Scripts.Model
{
    public class SkillTree
    {
        public IEnumerable<ISkillNode> Nodes => nodes.Values;

        private readonly Dictionary<SkillNodeConfig, SkillNode> nodes = new();

        public IEnumerable<SkillConfig> CanAddTree(SkillTreeConfig treeConfig)
        {
            foreach (var nethessary in treeConfig.Nodes
                .SelectMany(x => x.Necessary)
                .Except(treeConfig.Nodes.Select(x => x.Config))
                .Distinct())
            {
                if (!nodes.Keys.Any(x => x.Config == nethessary))
                {
                    yield return nethessary;
                }
            }
        }

        public void AddTree(SkillTreeConfig treeConfig)
        {
            if (CanAddTree(treeConfig).Any())
            {
                throw new InvalidOperationException();
            }

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
                    throw new InvalidOperationException($"{node} can't be added");
                }
            }
        }

        public IEnumerable<(SkillConfig config, IEnumerable<SkillConfig> dependents)> CanRemoveTree(SkillTreeConfig treeConfig)
        {
            foreach (var avalible in treeConfig.Nodes
                 .SelectMany(x => nodes[x].Available)
                 .Except(treeConfig.Nodes.Select(x => nodes[x]))
                 .Distinct())
            {
                if (nodes.Keys.Any(x => x.Config == avalible.Config))
                {
                    yield return (avalible.Config,
                        avalible.Necessary.Select(x => x.Config).Intersect(treeConfig.Nodes.Select(x => x.Config)));
                }
            }
        }

        public IEnumerable<ISkillNode> RemoveTree(SkillTreeConfig treeConfig)
        {
            if (CanRemoveTree(treeConfig).Any())
            {
                throw new InvalidOperationException();
            }

            var nodesCount = nodes.Count() - treeConfig.Nodes.Count();
            nodesCount = Math.Max(nodesCount, 0);

            while (nodesCount < nodes.Count())
            {
                var leaves = GetLeaves()
                    .Where(x => treeConfig.Contains(x.Config))
                    .ToArray();

                if (!leaves.Any())
                {
                    throw new InvalidOperationException();
                }

                foreach (var leaf in leaves)
                {
                    var leafNodeConfig = treeConfig.GetNodeConfig(leaf.Config);
                    Remove(leafNodeConfig);
                    yield return leaf;
                }
            }
        }

        public ISkillNode GetNode(SkillConfig config) => nodes.Values
            .FirstOrDefault(x => x.Config == config);

        public IEnumerable<ISkillNode> GetRoots() => nodes
            .Where(x => x.Value.nethessary.Count() == 0)
            .Select(x => x.Value);

        public IEnumerable<ISkillNode> GetLeaves() => nodes
            .Where(x => x.Value.available.Count() == 0)
            .Select(x => x.Value);

        private bool PassTree(SkillTreeConfig treeConfig)
        {
            var newNodes = new Dictionary<SkillNodeConfig, SkillNode>();
            foreach (var (nodeConfig, _) in nodes)
            {
                var available = treeConfig.GetAvailable(nodeConfig.Config);
                PassNecessary(available, treeConfig, newNodes);
            }

            foreach (var (nodeConfig, node) in newNodes)
            {
                nodes.Add(nodeConfig, node);
            }

            foreach (var (nodeConfig, node) in nodes)
            {
                var available = treeConfig.GetAvailable(nodeConfig.Config);
                PassAvailable(available, node);
            }

            return newNodes.Count > 0;
        }

        private void PassNecessary(IEnumerable<SkillNodeConfig> available, SkillTreeConfig treeConfig, Dictionary<SkillNodeConfig, SkillNode> newNodes)
        {
            foreach (var availableCfg in available)
            {
                SkillNode node;

                if (!nodes.TryGetValue(availableCfg, out node))
                {
                    newNodes.TryGetValue(availableCfg, out node);
                }

                var fromTree = treeConfig.GetNecessary(availableCfg.Config);
                var fromNodes = nodes
                    .Where(x => x.Value.available.Any(y => y.Config == availableCfg.Config))
                    .Select(x => x.Key);

                var necessary = Array.Empty<SkillNodeConfig>()
                    .Concat(fromTree)
                    .Concat(fromNodes)
                    .ToList();

                if (necessary.Any(x => !nodes.ContainsKey(x)))
                {
                    continue;
                }

                if (!nodes.ContainsKey(availableCfg) && !newNodes.ContainsKey(availableCfg))
                {
                    node ??= new SkillNode(availableCfg.Config);
                    newNodes.Add(availableCfg, node);
                }

                node.nethessary.AddRange(necessary.Select(x => nodes[x]));
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

        private void Remove(SkillNodeConfig nodeConfig)
        {
            var node = nodes[nodeConfig];

            if (node.Available.Count() != 0)
            {
                throw new InvalidOperationException();
            }

            foreach (var nethessary in node.nethessary)
            {
                nethessary.available.Remove(node);
            }

            if (Nodes.Any(x => x.Available.Contains(node)))
            {
                throw new InvalidOperationException();
            }

            nodes.Remove(nodeConfig);
        }
    }
}
