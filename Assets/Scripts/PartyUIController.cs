// PartyUIController.cs
// Este script controla la UI de las opciones de party

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PartyUIController : MonoBehaviour
{
    [System.Serializable]
    public class PartySlot
    {
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI descriptionText;
        public Button button;
    }

    public PartySlot[] partySlots;
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.OnNewRound += UpdateUI;
    }

    public void UpdateUI(List<GameManager.Party> parties)
    {
        for (int i = 0; i < partySlots.Length; i++)
        {
            var slot = partySlots[i];
            var party = parties[i];

            slot.nameText.text = party.template.name;
            slot.descriptionText.text = party.template.description;

            int index = i;
            slot.button.onClick.RemoveAllListeners();
            slot.button.onClick.AddListener(() => OnPartyChosen(index));
        }
    }

    void OnPartyChosen(int index)
    {
        gameManager.MakeChoice(index);
    }
}
