using System.Collections;
using System.Collections.Generic;
using System;

public class State {
    public string name;
    Action onEnterAction;

    public List<IStateAction> actions { get; set; }
    // conditions idk

    public State(string name, List<IStateAction> actions, Action onEnter) {
        this.name = name;
        this.actions = actions;
        this.onEnterAction = onEnter;
    }

    public State(List<IStateAction> actions, Action onEnter) {
        this.actions = actions;
        this.onEnterAction = onEnter;
    }

    public State(string name, Action onEnter) {
        this.name = name;
        this.actions = new List<IStateAction>();
        this.onEnterAction = onEnter;
    }

    public State(Action onEnter) {
        this.actions = new List<IStateAction>();
        this.onEnterAction = onEnter;
    }

    public State() {
        this.actions = new List<IStateAction>();
    }

    public void addActions(List<IStateAction> actions) {
        this.actions.AddRange(actions);
    }

    public void onEnter() {
        if (this.onEnterAction != null) this.onEnterAction();
    }

    public void setOnEnter(Action onEnter) {
        this.onEnterAction = onEnter;
    }
}