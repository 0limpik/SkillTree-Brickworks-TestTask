using System;
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

        private Color defaultColor;

        public RectTransform RectTransform => this.transform as RectTransform;

        void Awake()
        {
            defaultColor = selectImage.color;

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

        public void Learn() => button.targetGraphic.color = learnedColor;
        public void Forget() => button.targetGraphic.color = defaultColor;

        public void Select() => selectImage.enabled = true;
        public void Deselect() => selectImage.enabled = false;

        private void OnClick() => OnNodeSelect?.Invoke(this);

    }
}
