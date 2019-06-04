using System;
using System.Collections.Generic;

namespace rot1
{
    public class BinOp : Node{

        public BinOp(){
            this.children = new Node[2]{new NoOp(), new NoOp()};
        }

        override public (string,object) Evaluate(SymbolTable symbolTable){
            if(value.ToString() == "="){
                    (string,object) val = children[1].Evaluate(symbolTable);
                    symbolTable.Set((string)children[0].value ,val.Item2, val.Item1);
                    return("none",null);
            }

            (string, object) ret1 = children[0].Evaluate(symbolTable);
            (string, object) ret2 = children[1].Evaluate(symbolTable);

            switch ( value.ToString() ){
                case "+":
                    God.VerifyType("integer",ret1);
                    God.VerifyType("integer",ret2);
                    return ("integer", ((int)ret1.Item2 + (int)ret2.Item2));
                case "-":
                    God.VerifyType("integer",ret1);
                    God.VerifyType("integer",ret2);
                    return ("integer", ((int)ret1.Item2 - (int)ret2.Item2));
                case "*":
                    God.VerifyType("integer",ret1);
                    God.VerifyType("integer",ret2);
                    return ("integer", ((int)ret1.Item2 * (int)ret2.Item2));
                case "/":
                    God.VerifyType("integer",ret1);
                    God.VerifyType("integer",ret2);
                    return ("integer", ((int)ret1.Item2 / (int)ret2.Item2));
                case ">":
                    God.VerifyType("integer",ret1); 
                    God.VerifyType("integer",ret2);
                    return ("boolean", ((int)ret1.Item2 > (int)ret2.Item2));
                case "<":
                    God.VerifyType("integer",ret1);
                    God.VerifyType("integer",ret2);
                    return ("boolean", ((int)ret1.Item2 < (int)ret2.Item2));
                case "&":
                    God.VerifyType("boolean",ret1);
                    God.VerifyType("boolean",ret2);
                    return ("boolean", ((bool)ret1.Item2 && (bool)ret2.Item2));
                case "|":
                    God.VerifyType("boolean",ret1);
                    God.VerifyType("boolean",ret2);
                    return ("boolean", ((bool)ret1.Item2 || (bool)ret2.Item2));
                case "==":
                    if(ret1.Item1 != ret2.Item1){
                        throw new SystemException ($"Invalid relative operation == between variables of types {ret1.Item1} and {ret2.Item1}.");
                    }
                    switch(ret1.Item1){
                        case "boolean":
                            return ("boolean", (bool)ret1.Item2 == (bool)ret2.Item2);
                        case "integer":
                            return ("boolean", (int)ret1.Item2 == (int)ret2.Item2);
                        default:
                            throw new SystemException ($"Invalid variable type for == operation. {ret1.Item1}.");
                    }
                    
                case "vardec":
                    //get value of type node and interact with ident node to sabe the var type
                    //string type = (string)children[1].value;
                    string key = (string)children[0].value;

                    symbolTable.Set(key,null,"none");

                    return ("none",null);
                    
                default:
                    throw new SystemException ($"Invalid Binary Operator ( {value} was received at node on integer operation )");
            }
        }
        
    }
}