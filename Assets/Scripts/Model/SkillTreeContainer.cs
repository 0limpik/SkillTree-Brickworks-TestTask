using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Configuration;
using Unity.VisualScripting;

namespace Assets.Scripts.Model
{
    public class SkillTreeContainer
    {
        public readonly SkillTree Tree;

        private readonly Dictionary<SkillNodeConfig, SkillNode> nodes = new();

        public SkillTreeContainer()
        {
            Tree = new SkillTree(nodes.Values);
        }

        public IEnumerable<ISkillNode> AddTree(SkillTreeConfig treeConfig)
        {
            if (Tree.CanAddTree(treeConfig).Any())
            {
                throw new InvalidOperationException(nameof(Tree.CanAddTree));
            }

            var addedNodes = new HashSet<ISkillNode>(treeConfig.Nodes.Length);
            foreach (var root in treeConfig.GetRoots())
            {
                var node = new SkillNode(root.Config);
                nodes.Add(root, node);
                addedNodes.Add(node);
            }

            var newNodes = new Dictionary<SkillNodeConfig, SkillNode>();
            while (true)
            {
                PassTree(treeConfig, newNodes);

                if (newNodes.Count == 0)
                {
                    break;
                }
                addedNodes.AddRange(newNodes.Values);
                newNodes.Clear();
            }
            PassTree(treeConfig, newNodes);

            return addedNodes;
        }

        public IEnumerable<ISkillNode> RemoveTree(SkillTreeConfig treeConfig)
        {
            if (Tree.CanRemoveTree(treeConfig).Any())
            {
                throw new InvalidOperationException(nameof(Tree.CanRemoveTree));
            }

            var nodesCount = nodes.Count() - treeConfig.Nodes.Count();

            var removedNodes = new HashSet<ISkillNode>(nodesCount);

            while (nodesCount < nodes.Count())
            {
                var leaves = Tree.GetLeaves()
                    .Where(x => treeConfig.Contains(x.Config))
                    .ToArray();

                if (!leaves.Any())
                {
                    throw new InvalidOperationException("no leaves, but the tree was not completely removed");
                }

                RemoveLeaves(treeConfig, leaves, removedNodes);
            }

            return removedNodes;
        }

        private void RemoveLeaves(SkillTreeConfig treeConfig, ISkillNode[] leaves, HashSet<ISkillNode> removedNodes)
        {
            foreach (var leaf in leaves)
            {
                var leafNodeConfig = treeConfig.GetNodeConfig(leaf.Config);

                var node = nodes[leafNodeConfig];

                if (node.Available.Count() != 0)
                {
                    throw new InvalidOperationException("not leaf");
                }

                foreach (var nethessary in node.nethessary)
                {
                    nethessary.available.Remove(node);
                }

                if (Tree.Nodes.Any(x => x.Available.Contains(node)))
                {
                    throw new InvalidOperationException("has dependensies");
                }

                nodes.Remove(leafNodeConfig);
                removedNodes.Add(node);
            }
        }

        private void PassTree(SkillTreeConfig treeConfig, Dictionary<SkillNodeConfig, SkillNode> newNodes)
        {
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
        }

        private void PassNecessary(IEnumerable<SkillNodeConfig> available, SkillTreeConfig treeConfig, Dictionary<SkillNodeConfig, SkillNode> newNodes)
        {
            foreach (var availableCfg in available)
            {
                var fromTree = treeConfig.GetNecessary(availableCfg.Config);
                var fromNodes = nodes
                    .Where(x => x.Value.Available.Any(y => y.Config == availableCfg.Config))
                    .Select(x => x.Key);

                var necessary = Array.Empty<SkillNodeConfig>()
                    .Concat(fromTree)
                    .Concat(fromNodes)
                    .ToList();

                if (necessary.Any(x => !nodes.ContainsKey(x)))
                {
                    continue;
                }

                if (!nodes.TryGetValue(availableCfg, out var node) && !newNodes.TryGetValue(availableCfg, out node))
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
    }
}
