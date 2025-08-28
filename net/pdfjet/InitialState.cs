using System;
using System.Collections.Generic;

namespace PDFjet.NET {
public class InitialState {
    public bool visible = true;
    public bool printable = false;
    public bool exportable = false;

    public InitialState SetVisible(bool visible) {
        this.visible = visible;
        return this;
    }

    public InitialState SetPrintable(bool printable) {
        this.printable = printable;
        return this;
    }

    public InitialState SetExportable(bool exportable) {
        this.exportable = exportable;
        return this;
    }
}
}