using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YSRA
{
        public partial class BreakpointForm : Form
        {           
                public BreakpointForm()
                {
                        InitializeComponent();
                        for (int i = 1; i <= 100; i++)
                        {
                                this.comboBox1.Items.Add(i);
                        }
                        this.comboBox1.SelectedIndex = 0; 
                        //read breakpoint
                }

                private void BreakpointForm_Load(object sender, EventArgs e)
                {                      
                       
                }

                private void button1_Click(object sender, EventArgs e)
                {
                        //This function adds breakpoint 
                        //
                        string tmpItemText = "Breakpoint line: " + this.comboBox1.Text.ToString();
                        //add check for repetition
                        //add 
                        if(true)
                        {

                        }
                        ListViewItem tmp = new ListViewItem();
                        tmp.ImageIndex = 0;
                        tmp.Text = tmpItemText;
                        tmp.SubItems.Add("Breakpoint");
                        this.listView1.Items.Add(tmp);
                        UInt32 i;
                        // add breakpoint to array      
                        i = Convert.ToUInt32(this.comboBox1.Text.ToString());                  
                        //AddBreakpoint(i);
                        lib.my_basic.SetBreakpoint(i);
                }

                private void button2_Click(object sender, EventArgs e)
                {
                        //This function deletes breakpoint
                        //
                        UInt32 i;
                        foreach(ListViewItem item in this.listView1.Items)
                        { 
                                if(item.Checked)
                                {
                                        this.listView1.Items.Remove(item);
                                        //call breakpoint deletion callback function                                        
                                        string[] tmpstr = item.Text.ToString().Split(' ');
                                        i = Convert.ToUInt32(tmpstr[2]);
                                        //RemoveBreakpoint(i);
                                        lib.my_basic.RemoveBreakpoint(i);
                                }
                        }

                }

                private void button3_Click(object sender, EventArgs e)
                {
                        //This function removes checked breakpoint
                        //
                        UInt32 i;
                        foreach (ListViewItem item in this.listView1.Items)
                        {
                                this.listView1.Items.Remove(item);
                                //delete all breakpoints 
                                //RemoveBreakpoint(i);
                                this.listView1.Items.Remove(item);                                                                  
                                string[] tmpstr = item.Text.ToString().Split(' ');
                                i = Convert.ToUInt32(tmpstr[2]);                               
                                lib.my_basic.RemoveBreakpoint(i);
                        }

                }
        }
}
