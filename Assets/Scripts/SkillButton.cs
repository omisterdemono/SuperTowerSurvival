using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Infrastructure.UI
{
    public class SkillButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _activateButtonText;
        [SerializeField] private Image _skillIcon;
        [SerializeField] private Image _skillCooldown;

        public TextMeshProUGUI ActivateButtonText => _activateButtonText;
        public Image SkillIcon => _skillIcon;
        public Image SkillCooldown => _skillCooldown;

    }
}