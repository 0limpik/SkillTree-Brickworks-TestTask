using System;

namespace Assets.Scripts.UI
{
    internal class SkillSelector
    {
        public event Action<SkillNodeUI> OnSelect;
        public SkillNodeUI Selected { get; private set; }

        public void Register(SkillNodeUI skillNodeUI) => skillNodeUI.OnNodeSelect += SelectNode;
        public void Unregister(SkillNodeUI skillNodeUI) => skillNodeUI.OnNodeSelect -= SelectNode;

        private void SelectNode(SkillNodeUI node)
        {
            if (Selected != null)
            {
                Selected.Deselect();
            }
            Selected = node;
            OnSelect?.Invoke(node);
            Selected.Select();
        }
    }
}
