def fat(a):                     //to fat ask for a then do
    if a > 0:                   //  if a is higher than 0 do
        out = a * rec(a - 1)    //      put a times get fat with a minus 1 in out then
        #print (a)              //      (print a then)
        print (out)             //      print out then
        return out              //      put out in Return.
    else:                       //  else
        return 1                //      put 1 in Return.
                                //
fat(10)                         //do fat with 10.
