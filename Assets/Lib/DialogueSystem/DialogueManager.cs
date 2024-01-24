using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DialogueManager : MonoBehaviour
{
    public Text npcNameText;
    public Text dialogueText;
    public TextSettings textSettings;
    public GameObject dialogueBox;
    public GameManager gameManager;

    public Animator animator;

    private Queue<string> sentences;
    private Queue<string[]> options;

    private Font font;
    private int fontSize;
    private Color normalColor;
    private Color hoverColor;
    private Color pressedColor;
    private Color disabledColor;
    private Canvas canvas;
    private List<GameObject> createdButtons = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
        options = new Queue<string[]>();

        font = textSettings.font;
        fontSize = textSettings.fontSize;
        normalColor = textSettings.fontColor;
        hoverColor = textSettings.fontColorHover;
        pressedColor = textSettings.fontColorPressed;
        disabledColor = textSettings.fontColorDisabled;

        this.canvas = this.transform.parent.GetComponent<Canvas>();
    }

    public void StartDialogue (Dialogue dialogue)
    {
        animator.SetBool("isOpen", true);

        npcNameText.text = dialogue.npcName;

        sentences.Clear();

        List<List<string>> orderedOptions = dialogue.OrderOptionsBySentenceIndex();

        for (int i = 0; i < dialogue.sentences.Length; i++)
        {
            sentences.Enqueue(dialogue.sentences[i]);
            options.Enqueue(orderedOptions[i].ToArray());
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence ()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence (string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }

        yield return StartCoroutine(ShowOptions(
            this.options.Dequeue()
        ));
    }

    IEnumerator ShowOptions (string[] options)
    {
        if (this.createdButtons.Count > 0) {
            foreach (GameObject button in this.createdButtons) {
                Destroy(button);
            }
            this.createdButtons.Clear();
        }

        for (int i = 0; i < options.Length; i++)
        {
            string option = options[i];

            GameObject buttonObject = new GameObject("Option for sentence " + dialogueText.text + " - " + option);
            // buttonObject.transform.SetParent(this.canvas.transform, false);

            buttonObject.AddComponent<Text>();
            buttonObject.GetComponent<Text>().text = option;
            buttonObject.GetComponent<Text>().font = this.font;
            buttonObject.GetComponent<Text>().fontSize = this.fontSize;
            buttonObject.GetComponent<Text>().color = this.normalColor;

            buttonObject.AddComponent<TextButton>();
            buttonObject.GetComponent<TextButton>().onClick = () => {
                Debug.Log("Clicked " + option);
                
                if (dialogue.nextState > -1 && this.gameManager != null) {
                    this.gameManager.triggerNextState(dialogue.nextState);
                }

                if (dialogue.nextDialogueIndex > -1) {
                    this.sentences.Clear();
                    this.options.Clear();

                    this.sentences.Enqueue(dialogue.sentences[dialogue.nextDialogueIndex]);
                    this.options.Enqueue(dialogue.options[dialogue.nextDialogueIndex]);
                }
                else {
                    this.EndDialogue();
                }
            };
            buttonObject.GetComponent<TextButton>().NormalColor = this.normalColor;
            buttonObject.GetComponent<TextButton>().HoverColor = this.hoverColor;
            buttonObject.GetComponent<TextButton>().PressColor = this.pressedColor;
            buttonObject.GetComponent<TextButton>().DisabledColor = this.disabledColor;

            buttonObject.transform.SetParent(this.dialogueBox.transform, false);
            Vector3 position = buttonObject.GetComponent<RectTransform>().position;
            position.y -= (60 / options.Length) * i + 1;
            buttonObject.transform.position = position;

            buttonObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 372f / 1.5f);
            buttonObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 20f);

            buttonObject.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0f);
            buttonObject.GetComponent<RectTransform>().anchorMax = new Vector2(0f, 1f);
            buttonObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

            this.createdButtons.Add(buttonObject);

            yield return new WaitForSeconds(0.5f);
        }
    }

    void EndDialogue()
    {
        animator.SetBool("isOpen", false);
    }
}
