using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace archi_Template
{
    public class InstructionMemory
    {
        int PC; 
        string StrMAchine;
        public InstructionMemory(int PC,string StrMAchine)
        {
            this.PC= PC;
            this.StrMAchine = StrMAchine;
        }
        public string DedicatesFun()
        {
            string Str = GetFunction();
            if (Str == "100101") 
            {
                Str = "or";
            }
            else if (Str == "100000")
            {
                Str = "add";
            }
            else if (Str == "100010")
            {
                Str = "sub";
            }
            else if (Str == "100100")
            {
                Str = "and";
            }
            else
            {
                return null;
            }
            return Str;
        }
        public string GetOPCode()
        {
            string OP = StrMAchine.Substring(0, 6);
            return OP;
        }
        public string GetRS()
        {
            string RS = StrMAchine.Substring(6, 5);
            return RS;
        }
        public string GetRT()
        {
            string RT = StrMAchine.Substring(11, 5);
            return RT;
        }
        public string GetRD()
        {
            string RD = StrMAchine.Substring(16, 5);
            return RD;
        }
        public string GetShamt()
        {
            string Shamt = StrMAchine.Substring(21, 5);
            return Shamt;
        }
        public string GetFunction()
        {
            string Fun =StrMAchine.Substring(26,6);
            return Fun;
        }
        public string GetOffset()
        {
            string Offset = StrMAchine.Substring(16, 16);
            return Offset;
        }
        public string Control_unit(string opCode)
        {
            string result = "";
            if (opCode == "000000")
            {
                result = "110000010";
            }
            else
            {
                result = "x1x001000";
            }
            return result;
        }
    }

}