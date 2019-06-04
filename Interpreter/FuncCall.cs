using System;
using System.Collections.Generic;

namespace rot1
{
    public class FuncCall : Node{
        public LinkedList<Node> varList = new LinkedList<Node>();
        
        override public (string,object) Evaluate(SymbolTable symbolTable){

            //get func node
            FuncDec node = (FuncDec) symbolTable.Get((string)this.value).Item2;

            //create escope for inferior tables
            SymbolTable inferiorST = new SymbolTable();
            inferiorST.parent = symbolTable;
            
            //create a var for return
            inferiorST.Set("Return",null,node.type);



            //get number of arguments
            if(node.varList.Count != varList.Count){
                throw new SystemException ($"Function {node.value.ToString()} receives {node.varList.Count} arguments, but {varList.Count} where given. [Line: {Parser.CurrentLine}]");
            }
            Node[] valueNodeList = new Node[varList.Count]; 

            int counter = 0;
            foreach(Node n in varList){
                valueNodeList[counter++] = n;
            }

            //dec vars and set values
            counter = 0;
            foreach(BinOp dec in node.varList){

                dec.Evaluate(inferiorST);

                (string, object) retVal = valueNodeList[counter++].Evaluate(symbolTable);

                inferiorST.Set((string)dec.children[0].value, retVal.Item2 , retVal.Item1);
            }

            //call function
            node.children[0].Evaluate(inferiorST);

            if(node.type == "none"){
                return("none",null);
            }

            //return the value by the name
            return(inferiorST.Get("Return"));

        }

        public void Add(Node node){
            varList.AddLast(node);
        }
        
    }
}