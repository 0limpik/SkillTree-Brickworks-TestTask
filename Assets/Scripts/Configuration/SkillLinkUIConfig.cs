using UnityEngine;

namespace Assets.Scripts.Configuration
{
    [CreateAssetMenu(menuName = "TestTask/SkillLinksColors")]
    internal class SkillLinkUIConfig : ScriptableObject
    {
        [field: SerializeField] public Color LearnedColor { get; private set; } = new Color(.8f, .8f, .8f);
        [field: SerializeField] public Color NotLearnedColor { get; private set; } = new Color(.25f, .25f, .25f);
        [field: SerializeField] public float Thickness { get; private set; } = 8f;

        [field: SerializeField] public Color SelectionColor { get; private set; } = new Color(.5f, .5f, .5f);
        [field: SerializeField] public Color SelectionNecessaryColor { get; private set; } = new Color(1, .5f, .5f);
        [field: SerializeField] public float SelectionThickness { get; private set; } = 4f;
    }
}
