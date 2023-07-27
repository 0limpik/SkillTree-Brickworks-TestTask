using UnityEngine;

namespace Assets.Scripts.Configuration
{
    [CreateAssetMenu(menuName = "TestTask/SkillLinksColors")]
    internal class SkillLinkUIConfig : ScriptableObject
    {
        [SerializeField] private Color learned              = new Color(.8f, .8f, .8f);
        [SerializeField] private Color forgeted             = new Color(.25f, .25f, .25f);
        [SerializeField] private float thickness            = 8f;

        [Header("Selection")]
        [SerializeField] private Color selected             = new Color(.5f, .5f, .5f);
        [SerializeField] private Color selectedNecessary    = new Color(1, .5f, .5f);
        [SerializeField] private float selectedThickness    = 12f;

        public Color Learned => learned;
        public Color Forgeted => forgeted;
        public float Thickness => thickness;

        public Color Selected => selected;
        public Color SelectedNecessary => selectedNecessary;
        public float SelectedThickness => selectedThickness;
    }
}
