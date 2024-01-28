using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

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
    public Material copMaterial;
    public Texture copTexture;
    public Texture gayCopTexture;

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
        this.copMaterial.SetTexture("_BaseMap", this.copTexture);
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
        StaticData.passedStates = this._stateHistory;
        SceneManager.LoadScene("Final");
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
                "asked for drink",
                new State(
                    "asked for drink",
                    () => { this.finish(); }
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
                        this.dialogueTrigger[5].dialogue.npcName = "Sweet Joe";
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
            },
            {
                "drug request",
                new State(
                    "drug request",
                    () => {
                        this.dialogueTrigger[5].dialogue = new Dialogue("Sweet Joe", new List<string>());
                        this.dialogueTrigger[5].dialogue.addSentence("Have you found him?");
                        this.dialogueTrigger[5].dialogue.addOption(new Dialogue.option(
                            1,
                            "No, I haven't found him yet.",
                            "cop Investigation",
                            -1
                        ));
                    }
                )
            },
            {
                "dealer found",
                new State(
                    "dealer found",
                    () => {
                        this.dialogueTrigger[6].TriggerDialogue();
                    }
                )
            },
            {
                "dealer revealed",
                new State(
                    "dealer revealed",
                    () => {
                        this.dialogueTrigger[5].dialogue.addOption(new Dialogue.option(
                            0,
                            "Yes, he is in the toilet. Here is the evidence.",
                            "cop became gay",
                            -1
                        ));
                    }
                )
            },
            {
                "cop became gay",
                new State(
                    "cop became gay",
                    () => {
                        this.copMaterial.SetTexture("_BaseMap", this.gayCopTexture);
                        StartCoroutine(this.restartCopsDialogue());
                    }
                )
            },
            {
                "looking for jokes",
                new State(
                    "looking for jokes",
                    () => {
                        this.dialogueTrigger[0].dialogue.addSentence("Go ahead. I'm listening.");

                        this.dialogueTrigger[0].dialogue.addOption(
                            new Dialogue.option(
                                0,
                                "Wanna hear a joke?",
                                "",
                                this.dialogueTrigger[0].dialogue.sentences.Count - 1
                            )
                        );

                        this.dialogueTrigger[0].dialogue.addSentence("Are you kidding me? Go out of here! Security!");

                        this.dialogueTrigger[0].dialogue.addOption(
                            new Dialogue.option(
                                this.dialogueTrigger[0].dialogue.sentences.Count - 2,
                                "So, uhm... a snail walks into a bar?..",
                                "stupid joke",
                                this.dialogueTrigger[0].dialogue.sentences.Count - 1
                            )
                        );

                        this.dialogueTrigger[0].dialogue.addOption(
                            new Dialogue.option(
                                this.dialogueTrigger[0].dialogue.sentences.Count - 1,
                                "...",
                                "finish",
                                -1
                            )
                        );
                    }
                )
            },
            {
                "pistol investigation",
                new State(
                    "pistol investigation",
                    () => {
                        this.dialogueTrigger[7].TriggerDialogue();
                    }
                )
            },
            {
                "gun take",
                new State(
                    "gun take",
                    () => {
                        if (this.validateState("cop became gay") == false) {
                            this.dialogueTrigger[7].dialogue = new Dialogue("Cop Joe", new List<string>());
                            this.dialogueTrigger[7].dialogue.addSentence("Hey, what are you doing here? Is this a gun?! You are under arrest!");
                            this.dialogueTrigger[7].dialogue.addOption(new Dialogue.option(
                                0,
                                "...",
                                "finish",
                                -1
                            ));
                            
                            StartCoroutine(this.restartGunDialogue());

                            this._stateHistory.Add(new State("taken with gun", () => {}));

                            return;
                        }

                        this.dialogueTrigger[1].dialogue.sentences = new List<string>();
                        this.dialogueTrigger[1].dialogue.addSentence("K... Kill me... Please...");
                        this.dialogueTrigger[1].dialogue.addSentence("Thanks boy. Now I feel better. By the way, I now one good joke about that");
                        this.dialogueTrigger[1].dialogue.addSentence("I'm on a whiskey diet. I've lost three days already.");
                        this.dialogueTrigger[1].dialogue.addSentence("Here's another one. I'm not a complete idiot, some parts are missing.");
                        this.dialogueTrigger[1].dialogue.addSentence("Take this: A priest, a rabbi and a vicar walk into a bar. The barman says, “Is this some kind of joke?”");


                        this.dialogueTrigger[1].dialogue.addOption(new Dialogue.option(
                            0,
                            "What? No, never!",
                            "",
                            -1
                        ));
                        this.dialogueTrigger[1].dialogue.addOption(new Dialogue.option(
                            0,
                            "Put the trigger now he's dead",
                            "John awakened",
                            1
                        ));
                        this.dialogueTrigger[1].dialogue.addOption(new Dialogue.option(
                            1,
                            "...",
                            "",
                            2
                        ));
                        this.dialogueTrigger[1].dialogue.addOption(new Dialogue.option(
                            2,
                            "What a funny one!",
                            "",
                            3
                        ));
                        this.dialogueTrigger[1].dialogue.addOption(new Dialogue.option(
                            3,
                            "Ha-ha, one more!",
                            "",
                            4
                        ));
                        this.dialogueTrigger[1].dialogue.addOption(new Dialogue.option(
                            4,
                            "I'm laughing so hard! I can't stop! I need to tell these to someone else!",
                            "",
                            -1
                        ));
                    }
                )
            },
            {
                "John awakened",
                new State(
                    "John awakened",
                    () => {
                        this.dialogueTrigger[0].dialogue = new Dialogue("Bartender", new List<string>());
                        this.dialogueTrigger[0].dialogue.addSentence("Hey there");
                        this.dialogueTrigger[0].dialogue.addSentence("Snails are not allowed here. Go away.");
                        this.dialogueTrigger[0].dialogue.addSentence("Go ahead. I'm listening.");
                        this.dialogueTrigger[0].dialogue.addSentence("Are you kidding me? Go out of here! Security!");
                        this.dialogueTrigger[0].dialogue.addSentence("Ha-ha-ha! FINALLY! ABSOLUTE POWERFUL JOKE! AFTER ALL THESE YEARS!");
                        this.dialogueTrigger[0].dialogue.addSentence("Nevermind, take your drink.");

                        this.dialogueTrigger[0].dialogue.addOption(new Dialogue.option(
                            0,
                            "I need a drink",
                            "finish",
                            1
                        ));
                        this.dialogueTrigger[0].dialogue.addOption(new Dialogue.option(
                            0,
                            "Wanna hear a joke?",
                            "",
                            2
                        ));
                        this.dialogueTrigger[0].dialogue.addOption(new Dialogue.option(
                            2,
                            "I'm on a whiskey diet. I've lost three days already.",
                            "stupid joke",
                            3
                        ));
                        this.dialogueTrigger[0].dialogue.addOption(new Dialogue.option(
                            2,
                            "A rabbi, a priest and a vicar walk into a bar. The barman says, “Is this some kind of joke?”",
                            "good joke",
                            4
                        ));
                        this.dialogueTrigger[0].dialogue.addOption(new Dialogue.option(
                            2,
                            "I'm not a complete idiot, some parts are missing.",
                            "stupid joke",
                            3
                        ));
                        this.dialogueTrigger[0].dialogue.addOption(new Dialogue.option(
                            3,
                            "...",
                            "finish",
                            -1
                        ));
                        this.dialogueTrigger[0].dialogue.addOption(new Dialogue.option(
                            4,
                            "What?",
                            "",
                            5
                        ));
                        this.dialogueTrigger[0].dialogue.addOption(new Dialogue.option(
                            5,
                            "Thanks!",
                            "finish",
                            -1
                        ));
                    }
                )
            },
            {
                "called the cop gay",
                new State(
                    "called the cop gay",
                    () => { this.finish(); }
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

    private IEnumerator restartCopsDialogue() {
        yield return new WaitForSeconds(2);

        this.dialogueTrigger[5].dialogue = new Dialogue("Sweet Joe", new List<string>());
        this.dialogueTrigger[5].dialogue.addSentence("Ahh, what a good stuff");
        this.dialogueTrigger[5].dialogue.addSentence("Thanks for help!");
        this.dialogueTrigger[5].dialogue.addSentence("Oh, I forgot about that. Listen, the bartender is a big fan of good jokes. But you need to come up with a really good one");

        this.dialogueTrigger[5].dialogue.addOption(new Dialogue.option(
            0,
            "...",
            null,
            1
        ));
        this.dialogueTrigger[5].dialogue.addOption(new Dialogue.option(
            1,
            "No problem... So will you tell me how to become a bartender's friend?",
            null,
            2
        ));
        this.dialogueTrigger[5].dialogue.addOption(new Dialogue.option(
            2,
            "I see. Thanks for the information",
            "looking for jokes",
            -1
        ));
    }

    private IEnumerator restartGunDialogue() {
        yield return new WaitForSeconds(2);
        this.dialogueTrigger[7].TriggerDialogue();
    }
    
    [System.Serializable]
    public struct DialogueDictionary {
        public string name;
        public DialogTrigger value;
    }
}