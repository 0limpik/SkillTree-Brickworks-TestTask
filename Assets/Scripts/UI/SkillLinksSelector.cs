using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Configuration;
using Assets.Scripts.Services;
using UnityEngine;

namespace Assets.Scripts.UI
{
    internal class SkillLinksSelector : MonoBehaviour
    {
        private SkillSelector selector;
        private SkillLearnService skillLearn;

        private readonly HashSet<SkillLinkUI> lastSelected = new();
        private readonly Stack<SkillLinkUI> stackLinks = new();
        private readonly Stack<SkillLinkUI> selectedStackLinks = new();

        private Coroutine selectCoroutine;

        public void Consturct(SkillSelector skillSelector, SkillLearnService skillLearn)
        {
            this.selector = skillSelector;
            this.skillLearn = skillLearn;
        }

        public void Subscribe()
        {
            selector.OnSelect += Select;
            skillLearn.OnLearn += Select;
            skillLearn.OnForget += Select;
        }

        public void Unscribe()
        {
            selector.OnSelect -= Select;
            skillLearn.OnLearn -= Select;
            skillLearn.OnForget -= Select;
        }

        private void Select(SkillNodeUI node) => Select();
        private void Select(SkillConfig _) => Select();

        private void Select()
        {
            if (selectCoroutine != null)
            {
                StopCoroutine(selectCoroutine);
            }

            selectCoroutine = StartCoroutine(StartSelect(selector.Selected));
        }

        private IEnumerator StartSelect(SkillNodeUI node)
        {
            SelectNonNecessaty(node);

            foreach (var item in stackLinks)
            {
                item.Deselect();
            }
            stackLinks.Clear();
            selectedStackLinks.Clear();

            if (!NeedDisplay(node))
            {
                yield break;
            }

            while (true)
            {
                yield return null;

                if (stackLinks.Count == 0)
                {
                    foreach (var link in node.NecessaryNotLearned)
                    {
                        stackLinks.Push(link);
                    }
                }

                var curentLink = stackLinks.Peek();
                curentLink.Select(curentLink.Node);
                if (NeedDisplay(curentLink.Necessary))
                {
                    if (selectedStackLinks.TryPeek(out var lastLink) && lastLink == curentLink)
                    {
                        curentLink.Deselect();
                        stackLinks.Pop();
                        selectedStackLinks.Pop();
                    }
                    else
                    {
                        selectedStackLinks.Push(curentLink);
                        foreach (var link in curentLink.Necessary.NecessaryNotLearned)
                        {
                            stackLinks.Push(link);
                        }
                    }
                }
                else
                {
                    yield return new WaitForSeconds(1);
                    curentLink.Deselect();
                    stackLinks.Pop();
                }
            }
        }

        private void SelectNonNecessaty(SkillNodeUI node)
        {
            foreach (var link in lastSelected)
            {
                link.Deselect();
            }
            lastSelected.Clear();

            foreach (var link in node.Available)
            {
                link.Select(node);
                lastSelected.Add(link);
            }

            foreach (var link in node.NecessaryLearned)
            {
                link.Select(node);
                lastSelected.Add(link);
            }
        }

        private bool NeedDisplay(SkillNodeUI node) => node.NecessaryNotLearned.Any() && !node.NecessaryLearned.Any();
    }
}
