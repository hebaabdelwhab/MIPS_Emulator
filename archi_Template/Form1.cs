using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
namespace archi_Template
{
    public partial class Form1 : Form
    {
        private int NumOfInstr;
        private int NumOfCycle;
        private bool First = true;
        private List<Cycles> Instructions = new List<Cycles>();
        public static int[] Registers = new int[32];
        public static Dictionary<int, int> DataMemory = new Dictionary<int, int>();
        public Form1()
        {
            Registers[0] = 0;
            for (int i = 1; i < 32; i++)
            {
                Registers[i] = i + 100;
            }
            
            InitializeComponent();
         
        }
        //----------------------------------------------------------
        private void UpdateRegisters()
        {
            DataTable DT = new DataTable();
            DT.Columns.Add("Register");
            DT.Columns.Add("Value");
            for (int i = 0; i < 32; i++)
            {
                string name = "$" + i;
                DT.Rows.Add(new object[] { name,Registers[i]});
            }
            MipsRegisterGrid.DataSource = DT;
        }
        private void UpdateMemory()
        {
            DataTable DT = new DataTable();
            //------------------setDataMemoryVales-----------------
            DT = new DataTable();
            DT.Columns.Add("Address");
            DT.Columns.Add("Value");
            for (int i = 0; i < 256; i++)
            {
                    DT.Rows.Add(new object[] { DataMemory.Keys.ElementAt(i), DataMemory.Values.ElementAt(i) });   
            }
            MemoryGrid.DataSource = DT;
        }
        private void InializeBtn_Click(object sender, EventArgs e)
        {
            DataTable DT = new DataTable();
            DT.Columns.Add("Register");
            DT.Columns.Add("Value");
            DT.Rows.Add(new object[] { "$0", 0});
            for (int i=1;i<32;i++)
            {
                string name = "$" + i;
                DT.Rows.Add(new object[] { name, (i+100)});
            }
            MipsRegisterGrid.DataSource = DT;
            //------------------setDataMemoryVales-----------------
            DT = new DataTable();
            DT.Columns.Add("Address");
            DT.Columns.Add("Value");
            for (int i=0;i<256; i++)
            {
                DT.Rows.Add(new object[] { i,99});
                //if(!DataMemory.ContainsKey(99))
                DataMemory.Add(i,99);
            }
            MemoryGrid.DataSource = DT;
        }
        private void GetInstructions()
        {
            if (First == true)
            {
                NumOfInstr = UserCodetxt.Lines.Count();
                string TotalStr = UserCodetxt.Text;
                string[] Lines = TotalStr.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                for (int i = 0; i < NumOfInstr; i++)
                {
                    Lines[i] = Lines[i].Replace(" ", "");
                    string[] SplitStr = Lines[i].Split(':');
                    int Pcounter = Convert.ToInt32(SplitStr[0]);
                    if (SplitStr[1].Length > 32)
                        MessageBox.Show("Invalid Instruction");
                    Cycles Process = new Cycles(Pcounter, SplitStr[1]);
                    Instructions.Add(Process);
                }
                First = false;
            }
        }
        private Dictionary<string,string> GetValue(int NCurrentCycle)
        {
            GetInstructions(); //once enter.
            Dictionary<string, string> List = new Dictionary<string, string>();
            for (int k = 0; k < NCurrentCycle; k++)
            {
                //Fetch
                if (Instructions[k].F == 0)
                {
                    List.Add("IF/ID", Instructions[k].FetchInstration());
                    UpdateRegisters();
                    Instructions[k].F = 1;
                }
                else if (Instructions[k].D == 0)
                {
                    List.Add("ID/EX", Instructions[k].DecodeInstration());
                    UpdateRegisters();
                    Instructions[k].D = 1;
                }
                else if (Instructions[k].E == 0)
                {
                    List.Add("EX/MEM", Instructions[k].ExecuteInstration());
                    UpdateRegisters();
                    Instructions[k].E = 1;
                }
                else if (Instructions[k].M == 0)
                {
                    List.Add("MEM/WB", Instructions[k].MemoryAccess());
                    UpdateRegisters();
                    Instructions[k].M = 1;
                }
                else if (Instructions[k].EM == 0)
                {
                    List.Add("back", Instructions[k].WriteBack());
                    UpdateRegisters();
                    if (Instructions[k].WriteBack() == "Sdone")
                        UpdateMemory();
                    Instructions[k].EM = 1;
                }
            }
            return List;            
        }
        int index = 1;
        int temp = 0;
        private void RunCycleBtn_Click(object sender, EventArgs e)
        {
            try
            {
                NumOfCycle = 5 + (NumOfInstr - 1);
                DataTable DT = new DataTable();
                DT.Columns.Add("Register");
                DT.Columns.Add("Value");
                Dictionary<string, string> Result = new Dictionary<string, string>();
                if (temp < NumOfCycle)
                {
                    if (index >= NumOfInstr)
                    {
                        index = NumOfInstr;
                        Result = GetValue(index);
                    }
                    else
                        Result = GetValue(index);
                    for (int i = 0; i < Result.Keys.Count(); i++)
                    {
                        DT.Rows.Add(new Object[] { Result.Keys.ElementAt(i), Result.Values.ElementAt(i) });
                    }
                    PiplineGrid.DataSource = DT;
                    index++;
                    runCycleBtn.Text = "Run" + temp.ToString() + "Cycle";
                    temp++;
                }
                else
                {
                    MessageBox.Show("AllStagesCompleteSuccessfully");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("error !!!");
            }
        }
    }
}
