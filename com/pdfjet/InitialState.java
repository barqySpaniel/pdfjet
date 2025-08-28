package com.pdfjet;

public class InitialState {
    public boolean visible = false;
    public boolean printable = false;
    public boolean exportable = false;

    public InitialState setVisible(boolean visible) {
        this.visible = visible;
        return this;
    }

    public InitialState setPrintable(boolean printable) {
        this.printable = printable;
        return this;
    }

    public InitialState setExportable(boolean exportable) {
        this.exportable = exportable;
        return this;
    }
}
