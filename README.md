# SimpleTag is a new Open Source Project (like JSON) working with C# Programming Language
Here is Tutorail for Beginners

--------------------------------------------------------
1) First you have to create file with extension ".stg"
--------------------------------------------------------
2) Starting scripting in file
--------------------------------------------------------
    1. Opening Stg script "!stg_start;"
    
    2. Commenting you code is simples part just type like this "/* -----   Comment ----- */" opening /* and closing */
    
    3. Closing Stg script "!stg_end;"
    
    4. Writing just a variable in script " !var X = 48.5;  (for sample this is a double variable)"
    
    5. Writing Arrays in a script " !array NAME = [ 'P', 'R', 'O', 'G', 'R', 'A', 'M', 'M', 'E', 'R' ]; (for sample this one is a char[] array)
    
    6. writing string in a script " !str NAME = "David"; (Yes string is a special variable type in STG)"
    
3) Little Example to use it and get deeper in STG script
--------------------------------------------------------
# Your Object class in C#

    class TestClass
    {
        public int a = 5;
        public double b = 5.5;
        public long c = 500000000000000000;
        public string d = "A";
        public char e = 'A';
        public DateTime h = DateTime.Now;
        public int[] arrA = {1, 2, 3, 4 };
        public double[] arrB = {1.5, 2.5, 3.5, 4.5 };
        public char[] arrC = {'A', 'B', 'C', 'D' };
        public string[] arrD = { "A", "B", "C", "D" };
        public DateTime[] arrE = { DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, };
        public int number; // as you can see this variable is not serialized because it is in default value and the same thig works for all types
    }

# Result in STG Script

    !stg_start; 
    !obj TestClass { 
    !var a = 5; 
    !var b = 5.5; 
    !var c = 500000000000000000; 
    !str d = "A"; 
    !var e = 'A'; 
    !var h = "23.09.2019 03:02:50"; 
    !array arrA = [ 1, 2, 3, 4 ]; 
    !array arrB = [ 1.5, 2.5, 3.5, 4.5 ]; 
    !array arrC = [ 'A', 'B', 'C', 'D' ]; 
    !array arrD = [ "A", "B", "C", "D" ]; 
    !array arrE = [ "23.09.2019 03:02:50", "23.09.2019 03:02:50", "23.09.2019 03:02:50", "23.09.2019 03:02:50" ]; 
    } 
    !stg_end;
    --------------------------------------------------------
7) Thanks for using my Stg Script
    
