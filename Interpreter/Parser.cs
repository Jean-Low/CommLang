using System;
using System.Collections.Generic;


namespace rot1
{
    public static class Parser
    {
        public static Tokenizer tokens; //token queue 
        public static int CurrentLine;

        public static void Expect(string expected,bool skip){
            if(tokens.actual.type != expected){
                    God.Error($"Expecting a {expected} token");
                }
            if(skip)
                tokens.SelectNext();
        }

        public static Node ParseProgram(){
            //Diagram rules
            //1st -> SubDec | FuncDec
            //SubDec | FuncDec -> SubDec | FuncDec
            //SubDec | FuncDec -> EOF

            Statements root = new Statements();

            /*

            while(tokens.actual.type != "EOF"){
                if(tokens.actual.type == "SUB" || tokens.actual.type == "FUNCTION"){
                    root.Add(ParseFunction());
                }
                else {
                    if(tokens.actual.type == "LINEBREAK"){
                        CurrentLine++;
                        tokens.SelectNext();
                    } else 
                        throw new SystemException ($"Statement line outside functions are not allowed. Got a {tokens.actual.type}.(position {tokens.position}) [Line: {CurrentLine}]");
                }
            }

            */




            //TODO- treat func declarations

            //parse the statements
            while(tokens.actual.type != "EOF"){
                root.Add(ParseStatements("main"));
                Expect("PERIOD",true);
            }
            
            /*
            //add a main funccall
            FuncCall mainCall = new FuncCall();
            mainCall.value = "Main";
            root.Add(mainCall);
            */

            return root;
        }

        public static Node ParseFunction(){


            FuncDec root = new FuncDec();

            Expect("IDENTIFIER",false);
            root.value = tokens.actual.value;
            tokens.SelectNext();
            
            if(tokens.actual.type == "ASK"){
                tokens.SelectNext();
                Expect("FOR",true);
                do{
                    if(tokens.actual.type == "COMMA"){
                        tokens.SelectNext();
                    }
                    Expect("IDENTIFIER",false);
                    BinOp dim = new BinOp();
                    dim.value = "vardec";
                    dim.children[0] = new NoOp();
                    dim.children[0].value = (string)tokens.actual.value;
                    tokens.SelectNext();

                    root.Add(dim);

                } while(tokens.actual.type == "COMMA");

                Expect("THEN",true);
            }

            Expect("DO",true);

            root.children[0] = ParseStatements("func");

            return root;

/*

            return root;

            bool sub = tokens.actual.type == "SUB";

            if(sub){
                Expect("SUB",true);
                root.type = "none";
            } else {
                Expect("FUNCTION",true);;
            }

            Expect("IDENTIFIER",false);
            root.value = tokens.actual.value;
            tokens.SelectNext();

            Expect("POPEN",true);
            if(tokens.actual.type != "PCLOSE"){
                do{
                    if(tokens.actual.type == "COMMA"){
                        tokens.SelectNext();
                    }
                    Expect("IDENTIFIER",false);
                    BinOp dim = new BinOp();
                    dim.value = "vardec";
                    dim.children[0] = new NoOp();
                    dim.children[0].value = (string)tokens.actual.value;
                    tokens.SelectNext();

                    Expect("AS",true);

                    dim.children[1] = parseType();

                    root.Add(dim);

                } while(tokens.actual.type == "COMMA");
            }
            Expect("PCLOSE",true);

            if(!sub){
                Expect("AS",true);
                switch(tokens.actual.type){
                    case "INTEGER":
                        root.type = "integer";
                        break;
                    case "BOOLEAN":
                        root.type = "boolean";
                        break;
                    default:
                        throw new SystemException ($"Expecting a VARTYPE token, got a {tokens.actual.type} (position {tokens.position}) [Line: {CurrentLine}]");
                }
                tokens.SelectNext();
            }
            Expect("LINEBREAK",true);
            CurrentLine++;
            
            root.children[0] = ParseStatements(sub ? "sub":"func");

            if(sub){
                Expect("SUB",true);
            } else {
                Expect("FUNCTION",true);
            }

            Expect("LINEBREAK",true);
            CurrentLine++;

            
            return root;
            */
        }

        public static Statements ParseStatements(string statementsType){
            //Console.WriteLine("parsing statemwents");
            
            Statements root = new Statements();


            //Completly new statements parser:
            
            root.Add(ParseStatement());
            while(tokens.actual.type != "PERIOD"){
                Expect("THEN",true);
                root.Add(ParseStatement());
            }
            return root;
            //END of completlly new parser

            


            //END check
            bool ended = false;

            

            while(tokens.actual.type != "EOF"){


                /*
                if(tokens.actual.type == "START"){
                    root.Add(ParseStatements());
                }
                 */
                 //remove, tokenizer treats this now
                //If line is empty, skip this line;
                if(tokens.actual.type == "LINEBREAK"){
                    Console.WriteLine("LINEBREAK on 174 should never happen");
                    tokens.SelectNext();
                    CurrentLine++;
                    continue; //double check to see if this does what I think it does!!!
                }
                //remove, maybe leave the ELSE case. Every statements end with a . now
                List<String> exitTokens = new List<String>(){"ELSE","END","WEND"};

                //Return if ELSE line is found
                if(tokens.actual.type == "ELSE"){
                    ended = true;
                    break;
                }
                
                if(!exitTokens.Contains(tokens.actual.type)){
                     root.Add(ParseStatement());
                } else {

                    switch(tokens.actual.type){
                        case "ELSE":
                            if(statementsType != "if"){
                                throw new SystemException ($"Invalid END construction. 'Else' keyword outside if statements (position {tokens.position}) [Line: {CurrentLine}]");
                            }
                            ended = true;
                            break;
                        case "WEND":
                            if(statementsType != "while"){
                                throw new SystemException ($"Invalid END construction. 'Wend' keyword outside while statements (position {tokens.position}) [Line: {CurrentLine}]");
                            }
                            ended = true;
                            break;
                        case "END":
                            if(statementsType != "func" && statementsType != "sub" && statementsType != "if"){
                                throw new SystemException ($"Invalid END construction. 'end' keyword outside funtion statements (position {tokens.position}) [Line: {CurrentLine}]");
                            }
                            ended = true;
                            break;
                    }

                    tokens.SelectNext();
                    if(statementsType == "func" || statementsType == "sub"){
                        break;
                    }
                    
                    if(tokens.actual.type != "LINEBREAK" && tokens.actual.type != "IF" && tokens.actual.type != "EOF" && tokens.actual.type != "SUB" && tokens.actual.type != "FUNCTION"){
                        throw new SystemException ($"Invalid END construction. (position {tokens.position}) [Line: {CurrentLine}]");
                    }
                    if(tokens.actual.type == "LINEBREAK"){
                        tokens.SelectNext();
                        CurrentLine++;
                    }
                    break;
                }
            }
            
            if(ended){
                 return root;
            }
            throw new SystemException ($"END symbol not found, probably missing a correct END line (position {tokens.position}) [Line: {CurrentLine}]");
            
        }

        //public static Node ParseStatements(string poi){
        //    return ParseStatements("none");
       // }

        public static Node ParseStatement(){
            //Console.WriteLine("parsing expression");
            
            Node root;

            //TODO- condense this code by replacing similar code with some functions (expect line end for example)

            switch(tokens.actual.type){
                case "END":
                    return new NoOp();
                //assing, start wuth put
                case "PUT":
                    root = new BinOp();
                    root.value = "=";
                    tokens.SelectNext();

                    root.children[1] = ParseRelExpression();
                    Expect("IN",true);
                    Expect("IDENTIFIER",false);

                    Identifier ident = new Identifier((string)tokens.actual.value);
                    root.children[0] = ident;

                    tokens.SelectNext();

                    return root;

                /*case "IDENTIFIER":
                    // IDENTIFIER - = - EXPRESSION
                    // get identifier
                    Identifier ident = new Identifier((string)tokens.actual.value);
                    tokens.SelectNext();
                    if(tokens.actual.type != "EQUAL"){ //expecting a EQUAL
                        throw new SystemException ($"Identifier with no assignment ({tokens.actual.value}) (position {tokens.position}) [Line: {CurrentLine}]");
                    }
                    root = new BinOp(); //create the assign biop node
                    root.value = '=';
                    root.children[0] = ident; //left is the identfier
                    tokens.SelectNext();
                    root.children[1] = ParseRelExpression(); //right is a relative expression

                    if(tokens.actual.type != "LINEBREAK"){ //expect linebreak after the expression;
                        throw new SystemException ($"No LINEBREAK after assignment (position {tokens.position}) [Line: {CurrentLine}]");
                    }
                    CurrentLine++;
                    tokens.SelectNext(); //go to next token after linebreak
                    return root;*/
                case "DO":
                    tokens.SelectNext();
                    Expect("IDENTIFIER",false);

                    FuncCall call = new FuncCall();
                    call.value = (string)tokens.actual.value;

                    tokens.SelectNext();

                    if(tokens.actual.type == "WITH"){
                        tokens.SelectNext();
                        do{
                            if(tokens.actual.type == "COMMA"){
                                tokens.SelectNext();
                            }
                            call.Add(ParseRelExpression());
                        }while (tokens.actual.type == "COMMA");
                    }
                    return call;
                case "TO":
                    tokens.SelectNext();
                    return ParseFunction();
                /*
                case "CALL":
                    tokens.SelectNext();
                    Expect("IDENTIFIER",false);

                    FuncCall call = new FuncCall();
                    call.value = (string)tokens.actual.value;

                    tokens.SelectNext();
                    Expect("POPEN",true);
                    if(tokens.actual.type != "PCLOSE"){
                        do{
                            if(tokens.actual.type == "COMMA"){
                                tokens.SelectNext();
                            }
                            call.Add(ParseRelExpression());
                        }while (tokens.actual.type == "COMMA");
                    }
                    Expect("PCLOSE",true);
                    return call;
                    */
                case "PRINT":
                    //print - expression
                    root = new UnOp();
                    root.value = "print";
                    tokens.SelectNext();
                    root.children[0] = ParseRelExpression();
                    return root; //cl change
                    /*
                    if(tokens.actual.type != "LINEBREAK"){ //expect linebreak after the expression;
                        throw new SystemException ($"No LINEBREAK after PRINT (position {tokens.position}) [Line: {CurrentLine}]");
                    }
                    CurrentLine++;
                    tokens.SelectNext(); //go to next token after linebreak
                    return root;
                    */
                case "WHILE":
                    root = new WhileNode();
                    tokens.SelectNext();
                    root.children[0] = ParseRelExpression();

                    Expect("DO",true);
                    root.children[1] = ParseStatements("while");
                    
                    Expect("PERIOD",true);

                    return root;

                    /*
                    root = new WhileNode();
                    tokens.SelectNext();
                    root.children[0] = ParseRelExpression();
                    if(tokens.actual.type != "LINEBREAK"){ //expect linebreak after the expression;
                        throw new SystemException ($"No LINEBREAK after Relative Expression (position {tokens.position}) [Line: {CurrentLine}]");
                    }
                    CurrentLine++;
                    tokens.SelectNext(); //go to next token after linebreak
                    root.children[1] = ParseStatements("while"); //already goes to next line, no select next required
                    return root;
                    */
                    /*
                case "DIM":
                    root = new BinOp();
                    root.value = "vardec";
                    tokens.SelectNext();

                    if(tokens.actual.type != "IDENTIFIER"){ 
                        throw new SystemException ($"IDENTIFIER is required after an variable declaration declaration (position {tokens.position}) [Line: {CurrentLine}]");
                    }
                    root.children[0] = new NoOp();
                    root.children[0].value = (string)tokens.actual.value;
                    tokens.SelectNext();

                    if(tokens.actual.type != "AS"){ 
                        throw new SystemException ($"'As' is required in an variable declaration (position {tokens.position}) [Line: {CurrentLine}]");
                    }
                    tokens.SelectNext();

                    root.children[1] = parseType();

                    if(tokens.actual.type != "LINEBREAK"){ //expect linebreak after the expression;
                        throw new SystemException ($"No LINEBREAK after Variable declaration(position {tokens.position}) [Line: {CurrentLine}]");
                    }
                    CurrentLine++;
                    tokens.SelectNext(); //go to next token after linebreak

                    return root;
                    */
                case "IF":
                    root = new IfNode();
                    tokens.SelectNext();
                    root.children[0] = ParseRelExpression();

                    Expect("DO",true);
                    root.children[1] = ParseStatements("if");
                    
                    Expect("PERIOD",true);

                    if(tokens.actual.type == "ELSE"){
                        tokens.SelectNext();
                        root.children[2] = ParseStatements("else");
                    }

                    return root;

                    /*
                    if(tokens.actual.type != "THEN"){ //expect THEN after the rel expression;
                        throw new SystemException ($"THEN is required after an IF's Relative Expression (position {tokens.position}) [Line: {CurrentLine}]");
                    }
                    tokens.SelectNext();

                    if(tokens.actual.type != "LINEBREAK"){ //expect linebreak after THEN;
                        throw new SystemException ($"No LINEBREAK after Relative Expression (position {tokens.position}) [Line: {CurrentLine}]");
                    }
                    CurrentLine++;
                    tokens.SelectNext(); //go to next token after linebreak
                    
                    root.children[1] = ParseStatements("if"); //already goes to next line, no select next required
                    
                    if(tokens.actual.type == "ELSE"){
                        tokens.SelectNext();
                        if(tokens.actual.type != "LINEBREAK"){ //expect linebreak after ELSE;
                            throw new SystemException ($"No LINEBREAK after ELSE (position {tokens.position}) [Line: {CurrentLine}]");
                        }
                        CurrentLine++;
                        tokens.SelectNext(); //go to next token after linebreak
                        root.children[2] = ParseStatements("else"); //left the token after the END, so it is probably on IF (END IF\n)
                    }

                    if(tokens.actual.type != "IF"){ //expect IF after the statements END for END IF\n construction;
                        throw new SystemException ($"IF section end symbol not found, found END instead of END IF (position {tokens.position}) [Line: {CurrentLine}]");
                    }
                    tokens.SelectNext();

                    if(tokens.actual.type != "LINEBREAK"){ //expect linebreak after THEN;
                        throw new SystemException ($"No LINEBREAK after Relative Expression (position {tokens.position}) [Line: {CurrentLine}]");
                    }
                    CurrentLine++;
                    tokens.SelectNext(); //go to next token after linebreak

                    return root; */
                
                default:
                    return new NoOp();
            }

        }

        public static Node parseType(){
            //Console.WriteLine("parsing type");
            //Diagram rules
            //1st - Integer
            //1st - Boolean
            Node root = new NoOp();
            switch(tokens.actual.type){
                case "INTEGER":
                    root.value = "integer";
                    break;
                case "BOOLEAN":
                    root.value = "boolean";
                    break;
                default:
                    throw new SystemException ($"variable type {tokens.actual.type} does not exist (position {tokens.position}) [Line: {CurrentLine}]");
            }
            tokens.SelectNext();
            return root;
        }
        public static Node ParseRelExpression(){
            //Console.WriteLine("parsing relative expression");
            //Diagram rules 
            //1st - expression
            //expression -> equal | higher | lower
            //equal | higher| lower -> expression
            //expression ->
            
            Node root = ParseExpression();

            //TODO make rel expressions
            List<String> allowedTokens = new List<String>(){"IS", "EQUALS", "NOT"};
            
            if(allowedTokens.Contains(tokens.actual.type)){
                Node temp = root;
                root = new BinOp();
                root.children[0]= temp;

                bool not = false;

                if(tokens.actual.type == "IS"){
                    tokens.SelectNext();
                    if(tokens.actual.type == "NOT"){
                        //Node notChild= root;
                        //root = new UnOp();
                        //notNode.value = "not";
                        //root.children[1] = notNode;
                        not = true;
                        tokens.SelectNext();

                    }
                    switch(tokens.actual.type){
                        case "EQUAL":
                            tokens.SelectNext();
                            Expect("TO",true);
                            root.value = "==";
                            break;
                        case "HIGHER":
                            tokens.SelectNext();
                            Expect("THAN",true);
                            root.value = ">";
                            break;
                        case "LOWER":
                            tokens.SelectNext();
                            Expect("THAN",true);
                            root.value = "<";
                            break;
                        default:
                            root.value = "==";
                            break;
                    }
                } else if(tokens.actual.type == "EQUALS"){
                    root.value = "==";
                    tokens.SelectNext();
                } else if(tokens.actual.type == "NOT"){
                    tokens.SelectNext();
                    Expect("EQUALS",true);
                    root.value = "==";
                }

                root.children[1] = ParseExpression();

                //apply not
                if(not){
                    Node oldRoot = root;
                    root = new UnOp();
                    root.value = "not";
                    root.children[0] = oldRoot;
                }
            }

            //if and or or, apply it to new relexp
            if(tokens.actual.type == "AND"){
                tokens.SelectNext();
                Node oldRoot = root;
                root = new BinOp();
                root.value = '&';
                root.children[0] = oldRoot;
                root.children[1] = ParseRelExpression();
            }
            if(tokens.actual.type == "OR"){
                tokens.SelectNext();
                Node oldRoot = root;
                root = new BinOp();
                root.value = '|';
                root.children[0] = oldRoot;
                root.children[1] = ParseRelExpression();
            }


            return root;
        }

            public static Node ParseExpression(){
            //Console.WriteLine("parsing expression");
            //Diagram rules
            //1st - term
            //term -> minus | plus | or
            //term -> EOF
            //minus | plus | or -> term
            
            Node root = ParseTerm();

            //TODO verify if only these three possible operations are expressions
            //list to check if token is valid in a term
            List<String> allowedTokens = new List<String>(){"PLUS","MINUS"};

            while(allowedTokens.Contains(tokens.actual.type)){

                //make a new node, current root is the new node left child
                Node temp = root;
                root = new BinOp();
                root.children[0] = temp;
                switch(tokens.actual.type){
                    case "MINUS":
                        root.value = '-';
                        break;
                    case "PLUS":
                        root.value = '+';
                        break;
                    /*
                    case "OR":
                        root.value = '|';
                        break;
                        */
                }
                tokens.SelectNext();
                root.children[1] = ParseTerm();

            }

            return root;
        }

        public static Node ParseTerm(){

            //Console.WriteLine("Parsing term");
            //Term rules
            //1st - factor
            //factor -> times | div | and
            //factor -> exit term
            //times | div | and -> factor

            //Get first factor
            Node root = ParseFactor();
            
            //TODO check if there 3 are all possibilities
            //list to check if token is valid in a term
            List<String> allowedTokens = new List<String>(){"TIMES","DIVIDED"}; //removed and
            //check if token type is allowed
            while(allowedTokens.Contains(tokens.actual.type)){
                //make a new node, current root is the new node left child
                Node temp = root;
                root = new BinOp();
                root.children[0] = temp;

                switch(tokens.actual.type){
                    case "TIMES":
                        root.value = '*';
                        break;
                    case "DIVIDED":
                        tokens.SelectNext();
                        Expect("BY",false);
                        root.value = '/';
                        break;
                    /*
                    case "AND":
                        root.value = '&';
                        break;
                    */
                }
                tokens.SelectNext();
                root.children[1] = ParseFactor();
            }
            return root;
        }

        public static Node ParseFactor(){
            
            //Console.WriteLine("Parsing Factor");

            //Factor rules
            //1st -> num
            //1st -> +
            //1st -> -
            //1st -> not
            //1st -> (
            //+ -> factor
            //- -> factor
            //not -> factor
            //( -> expression
            //expression -> )
            //identifier acts as num
            //True | False acts as num

            Node root;

            switch(tokens.actual.type){
                    case "INT":
                        root = new IntVal();
                        root.value = tokens.actual.value;
                        tokens.SelectNext();
                        return(root);
                    case "IDENTIFIER":
                        root = new Identifier((string)tokens.actual.value);
                        root.value = tokens.actual.value;
                        tokens.SelectNext();

                        return root;
                        /*
                        //check if this is a function
                        if(tokens.actual.type == "POPEN"){
                            FuncCall call = new FuncCall();
                            call.value = root.value;
                            tokens.SelectNext();

                            if(tokens.actual.type != "PCLOSE"){
                                do{
                                    if(tokens.actual.type == "COMMA"){
                                        tokens.SelectNext();
                                    }
                                    call.Add(ParseRelExpression());
                                } while(tokens.actual.type == "COMMA");
                            }
                            Expect("PCLOSE",true);
                            return call;
                        }
                        */
                    case "TRUE":
                    case "FALSE":
                        root = new UnOp();
                        root.value = tokens.actual.type == "TRUE" ? "true": "false";
                        tokens.SelectNext();
                        return(root);
                    /*
                    case "PLUS":
                    case "MINUS":
                        root = new UnOp();
                        root.value = tokens.actual.type == "PLUS" ? '+': '-';
                        tokens.SelectNext();
                        root.children[0] = ParseFactor();
                        return root;
                    */
                    case "NOT":
                        root = new UnOp();
                        root.value = "not";
                        tokens.SelectNext();
                        root.children[0] = ParseFactor();
                        return root;
                        //way to call func in assigns
                    case "GET":
                        tokens.SelectNext();
                        Expect("IDENTIFIER",false);

                        FuncCall call = new FuncCall();
                        call.value = (string)tokens.actual.value;

                        tokens.SelectNext();

                        if(tokens.actual.type == "WITH"){
                            tokens.SelectNext();
                            do{
                                if(tokens.actual.type == "COMMA"){
                                    tokens.SelectNext();
                                }
                                call.Add(ParseRelExpression());
                            }while (tokens.actual.type == "COMMA");
                        }
                        return call;
                    /*
                    case "POPEN":
                        tokens.SelectNext();
                        root = ParseRelExpression();
                        if(tokens.actual.type != "PCLOSE"){
                            throw new SystemException ($"Missing closing parentesis (position {tokens.position}) [Line: {CurrentLine}]");
                        }
                        tokens.SelectNext();
                        return root;
                        */
                    case "INPUT":
                        root = new UnOp();
                        root.value = "input";
                        tokens.SelectNext();
                        return(root);
                }

            //End of term reached, but exiting was not allowed
            God.Error($"Invalid expression format. Expression end was unexpected.");
            return null;
        }
        
        public static int Run(string code, bool debug){
            
            
            SymbolTable table = new SymbolTable();
            God.godSt = table;

            tokens = new Tokenizer();
            tokens.origin = code;

            tokens.SelectNext(); //initialize a token
            CurrentLine = 1;

            Console.WriteLine("Now parsing!");

            Node root = ParseProgram(); //pass main flag to tell it is the main's statements

            Console.WriteLine("PARSED!\nNow evaluating!\n");
            root.Evaluate(table);

            return(1);
        }
    }
}