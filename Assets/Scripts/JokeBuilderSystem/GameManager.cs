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
    // public DialogueDictionary[] dialogues;
    public string initialLine;

    public DialogTrigger[] dialogueTrigger;
    public GameObject wcEnter;
    public GameObject wcExit;

    private Joke _joke = new Joke();
    private Dictionary<string, State> _allPossibleStates = new Dictionary<string, State>();
    private string _currentStateName = "initial";
    private State _currentState;
    private List<Line> _currentLines = new List<Line>();
    private List<Button> _currentButtons = new List<Button>();
    private GameObject _player;

    private GameObject buttonPrefab;
    private Canvas canvas;

    void Start() {
        this.initStates();
        this._currentState = this._allPossibleStates.Count > 0 ? this._allPossibleStates["initial"] : null;
        this._currentLines = this._currentState.actions.Where(action => action.type == "Line").Select(action => (Line)action).ToList();
        this._player = GameObject.FindWithTag("Player");
    }

    public List<Line> getLines() {
        return this._currentState.actions.Where(action => action.type == "Line").Select(action => (Line)action).ToList();
    }

    public void selectLine(string lineText) {
        // Line selectedLine = this._currentLines.Find(line => line.text == lineText);
        // this._joke.addSelectedLine(selectedLine);

        // if (selectedLine.nextState == null) {
        //     this.finish();
        //     return;
        // }

        // this._currentState = selectedLine.nextState;
        // this._currentState.onEnter();
        // this._currentLines = this._currentState.actions.Where(action => action.type == "Line").Select(action => (Line)action).ToList();
        // this.createButtonsFromLines();
    }

    public void triggerNextState(EnvironmentTrigger trigger) {
        if (this._currentState == null) return;
        if (_currentStateName != trigger.currentState && trigger.currentState != "any") return;

        // if (this._currentState.actions.Count == 0) return;

        State toState = this._allPossibleStates[trigger.nextState];

        if (toState == null || trigger.nextState == "finish") {
            this.finish();
            return;
        }

        // if (!this._currentState.actions.Any(action => action.nextState == toState)) return;

        this._currentState = toState;
        this._currentStateName = trigger.nextState;
        this._currentLines = this._currentState.actions.Where(action => action.type == "Line").Select(action => (Line)action).ToList();
        this._joke.addSelectedLine(new Line(trigger.lineToAddOnSelect));

        Debug.Log("triggerNextState " + trigger.nextState);
        this._currentState.onEnter();
    }

    public void triggerNextState(string stateName) {
        Debug.Log("triggerNextState (string) " + stateName);

        if (this._currentState == null) return;
        if (_currentStateName == stateName) return;

        State toState = this._allPossibleStates[stateName];

        if (toState == null || stateName == "finish") {
            this.finish();
            return;
        }

        // if (!this._currentState.actions.Any(action => action.nextState == toState)) return;

        this._currentState = toState;
        this._currentStateName = stateName;
        this._currentLines = this._currentState.actions.Where(action => action.type == "Line").Select(action => (Line)action).ToList();
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
        this._allPossibleStates = new Dictionary<string, State> {
            { 
                "initial", 
                new State(() => {})
            },
            {
                "initial bartender",
                new State(() => { this.dialogueTrigger[0].TriggerDialogue(); })
            },
            {
                "finish",
                new State(() => { this.finish(); })
            },
            {
                "wc enter",
                new State(() => { this._player.transform.position = this.wcEnter.transform.position; })
            },
            {
                "wc exit",
                new State(() => { this._player.transform.position = this.wcExit.transform.position; })
            }
        };

        this._joke.addSelectedLine(new Line(this.initialLine));
    }

    public string getCurrentStateName() {
        return this._currentStateName;
    }

    [System.Serializable]
    public struct DialogueDictionary {
        public string name;
        public DialogTrigger value;
    }
}