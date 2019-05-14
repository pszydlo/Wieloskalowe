using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RozrostZiaren
{
    public partial class Form1 : Form
    {
        private Graphics g;
        private Pen p;
        private int width, height;
        private const int SIZECELL = 10;
        private  int [][] oldBoard;
        private int[][] currentBoard;
        private int numberOfseed;
        private int radius;
        private int typeBoundaryConditions;
        private bool isManualDefinitionStart = false;
        
        private Color [] seedColorTable;
        private Random rnd = new Random();


        public Form1()
        {
            InitializeComponent();
            set();
            
            comboBox1.Items.Add("Jednorodne");
            comboBox1.Items.Add("Z promieniem");
            comboBox1.Items.Add("Losowe");
            comboBox1.Items.Add("Ręczna definicja");
        }
        int mod(int x, int m)
        {
            return (x % m + m) % m;
        }
        int mod2(int x, int y)
        {
            if (x >= width || x < 0 || y < 0 || y >= height)
                return -1;
            else
                return oldBoard[x][y];
        }
        void set()
        {
            pictureBox1.Width = Int32.Parse(textBox1.Text);
            pictureBox1.Height = Int32.Parse(textBox2.Text);
            width = Int32.Parse(textBox1.Text) / SIZECELL;
            height = Int32.Parse(textBox2.Text) / SIZECELL;
            oldBoard = new int[width][];
            currentBoard = new int[width][];
            label5.Visible = false;
            label6.Visible = false;
            button4.Visible = false;

            p = new Pen(Color.Black, 1);
            for (int i = 0; i < width; i++)
            {
                oldBoard[i] = new int[height];
                currentBoard[i] = new int[height];
            }
            g = pictureBox1.CreateGraphics();
        }
        void initial()
        {
            pictureBox1.Refresh();
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    g.DrawRectangle(p, x * SIZECELL, y * SIZECELL, SIZECELL, SIZECELL);
                    oldBoard[x][y] = -1;

                }
            int actualseed = 0;
            int leftSeed = 0;
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    int row = Int32.Parse(textBox3.Text);
                    int column = Int32.Parse(textBox4.Text);
                    numberOfseed = row * column;
                    setColorTable();
                    for (int x = 2; x < width && actualseed<numberOfseed; x+= width / row )
                        for (int y = 2; y < height && actualseed < numberOfseed; y+= height / column )
                        {
                            oldBoard[x][y] = actualseed;
                            g.FillRectangle(new SolidBrush(seedColorTable[actualseed]), x * SIZECELL, y * SIZECELL, SIZECELL, SIZECELL);
                            actualseed++;
                        }
                    timer1.Start();

                    break;

                case 1:
                    radius = Int32.Parse(textBox3.Text);
                    numberOfseed = Int32.Parse(textBox4.Text);
                    setColorTable();
                    leftSeed = numberOfseed;
                    for (int x = radius; x < width && leftSeed>0; x += radius+1)
                        for (int y = radius; y < height && leftSeed > 0; y +=radius+1)
                        {
                            oldBoard[x][y] = actualseed;
                            g.FillRectangle(new SolidBrush(seedColorTable[actualseed]), x * SIZECELL, y * SIZECELL, SIZECELL, SIZECELL);
                            actualseed++;
                            leftSeed--;
                        }
                    if(leftSeed>0)
                    {
                        label5.Text = "Pozostało \n" + leftSeed + "\nziaren których nie dało się pozadzić";
                        label5.Visible = true;
                    }

                    timer1.Start();
                    break;

                case 2:

                    numberOfseed = Int32.Parse(textBox4.Text);
                    setColorTable();
                    while(actualseed<numberOfseed)
                    {
                        int x = rnd.Next(width);
                        int y = rnd.Next(height);
                        if (oldBoard[x][y] != 0)
                        {
                            oldBoard[x][y] = actualseed;
                            g.FillRectangle(new SolidBrush(seedColorTable[actualseed]), x * SIZECELL, y * SIZECELL, SIZECELL, SIZECELL);
                            actualseed++;
                        }
                    }

                    timer1.Start();
                    break;
                case 3:

                    setColorTable();
                   
                    break;
                default:
                    break;
            }
            
        }

        void live()
        {
            if(typeBoundaryConditions==0)
            {
                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                    {
                        if (oldBoard[x][y] == -1)
                        {
                            int[] neighbor = new int[numberOfseed];
                            for (int i = 0; i < numberOfseed; i++)
                                neighbor[i] = 0;

                            if (oldBoard[mod(x - 1, width)][mod(y, height)]>-1)
                            {
                                neighbor[oldBoard[mod(x - 1, width)][mod(y, height)]]++;
                            }
                            if (oldBoard[mod(x + 1, width)][mod(y, height)] > -1)
                            {
                                neighbor[oldBoard[mod(x + 1, width)][mod(y, height)]]++;
                            }
                            if (oldBoard[mod(x , width)][mod(y - 1, height)] > -1)
                            {
                                neighbor[oldBoard[mod(x, width)][mod(y - 1, height)]]++;
                            }
                            if (oldBoard[mod(x, width)][mod(y + 1, height)] > -1)
                            {
                                neighbor[oldBoard[mod(x , width)][mod(y + 1, height)]]++;
                            }

                            
                            if(neighbor.Max()>0)
                            {
                                currentBoard[x][y] = Array.IndexOf(neighbor, neighbor.Max());
                            }
                            else
                            {
                                currentBoard[x][y] = -1;
                            }

                        }
                        else
                            currentBoard[x][y] = oldBoard[x][y];
                    }
            }
            else
            {
                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                    {
                        if (oldBoard[x][y] == -1)
                        {
                            int[] neighbor = new int[numberOfseed];
                            for (int i = 0; i < numberOfseed; i++)
                                neighbor[i] = 0;

                            if (mod2(x - 1, y) > -1)
                                neighbor[oldBoard[x - 1][y]]++;
                            if (mod2(x + 1, y) > -1)
                                neighbor[oldBoard[x + 1][y]]++;
                            if (mod2(x, y + 1) > -1)
                                neighbor[oldBoard[x][y + 1]]++;
                            if (mod2(x, y - 1) > -1)
                                neighbor[oldBoard[x][y - 1]]++;

                            if (neighbor.Max() > 0)
                            {
                                currentBoard[x][y] = Array.IndexOf(neighbor, neighbor.Max());
                            }
                            else
                            {
                                currentBoard[x][y] = -1;
                            }

                        }
                        else
                            currentBoard[x][y] = oldBoard[x][y];
                    }
            }
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    if (oldBoard[x][y] != currentBoard[x][y])
                    {
                        if (currentBoard[x][y] > -1)
                            g.FillRectangle(new SolidBrush(seedColorTable[currentBoard[x][y]]), x * SIZECELL, y * SIZECELL, SIZECELL, SIZECELL);
                    }
                    oldBoard[x][y] = currentBoard[x][y];
                }
        }

        void setColorTable()
        {
            seedColorTable = new Color[numberOfseed];
            for (int i = 0; i < numberOfseed; i++)
            {
                seedColorTable[i] = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            set();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            typeBoundaryConditions = 1;
            initial();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            typeBoundaryConditions = 0;
            initial();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            live();
            if(!Array.Exists(oldBoard, w => Array.Exists(w, k => k.Equals(-1))))
            {
                timer1.Stop();
                label6.Text = "End procesing";
            }
            else
            {
                label6.Text = "Procesing";
            }

        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if(oldBoard[e.X/SIZECELL][e.Y / SIZECELL] !=-1)
            {
                numberOfseed++;
                Color[] newseedColorTable=new Color[numberOfseed];
                for (int i = 0; i < numberOfseed - 1; i++)
                    newseedColorTable[i] = seedColorTable[i];

                oldBoard[e.X / SIZECELL][e.Y / SIZECELL] = numberOfseed;
                newseedColorTable[numberOfseed-1] = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
               
                
                seedColorTable = newseedColorTable;

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            
            if(isManualDefinitionStart)
            {
                timer1.Start();
                button4.Visible = false;
                button4.Text = "Rozpocznij definicje ręczną";
            }
            else
            {
                initial();
                button4.Text = "Zakończ ręczną definicję";
                isManualDefinitionStart = true;

            }
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            timer1.Stop();
            label4.Visible = true;
            textBox4.Visible = true;
            label3.Visible = true;
            textBox3.Visible = true;
            label5.Visible = false;
            label6.Visible = true;
            button4.Visible = false;
            label6.Text = "Status";
            pictureBox1.Refresh();
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    label3.Text = "Ilość w wierszu";
                    label4.Text = "Ilość w kolumnie";
                    break;
                case 1:
                    label3.Text = "Promień";
                    label4.Text = "Ilość";
                    break;
                case 2:
                    label3.Visible = false;
                    textBox3.Visible = false;
                    label4.Text = "Ilość";
                    break;
                case 3:
                    label3.Visible = false;
                    textBox3.Visible = false;
                    label4.Visible=false;
                    textBox4.Visible = false;
                    button4.Visible = true;
                    break;
                default:
                    break;
            }
        }
    }
}
