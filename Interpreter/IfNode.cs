using System;
using System.Collections.Generic;

namespace rot1
{
    public class IfNode : Node{

        public IfNode(){
            this.children = new Node[3]{new NoOp(), new NoOp(), new NoOp()};
        }

        override public (string,object) Evaluate(SymbolTable symbolTable){
            (string, object) condition = children[0].Evaluate(symbolTable);
            God.VerifyType("boolean",condition);
            if((bool)condition.Item2){
                children[1].Evaluate(symbolTable);
                return("none",null);
            } else {
                children[2].Evaluate(symbolTable);
                return("none",null);
            }
        }
        
    }
}