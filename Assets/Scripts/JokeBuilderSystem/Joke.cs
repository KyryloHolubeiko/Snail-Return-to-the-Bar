using System.Collections;
using System.Collections.Generic;

class Joke {
    private List<Line> _selectedLines = new List<Line>();

    public Joke() {}

    public void addSelectedLine(Line line) {
        this._selectedLines.Add(line);
        line.executeAction();
    }

    public string build() {
        string joke = "";

        foreach (Line line in this._selectedLines) {
            joke += line.text + "\n";
        }

        return joke;
    }

}