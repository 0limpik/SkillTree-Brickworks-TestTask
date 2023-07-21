using System;
using Assets.Scripts.Configuration;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.View
{
    internal class SkillNodeUI : MonoBehaviour
    {
        public event Action<SkillNodeUI> OnNodeSelect;

        [field: SerializeField] public SkillConfig Config { get; private set; }

        [SerializeField] private Button button;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image selectImage;

        public RectTransform RectTransform => this.transform as RectTransform;

        void Awake() => Deselect();

        public void Set(SkillConfig skill)
        {
            Config = skill;
            name = skill.Name;
            nameText.text = skill.Name;
        }

        void OnEnable() => button.onClick.AddListener(OnClick);
        void OnDisable() => button.onClick.RemoveListener(OnClick);
        void OnDestroy() => OnNodeSelect = null;

        public void Select() => selectImage.enabled = true;
        public void Deselect() => selectImage.enabled = false;

        private void OnClick() => OnNodeSelect?.Invoke(this);

    }
}
