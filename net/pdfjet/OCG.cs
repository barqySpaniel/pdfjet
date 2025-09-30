using System;

namespace PDFjet.NET {
public class OCG {
    // Fields to hold object number and name
    internal int objNumber;
    internal String name;
    internal bool locked;

    // Constructor to initialize the OCG object with objNumber and name
    internal OCG(int objNumber, String name, bool locked) {
        this.objNumber = objNumber;
        this.name = name;
        this.locked = locked;
    }
}
}
