using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace archi_Template
{
    class Cycles
    {
        //Ckeck if the process is done or not;
        public int F = 0;
        public int D = 0;
        public int E = 0;
        public int M = 0;
        public int EM = 0;
        //------------------------------------------
        int[] PathValues;
        string Funct;
        //bool Shamt;
        int RD;
        int RT;
        int RS;
        int OP;
        int Offset; //if I-type.
        //---------------------------------
        //DataMemory DM;
        InstructionMemory IM;
        int Pcounter;
        public Cycles(int Pcounter,string Machine)
        {
            this.Pcounter = Pcounter;
            IM = new InstructionMemory(Pcounter, Machine);
            PathValues = new int[31];//to set values.
            SetPipelineValues();
        }
        public void SetPipelineValues()
        {
            //fetch Strategy;
            PathValues[1] = Pcounter;
            Pcounter += 4;
            PathValues[2] = Pcounter;
            //DecodeStrategy;
            Funct = IM.DedicatesFun();
            if (Funct != null)
            {
                RS =Convert.ToInt32(IM.GetRS(),2);//ReadRegister1
                RT =Convert.ToInt32(IM.GetRT(),2);//ReadRegister
                RD =Convert.ToInt32(IM.GetRD(),2); //WriteRegister
                PathValues[3] = RS; //registerName
                if (PathValues[3] != Convert.ToInt32("00000",2))
                {
                    PathValues[5] = Form1.Registers[(int)Convert.ToDecimal(RS.ToString())];
                }
                else
                {
                    PathValues[5] =Convert.ToInt32("00000",2);
                }
                PathValues[4] = RT;
                if (PathValues[4] != Convert.ToInt32("00000",2))
                {
                    PathValues[6] = Form1.Registers[(int)Convert.ToDecimal(RT.ToString())];
                }
                else
                {
                    PathValues[6] = Convert.ToInt32("000000",2);
                }
                PathValues[7] = RD;
                PathValues[9] = Convert.ToInt32("00000",2); // Signextend.
                PathValues[10] = PathValues[4];
                PathValues[11] = PathValues[7];
                //stage2
                PathValues[12] = PathValues[5];
                PathValues[13] = PathValues[6];
                PathValues[14] = 0;
                PathValues[15] = PathValues[13];
                int Result=AluResult(PathValues[12], PathValues[15],0,Funct);
                PathValues[16] = Result;
                PathValues[17] = PathValues[10];
                PathValues[18] = PathValues[11];
                PathValues[19] = PathValues[18]; //as RegDSt
                PathValues[20] = PathValues[2];
                PathValues[21] = 0;
                PathValues[22] = 0;
                PathValues[23] = PathValues[16];
                PathValues[24] = 0;
                PathValues[25] = 0;
                PathValues[26] = PathValues[19];
                PathValues[27] = 0;
                PathValues[28] = PathValues[23];
                PathValues[29] = PathValues[28];
                PathValues[8] = PathValues[29];
                PathValues[30] = PathValues[26];
                PathValues[7] = PathValues[30];
                int RIndex = (int)Convert.ToDecimal(PathValues[30].ToString());
                int Value = (int)Convert.ToDecimal(PathValues[29].ToString());
                Form1.Registers[RIndex] = Value;
            }
            else //sw;
            {
                Offset =Convert.ToInt32(IM.GetOffset(),2);
                RS =Convert.ToInt32(IM.GetRS(),2);
                RT = Convert.ToInt32(IM.GetRT(),2);
                OP = Convert.ToInt32(IM.GetOPCode(),2);
                PathValues[3] =RS;
                if (PathValues[3] != Convert.ToInt32("000000",2))
                {
                    //PathValues[5] = PathValues[3] +Convert.ToInt32("1100100",2);
                    PathValues[5] = Form1.Registers[(int)Convert.ToDecimal(RS.ToString())];
                }
                else
                {
                    PathValues[5] =0;
                }
                PathValues[4] = RT;
                if (PathValues[4] != Convert.ToInt32("00000", 2))
                {
                    //PathValues[6] = PathValues[4] +Convert.ToInt32("1100100",2);
                    PathValues[6] = Form1.Registers[(int)Convert.ToDecimal(RT.ToString())];
                }
                else
                {
                    PathValues[6] = 0;
                }
                PathValues[7] = 0;
                PathValues[8] = 0;
                PathValues[9] = Offset;
                PathValues[10] = PathValues[4];
                PathValues[11] = 0;

                PathValues[12] = PathValues[5];
                PathValues[13] = PathValues[6];
                string valoffset = SignExtend(IM.GetOffset());
                PathValues[14] = Convert.ToInt32(valoffset,2);
                PathValues[15] = PathValues[14]; //as Alusrc
                int Result = PathValues[5] +PathValues[14]; //rt = rs+c(offset)
                PathValues[16] = Result; //result of SW;

                PathValues[17] = PathValues[10];
                PathValues[18] = PathValues[11];

                PathValues[19] = PathValues[17];
                PathValues[20] = PathValues[2];
                PathValues[21] = PathValues[20];
                PathValues[22] = PathValues[21];
                PathValues[23] = PathValues[16];
                if (!Form1.DataMemory.ContainsKey((int)Convert.ToDecimal(Result.ToString())))
                    Form1.DataMemory.Add((int)Convert.ToDecimal(Result.ToString()), (int)Convert.ToDecimal(PathValues[6].ToString()));
                Form1.DataMemory[(int)Convert.ToDecimal(Result.ToString())] = (int)Convert.ToDecimal(PathValues[6].ToString());
                PathValues[25] = 0;
                PathValues[26] = PathValues[19];
                PathValues[27] = 0;
                PathValues[28] = 0;
                PathValues[29] = 0;
                PathValues[30] = 0;
                PathValues[8] = 0;
            }
        }
        public string FetchInstration()
        {
            string Fun = IM.DedicatesFun();
            string result =Convert.ToString(Pcounter,2);
            if (Fun!=null)
            {
                result +="  "+IM.GetOPCode()+ "  " +IM.GetRS()+ "  " +IM.GetRT()+
                "  " +IM.GetRD()+ "  " +IM.GetShamt()+ "  "+IM.GetFunction();
            }
            else
            {
               result += "  " +IM.GetOPCode()+ "  " +IM.GetRS()+ "  " +IM.GetRT()+
               "  " +IM.GetOffset();
            }
            return result;
        }
        public string DecodeInstration()
        {
            string Result = "110000010";
            Result +="  "+Convert.ToString(PathValues[2],2);
            Funct = IM.DedicatesFun(); //string
            if (Funct != null)
            {
                Result +="  "+Convert.ToString(PathValues[5],2)+"  "+Convert.ToString(PathValues[6],2)+
                    "  "+Convert.ToString(PathValues[9],2)+"  "+Convert.ToString(PathValues[10], 2)+"  "+
                    Convert.ToString(PathValues[11], 2);
            }
            else //sw;
            {
                Result = "x1x001000";
                Result += "  " + Convert.ToString(PathValues[5], 2) + "  " + Convert.ToString(PathValues[6], 2)
                +"  " + Convert.ToString(PathValues[14], 2) + "  " + Convert.ToString(PathValues[10], 2) + "  " +
                Convert.ToString(PathValues[11], 2);
            }
            return Result;
        }
        //Strategy 3
        public string ExecuteInstration()
        {
            string Result = "010001";
            if (Funct != null)
            {
                Result += "  " + Convert.ToString((Pcounter + 4), 2) + " " + Convert.ToString(PathValues[16],2)
                + " " +Convert.ToString(PathValues[19],2);
            }
            else
            {
                Result += "x00100"+"  "+Convert.ToString(PathValues[21], 2)
                +"  "+Convert.ToString(PathValues[16], 2)+"  "+Convert.ToString(PathValues[13], 2);
            }
            return Result;
        }
        //Strategy 4;
        public string MemoryAccess()
        {
            string result = "01";
            if (Funct != null) //r
            {
                result +="  "+Convert.ToString(PathValues[16], 2)+" "+Convert.ToString(PathValues[26], 2);
            }
            else //i
            {
                result = "x0";
                result +="  "+Convert.ToString(PathValues[25], 2);
            }
            return result;
        }
        //[5]MemToReg;
        public string WriteBack()
        {
            string result = "";
            if (Funct != null) //r
            {
                PathValues[29] = PathValues[28]; //data
                PathValues[30] = PathValues[26]; //registerNumber
                int RIndex =(int)Convert.ToDecimal(PathValues[30].ToString());
                int Value =(int)Convert.ToDecimal(PathValues[29].ToString());
                Form1.Registers[RIndex] = Value;
                result = "done";
            }
            else
            {
                result = "Sdone";
            }
            return result;
        }
        public int AluResult(int R1,int R2 , int op , string Fun)
        {
            int DecimalValue = 0;
            if (op == 0) //r-type
            {
                if (Fun == "or")
                {
                    DecimalValue = R1|R2;
                }
                else if (Fun == "add")
                {
                    DecimalValue = R1 + R2;
                }
                else if (Fun == "sub")
                {
                    DecimalValue = R1 - R2;
                }
                else if (Fun == "and")
                {
                    DecimalValue = R1 & R2;
                }
            }
            return DecimalValue;
        }
        private string SignExtend(string offset)
        {
            string value =offset; // 16bit
            if (value[0].CompareTo('0')==0)
                value = value.Insert(0,Convert.ToString(0000000000000000,2));
            else
                value = value.Insert( 0,Convert.ToString(1111111111111111,2));
            return value;
        }
    }
}