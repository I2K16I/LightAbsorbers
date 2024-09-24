using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BSA.UI
{
    public class Carousel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] private TextMeshProUGUI _titleLabel;
        [SerializeField] private TextMeshProUGUI _valueLabel;
        [SerializeField] private SimpleButton _leftButton;
        [SerializeField] private SimpleButton _rightButton;

        // --- Properties ---------------------------------------------------------------------------------------------

        // --- Events -------------------------------------------------------------------------------------------------

        // --- Unity Functions ----------------------------------------------------------------------------------------
        private void OnEnable()
        {
            _leftButton.OnClick += OnLeftClicked;
            _rightButton.OnClick += OnRightClicked;
        }

        private void OnDisable()
        {
            _leftButton.OnClick -= OnLeftClicked;
            _rightButton.OnClick -= OnRightClicked;
        }

        // --- Interface implementations ------------------------------------------------------------------------------
        public void OnPointerEnter(PointerEventData eventData)
        {

        }

        public void OnPointerExit(PointerEventData eventData)
        {

        }

        // --- Event callbacks ----------------------------------------------------------------------------------------
        private void OnLeftClicked(SimpleButton obj)
        {

        }

        private void OnRightClicked(SimpleButton obj)
        {

        }

        // --- Public/Internal Methods --------------------------------------------------------------------------------

        // --- Protected/Private Methods ------------------------------------------------------------------------------

        // ----------------------------------------------------------------------------------------
    }
}