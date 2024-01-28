using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class FinishController : MonoBehaviour {
    public GameObject finishStory;
    public Canvas can;

    void Start() {
        this.finishStory.GetComponent<Text>().text = buildUpTheJoke();
        // this.finishStory.GetComponent<RectTransform>().position = new Vector3(
        //                                                 0, 
        //                                                 (-200 + (StaticData.passedStates.Count / 3) * 14),
        //                                                 0);
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("Menu");
        }
    }

    void FixedUpdate() {
        Vector3 oldPosition = this.finishStory.transform.position;

        if ((oldPosition.y - 190) > (StaticData.passedStates.Count * 28 * 4 * 3.5)) {
            // show the restart button
            return;
        }
        Vector3 newPosition = new Vector3(can.GetComponent<RectTransform>().position.x, oldPosition.y + (14f / 60f), 0);

        this.finishStory.transform.position = newPosition;
    }

    private Dictionary<string, string> stateNameToJokeLine = new Dictionary<string, string> {
        {
            "initial",
            "So... Once upon a time... A snail entered a bar."
        },
        {
            "initial bartender",
            "He reached the bartender"
        },
        {
            "asked for drink",
            "and the snail asked for some drink. But the bartender said: \"Hey, snails are not allowed there!\" and kicked him out. Weeks later, the snail came back to the bar and asked the bartender: \"Why did you kick me out?\""
        },
        {
            "initial John",
            "The snail accidentally found a guy who drank so much that he was lying on the floor. The snail tried to talk to him, but he wasn't responding"
        },
        {
            "initial Ken",
            "And the snail noticed that some redhead guy was sitting on a barrel. The snail decided to talk to him, but he was only asking for more drink"
        },
        {
            "StrangeBottle dialogue",
            "Then the snail found a strange red bottle laying on the floor. He took it, made a little drink and suddenly he started to feel pretty strange"
        },
        {
            "Ken FinishLine",
            "The snail gave the weird red bottle to the redhead guy, and he drank it. He started to talk about some weird stuff, and said that the cop sitting next to him is bartender's good friend"
        },
        {
            "cop Investigation",
            "The snail reached the cop sitting at a table and drinking some beer"
        },
        {
            "wc enter",
            "Our snail enter a toilet"
        },
        {
            "wc exit",
            "The the snail left the toilet"
        },
        {
            "dealer found",
            "The snail found a drug dealer, Carl, sitting at a toilet and doing some weird stuff"
        },
        {
            "drug request",
            "The cop asked our snail to help him with investigation. The snail had to find someone who sells drugs in the bar"
        },
        {
            "cop became gay",
            "The snail took the drugs from Carl and gave them to the cop as evidence. The cop was very happy about that and even changed his costume. Then he told the snail that the bartender really loves good jokes"
        },
        {
            "pistol investigation",
            "The snail noticed a gun laying on a table near the unconscious guy"
        },
        {
            "gun take",
            "The snail decided to take the gun"
        },
        {
            "taken with gun",
            "The cop noticed that the snail has a gun and arrested him"
        },
        {
            "John awakened",
            "The guy who was lying on the floor, John, asked the snail to kill him. Snail agreed and shot him. Then John said that now he feels much better and told a few good jokes to the snail"
        },
        {
            "stupid joke",
            "The snail told a stupid joke to the bartender. The bartender was very angry and kicked the snail out of the bar"
        },
        {
            "good joke",
            "The snail told a good joke to the bartender. The bartender was very happy and finally let the snail drink some beer"
        },
        {
            "called the cop gay",
            "The snail asked the cop: 'Why are you gay?' The cop was very angry and decided to kill the snail. Just in case."
        }
    };
    
    public string buildUpTheJoke() {
        List<State> passedStates = StaticData.passedStates;

        string joke = this.stateNameToJokeLine["initial"] + "\n\n";

        foreach (State state in passedStates) {
            if (stateNameToJokeLine.ContainsKey(state.name)) {
                joke += stateNameToJokeLine[state.name] + "\n";
            }
        }

        joke += "The End. \n\n Your score: " + calculateScore().ToString() + "\n\n";

        return joke;
    }

    public int calculateScore() {
        List<State> passedStates = StaticData.passedStates;

        int score = passedStates.Count * 100;
        System.Random random = new System.Random();

        score += random.Next(0, 100);

        return score;
    }
}