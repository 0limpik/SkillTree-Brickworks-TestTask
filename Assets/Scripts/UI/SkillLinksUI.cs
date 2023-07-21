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

        void Awake()
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
                    if (factory.Items.Any(x => x.Is(skill.RectTransform, avalibleSkill.RectTransform)))
                    {
                        continue;
                    }

                    var link = factory.Create();
                    link.Set(skill.RectTransform, avalibleSkill.RectTransform);
                }
            }
        }

        public void Clear() => factory.Clear();
    }
}
