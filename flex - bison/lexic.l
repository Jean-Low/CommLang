%{
#include <string>
#include "node.h"
#include "parser.hpp"
#define SAVE_TOKEN yylval.string = new std::string(yytext, yyleng)
#define TOKEN(t) (yylval.token = t)
extern "C" int yywrap() { }
%}

%%

"put"					{return tk_put;}
"swap"					{return tk_swap;}

"in"                    {return tk_in;}
"by"                    {return tk_by;}
"for"                   {return tk_for;}
"is"                    {return tk_is;}
"then"                  {return tk_then;}
"than"                  {return tk_than;}
"do"                    {return tk_do;}

"to"                    {return tk_to;}
"get"                   {return tk_get;}
"with"                  {return tk_with;}
"ask"                   {return tk_ask;}

"print"                 {return tk_print;}
"input"                 {return tk_input;}

"not"                   {return tk_not;}
"and"                   {return tk_and;}
"or"                    {return tk_or;}

"plus"                  {return tk_plus;}
"minus"                 {return tk_minus;}
"times"                 {return tk_times;}
"divided"               {return tk_divided;}

"if"                    {return tk_if;}
"while"                 {return tk_while;}
"else"                  {return tk_else;}

"higher"                {return tk_higher;}
"lower"                 {return tk_lower;}
"equal"                 {return tk_equal;}
"equals"                {return tk_equals;}

"true"                  {return tk_true;}
"false"                 {return tk_false;}

"("                     {return tk_popen;}
")"                     {return tk_pclose;}
","                     {return tk_comma;}
"."                     {return tk_period;}

[ \t\n]                 ; //ignore whitespaces and linebreaks
[a-zA-Z_][a-zA-Z0-9_]*  SAVE_TOKEN; return TIDENTIFIER;
[0-9]+.[0-9]+           SAVE_TOKEN; return TFLOAT;
[0-9]+                  SAVE_TOKEN; return TINT;
0x[0-9a-f]+             SAVE_TOKEN; return THEXA; //right?

.                       printf("Found undefined token"); yyterminate();

%%
