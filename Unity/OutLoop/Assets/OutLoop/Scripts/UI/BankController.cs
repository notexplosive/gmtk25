using System;
using OutLoop.Core;
using OutLoop.Data;
using TMPro;
using UnityEngine;

namespace OutLoop.UI
{
    public class BankController : MonoBehaviour
    {
        [SerializeField]
        private AnswerType _bankType;

        [SerializeField]
        private LoopDataRelay? _relay;

        [SerializeField]
        private TextMeshProUGUI? _titleText;

        [SerializeField]
        private BankTextContent? _bankText;

        private void Awake()
        {
            if (_titleText != null)
            {
                _titleText.text = GetTitleForBankType();
            }

            if (_relay != null)
            {
                _relay.State().WordAddedToBank += AppendToBank;

                if (_relay.State().GetWordsFromBank(_bankType).Count == 0)
                {
                    gameObject.SetActive(false);
                }
            }
        }

        private void AppendToBank(AnswerType answerType, string word)
        {
            if (answerType != _bankType)
            {
                return;
            }

            if (_bankText == null)
            {
                return;
            }

            gameObject.SetActive(true);
            _bankText.Add(_bankType,word);
        }

        private string GetTitleForBankType()
        {
            if (_relay == null)
            {
                return "???";
            }
            
            if (_bankType == AnswerType.Hashtag)
            {
                return "#tags (0/10)";
            }

            if (_bankType == AnswerType.Username)
            {
                return "@users (0/10)";
            }

            return "???";
        }
    }
}