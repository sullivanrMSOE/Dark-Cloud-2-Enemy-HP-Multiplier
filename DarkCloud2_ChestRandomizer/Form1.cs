using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace DarkCloud2_EnemyHPMultiplier
{
    public partial class Form1 : Form
    {
        public static double multiplier = 1;
        public bool gameCheck = false;
        public static bool versionCheck;
        public Form1()
        {
            InitializeComponent();
            if (Memory.PID != 0)
            {
                label2.Text = "This mod multiplies enemy HP in Dark Cloud 2. You must be using PCSX2 and have Dark Cloud 2 (USA)";

                if (Memory.ReadInt(0x203694D0) == 1701667175)
                {
                    gameCheck = false;
                    Randomizer.gameVersion = 0;
                    label3.Text = "Detected Dark Chronicle (PAL version). However, this mod only works with the USA version. Sorry.";
                }
                else if (Memory.ReadInt(0x20364BD0) == 1701667175)
                {
                    gameCheck = true;
                    Randomizer.gameVersion = 2;
                    label3.Text = "Detected Dark Cloud 2 (USA version)! Press begin to start multiplying enemy HP.";
                }
                else
                {
                    button1.Enabled = false;
                    label3.Text = "Cannot find active Dark Cloud 2 game, please launch Dark Cloud 2 and restart this mod.";
                }
            }
            else
            {
                label3.Enabled = false;
                button1.Enabled = false;               
            }
        }
        
        public static Thread chestThread = new Thread(() => Randomizer.ChestRandomizer(Form1.multiplier));

        private void button1_Click(object sender, EventArgs e)
        {

            
            try
            {
                multiplier = Convert.ToDouble(textBox1.Text);
                if(multiplier <= 0 || multiplier > 100)
                {
                    throw new FormatException();
                }
                label3.Text = "Multiplying enemy HP! Do not use save states, as they can break the mod. If or when you exit the game, this program closes automatically.";
                label3.Font = new Font(label3.Font, FontStyle.Bold);
                button1.Enabled = false;
                textBox1.ReadOnly = true;
                if (!chestThread.IsAlive) //If we are not already running
                    chestThread.Start(); //Start thread
            }
            catch (FormatException)
            {
                if (textBox1.Text == "")
                {
                    MessageBox.Show("Please enter a multiplier.", "Invalid Multiplier", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    MessageBox.Show("The multiplier you selected is invalid.", "Invalid Multiplier", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}
