%{
#include <stdio.h>
#include <stdlib.h>
extern int yylex();
extern int yyparse();
extern FILE* yyin;
void yyerror(const char* s);
%}

%union {
	int ival;
	char *idval;
}

%token<ival> TINT
%token<idval> TIDENTIFIER

%token tk_put tk_swap
%token tk_in tk_by tk_for tk_is tk_then tk_than tk_do
%token tk_to tk_get tk_with tk_ask
%token tk_print tk_input
%token tk_not tk_and tk_or
%token tk_plus tk_minus tk_times tk_divided
%token tk_if tk_while tk_else
%token tk_higher tk_lower tk_equal tk_equals
%token tk_true tk_false
%token tk_popen tk_pclose tk_comma tk_period
%token tk_placeholder

%start program

%%

program: // ex: statements . statements . statements .
         statements tk_period program
	   | statements tk_period
;

statements: 
         statement tk_then statements
       | statement  
;

statement:
      tk_put relExpressionComp tk_in TIDENTIFIER                                        //put a in b
    | tk_do TIDENTIFIER                                                                 //do func
    | tk_do TIDENTIFIER tk_with arguments                                               //do func with a,b,c
    | tk_to function                                                                    //to func...
    | tk_print relExpressionComp                                                        //print a + 2
    | tk_while relExpressionComp tk_do statements                                       //while a < 2 do print a .
    | tk_if relExpressionComp tk_do statements                                          //if a > 10 do print True
    | tk_if relExpressionComp tk_do statements tk_period tk_else statements             //if a > 10 do print True . else print False 
;

function:
      TIDENTIFIER tk_do statements
    | TIDENTIFIER tk_ask tk_for argumentDeclaration tk_then tk_do statements
;

argumentDeclaration:
      TIDENTIFIER
    | TIDENTIFIER tk_comma argumentDeclaration
;

relExpressionComp:
      relExpression
    | relExpression tk_and relExpression
    | relExpression tk_or relExpression
;

relExpression:
      expression
    | expression tk_is expression
    | expression tk_is tk_not expression
    
    | expression tk_is tk_equal tk_to  expression
    | expression tk_is tk_not tk_equal tk_to expression
    
    | expression tk_is tk_higher tk_than expression
    | expression tk_is tk_not tk_higher tk_than expression
    
    | expression tk_is tk_lower tk_than expression
    | expression tk_is tk_not tk_lower tk_than expression
    
    | expression tk_equals expression
    | expression tk_not tk_equals expression
    
    
;

expression:
      term
    | term tk_plus expression
    | term tk_minus expression
;

term:
      factor
    | factor tk_times term
    | factor tk_divided tk_by term
;

factor:
      TINT
    | TIDENTIFIER
    | tk_true
    | tk_false
    | tk_not factor
    | tk_get TIDENTIFIER
    | tk_get TIDENTIFIER tk_with arguments
    | tk_input
;

arguments:
      relExpression 
    | relExpression tk_comma arguments
;

%%
int main() {
	yyin = stdin;
	do {
		yyparse();
	} while(!feof(yyin));
	return 0;
}
void yyerror(const char* s) {
	fprintf(stderr, "Parse error: %s\n", s);
	exit(1);
}
