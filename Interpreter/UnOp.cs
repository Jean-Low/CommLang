using System;
using System.Collections.Generic;

namespace rot1
{
    public class UnOp : Node{
        
        public UnOp(){
            this.children = new Node[1]{new NoOp()};
        }

        override public (string,object) Evaluate(SymbolTable symbolTable){
            (string,object) ret = children[0].Evaluate(symbolTable);
            switch (value){
                case '+':
                    God.VerifyType("integer",ret);
                    return ret;
                case '-':
                    God.VerifyType("integer",ret);
                    return ("integer", -(int)ret.Item2);
                case "not":
                    God.VerifyType("boolean",ret);
                    return ("boolean", !(bool)ret.Item2);
                case "print":
                    Console.WriteLine(children[0].Evaluate(symbolTable).Item2);
                    return ("none",null);
                case "input":
                    Console.Write("input:");
                    string inputed = Console.ReadLine();
                    if(inputed.ToUpper() == "TRUE"){
                        return ("boolean",true);
                    }
                    if(inputed.ToUpper() == "FALSE"){
                        return ("boolean",false);
                    }
                    if(int.TryParse(inputed,out int parsed)){
                        return ("integer",parsed);
                    }
                    throw new SystemException ($"Unparsable input. {parsed}.");
                case "false":
                    return ("boolean",false);
                case "true":
                    return ("boolean",true);
                default:
                    throw new SystemException ($"Invalid operation. cannot make unary op on variable of type: {ret.Item1}.");
            }
            
            
        }
    }
}