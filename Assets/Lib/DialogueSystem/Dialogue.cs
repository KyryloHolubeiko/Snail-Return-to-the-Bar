using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue {   
    public string npcName;

    [TextArea(3, 10)]
    public List<string> sentences;
    public List<option> options;

    public Dialogue(string npcName, List<string> sentences) {
        this.npcName = npcName;
        this.sentences = sentences;
        this.options = new List<option>();
    }

    public Dialogue(string npcName, List<string> sentences, List<option> options) {
        if (sentences.Count != options.Count) throw new System.Exception("Dialogue sentences and options must have the same Count");

        this.npcName = npcName;
        this.sentences = sentences;
        this.options = options;
    }

    public List<List<option>> OrderOptionsBySentenceIndex() {
        List<List<option>> result = new List<List<option>>();

        for (int i = 0; i < this.sentences.Count; i++) {
            result.Add(new List<option>());
            foreach (option option in this.options) {
                if (option.sentenceIndex == i) result[i].Add(option);
            }
        }

        return result;
    }

    public List<List<option>> OrderOptionsBySentenceIndex(int index) {
        List<List<option>> result = new List<List<option>>();
        result.Add(new List<option>());

        foreach (option option in this.options) {
            if (option.sentenceIndex == index) result[0].Add(option);
        }

        return result;
    }

    [System.Serializable]
    public struct option {

        public option(int _sentenceIndex, string _optionText, string _nextState, int _nextDialogueIndex) {
            sentenceIndex = _sentenceIndex;
            optionText = _optionText;
            nextState = _nextState;
            nextDialogueIndex = _nextDialogueIndex;
        }

        public int sentenceIndex;
        public string optionText;
        public string nextState; // -1 to not change state
        public int nextDialogueIndex; // -1 to finish dialogue
    }
}
