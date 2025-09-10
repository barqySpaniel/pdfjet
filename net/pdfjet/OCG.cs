using System;

namespace PDFjet.NET {
public class OCG {
    // Fields to hold object number and name
    internal int objNumber;
    internal string name;

    // Constructor to initialize the OCG object with objNumber and name
    internal OCG(int objNumber, string name) {
        this.objNumber = objNumber;
        this.name = name;
    }
}
}

