using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraWZycie
{
    public partial class Form1 : Form
    {
        Graphics g;
        Brush brush;
        Pen p = new Pen(Color.Black, 1);
        int width, height;
        const int SIZECELL = 10;
        int MAXLOSLIVECELL = 100;
        const int LIVETIME = 100;
        bool[][] oldBoard;
        bool[][] currentBoard;
        Random random = new Random(222);
        bool chooden = false;

        public Form1()
        {
            
            InitializeComponent();
            
            g = pictureBox1.CreateGraphics();
            pictureBox1.Width = Int32.Parse(textBox1.Text);
            pictureBox1.Height= Int32.Parse(textBox2.Text);
            width = pictureBox1.Width/SIZECELL;
            height = pictureBox1.Height/SIZECELL;

            MAXLOSLIVECELL = width * height / 10;

            comboBox1.Items.Add("Pusty");
            comboBox1.Items.Add("Niezmienne");
            comboBox1.Items.Add("Glider");
            comboBox1.Items.Add("Reczna definicja");
            comboBox1.Items.Add("Oscylator");
            comboBox1.Items.Add("Losowy");

            oldBoard = new bool[width][];
            currentBoard = new bool[width][];

            for(int i=0;i<width;i++)
            {
                oldBoard[i] = new bool[height];
                currentBoard[i] = new bool[height];
            }

            brush = new SolidBrush(Color.Black);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }
        int mod(int x, int m)
        {
            return (x % m + m) % m;
        }
        void live ()
        {
           
            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                int neighbor = 0;
                if (oldBoard[mod(x-1, width)][mod(y - 1, height)])
                    neighbor++;

                if (oldBoard[mod(x-1, width)][mod(y, height)])
                    neighbor++;

                if (oldBoard[mod(x-1, width)][mod(y + 1, height)])
                    neighbor++;

                if (oldBoard[mod(x, width)][mod(y - 1, height)])
                    neighbor++;

                if (oldBoard[mod(x, width)][mod(y + 1, height)])
                    neighbor++;

                if (oldBoard[mod(x+1, width)][mod(y - 1, height)])
                    neighbor++;

                if (oldBoard[mod(x+1, width)][mod(y, height)])
                    neighbor++;

                if (oldBoard[mod(x+1, width)][mod(y + 1, height)])
                    neighbor++;


                if (!oldBoard[x][y] && neighbor == 3)
                    currentBoard[x][y] = true;
                else if (oldBoard[x][y] && neighbor > 3)
                    currentBoard[x][y] = false;
                else if (oldBoard[x][y] && (neighbor == 2 || neighbor == 3))
                    currentBoard[x][y] = true;
                else if (oldBoard[x][y] && neighbor < 2)
                    currentBoard[x][y] = false;
                else
                    currentBoard[x][y] = oldBoard[x][y];

            }
               
            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                if (oldBoard[x][y]!=currentBoard[x][y])
                {
                if (currentBoard[x][y])
                    g.FillRectangle(brush, x * SIZECELL, y * SIZECELL, SIZECELL, SIZECELL);
                else
                {
                    g.FillRectangle(new SolidBrush(Color.White), x * SIZECELL, y * SIZECELL, SIZECELL, SIZECELL);
                    g.DrawRectangle(p, x * SIZECELL, y * SIZECELL, SIZECELL, SIZECELL);
                }
                    
                }
                oldBoard[x][y] = currentBoard[x][y];
            }

           
            
            
            
        }
        void initial(int initialBoard)
        {
            switch (initialBoard)
            {
                case 0:
                    for (int x = 0; x < width; x ++)
                        for (int y = 0; y < height; y ++)
                        {
                            g.DrawRectangle(p, x*SIZECELL, y*SIZECELL, SIZECELL, SIZECELL);
                            oldBoard[x][y] = false;
                        }
                    chooden = true;
                    break;
                case 1:
                    for (int x = 0; x < width; x++)
                        for (int y = 0; y < height; y++)
                        {
                            if (((x == (width / 2 - 1) || x == (width / 2 + 2)) && y == (height / 2))
                                || ((x == (width / 2) || x == (width / 2 + 1)) && y == (height / 2 - 1))
                                || ((x == (width / 2) || x == (width / 2 + 1)) && y == (height / 2 + 1)))
                            {
                                g.FillRectangle(brush, x * SIZECELL, y * SIZECELL, SIZECELL, SIZECELL);
                                oldBoard[x][y] = true;
                            }
                            else
                            {
                                g.DrawRectangle(p, x * SIZECELL, y * SIZECELL, SIZECELL, SIZECELL);
                                oldBoard[x][y] = false;
                            }

                        }
                    chooden = true;
                    break;

                case 2:
                    for (int x = 0; x < width; x++)
                        for (int y = 0; y < height; y++)
                        {
                            if (((x == (width / 2) || x == (width / 2 +1)) && y == (height / 2))
                                || ((x == (width / 2+1) || x == (width / 2 + 2)) && y == (height / 2 - 1))
                                || (( x == (width / 2 + 2))&& y == (height / 2 + 1)))
                            {
                                g.FillRectangle(brush, x * SIZECELL, y * SIZECELL, SIZECELL, SIZECELL);
                                oldBoard[x][y] = true;
                            }
                            else
                            {
                                g.DrawRectangle(p, x * SIZECELL, y * SIZECELL, SIZECELL, SIZECELL);
                                oldBoard[x][y] = false;
                            }

                        }
                    chooden = true;
                    break;

                case 3:
                    if(!chooden)
                    for (int x = 0; x < width; x++)
                        for (int y = 0; y < height; y++)
                        {
                            g.DrawRectangle(p, x * SIZECELL, y * SIZECELL, SIZECELL, SIZECELL);
                            oldBoard[x][y] = false;
                        }

                    break;

                case 4:
                    for (int x = 0; x < width; x++)
                        for (int y = 0; y < height; y++)
                        {
                            if (x == (width / 2) && (y == (height / 2)|| y == (height / 2-1)|| y == (height / 2+1)))
                            {
                                g.FillRectangle(brush, x * SIZECELL, y * SIZECELL, SIZECELL, SIZECELL);
                                oldBoard[x][y] = true;
                            }
                            else
                            {
                                g.DrawRectangle(p, x * SIZECELL, y * SIZECELL, SIZECELL, SIZECELL);
                                oldBoard[x][y] = false;
                            }

                        }
                    chooden = true;
                    break;

                case 5:
                    int losCount = 0;
                    for (int x = 0; x < width; x++)
                        for (int y = 0; y < height; y++)
                        {
                         
                            int los = random.Next(5);

                            if (los==1 && losCount<MAXLOSLIVECELL && x > width/10 )
                            {
                                g.FillRectangle(brush, x * SIZECELL, y * SIZECELL, SIZECELL, SIZECELL);
                                oldBoard[x][y] = true;
                                losCount++;
                            }
                            else
                            {
                                g.DrawRectangle(p, x * SIZECELL, y * SIZECELL, SIZECELL, SIZECELL);
                                oldBoard[x][y] = false;
                            }

                        }
                    chooden = true;
                    break;

                default:
                    break;
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
            initial(comboBox1.SelectedIndex);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Width = Int32.Parse(textBox1.Text);
            pictureBox1.Height = Int32.Parse(textBox2.Text);
            width = Int32.Parse(textBox1.Text) / SIZECELL;
            height = Int32.Parse(textBox2.Text) / SIZECELL;
            oldBoard = new bool[width][];
            currentBoard = new bool[width][];
            for (int i = 0; i < width; i++)
            {
                oldBoard[i] = new bool[height];
                currentBoard[i] = new bool[height];
            }
            g = pictureBox1.CreateGraphics();

            MAXLOSLIVECELL = width * height / 10;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            live();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            int Xmouse = e.X/SIZECELL, Ymouse = e.Y/SIZECELL;
            oldBoard[Xmouse][Ymouse] = true;
            g.FillRectangle(brush, Xmouse * SIZECELL, Ymouse * SIZECELL, SIZECELL, SIZECELL);
        }
    }
}