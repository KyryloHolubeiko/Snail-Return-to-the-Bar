using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue {   
    public string npcName;

    [TextArea(3, 10)]
    public string[] sentences;
    public option[] options;

    public Dialogue(string npcName, string[] sentences) {
        this.npcName = npcName;
        this.sentences = sentences;
        this.options = new option[sentences.Length];
    }

    public Dialogue(string npcName, string[] sentences, option[] options) {
        if (sentences.Length != options.Length) throw new System.Exception("Dialogue sentences and options must have the same length");

        this.npcName = npcName;
        this.sentences = sentences;
        this.options = options;
    }

    public List<List<string>> OrderOptionsBySentenceIndex() {
        List<List<string>> result = new List<List<string>>();

        for (int i = 0; i < this.sentences.Length; i++) {
            result.Add(new List<string>());
            foreach (option option in this.options) {
                if (option.sentenceIndex == i) result[i].Add(option.optionText);
            }
        }

        return result;
    }

    [System.Serializable]
    public struct option {
        public int sentenceIndex;
        public string optionText;
        public string nextState; // -1 to not change state
        public int nextDialogueIndex; // -1 to finish dialogue
    }
}
