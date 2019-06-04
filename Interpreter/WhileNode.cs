using System;
using System.Collections.Generic;

namespace rot1
{
    public class WhileNode : Node{

        public WhileNode(){
            this.children = new Node[2]{new NoOp(), new NoOp()};
        }

        override public (string,object) Evaluate(SymbolTable symbolTable){
            
            while(whileCheck(symbolTable)){
                children[1].Evaluate(symbolTable);
            }
            return ("none",null);
        }

        bool whileCheck (SymbolTable symbolTable) {
            (string,object) ret = children[0].Evaluate(symbolTable);
            God.VerifyType("boolean",ret);
            return (bool) ret.Item2;
        }

    }
}