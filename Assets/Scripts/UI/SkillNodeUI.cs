using System;
using System.Collections.Generic;
using TestTask.Configuration;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TestTask.UI
{
    internal class SkillNodeUI : MonoBehaviour
    {
        [SerializeField] private SkillConfig config;
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image selectImage;
        [SerializeField] private Color learnedColor = Color.white;

        public event Action<SkillNodeUI> OnNodeSelect;

        public SkillConfig  Config => config;
        public RectTransform RectTransform => this.transform as RectTransform;

        public bool IsLearned { get; private set; }

        public IEnumerable<SkillLinkUI> Links => links;

        private readonly List<SkillLinkUI> links = new();
        private Color defaultColor;

        void Awake()
        {
            defaultColor = button.targetGraphic.color;

            Deselect();
        }

        void OnEnable() => button.onClick.AddListener(OnClick);
        void OnDisable() => button.onClick.RemoveListener(OnClick);
        void OnDestroy() => OnNodeSelect = null;

        public void Set(SkillConfig config)
        {
            this.config = config;
            this.name = config.Name;
            this.nameText.text = config.Name;
        }

        public void AddLink(SkillLinkUI link)
        {
            if (!links.Contains(link))
            {
                links.Add(link);
            }
        }

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
