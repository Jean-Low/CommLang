using System;
using System.Collections.Generic;

namespace rot1
{
    public class FuncDec : Node{
        public LinkedList<Node> varList = new LinkedList<Node>();
        public string type; //none or a var type (integer, bool)
        public FuncDec(){
            this.children = new Node[1] {new NoOp()};
        }
        
        override public (string,object) Evaluate(SymbolTable symbolTable){

            //one set to dim the func, the other one assign it
            symbolTable.Set(this.value.ToString(), null, "function");
            symbolTable.Set(this.value.ToString(), this, "function");
            return("none",null);

        }

        public void Add(BinOp node){
            if(node.value.ToString() != "vardec"){
                throw new SystemException ($"Function declaration vardec node is not a vardec. received a {node.value.ToString()}. [Line: {Parser.CurrentLine}]");
            }
            varList.AddLast(node);
        }
        
    }
}