using System;

namespace rot1
{
    public class Token
    {
        public object value; //Relative to token type
        public string type; //Int, Plus, Minus, Times, Div, Equal, Eof, POpen, PClose, End, WEnd, Identifier, Print, Input, If, Then, Else, While, Higher, Lower
    }
}