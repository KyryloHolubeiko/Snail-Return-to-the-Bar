using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/*
    tl;dr; just a state machine
    ---------------------------

    depending on current state of the game,
    the player is provided with a set of lines to choose from
    selecting the line will trigger an action
    and change the state of the game as well
    the game will end when the player reaches the final state
    then the joke will be built up and displayed
*/
public class JokeManager : MonoBehaviour {
    public GameObject buttonPrefab;
    public Canvas canvas;

    private Joke _joke = new Joke();
    private List<State> _allPossibleStates = new List<State>();
    private State _currentState = null;
    private List<Line> _currentLines = new List<Line>();
    private List<Button> _currentButtons = new List<Button>();

    void Start() {
        this.initStates();
        this._currentState = this._allPossibleStates.Count > 0 ? this._allPossibleStates[0] : null;
        this._currentLines = this._currentState.lines;

        this.createButtonsFromLines();
    }

    public List<Line> getLines() {
        return this._currentState.lines;
    }

    public void selectLine(string lineText) {
        Line selectedLine = this._currentLines.Find(line => line.text == lineText);
        this._joke.addSelectedLine(selectedLine);
        
        if (selectedLine.nextState == null) {
            Debug.Log(this._joke.build());
            return;
        }

        this._currentState = selectedLine.nextState;
        this._currentLines = this._currentState.lines;
        this.createButtonsFromLines();
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
            buttons.Add(button);
        }

        this._currentButtons = buttons;

        return buttons;
    }

    // yes, it is very bad and ugly code. but I can't come up with anything better
    private void initStates() {
        this._allPossibleStates = new List<State> {
            new State(
                "initial state",
                new List<Line> {
                    new Line(
                        "This is the first line of the initial state"
                    ),
                    new Line(
                        "This is the second line of the initial state"
                    )
                }
            ),
            new State(
                "second state",
                new List<Line> {
                    new Line(
                        "This is the first line of the second state"
                    ),
                    new Line(
                        "This is the second line of the second state"
                    )
                }
            ),
            new State(
                "last state",
                new List<Line> {
                    new Line(
                        "This is the end."
                    )
                }
            )
        };

        this._allPossibleStates[0].lines[0].nextState = this._allPossibleStates[1];
        this._allPossibleStates[0].lines[1].nextState = this._allPossibleStates[2];
        this._allPossibleStates[1].lines[0].nextState = this._allPossibleStates[2];
        this._allPossibleStates[1].lines[1].nextState = this._allPossibleStates[0];
    }
}