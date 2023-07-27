using System.Collections.Generic;
using System.Linq;
using TestTask.Configuration;

namespace TestTask.Model
{
    public interface ISkillNode
    {
        SkillConfig Config { get; }

        IEnumerable<ISkillNode> Available { get; }
        IEnumerable<ISkillNode> Necessary { get; }

        IEnumerable<SkillConfig> NecessaryConfigs => Necessary.Select(x => x.Config);
        IEnumerable<SkillConfig> AvailableConfigs => Available.Select(x => x.Config);
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
