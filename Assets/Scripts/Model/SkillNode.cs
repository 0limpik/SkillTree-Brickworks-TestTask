using System.Collections.Generic;
using Assets.Scripts.Configuration;

namespace Assets.Scripts.Model
{
    public interface ISkillNode
    {
        SkillConfig Config { get; }

        IEnumerable<ISkillNode> Available { get; }
        IEnumerable<ISkillNode> Necessary { get; }
    }

    public class SkillNode : ISkillNode
    {
        public SkillConfig Config { get; }

        public IEnumerable<ISkillNode> Necessary => nethessary;
        public IEnumerable<ISkillNode> Available => available;

        public readonly HashSet<SkillNode> nethessary = new();
        public readonly HashSet<SkillNode> available = new();

        public SkillNode(SkillConfig skill)
        {
            this.Config = skill;
        }

        public override string ToString()
            => $"{Config.Name} N:{nethessary.Count} A:{available.Count}";
    }
}
