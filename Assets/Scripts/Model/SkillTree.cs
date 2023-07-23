using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Configuration;

namespace Assets.Scripts.Model
{
    public class SkillTree
    {
        public readonly IEnumerable<ISkillNode> Nodes;

        public SkillTree(IEnumerable<ISkillNode> nodes)
        {
            Nodes = nodes;
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
