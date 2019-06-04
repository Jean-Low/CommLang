using System;
using System.Collections.Generic;

namespace rot1
{
    public class Statements : Node{
        public LinkedList<Node> StatementList = new LinkedList<Node>();
        
        override public (string,object) Evaluate(SymbolTable symbolTable){

            foreach(Node statement in StatementList){
                
                statement.Evaluate(symbolTable);
            }
            return("none",null);

        }

        public void Add(Node node){
            StatementList.AddLast(node);
        }
        
    }
}