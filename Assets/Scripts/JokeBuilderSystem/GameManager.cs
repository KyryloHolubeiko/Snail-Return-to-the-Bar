using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/*
    tl;dr; just a state machine
    ---------------------------

    depending on current state of the game,
    the player is provided with a set of lines to choose from
    or triggers that can invoke actions
    selecting the line will trigger an action
    and change the state of the game as well
    the game will end when the player reaches the final state
    then the joke will be built up and displayed
*/
public class GameManager : MonoBehaviour {
    public GameObject buttonPrefab;
    public Canvas canvas;
    public DialogueDictionary[] dialogues;

    private Joke _joke = new Joke();
    private List<State> _allPossibleStates = new List<State>();
    private State _currentState = null;
    private List<Line> _currentLines = new List<Line>();
    private List<Button> _currentButtons = new List<Button>();

    void Start() {
        this.initStates();
        this._currentState = this._allPossibleStates.Count > 0 ? this._allPossibleStates[0] : null;
        this._currentLines = this._currentState.actions.Where(action => action.type == "Line").Select(action => (Line)action).ToList();
    }

    public List<Line> getLines() {
        return this._currentState.actions.Where(action => action.type == "Line").Select(action => (Line)action).ToList();
    }

    public void selectLine(string lineText) {
        Line selectedLine = this._currentLines.Find(line => line.text == lineText);
        this._joke.addSelectedLine(selectedLine);

        if (selectedLine.nextState == null) {
            this.finish();
            return;
        }

        this._currentState = selectedLine.nextState;
        this._currentState.onEnter();
        this._currentLines = this._currentState.actions.Where(action => action.type == "Line").Select(action => (Line)action).ToList();
        this.createButtonsFromLines();
    }

    public void triggerNextState(EnvironmentTrigger trigger) {
        if (this._currentState == null) return;
        if (trigger.currentStateIndex != this._allPossibleStates.IndexOf(this._currentState)) return;

        if (this._currentState.actions.Count == 0) return;

        State toState = this._allPossibleStates[trigger.nextStateIndex];

        if (toState == null) {
            this.finish();
            return;
        }

        if (!this._currentState.actions.Any(action => action.nextState == toState)) return;

        this._currentState = toState;
        this._currentLines = this._currentState.actions.Where(action => action.type == "Line").Select(action => (Line)action).ToList();
        this._joke.addSelectedLine(new Line(trigger.lineToAddOnSelect));
        this._currentState.onEnter();
    }

    public List<Button> createButtonsFromLines() {
        this._currentButtons.ForEach(button => Destroy(button.gameObject));

        List<Button> buttons = new List<Button>();

        foreach (Line line in this._currentLines) {
            GameObject buttonObject = Instantiate(this.buttonPrefab);
            Button button = buttonObject.GetComponent<Button>();
            button.GetComponentInChildren<Text>().text = line.text;
            button.onClick.AddListener(() => this.selectLine(line.text));
            button.transform.SetParent(this.canvas.transform, false);
            button.gameObject.SetActive(false);
            buttons.Add(button);
        }

        this._currentButtons = buttons;

        return buttons;
    }

    public void showButtons() {
        this.createButtonsFromLines();
        this._currentButtons.ForEach(button => button.gameObject.SetActive(true));
    }

    public void hideButtons() {
        this._currentButtons.ForEach(button => Destroy(button.gameObject));
        this._currentButtons.Clear();
    }

    private void finish() {
        Debug.Log(this._joke.build());
    }

    // yes, it is very bad and ugly code. but I can't come up with anything better
    private void initStates() {
        this._allPossibleStates = new List<State> {
            new State(
                "A snail walks into a bar",
                new List<IStateAction> {
                    new Line(
                        "Selects the first line of the initial state"
                    ),
                    new Line(
                        "Selects the second line of the initial state"
                    )
                },
                () => {
                    // this.dialogues.Get('bartenderDialogue').dialogue.sentences = new List<String> {
                    //     "What can I get you?",
                    //     "We don't serve snails here.",
                    //     "Get out!"
                    // };
                }
            ),
            new State(
                "second state",
                new List<IStateAction> {
                    new Line(
                        "Selects the first line of the second state"
                    ),
                    new Line(
                        "Selects the second line of the second state"
                    )
                },
                () => { this.showButtons(); }
            ),
            new State(
                "last state",
                new List<IStateAction> {
                    new Line(
                        "This is the end."
                    )
                },
                () => { this._joke.addSelectedLine(new Line("This is the end.")); this.finish(); }
            )
        };

        this._allPossibleStates[0].actions[0].nextState = this._allPossibleStates[1];
        this._allPossibleStates[0].actions[1].nextState = this._allPossibleStates[2];
        this._allPossibleStates[1].actions[0].nextState = this._allPossibleStates[2];
        this._allPossibleStates[1].actions[1].nextState = this._allPossibleStates[0];

        this._joke.addSelectedLine(new Line(this._allPossibleStates[0].name));
    }

    [System.Serializable]
    public struct DialogueDictionary {
        public string name;
        public DialogueTrigger value;
    }
}