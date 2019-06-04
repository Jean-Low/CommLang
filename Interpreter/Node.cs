using System;
using System.Collections.Generic;

namespace rot1
{
    public abstract class Node//T is value type, U is return type
    {
        //generic node field
        public Node[] children;
        public Object value;
        public abstract (string,object) Evaluate(SymbolTable symbolTable);
    }
    
}