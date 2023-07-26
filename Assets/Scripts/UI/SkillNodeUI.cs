using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Configuration;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    internal class SkillNodeUI : MonoBehaviour
    {
        public event Action<SkillNodeUI> OnNodeSelect;

        [field: SerializeField] public SkillConfig Config { get; private set; }

        [SerializeField] private Button button;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image selectImage;
        [SerializeField] private Color learnedColor = Color.white;

        public RectTransform RectTransform => this.transform as RectTransform;

        public bool IsLearned { get; private set; }

        public IEnumerable<SkillLinkUI> Available => links.Where(x => x.Necessary == this);
        public IEnumerable<SkillLinkUI> Necessary => links.Where(x => x.Node == this);
        public IEnumerable<SkillLinkUI> NecessaryLearned => links.Where(x => x.Node == this && x.Necessary.IsLearned);
        public IEnumerable<SkillLinkUI> NecessaryNotLearned => links.Where(x => x.Node == this && !x.Necessary.IsLearned);

        private readonly HashSet<SkillLinkUI> links = new();
        private Color defaultColor;

        void Awake()
        {
            defaultColor = button.targetGraphic.color;

            Deselect();
        }

        void OnEnable() => button.onClick.AddListener(OnClick);
        void OnDisable() => button.onClick.RemoveListener(OnClick);
        void OnDestroy() => OnNodeSelect = null;

        public void Set(SkillConfig skill)
        {
            this.Config = skill;
            this.name = skill.Name;
            this.nameText.text = skill.Name;
        }

        public void AddLink(SkillLinkUI link) => links.Add(link);
        public void RemoveLink(SkillLinkUI link) => links.Remove(link);

        public void Learn()
        {
            button.targetGraphic.color = learnedColor;
            IsLearned = true;

            foreach (var link in links)
            {
                link.NodeUpdate();
            }
        }

        public void Forget()
        {
            button.targetGraphic.color = defaultColor;
            IsLearned = false;

            foreach (var link in links)
            {
                link.NodeUpdate();
            }
        }

        public void Select() => selectImage.enabled = true;
        public void Deselect() => selectImage.enabled = false;
        private void OnClick() => OnNodeSelect?.Invoke(this);

    }
}
