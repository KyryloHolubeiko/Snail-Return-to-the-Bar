public class Line : IStateAction {
    public string text;
    public State nextState { get; set; }
    public string type { get { return "Line"; } }

    private ILineAction _action = null;

    public Line(string text, ILineAction action, State nextState) {
        this.text = text;
        this._action = action;
        this.nextState = nextState;
    }

    public Line(string text, ILineAction action) {
        this.text = text;
        this._action = action;
    }

    public Line(string text) {
        this.text = text;
    }

    public void executeAction() {
        if (this._action != null) this._action.Execute();
    }
}