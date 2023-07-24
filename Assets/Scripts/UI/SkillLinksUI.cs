using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Configuration;
using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.UI
{
    internal class SkillLinksUI : MonoBehaviour
    {
        [SerializeField] private Transform linksRoot;
        [SerializeField] private SkillLinkUI linkTemplate;

        private SkillViewFactory<SkillLinkUI> factory;

        private Dictionary<ISkillNode, SkillNodeUI> nodes = new();

        public void Consturct()
        {
            factory = new(linkTemplate, linksRoot);
        }

        public void CreateLink(ISkillNode node, SkillNodeUI nodeUI)
        {
            nodes.Add(node, nodeUI);
            foreach (var necessary in node.Necessary)
            {
                var necessaryNodeUI = nodes[necessary];
                if (factory.Items.Any(x => x.Is(nodeUI, necessaryNodeUI)))
                {
                    continue;
                }

                var link = factory.Create();
                link.Set(nodeUI, necessaryNodeUI);
            }
        }

        public void RemoveLink(ISkillNode node)
        {
            factory.Remove(x => x.Is(node.Config));
            nodes.Remove(node);
        }

        public IEnumerable<SkillLinkUI> GetLinks(SkillConfig config)
            => factory.Items.Where(x => x.Is(config));
    }
}
