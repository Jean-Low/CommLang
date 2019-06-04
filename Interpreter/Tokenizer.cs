using System;
using System.Collections.Generic;
using System.Linq;

namespace rot1
{
    public class Tokenizer
    {
        public string origin; //source string
        public int position = 0; //actual token separation pos
        public Token actual; //last parsed token
        public int discard = 0; //discard anything outside a C comment (0 - discard, 1 - dont until end of line, 2 - dont until */ )
        public bool pcomment = false; //check if inside a parenthesis for comments

        /*private List<String> reservedWords = new List<String>(){"END","WEND","PRINT","INPUT","IF","ELSE","THEN","WHILE","SUB",
                                                                "INTEGER","BOOLEAN","DIM","AS","TRUE","FALSE","AND","OR","NOT",
                                                                "FUNCTION","CALL"};
                                                                */


        private List<String> reservedWords = new List<String>(){
        "PUT", "SWAP",
        "IN", "BY", "FOR", "IS", "THEN", "TO", "THAN", "DO",
        "TO", "GET", "WITH", "ASK", 
        "PRINT", "INPUT",
        "NOT", "AND", "OR",
        "PLUS", "MINUS", "TIMES", "DIVIDED",
        "IF", "WHILE", "ELSE", 
        "HIGHER", "LOWER", "EQUAL", "EQUALS", 
        "TRUE", "FALSE"};
        public Token SelectNext(){
            //Console.WriteLine("next token");
            Token ret =  new Token();
            string tokenizable = "";
            while(true){

                //If origin got to the end or comment starter (') detected
                //TODO rethink this!!!
                if(position >= origin.Length){
                    if(tokenizable.Length > 0){
                        God.Error("Unexpected EOF.");
                    }
                    ret.type = "EOF";
                    actual = ret;
                    return ret;
                }
                //end TODO

                if(discard == 0){
                    if(origin[position] == '/'){
                        if(origin[++position] == '/'){
                            discard = 1;
                        } else if(origin[position] == '*'){
                            discard = 2;
                        }
                    } else if(origin[position] == '\n'){
                        God.line++; 
                        God.posDelta = position;
                    }
                    position++;
                    continue;
                }

                if(pcomment) {
                    if(origin[position] == ')'){
                        pcomment = false;
                    }
                    position++;
                    continue;
                }

                //is it starting a comment with ()
                if(origin[position] == '('){
                    pcomment = true;
                    position++;
                    continue;
                }

                //C comment escaped?
                if(origin[position] == '\n'){
                    if(discard == 1){
                        discard = 0;
                    }
                    God.line++; 
                    God.posDelta = position;
                    position ++;
                    continue;
                }

                if(origin[position] == '*'){
                    if(origin[++position] == '/'){
                        if(discard == 2){
                            discard = 0;
                        } else {
                            God.Error("Unexpected token *.");
                        }
                    }
                    continue;
                }
                

                //Handle sign case
                if("(),.".Contains(origin[position])){
                    if(tokenizable.Length == 0){ //token is a sign or simbol
                        switch(origin[position]){
                            case '(':
                                ret.type = "POPEN";
                                break;
                            case ')':
                                ret.type = "PCLOSE";
                                break;
                            case ',':
                                ret.type = "COMMA";
                                break;
                            case '.':
                                ret.type = "PERIOD";
                                break;
                        }
                        position++;
                        actual = ret;
                        return ret;
                    }
                    //token was a number or ident and ended in a sign (so the sign is not included and the token ended)
                    if(ret.type == "IDENTIFIER"){
                        //same as identifier ending in space
                        string upToken = tokenizable.ToUpper();
                        if(reservedWords.Contains(upToken)){
                            ret.type = upToken;
                            actual = ret;
                            return ret;
                        }
                        ret.type = "IDENTIFIER";
                        ret.value = tokenizable;
                        actual = ret;
                        return ret;
                    } else {
                        int value;
                        if(int.TryParse(tokenizable, out value)){
                            ret.type = "INT";
                            ret.value = value;
                            actual = ret;
                            return actual;
                        } else {
                            God.Error("Unexpected not parseble integer found.");
                        }
                    }
                }

                //char is part of a number
                if(int.TryParse(origin[position].ToString(), out int dummy)) { //not +|- and is a number;
                    tokenizable += origin[position];
                    position++;
                }

                //char is a letter (can be identifier, space or a invalid char token)
                else { 
                    if(origin[position] == ' ' || origin[position] == '\t'){ //is a space?
                        if(tokenizable.Length == 0){ //ignore space
                            position++;
                        } else { //handle end of ident or number token
                            //v2.1: Bug where a number is considered a identifier if there is a space after it. doublechecking it here
                            if(int.TryParse(tokenizable, out int value)){
                                ret.type = "INT";
                                ret.value = value;
                                actual = ret;
                                return actual;
                            }
                            string upToken = tokenizable.ToUpper();
                            if(reservedWords.Contains(upToken)){
                                ret.type = upToken;
                                actual = ret;
                                return ret;
                            }
                            //check if word is reserved
                            ret.type = "IDENTIFIER";
                            ret.value = tokenizable;
                            actual = ret;
                            return ret;
                        }
                        
                    } else if(tokenizable.Length == 0){ //start with letter or '_'
                        if(Char.IsLetter(origin[position]) || origin[position] == '_'){
                            ret.type = "IDENTIFIER";
                            tokenizable+= origin[position];
                            position++;
                        } else {
                            God.Error("Unexpected token found, identifiers should start with '_' or a letter.");
                        }
                    } else{ //token exist, found found chars after letter or '_'
                        if(ret.type == "IDENTIFIER"){
                            tokenizable+= origin[position];
                            position++;
                        } else {
                            God.Error("Unexpected token found, unknow char at position.");
                        }
                    }
                    
                }
                /* (V1.1.1) (Changed to last two if statements. Now throws an error if invalid char to showcase comment feature better)
                else { //if this branch runs, this char is not in my alphabet, so it will be ignored;
                    position++;
                }
                */

            }
        }

    }
}