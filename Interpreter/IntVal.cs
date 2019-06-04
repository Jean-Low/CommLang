using System;
using System.Collections.Generic;

namespace rot1
{
    public class IntVal : Node{
        override public (string,object) Evaluate(SymbolTable symbolTable){
            return ("integer",(int) value);
            
        }
    }
}