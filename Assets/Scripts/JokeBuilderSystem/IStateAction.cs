/*
    not the best name
    every state must contain sort of action
    that leads to the next state
    it can be a line, a gameObject with trigger, a button (in-game), etc.
    e.g. Line class inherits from IStateAction
*/

public interface IStateAction {
    string type { get; }
    State nextState { get; set; }
    void executeAction();
}