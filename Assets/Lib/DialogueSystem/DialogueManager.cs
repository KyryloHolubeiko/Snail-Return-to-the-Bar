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
    private Queue<Dialogue.option[]> options;

    private Font font;
    private int fontSize;
    private Color normalColor;
    private Color hoverColor;
    private Color pressedColor;
    private Color disabledColor;
    private Canvas canvas;
    private List<GameObject> createdButtons = new List<GameObject>();

    private Dialogue _currentDialogue;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
        options = new Queue<Dialogue.option[]>();

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
        options.Clear();

        List<List<Dialogue.option>> orderedOptions = dialogue.OrderOptionsBySentenceIndex();

        for (int i = 0; i < dialogue.sentences.Count; i++)
        {
            sentences.Enqueue(dialogue.sentences[i]);
            options.Enqueue(orderedOptions[i].ToArray());
        }

        this._currentDialogue = dialogue;

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

    IEnumerator ShowOptions (Dialogue.option[] options)
    {
        if (this.createdButtons.Count > 0) {
            foreach (GameObject button in this.createdButtons) {
                Destroy(button);
            }
            this.createdButtons.Clear();
        }

        for (int i = 0; i < options.Length; i++)
        {
            Dialogue.option option = options[i];

            GameObject buttonObject = new GameObject("Option for sentence " + dialogueText.text + " - " + option);
            // buttonObject.transform.SetParent(this.canvas.transform, false);

            buttonObject.AddComponent<Text>();
            buttonObject.GetComponent<Text>().text = "> " + option.optionText;
            buttonObject.GetComponent<Text>().font = this.font;
            buttonObject.GetComponent<Text>().fontSize = this.fontSize;
            buttonObject.GetComponent<Text>().color = this.normalColor;

            buttonObject.AddComponent<TextButton>();
            buttonObject.GetComponent<TextButton>().onClick = () => {
                this.createdButtons.ForEach(button => Destroy(button));
                
                if (option.nextState != "-1" && this.gameManager != null) {
                    this.gameManager.triggerNextState(option.nextState);
                }

                if (option.nextDialogueIndex > -1) {
                    this.sentences.Clear();
                    this.options.Clear();

                    this.sentences.Enqueue(_currentDialogue.sentences[option.nextDialogueIndex]);
                    this.options.Enqueue(_currentDialogue.OrderOptionsBySentenceIndex(option.nextDialogueIndex)[0].ToArray());

                    string sentence = _currentDialogue.sentences[option.nextDialogueIndex];
                    StopAllCoroutines();
                    StartCoroutine(TypeSentence(sentence));
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
            position.y -= 25 * i + 1;
            buttonObject.transform.position = position;

            buttonObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 900);
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
        if (this.gameManager != null) {
            this.gameManager.inDialogue = false;
        }

        this.createdButtons.ForEach(button => Destroy(button));
    }
}
