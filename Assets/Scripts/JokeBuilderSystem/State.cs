using System.Collections;
using System.Collections.Generic;
using System;

public class State {
    public string name;

    public List<Line> lines { get; private set; }
    // conditions idk

    public State(string name, List<Line> lines) {
        this.name = name;
        this.lines = lines;
    }

    public State(List<Line> lines) {
        this.lines = lines;
    }

    public State(string name) {
        this.name = name;
        this.lines = new List<Line>();
    }

    public State() {
        this.lines = new List<Line>();
    }

    public void addLines(List<Line> lines) {
        this.lines.AddRange(lines);
    }
}