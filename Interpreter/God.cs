using System;
using System.Collections.Generic;

namespace rot1
{
    public class God
    {
        public static int line = 1;
        public static int posDelta = 0;

        public static SymbolTable godSt;

        public static int getCursor(){
            return (Parser.tokens.position - posDelta);
        }

        public static void Error(string message){
            throw new SystemException($"{message} [line {line}, pos {getCursor()}]");
        }
        public static void VerifyType(string type,(string,object) ret){
            if(type != ret.Item1){
                throw new SystemException ($"Invalid variable type! expecting '{type}' but received '{ret.Item1}'.");
            }
        }
    }
    
}