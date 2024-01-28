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
    public Animator getDrunkAnimator;

    [HideInInspector]
    public bool inDialogue {
        get {
            return this._inDialogue;
        }
        set {
            this._inDialogue = value;
            GameObject.FindWithTag("Player").GetComponent<PlayerMovementController>().locked = value;
        }
    }

    private bool _inDialogue = false;

    private Joke _joke = new Joke();
    private Dictionary<string, State> _allPossibleStates = new Dictionary<string, State>();
    private string _currentStateName = "initial";
    private State _currentState;
    private List<Line> _currentLines = new List<Line>();
    private List<Button> _currentButtons = new List<Button>();
    private GameObject _player;

    private List<State> _stateHistory = new List<State>();

    private GameObject buttonPrefab;
    private Canvas canvas;

    void Start() {
        this.initStates();
        this._currentState = this._allPossibleStates.Count > 0 ? this._allPossibleStates["initial"] : null;
        this._currentLines = this._currentState.actions.Where(action => action.type == "Line").Select(action => (Line)action).ToList();
        this._player = GameObject.FindWithTag("Player");

        this.getDrunkAnimator.speed = 0;
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
        if (!this.validateState(trigger.currentState) && trigger.currentState != "any") return;

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

        this._currentState.onEnter();

        if (_currentState.name == null) _currentState.name = trigger.nextState;
        this._stateHistory.Add(this._currentState);
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

        if (_currentState.name == null) _currentState.name = stateName;
        this._stateHistory.Add(this._currentState);
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
                new State(
                    "initial",
                    () => {}
                )
            },
            {
                "initial bartender",
                new State(
                    "initial bartender",
                    () => { this.dialogueTrigger[0].TriggerDialogue(); }
                )
            },
            {
                "initial John",
                new State(
                    "initial John",
                    () => { 
                        this.dialogueTrigger[1].TriggerDialogue(); 

                        this.dialogueTrigger[0].dialogue.addSentence("There is nothing we can do...");

                        this.dialogueTrigger[0].dialogue.addOption(new Dialogue.option(
                            0,
                            "This man, John, looks really bad.",
                            "John investigation",
                            this.dialogueTrigger[0].dialogue.sentences.Count - 1
                        ));

                        this.dialogueTrigger[0].dialogue.addOption(new Dialogue.option(
                            this.dialogueTrigger[0].dialogue.sentences.Count - 1,
                            "I will try to help him...",
                            "John investigation",
                            -1
                        ));
                    }
                )
            },
            {
                "John investigation",
                new State(
                    "John investigation",
                    () => {
                    
                    }
                )
            },
            {
                "initial Ken",
                new State(
                    "initial Ken",
                    () => {
                    this.dialogueTrigger[3].TriggerDialogue();
                    //     this.dialogueTrigger[0].dialogue.addSentence("There are rumors that he works as a clown... A very sad clown... By the way, did I already tell you that snails are not welcome here?...");
                    //     this.dialogueTrigger[0].dialogue.addOption(new Dialogue.option(
                    //         0,
                    //         "This man, Ken, looks really sad.",
                    //         "KenTheClown Investigation",
                    //         this.dialogueTrigger[0].dialogue.sentences.Count - 1
                    //     ));

                    //     this.dialogueTrigger[0].dialogue.addOption(new Dialogue.option(
                    //         this.dialogueTrigger[0].dialogue.sentences.Count - 1,
                    //         "...",
                    //         "KenTheClown Investigation",
                    //         -1
                    //     ));
                    }
                )
            },
            // {
            //     "KenTheClown Investigation",
            //     new State(() => {
                    
            //     })
            // },
            {
                "initial Rock",
                new State(
                    "initial Rock",
                    () => {
                        this.dialogueTrigger[4].TriggerDialogue();
                        this.dialogueTrigger[0].dialogue.addSentence("He recently beat three men in this bar. Since then, people without weapons do not approach him... By the way, why are you still here?...");
                        this.dialogueTrigger[0].dialogue.addOption(new Dialogue.option(
                            0,
                            "This man, Rock, looks really dangerous.",
                            "BaldRock Investigation",
                            this.dialogueTrigger[0].dialogue.sentences.Count - 1
                        ));

                        this.dialogueTrigger[0].dialogue.addOption(new Dialogue.option(
                            this.dialogueTrigger[0].dialogue.sentences.Count - 1,
                            "...",
                            "BaldRock Investigation",
                            -1
                        ));
                    }
                )
            },
            {
                "BaldRock Investigation",
                new State(
                    "BaldRock Investigation",
                    () => {
                    
                    }
                )
            },
            {
                "StrangeBottle dialogue",
                new State(
                    "StrangeBottle dialogue",
                    () => {
                        this.dialogueTrigger[2].TriggerDialogue(); 
                    }
                )
            },
            {
                "StrangeBottle investigation",
                new State(
                    "StrangeBottle investigation",
                    () => { 
                        this.getDrunkAnimator.speed = 1;

                        this.dialogueTrigger[3].dialogue.addSentence("Thanks, it really did! By the way, there is a fan of men's asses who always follows the rules and has earned a great deal of authority in front of the Bartender...");
                        this.dialogueTrigger[3].dialogue.options = new List<Dialogue.option>();
                        this.dialogueTrigger[3].dialogue.addOption(new Dialogue.option(
                            0,
                            "Take it, this will help",
                            "Ken FinishLine",
                            this.dialogueTrigger[3].dialogue.sentences.Count - 1
                        ));

                        this.dialogueTrigger[3].dialogue.addOption(new Dialogue.option(
                            1,
                            "Thanks for the information!",
                            "Ken FinishLine",
                            -1
                        ));
                    }
                )
            },
            {
                "Ken FinishLine",
                new State(
                    "Ken FinishLine",
                    () => {

                    }
                )
            },
            {
                "cop Investigation",
                new State(
                    "cop Investigation",
                    () => {
                        this.dialogueTrigger[5].TriggerDialogue();
                    }
                )
            },
            {
                "Left bottle",
                new State(
                    "Left bottle",
                    () => {

                    }
                )
            },
            {
                "finish",
                new State(
                    "finish",
                    () => { this.finish(); }
                )
            },
            {
                "wc enter",
                new State(
                    "wc enter",
                    () => { this._player.transform.position = this.wcEnter.transform.position; }
                )
            },
            {
                "wc exit",
                new State(
                    "wc exit",
                    () => { this._player.transform.position = this.wcExit.transform.position; }
                )
            },
            {
                "initial Dealer",
                new State(
                    "initial Dealer",
                    () => {
                        this.dialogueTrigger[5].TriggerDialogue();
                        this.dialogueTrigger[0].dialogue.addSentence("");
                        this.dialogueTrigger[0].dialogue.addOption(new Dialogue.option(
                            0,
                            "",
                            "DrugDealer Investigation",
                            this.dialogueTrigger[0].dialogue.sentences.Count - 1
                        ));
                    }
                )
            },
            {
                "DrugDealer Investigation",
                new State(
                    "DrugDealer Investigation",
                    () => {

                    }
                )
            }
        };

        this._joke.addSelectedLine(new Line(this.initialLine));
    }

    public string getCurrentStateName() {
        return this._currentStateName;
    }

    private bool validateState(string stateName) {
        Debug.Log("validateState " + stateName);
        string alreadyPassedStates = "";
        this._stateHistory.ForEach(state => {
            alreadyPassedStates += state.name + ", ";
        });
        Debug.Log("alreadyPassedStates: " + alreadyPassedStates);

        return this._stateHistory.Any(state => state.name == stateName);
    }
    
    [System.Serializable]
    public struct DialogueDictionary {
        public string name;
        public DialogTrigger value;
    }
}