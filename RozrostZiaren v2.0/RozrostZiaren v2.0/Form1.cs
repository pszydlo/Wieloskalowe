using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RozrostZiaren_v2._0
{
    public partial class Form1 : Form
    {
        private Graphics g;
        private Pen p;
        private int width, height;
        private const int SIZECELL = 2;
        private Cell[][] oldBoard;
        private Cell[][] currentBoard;
        private int numberOfseed = 0;
        private int seeded = 0;
        private double radius;
        private bool IsPeriodic;
        private Color [] colorArray;
        private List<Point> listPoints;
        private Dictionary<int, Color> energyColor;
        private List<double> roList;
        private List<double> sigmaList;
        private double procRo = 0.3;
        

        private Random rnd = new Random();
        public class Cell
        {
            public double x;
            public double y;
            public int state;
            public Color color;
            public int energy;
            public double densityOfDislocations;
            public bool IsRecrystallized;
            public bool IsNeighbor;
            public Cell(int x,int y,int state)
            {
                Random random = new Random();
                this.x = random.NextDouble() * SIZECELL + x*SIZECELL;
                this.y = random.NextDouble() * SIZECELL + y*SIZECELL;
                this.state = state;
                this.color = Color.White;
                this.IsRecrystallized = false;
                densityOfDislocations = 0;
                energy = 0;
                IsNeighbor = false;
            }

            public double len(double x1,double y1)
            {
                return Math.Sqrt(Math.Pow(this.x - x1, 2) + Math.Pow(this.y - y1, 2));
            }
        }
        public Form1()
        {
            InitializeComponent();
     
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Von Neumann");
            comboBox1.Items.Add("Pentagonalne lewe");
            comboBox1.Items.Add("Heksagonalne Prawe");
            comboBox1.Items.Add("Heksagonalne losowe");
            comboBox1.Items.Add("Heksagonalne Losowe");
            comboBox1.Items.Add("Moore");
            comboBox1.Items.Add("Z promieniem");

            comboBox2.Items.Clear();
            comboBox2.Items.Add("Jednorodne");
            comboBox2.Items.Add("Z promieniem");
            comboBox2.Items.Add("Losowe");
            comboBox2.Items.Add("Ręczna definicja");
        }
        int mod(int x, int m)
        {
            return (IsPeriodic?((x % m + m) % m):((x<0||x>m)?-1:x));
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            IsPeriodic = true;
            radioButton2.Checked = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            IsPeriodic = false;
            radioButton1.Checked = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            seeded = 0;
            set();
            initial();
        }
        void copyCell(Cell from, Cell to)
        {
            to.x=from.x;
            to.y=from.y;
            to.state=from.state;
            to.color=from.color;
            to.energy = from.energy;
         }
        void set()
        {
            width = (int)numericUpDown1.Value;
            height = (int)numericUpDown2.Value;
            pictureBox1.Width = width*SIZECELL;
            pictureBox1.Height = height*SIZECELL;
            oldBoard = new Cell[width][];
            currentBoard = new Cell[width][];
            numericUpDown1.Minimum = 1;
            numericUpDown2.Minimum = 1;
            numericUpDown3.Minimum = 1;
            numericUpDown4.Minimum = 1;
            listPoints = new List<Point>();
            energyColor = new Dictionary<int, Color>();
            energyColor.Add(0, Color.White);
            roList = new List<double>();
            sigmaList = new List<double>();
            
            p = new Pen(Color.Black, 1);
            for (int i = 0; i < width; i++)
            {
                oldBoard[i] = new Cell[height];
                currentBoard[i] = new Cell[height];
            }
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    oldBoard[i][j] = new Cell(i, j, -1);
                    currentBoard[i][j]= new Cell(i, j, -1);
                    listPoints.Add(new Point(i, j));
                }
            g = pictureBox1.CreateGraphics();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            live();
            if (!Array.Exists(oldBoard, w => Array.Exists(w, k => k.state==-1)))
            {
                timer1.Stop();
                label7.Text = "End procesing";
            }
            else
            {
                label7.Text = "Procesing";
            }
        }

        void initial()
        {
            pictureBox1.Refresh();
            
            int actualseed = 0;
            int leftSeed = 0;
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    {
                        int row = Int32.Parse( numericUpDown3.Value.ToString());
                        int column = Int32.Parse(numericUpDown4.Value.ToString());
                        numberOfseed = row * column;
                        colorArray = new Color[numberOfseed];
                        int xDif = width / (row);
                        int yDif = height / (column);
                        for (int a = 0, x1 = xDif / 2; a < row; a++, x1 += xDif)
                        {
                            for (int b = 0, y1 = yDif / 2; b < column; b++, y1 += yDif)
                            {
                                oldBoard[x1][y1].state= actualseed;
                                oldBoard[x1][y1].color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                                colorArray[actualseed] = oldBoard[x1][y1].color;
                                g.FillRectangle(new SolidBrush(oldBoard[x1][y1].color), x1 * SIZECELL, y1 * SIZECELL, SIZECELL, SIZECELL);
                                actualseed++;
                            }
                        }
                        timer1.Start();
                    }
                    break;

                case 1:
                    {
                        radius = (double) numericUpDown3.Value;
                        numberOfseed = (int)numericUpDown4.Value;
                        leftSeed = numberOfseed;
                        colorArray = new Color[numberOfseed];
                        int radiausInt = (int)radius + 1;

                        for (int i = 0; i < (20 * numberOfseed) && actualseed < numberOfseed; i++)
                        {
                            bool isSeeded = true;
                            int x = rnd.Next(width);
                            int y = rnd.Next(height);

                            for (int a = mod(x - radiausInt, width) < 0 ? 0 : mod(x - radiausInt, width);
                                    mod(a, width) < (mod(x + radiausInt+1, width) < 0 ? width : mod(x + radiausInt +1, width))
                                    ; a++)
                                for (int b = mod(y - radiausInt, height) < 0 ? 0 : mod(y - radiausInt, height);
                                        mod(b, height) < (mod(y + radiausInt +1, height) < 0 ? height : mod(y + radiausInt +1, height))
                                        ; b++)
                                {
                                    if (oldBoard[a][b].len(oldBoard[x][y].x, oldBoard[x][y].y) < radius && oldBoard[a][b].state > -1)
                                        isSeeded = false;
                                }

                            if (isSeeded)
                            {
                                oldBoard[x][y].state = actualseed;
                                oldBoard[x][y].color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                                colorArray[actualseed] = oldBoard[x][y].color;
                                g.FillRectangle(new SolidBrush(oldBoard[x][y].color), x * SIZECELL, y * SIZECELL, SIZECELL, SIZECELL);
                                actualseed++;
                                leftSeed--;

                            }
                        }

                        if (leftSeed > 0)
                        {
                            label7.Text = "Pozostało \n" + leftSeed + "\nziaren których nie dało się pozadzić";
                            numberOfseed = actualseed;
                        }

                        timer1.Start();
                    }
                    break;

                case 2:
                    {
                        numberOfseed = (int)numericUpDown3.Value;
                        radius = (double)numericUpDown4.Value;
                        colorArray = new Color[numberOfseed];
                        while (actualseed < numberOfseed)
                        {
                            int x = rnd.Next(width);
                            int y = rnd.Next(height);
                            if (oldBoard[x][y].state == -1)
                            {
                                oldBoard[x][y].state = actualseed;
                                oldBoard[x][y].color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                                colorArray[actualseed] = oldBoard[x][y].color;
                                g.FillRectangle(new SolidBrush(oldBoard[x][y].color), x * SIZECELL, y * SIZECELL, SIZECELL, SIZECELL);
                                actualseed++;
                            }
                        }

                        timer1.Start();
                    }
                    break;
                case 3:
                    {
                        numberOfseed = (int)numericUpDown3.Value;
                        radius= (double)numericUpDown4.Value;
                        colorArray = new Color[numberOfseed];
                    }
                    break;

                default:
                    break;
            }
        }
    
        Dictionary<int,int> popularNeighbor(int x, int y)
        {
            int[] neighbor = new int[numberOfseed];
            for (int i = 0; i < numberOfseed; i++)
                neighbor[i] = 0;
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    {
                        if ((mod(x - 1, width) < 0 || (mod(y, height) < 0)) ? false : (oldBoard[mod(x - 1, width)][y].state > -1 ? true : false))
                            neighbor[oldBoard[mod(x - 1, width)][y].state]++;
                        if ((mod(x + 1, width) < 0 || (mod(y, height) < 0)) ? false : (oldBoard[mod(x + 1, width)][y].state > -1 ? true : false))
                            neighbor[oldBoard[mod(x + 1, width)][y].state]++;
                        if ((mod(x, width) < 0 || (mod(y - 1, height) < 0)) ? false : (oldBoard[x][mod(y - 1, height)].state > -1 ? true : false))
                            neighbor[oldBoard[x][mod(y - 1, height)].state]++;
                        if ((mod(x, width) < 0 || (mod(y + 1, height) < 0)) ? false : (oldBoard[x][mod(y + 1, height)].state > -1 ? true : false))
                            neighbor[oldBoard[x][mod(y + 1, height)].state]++;
                    }
                    break;

                case 1:
                    {
                        neighbor = neighborCounter(3, true, false, 1, 1, neighbor, x, y, x, y);
                    }
                    break;

                case 2:
                    {
                        neighbor = neighborCounter(0, false, false, 1, 1, neighbor, x, y, x, y);
                    }
                    break;
                case 3:
                    {
                        neighbor = neighborCounter(1, false, false, 1, 1, neighbor, x, y, x, y);
                    }
                    break;
                case 4:
                    {
                        neighbor = neighborCounter(rnd.Next(3), false, false, 1, 1, neighbor, x, y, x, y);
                    }
                    break;
                case 5:
                    {
                        neighbor = neighborCounter(3, false, false, 1, 1, neighbor, x, y, x, y);
                    }
                    break;
                case 6:
                    {
                        neighbor = neighborCounter(3, false, true, (int)radius + 1, radius * SIZECELL, neighbor, x, y, oldBoard[x][y].x, oldBoard[x][y].y);
                    }
                    break;

                default:
                    break;
            }
            Dictionary<int,int> neighborList=new Dictionary<int,int>();

                for (int i = 0; i < numberOfseed; i++)
                    if (neighbor[i] != 0)
                        neighborList.Add(i, neighbor[i]);
            return neighborList;
        }

        int [] neighborCounter (int Hex , bool IsTypePentagonal, bool IsRadiaus, int radiausInt, double radiausDouble, int [] neighbor, int x, int y, double x1,double y1)
        {
            int type = 5;
            if(IsTypePentagonal)
                type= rnd.Next(5);

            for (int a =( mod(x - radiausInt + (type == 1 ? 1 : 0), width) < 0 ? 0 : mod(x - radiausInt + (type == 1 ? 1 : 0), width));
                                    mod(a, width) < (mod(x + radiausInt +1 - (type == 0 ? 1 : 0), width) < 0 ? width : mod(x + radiausInt +1  - (type == 0 ? 1 : 0), width))
                                    ; a++)
                for (int b =( mod(y - radiausInt + (type == 2 ? 1 : 0), height) < 0 ? 0 : mod(y - radiausInt + (type == 2 ? 1 : 0), height));
                        mod(b, height) < (mod(y + radiausInt + 1 - (type == 3 ? 1 : 0), height) < 0 ? height : mod(y + radiausInt + 1 - (type == 3 ? 1 : 0), height))
                        ; b++)
                {
                    if (oldBoard[a][b].state > -1 &&
                        (Hex==0 ? ((a == x - 1 && b == y - 1) || (a == x + 1 && b == y + 1) ? false : true) 
                        : (Hex == 1 ?((a == x - 1 && b == y + 1) || (a == x + 1 && b == y - 1) ? false : true)
                        : true)))
                        if(IsRadiaus?oldBoard[a][b].len(x1,y1)<radiausDouble:true)
                            neighbor[oldBoard[a][b].state]++;

                }
            return neighbor;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    {
                        label5.Text = "X";
                        label6.Text = "Y";
                    }
                    break;
                case 1:
                    {
                        label5.Text = "Radiaus";
                        label6.Text = "Number of seed";
                    }
                    break;
                case 2:
                    {
                        label5.Text = "Number of seed";
                        label6.Text = "Radiaus";
                    }
                    break;
                default:
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            pictureBox1.Refresh();
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    oldBoard[i][j] = new Cell(i, j, -1);
                }

        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            int Xmouse = e.X / SIZECELL, Ymouse = e.Y / SIZECELL;
            if (oldBoard[Xmouse][Ymouse].state == -1)
            {
                oldBoard[Xmouse][Ymouse].state = seeded;
                colorArray[seeded] = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                oldBoard[Xmouse][Ymouse].color = colorArray[seeded];
                g.FillRectangle(new SolidBrush(colorArray[seeded]), Xmouse * SIZECELL, Ymouse * SIZECELL, SIZECELL, SIZECELL);
                seeded++;
                

            }
            if (seeded>=numberOfseed)
                {
                    timer1.Start();
                }
                
                    
        }

        void live()
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    copyCell(oldBoard[x][y], currentBoard[x][y]);
                    if(currentBoard[x][y].state==-1)
                    {
                        List<int> state=popularNeighbor(x, y).Keys.ToList();
                        currentBoard[x][y].state = state.Count == 0 ? -1 : state.Max();
                        if (currentBoard[x][y].state != -1)
                        {
                            currentBoard[x][y].color = colorArray[currentBoard[x][y].state];
                            g.FillRectangle(new SolidBrush(currentBoard[x][y].color), x * SIZECELL, y * SIZECELL, SIZECELL, SIZECELL);
                        }
                    }  
                }
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    copyCell(currentBoard[x][y], oldBoard[x][y]);
                }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            monteCarlo();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    g.FillRectangle(new SolidBrush(energyColor[oldBoard[x][y].energy]), x * SIZECELL, y * SIZECELL, SIZECELL, SIZECELL);
                }
        }

        void monteCarlo()
        {
            label7.Text = "Procesing";
            double k = double.Parse(textBox1.Text);
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    listPoints.Add(new Point(i, j));
                }
            for(int i=0;i<(width-1)*(height-1);i++)
            {
                int los=rnd.Next(listPoints.Count);
                Point p = listPoints[los];
                listPoints.Remove(p);
                List<int> vs=popularNeighbor(p.X, p.Y).Keys.ToList();
                if (vs.Contains(oldBoard[p.X][ p.Y].state))
                {
                    vs.Remove(oldBoard[p.X][p.Y].state);
                }
                int Ebefore = vs.Count;
                int state =Ebefore>0? vs[rnd.Next(Ebefore)]: oldBoard[p.X][p.Y].state;
                vs = popularNeighbor(p.X, p.Y).Keys.ToList();
                if (vs.Contains(state))
                {
                    vs.Remove(state);
                }
                int Eafter = vs.Count;

                int delta = Eafter - Ebefore;

                if(delta<=0)
                {
                    oldBoard[p.X][p.Y].state = state;
                    oldBoard[p.X][p.Y].color = colorArray[state];
                    oldBoard[p.X][p.Y].energy = Eafter;
                    if(energyColor.Keys.Count==0|| !energyColor.ContainsKey(Eafter))
                    {
                        energyColor.Add(Eafter, Color.FromArgb(255, rnd.Next(256), rnd.Next(256)));
                    }
                        


                }
                else
                {
                    if(rnd.NextDouble()<Math.Exp(-delta/k))
                    {
                        oldBoard[p.X][p.Y].state = state;
                        oldBoard[p.X][p.Y].color = colorArray[state];
                        oldBoard[p.X][p.Y].energy = Eafter;
                        if (energyColor.Keys.Count == 0 || !energyColor.ContainsKey(Eafter))
                        {
                            energyColor.Add(Eafter, Color.FromArgb(255, rnd.Next(256), rnd.Next(256)));
                        }
                    }
                }

            }
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    g.FillRectangle(new SolidBrush(oldBoard[x][y].color), x * SIZECELL, y * SIZECELL, SIZECELL, SIZECELL);
                }
            label7.Text = "End Procesing";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DRX();
        }

        void DRX ()
        {
            double A = double.Parse(textBox2.Text);
            double B = double.Parse(textBox3.Text);
            double time = double.Parse(textBox8.Text);
            double stepTime = double.Parse(textBox9.Text);
            double roCritical = double.Parse(textBox4.Text)/(width*height);
            Console.WriteLine(roCritical);
            double ro,deltaRo;
            double roForAll;
            
            List<Point> recrystalizedCellOld = new List<Point>();
            List<Point> recrystalizedCellNew = new List<Point>();
            roList.Add(0);

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    List<int> vs = popularNeighbor(i, j).Keys.ToList();
                    if (vs.Contains(oldBoard[i][j].state))
                    {
                        vs.Remove(oldBoard[i][j].state);
                    }
                    if (vs.Count > 0)
                        oldBoard[i][j].IsNeighbor = true;
                }
            

            for (double t=0;t<time;t+=stepTime)
            {
                ro = A / B + (1 - A / B) * Math.Exp(-B * t);
                deltaRo = -roList.Last();
                roList.Add(ro);
                deltaRo += ro;
                roForAll =procRo* deltaRo / (height * width);
                
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                    {
                        oldBoard[i][j].densityOfDislocations = roForAll;
                    }
                
                deltaRo = (1 - procRo) * deltaRo;
                bool IsAssigned;

                roForAll = deltaRo * (rnd.NextDouble() / 100);
                while (deltaRo>0)
                {
                    IsAssigned = false;
                    
                    if (deltaRo-roForAll<0)
                    {
                        roForAll = deltaRo;
                        deltaRo = 0;
                    }

                    int x = rnd.Next(width);
                    int y = rnd.Next(height);
                    if(oldBoard[x][y].IsNeighbor)
                    {
                        if(rnd.NextDouble()<0.8)
                        {
                            oldBoard[x][y].densityOfDislocations += roForAll;
                            IsAssigned = true;
                        }
                    }
                    else
                    {
                        if (rnd.NextDouble() < 0.2)
                        {
                            oldBoard[x][y].densityOfDislocations += roForAll;
                            IsAssigned = true;
                        }
                    }
          
                    if(IsAssigned)
                    {
                        deltaRo -= roForAll;
                    }
                }

                if(recrystalizedCellOld.Count>0)
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                    {
                        if(recrystalizedCellOld
                            .Any(v=>(
                                    (v.X==mod(i-1,width) && v.Y==j) ||
                                    (v.X == mod(i + 1, width) && v.Y == j) ||
                                    (v.X  == i && v.Y == mod(j - 1, height)) ||
                                    (v.X == i && v.Y == mod(j + 1, height)) 
                                    )))
                            
                            if(
                                oldBoard[i][j].densityOfDislocations> oldBoard[mod(i - 1, width)][j].densityOfDislocations &&
                                oldBoard[i][j].densityOfDislocations > oldBoard[mod(i + 1, width)][j].densityOfDislocations &&
                                oldBoard[i][j].densityOfDislocations > oldBoard[i][mod(j - 1, height)].densityOfDislocations &&
                                oldBoard[i][j].densityOfDislocations > oldBoard[i][mod(j + 1, height)].densityOfDislocations 
                                )
                            {
                                oldBoard[i][j].color = Color.GreenYellow;
                                oldBoard[i][j].densityOfDislocations = 0;
                                oldBoard[i][j].state = numberOfseed;
                                recrystalizedCellNew.Add(new Point(i, j));
                            }
                    }


                  for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                    {
                       if( oldBoard[i][j].densityOfDislocations>roCritical && oldBoard[i][j].IsNeighbor)
                        {
                            oldBoard[i][j].color = Color.GreenYellow;
                            oldBoard[i][j].densityOfDislocations = 0;
                            oldBoard[i][j].state = numberOfseed;
                            recrystalizedCellNew.Add(new Point(i, j));
                        }
                    }
                recrystalizedCellOld.RemoveRange(0, recrystalizedCellOld.Count);
                recrystalizedCellOld.AddRange(recrystalizedCellNew);

                label7.Refresh();
                label7.Text = "Time: " + t;

                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                    {
                        g.FillRectangle(new SolidBrush(oldBoard[x][y].color), x * SIZECELL, y * SIZECELL, SIZECELL, SIZECELL);
                    }
            }
            string path = @"C:\Users\piotr\source\repos\Wieloskalowe\RozrostZiaren v2.0\RozrostZiaren v2.0\data.txt";
            StreamWriter sw = File.CreateText(path);
            
            for(int i=0;i<roList.Count;i++)
            {
                sw.WriteLine(i + " " + roList[i]);
            }
            sw.Close();

                
            
            label7.Refresh();
            label7.Text = "End";

        }
    }
}
