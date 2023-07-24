using System;
using System.Collections.Generic;
using Assets.Scripts.Configuration;
using TMPro;
using Unity.VisualScripting;
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

        private readonly HashSet<SkillNodeUI> necessary = new();

        public RectTransform RectTransform => this.transform as RectTransform;

        void Awake() => Deselect();

        public void Set(SkillConfig skill, IEnumerable<SkillNodeUI> necessary)
        {
            Config = skill;
            name = skill.Name;
            nameText.text = skill.Name;
            this.necessary.AddRange(necessary);
        }

        void OnEnable() => button.onClick.AddListener(OnClick);
        void OnDisable() => button.onClick.RemoveListener(OnClick);
        void OnDestroy() => OnNodeSelect = null;

        public void Select() => selectImage.enabled = true;
        public void Deselect() => selectImage.enabled = false;

        private void OnClick() => OnNodeSelect?.Invoke(this);

    }
}
