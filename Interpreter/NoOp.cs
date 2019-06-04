using System;
using System.Collections.Generic;

namespace rot1
{
    public class NoOp : Node{
        override public (string,object) Evaluate(SymbolTable symbolTable){
            return ("none",null);
        }
    }
}