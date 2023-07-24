using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{
    internal class SkillLinksSelector : MonoBehaviour
    {
        [SerializeField] private Color necessaryColor = Color.red;
        [SerializeField] private Color availableColor = Color.green;

        private SkillLinksUI links;
        private SkillSelector selector;

        private readonly HashSet<(SkillLinkUI link, Color defaultColor)> lastSelected = new();

        public void Consturct(SkillLinksUI links, SkillSelector selector)
        {
            this.links = links;
            this.selector = selector;
        }

        public void Subscribe() => selector.OnSelect += Select;
        public void Unscribe() => selector.OnSelect -= Select;

        public void Select(SkillNodeUI node)
        {
            if (lastSelected != null)
            {
                foreach (var (link, defaultColor) in lastSelected)
                {
                    link.color = defaultColor;
                }
                lastSelected.Clear();
            }

            foreach (var link in links.GetLinks(node.Config))
            {
                lastSelected.Add((link, link.color));

                if(link.Necessary.Config == node.Config)
                {
                    link.color = availableColor;
                }

                if (link.Node.Config == node.Config)
                {
                    link.color = necessaryColor;
                }
            }
        }
    }
}
