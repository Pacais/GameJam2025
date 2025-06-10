// GameManager para "Go To Parties"
// Controla el flujo del juego: rondas, elección de parties, antifragilidad, estrés

using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class PartyTemplate
    {
        public string name;
        public string description;
        public int antifragilityGain;
        public int stressImpact;
        public bool canBeAdequate;
    }

    public class Party
    {
        public PartyTemplate template;
        public bool isAdequate = false;
        public bool isRest = false;
    }

    public List<PartyTemplate> allTemplates;

    public int round = 1;
    public int antifragility = 0;
    public int stress = 0;
    public int maxStress = 10;

    private List<Party> currentParties;

    public event Action<List<Party>> OnNewRound;
    public event Action<string> OnMessage;

    private void Start()
    {
        StartRound();
    }

    void StartRound()
    {
        Debug.Log("\n--- Ronda " + round + " ---");
        currentParties = GenerateParties();

        OnNewRound?.Invoke(currentParties);

        for (int i = 0; i < currentParties.Count; i++)
        {
            var p = currentParties[i];
            Debug.Log("[" + i + "] " + p.template.name +
                      (p.isRest ? " (descanso)" : "") +
                      " - " + p.template.description);
        }
    }

    List<Party> GenerateParties()
    {
        List<Party> parties = new List<Party>();

        List<PartyTemplate> adequateCandidates = allTemplates.FindAll(t => t.canBeAdequate);
        PartyTemplate adequateTemplate = adequateCandidates[UnityEngine.Random.Range(0, adequateCandidates.Count)];

        Party adequate = new Party { template = adequateTemplate, isAdequate = true };
        parties.Add(adequate);

        List<PartyTemplate> incorrectTemplates = new List<PartyTemplate>(allTemplates);
        incorrectTemplates.Remove(adequateTemplate);

        while (parties.Count < 3 && incorrectTemplates.Count > 0)
        {
            PartyTemplate pt = incorrectTemplates[UnityEngine.Random.Range(0, incorrectTemplates.Count)];
            incorrectTemplates.Remove(pt);

            Party p = new Party { template = pt };
            parties.Add(p);
        }

        Party rest = new Party
        {
            template = new PartyTemplate { name = "Tomarte un Respiro", description = "Un momento de calma para reflexionar.", antifragilityGain = 0, stressImpact = -1 },
            isRest = true
        };
        parties.Add(rest);

        for (int i = 0; i < parties.Count; i++)
        {
            var tmp = parties[i];
            int r = UnityEngine.Random.Range(i, parties.Count);
            parties[i] = parties[r];
            parties[r] = tmp;
        }

        return parties;
    }

    public void MakeChoice(int index)
    {
        Party chosen = currentParties[index];

        Debug.Log("\nHas elegido: " + chosen.template.name);

        if (chosen.isAdequate)
        {
            antifragility += chosen.template.antifragilityGain;
            Debug.Log("¡Buena elección! Antifragilidad aumentada en " + chosen.template.antifragilityGain);
        }
        else if (chosen.isRest)
        {
            stress = Mathf.Max(0, stress + chosen.template.stressImpact);
            Debug.Log("Te tomaste un descanso. Estrés reducido en 1.");
        }
        else
        {
            stress += chosen.template.stressImpact;
            Debug.Log("No era la mejor party... Estrés +" + chosen.template.stressImpact);
        }

        Debug.Log("Antifragilidad: " + antifragility);
        Debug.Log("Estrés: " + stress + "/" + maxStress);

        if (stress >= maxStress)
        {
            Debug.Log("\n\uD83D\uDC80 GAME OVER: Tu estrés te ha superado.");
        }
        else
        {
            round++;
            StartRound();
        }
    }
}