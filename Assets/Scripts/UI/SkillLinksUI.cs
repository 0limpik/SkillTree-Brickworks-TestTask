using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.UI
{
    internal class SkillLinksUI : MonoBehaviour
    {
        [SerializeField] private Transform linksRoot;
        [SerializeField] private SkillLinkUI linkTemplate;

        private SkillViewFactory<SkillLinkUI> factory;

        public void Consturct()
        {
            factory = new(linkTemplate, linksRoot);
        }

        public void CreateLinks(Dictionary<ISkillNode, SkillNodeUI> skills)
        {
            foreach (var node in skills.Keys)
            {
                var skill = skills[node];
                foreach (var avalible in node.Available)
                {
                    var avalibleSkill = skills[avalible];
                    if (factory.Items.Any(x => x.Is(skill, avalibleSkill)))
                    {
                        continue;
                    }

                    var link = factory.Create();
                    link.Set(skill, avalibleSkill);
                }
            }
        }

        public void RemoveLinks(IEnumerable<ISkillNode> nodes)
        {
            foreach (var node in nodes)
            {
                factory.Remove(x => x.First.Config == node.Config || x.Second.Config == node.Config);
            }
        }
    }
}
