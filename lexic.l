%{
#include <string>
#include "node.h"
#include "parser.hpp"
#define SAVE_TOKEN yylval.string = new std::string(yytext, yyleng)
#define TOKEN(t) (yylval.token = t)
extern "C" int yywrap() { }
%}

%%

""                  {return tk_;} //remove

"put"					{return tk_putasign;}
"in"                    {return tk_in;}
"swap"					{return tk_swapasign;}
"for"					{return tk_for ;}
"increase"              {return tk_increase;}
"decrease"              {return tk_decrease;}
"by"                    {return tk_by;}
""                  {return tk_;}

"plus"					{return tk_plus;}
"minus"	    			{return tk_minus;}
"times"					{return tk_times;}
"divided by"			{return tk_division;}
"squared by"			{return tk_squared;}
"mod"					{return tk_mod;}
"module"			    {return tk_mod;}
"bxor"					{return tk_bitxor;}
"bor"					{return tk_bitor;}
"band"					{return tk_bitand;}

"and"					{return tk_and;}
"."						{return tk_dot;}

"if"				    {return tk_if ;}
"else"				    {return tk_else ;}
"then"				    {return tk_then ;}

"function"				{return tk_function ;}

"true"				    {return tk_true;}
"false"				    {return tk_false;}
"null"			    	{return tk_null ;}
"not"					{return tk_not;}

"while"				    {return tk_while;}
"do"                    {return tk_do;}

"is"                    {return tk_is;}
"equal to"			    {return tk_equal;}
"equals"			    {return tk_equal;}
"higher than"           {return tk_higher;}
"lower than"            {return tk_lower;}

"zero"                  {return tk_zero;}
"higher than zero"      {return tk_;}
"is even"               {return tk_;}
"is not even"           {return tk_;}
""                  {return tk_;}
""                  {return tk_;}
""                  {return tk_;}
""                  {return tk_;}

"\""				    {return tk_quote;}

[ \t\n]                 ;
[a-zA-Z_][a-zA-Z0-9_]*  SAVE_TOKEN; return TIDENTIFIER;
[0-9]+.[0-9]+           SAVE_TOKEN; return TFLOAT;
[0-9]+,[0-9]+           SAVE_TOKEN; return TFLOAT;
[0-9]+                  SAVE_TOKEN; return TINT;
0x[0-9a-f]+             SAVE_TOKEN; return THEXA; //right?

.                       printf("Found undefined token"); yyterminate();

%%
