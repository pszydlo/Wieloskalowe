using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutomatyKom
{
    public partial class Form1 : Form
    {


        int width, height, rule;
        private bool[] currentRow;
        private bool[] oldRow;
        private int liveCell;

        private bool[] ruleBin;

        public Form1()
        {
            InitializeComponent();

        }
        private void button1_Click(object sender, EventArgs e)
        {
            int.TryParse(textBox1.Text, out width);
            int.TryParse(textBox2.Text, out height);
            int.TryParse(textBox3.Text, out rule);

            this.currentRow = new bool[this.width];
            this.oldRow = new bool[this.width];
            this.ruleBin = new bool[8];
 
            liveCell = width / 2;
            for(int i=0;i<8;i++)
            {
                if (rule % 2 == 1)
                    ruleBin[i] = true;
                else
                    ruleBin[i] = false;
                rule = rule / 2;
            }
            initialrow();

            for (int i = 1; i < height; i++)
            {
               iteration(i);
            }
        }

        private void drawOneRow(int line)
        {
            Graphics g = pictureBox1.CreateGraphics();
            Brush brush;
            for (int x = 0; x < width; x++)
            {
                if(currentRow[x])
                {
                    brush = new SolidBrush(Color.Red);
                    g.FillRectangle(brush, x, line, 1, 1);
                }
                else
                {
                    brush = new SolidBrush(Color.Black);
                    g.FillRectangle(brush, x, line, 1, 1);
                }

            }     
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }

        void initialrow()
        {
            for(int i=0;i<width;i++)
            {
                if (i == liveCell)
                    currentRow[i] = true;
                else
                    currentRow[i] = false;
            }
            drawOneRow(0);

            for (int i = 0; i < width; i++)
            {
                oldRow[i] = currentRow[i];
            }
        }
        void iteration(int line)
        {

            for (int i = 0; i < width; i++)
            {
                int idRule = 0;
                if(i+1==width)
                {
                    if (oldRow[0])
                        idRule += 1;
                }
                else if (oldRow[i+1])
                        idRule += 1;

                if (oldRow[i])
                    idRule += 2;

                if (i - 1 == -1)
                {
                    if (oldRow[width - 1])
                        idRule += 4;
                }
                else if (oldRow[i - 1])
                         idRule += 4;

                currentRow[i] = ruleBin[idRule];
            }
            drawOneRow(line);
            for (int i = 0; i < width; i++)
            {
                oldRow[i] = currentRow[i];
            }

        }


        
    }
}
