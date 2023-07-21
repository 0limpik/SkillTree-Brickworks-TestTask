using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Configuration;
using Assets.Scripts.Model;
using Unity.VisualScripting;

namespace Assets.Scripts.Services
{
    internal class SkillLearnService
    {
        public Action<SkillConfig> SkillChanged;

        private readonly HashSet<SkillConfig> learnedSkills = new();
        private readonly HashSet<SkillConfig> rootSkills = new();

        private readonly SkillTree tree;

        public SkillLearnService(SkillTree tree)
        {
            this.tree = tree;
        }

        public void AddRootSkill(IEnumerable<SkillConfig> rootSkill)
        {
            rootSkills.AddRange(rootSkill);
        }

        public bool CanLearn(SkillConfig config)
        {
            if (rootSkills.Contains(config))
            {
                return false;
            }

            if (learnedSkills.Contains(config))
            {
                return false;
            }

            var node = tree.GetNode(config);

            if (node.Necessary.Any(IsLearn))
            {
                return true;
            }

            return false;
        }

        private bool IsLearn(ISkillNode node)
        {
            if (rootSkills.Contains(node.Config))
            {
                return true;
            }

            if (learnedSkills.Contains(node.Config))
            {
                return true;
            }

            return false;
        }

        public void Learn(SkillConfig config)
        {
            if (!CanLearn(config))
            {
                throw new InvalidOperationException();
            }

            learnedSkills.Add(config);
            SkillChanged?.Invoke(config);
        }

        public bool CanForget(SkillConfig config)
        {
            if (rootSkills.Contains(config))
            {
                return false;
            }

            if (!learnedSkills.Contains(config))
            {
                return false;
            }

            var node = tree.GetNode(config);

            if (node.Available.Any(IsLearn))
            {
                return false;
            }

            return true;
        }

        public void Forget(SkillConfig config)
        {
            if (!CanForget(config))
            {
                throw new InvalidOperationException();
            }

            learnedSkills.Remove(config);
            SkillChanged?.Invoke(config);
        }
    }
}
