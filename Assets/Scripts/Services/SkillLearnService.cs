using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Configuration;
using Assets.Scripts.Model;

namespace Assets.Scripts.Services
{
    internal class SkillLearnService
    {
        public event Action<SkillConfig> OnLearn;
        public event Action<SkillConfig> OnForget;

        private readonly List<SkillTree> trees = new();

        private readonly List<SkillConfig> learnedSkills = new();
        private readonly List<SkillConfig> rootSkills = new();

        public void AddTree(SkillTree tree)
        {
            trees.Add(tree);
            foreach (var node in tree.GetRoots())
            {
                rootSkills.Add(node.Config);
            }
        }

        public void RemoveTree(SkillTree tree)
        {
            trees.Remove(tree);
            foreach (var node in tree.GetRoots())
            {
                rootSkills.Remove(node.Config);
            }
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

            var node = GetNode(config);

            if (node.Necessary.Any(IsLearn))
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
            OnLearn?.Invoke(config);
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

            var node = GetNode(config);

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
            OnForget?.Invoke(config);
        }

        public IEnumerable<SkillConfig> ForgetAll()
        {
            var forgeted = new List<SkillConfig>();

            while (learnedSkills.Count != 0)
            {
                var learnedSkillsCount = learnedSkills.Count;
                foreach (var skill in learnedSkills)
                {
                    if (CanForget(skill))
                    {
                        forgeted.Add(skill);
                    }
                }

                foreach (var skill in forgeted)
                {
                    if (learnedSkills.Contains(skill))
                    {
                        Forget(skill);
                    }
                }

                if (learnedSkillsCount == learnedSkills.Count)
                {
                    throw new ApplicationException("prevent while stuck");
                }
            }

            return forgeted;
        }

        public bool IsLearn(ISkillNode node)
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

        private ISkillNode GetNode(SkillConfig config)
        {
            foreach (var tree in trees)
            {
                var node = tree.GetNode(config);

                if (node != null)
                {
                    return node;
                }
            }

            throw new ArgumentException(nameof(config));
        }
    }
}
