using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CopterGame
{
    public partial class Form1 : Form
    {
        Copter Tardis = new Copter();
        Graphics g;
        Timer GameTimer = new Timer();
        Rectangle recBack = new Rectangle();
        Bitmap bmpBack = CopterGame.Properties.Resources._new;
        Rectangle recBack2 = new Rectangle();
        Bitmap bmpBack2 = CopterGame.Properties.Resources.Galaxy2;
        ImageAttributes attr = new ImageAttributes();
        List<Terrain> terrainList = new List<Terrain>();
        int[,]  coordsArray = new int[5000, 90];
        double  nextImage = 0;
        int throttle = 0;
        bool MouseD = false;
        int XGenerate = 91; 
        Random newrandom = new Random();
        bool alive = true;
        double  ExImage = -1;
        Button Play = new Button();
        Button createLevel = new Button();
        Button btnQuit = new Button();
        Font buttonFont = new Font("Impact", 25);
        Button btnRestart = new Button();
        Button btnMenu = new Button();
        Font gameFont = new Font("Impact", 20); Font gameFont2 = new Font("Impact", 20);
        Label lblScore = new Label();
        Score gameScore = new Score();
        public Form1()
        {
            InitializeComponent();
            lblScore.FlatStyle = FlatStyle.Flat; //lblScore.TextAlign = ContentAlignment.MiddleCenter;
            lblScore.Height = 40; lblScore.Width = 200; lblScore.Font = gameFont2; lblScore.Visible =false; lblScore.BackColor = Color.FromArgb(61,73,128);
            lblScore.ForeColor = Color.Black; lblScore.Text = "Score: 0"; lblScore.BorderStyle = BorderStyle.FixedSingle; 
           // this.Controls.Add(btnMenu); this.Controls.Add(btnRestart);
            this.Controls.Add(lblScore); lblScore.Top = 511; lblScore.Left = 3;
            recBack.X = 0; recBack.Y = 0; recBack.Width = bmpBack.Width; recBack.Height = bmpBack.Height;
            recBack2.X = recBack.Right ; recBack2.Y = 0; recBack2.Width = bmpBack2.Width; recBack2.Height = bmpBack2.Height;
            Play.FlatStyle = FlatStyle.Flat;
            createLevel.FlatStyle = FlatStyle.Flat; btnQuit.FlatStyle = FlatStyle.Flat;
            Play.Click += Play_Click;
            Play.Height = 50;
            Play.Width = 160;
            Play.BackColor = Color.FromArgb(61,73,128);
            Play.ForeColor = Color.Black;
            Play.TextAlign = ContentAlignment.MiddleCenter; createLevel.TextAlign = ContentAlignment.MiddleCenter; btnQuit.TextAlign = ContentAlignment.MiddleCenter;
            btnMenu.TextAlign = ContentAlignment.MiddleCenter; btnRestart.TextAlign = ContentAlignment.MiddleCenter;
            btnQuit.Height = 50;
            btnQuit.Width = 160;
            btnQuit.BackColor = Color.FromArgb(61, 73, 128);
            btnQuit.ForeColor = Color.Black;

            createLevel.Height = 50;
            createLevel.Width = 160;
            createLevel.BackColor = Color.FromArgb(61, 73, 128);
            createLevel.ForeColor = Color.Black;

            createLevel.Text = "INFO";
            createLevel.Font = buttonFont;
            btnQuit.Text = "QUIT";
            btnQuit.Font = buttonFont;

            btnRestart.Height = 40; btnRestart.Width = 150; btnRestart.Text = "RESTART"; btnRestart.Left = 570; btnRestart.Top = 511;
            btnRestart.Visible = false; btnRestart.Font = gameFont; btnRestart.FlatStyle = FlatStyle.Flat;

            btnRestart.BackColor = Color.FromArgb(61, 73, 128);
            btnRestart.ForeColor = Color.Black;
            btnMenu.BackColor = Color.FromArgb(61, 73, 128);
            btnMenu.ForeColor = Color.Black;


            btnMenu.Height = 40; btnMenu.Width = 150; btnMenu.Text = "MENU"; btnMenu.Left = 730; btnMenu.Top = 511;
            btnMenu.Visible = false; btnMenu.Font = gameFont; btnMenu.FlatStyle = FlatStyle.Flat;

            this.Controls.Add(btnMenu); this.Controls.Add(btnRestart);

            btnMenu.Click += btnMenu_Click;
            btnRestart.Click += btnRestart_Click;
            Play.Text = "PLAY";
            Play.Font = buttonFont;
            Play.Left = 195; Play.Top = 300; btnQuit.Left = 530; btnQuit.Top = 300; createLevel.Left = 365; createLevel.Top = 300;
            this.Controls.Add(Play); this.Controls.Add(btnQuit); this.Controls.Add(createLevel);
            btnQuit.Click += btnQuit_Click;
            createLevel.Click += createLevel_Click;
            GameTimer.Enabled = false;
            GameTimer.Interval = 50;
            GameTimer.Tick += GameTimer_Tick;
            this.BackColor = Color.Black;
            this.Text = "Doctor Who Copter";
            this.Paint += Form1_Paint;
            this.DoubleBuffered = true;
            this.MouseDown += Form1_MouseDown;
            this.MouseUp += Form1_MouseUp;
            this.Height = 590;
            this.Width = 900;
            //this.BackgroundImage = CopterGame.Properties.Resources.Galaxy;
            //Create the 90*51 screen of terrain

            for (int j = 0; j < 51; j++)
            {

                for (int i = 0; i < 90; i++)
                {
                    Terrain trn = new Terrain(i * 10, j * 10);
                    terrainList.Add(trn);
                    //Mark that piece of terrain to not show up in program 
                    terrainList[(j * 90) + i].SetShow(false);
                }
            }

            
        }

        void btnRestart_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 4999; i++)
            {
                for (int j = 0; j < 90; j++)
                {
                    coordsArray[i, j] = 0;
                }
            }
            Tardis.Reset();
            GameTimer.Enabled = true;
            gameScore.Reset();lblScore.Visible = true;
            XGenerate = 91; ExImage = -1; alive = true; MouseD = false;
            Play.Visible = false; createLevel.Visible = false;
            btnQuit.Visible = false; btnRestart.Visible = true; 
            btnMenu.Visible = true;
            bmpBack = CopterGame.Properties.Resources.Galaxy;
            recBack.X = 0; recBack.Y = 0; recBack.Width = bmpBack.Width; recBack.Height = bmpBack.Height;
            recBack2.X = recBack.Right; recBack2.Y = 0; recBack2.Width = bmpBack2.Width; recBack2.Height = bmpBack2.Height;
        }

        void btnMenu_Click(object sender, EventArgs e)
        {
            GameTimer.Enabled = false;
            bmpBack = CopterGame.Properties.Resources._new;
            recBack.X = 0; recBack.Y = 0; recBack.Width = bmpBack.Width; recBack.Height = bmpBack.Height;
            recBack2.X = recBack.Right; recBack2.Y = 0; recBack2.Width = bmpBack2.Width; recBack2.Height = bmpBack2.Height;
            Invalidate();
            Play.Visible = true; createLevel.Visible = true; btnQuit.Visible = true; btnRestart.Visible = false; btnMenu.Visible = false; lblScore.Visible = false;

        }

        void btnQuit_Click(object sender, EventArgs e)
        {
            Close();

        }

        void createLevel_Click(object sender, EventArgs e)
        {
            MessageBox.Show("             Welcome to Doctor Who Copter! See how far you can venture into the time vortex without destroying the space time continuum. Click the mouse anywhere on the screen to raise the Tardis and save it from crashing. Don't go too high though, you could crash right into the top. ", "Read Me");
        }

        void Play_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 4999; i++)
            {
                for (int j = 0; j < 90; j++)
                {
                    coordsArray[i, j] = 0;
                }
            }
            Tardis.Reset();
            GameTimer.Enabled = true;lblScore.Visible = true;
            gameScore.Reset();
            XGenerate = 91; ExImage = -1; alive = true; MouseD = false;
            Play.Visible = false; createLevel.Visible = false; 
            btnQuit.Visible = false; btnRestart.Visible = true;
            btnMenu.Visible = true ; 
            bmpBack = CopterGame.Properties.Resources.Galaxy;
            recBack.X = 0; recBack.Y = 0; recBack.Width = bmpBack.Width; recBack.Height = bmpBack.Height;
            recBack2.X = recBack.Right; recBack2.Y = 0; recBack2.Width = bmpBack2.Width; recBack2.Height = bmpBack2.Height;

            
            // Create the entire map. \

           // fillCoords();
        }
        

        void Form1_MouseUp(object sender, MouseEventArgs e)
        {

            if (alive == true)
            {
                MouseD = false;
            }
        }

        void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (alive == true)
            {
                MouseD = true;
            }
        }

        void Form1_Paint(object sender, PaintEventArgs e)
        {
            g = e.Graphics;

            DrawBack(g);
            if (GameTimer.Enabled == true)
            {
                foreach (Terrain trn in terrainList)
                {
                    trn.Draw(g);

                }
                Tardis.DrawTardis(g);
                if (alive == false)
                {
                    Tardis.DrawExplosion(g);
                }
            }
        }

        public void DrawBack(Graphics gg)
        {
             gg.DrawImage(bmpBack, recBack, 0, 0, bmpBack.Width, bmpBack.Height, GraphicsUnit.Pixel, attr);
             gg.DrawImage(bmpBack2, recBack2, 0, 0, bmpBack2.Width, bmpBack2.Height, GraphicsUnit.Pixel, attr);
        }

        public void doesCollide()
        {
            
            foreach (Terrain trn in terrainList)
            {
                if (trn.GetRec().X < 320 && trn.GetRec().X > 230 && trn.GetShow() == true)
                {
                    
                        if (Tardis.GetBody().IntersectsWith(trn.GetRec()) || (Tardis.GetLight().IntersectsWith(trn.GetRec())))
                        {
                            alive = false;
                            //GameTimer.Enabled = false;
                        }

                   
                }
            }
            
            
           
        }

        void GameTimer_Tick(object sender, EventArgs e)
        {
            nextImage = nextImage + 1;
            Tardis.changePic(nextImage);

            if (nextImage == 28)
            {
                nextImage = 0;

            }
            if (alive == true)
            {
               
                if (MouseD == true && throttle <= 11)
                    throttle += 3;
                else if (MouseD == false && throttle >= -14)
                {
                    throttle -= 5;
                }
                Tardis.changeTop(throttle);
                

            
            fillCoords();
            ShiftTerrain();
            DrawOrNot();
            }

            if (alive == false)
            {
                ExImage+=1;
                //this.Text = ExImage.ToString();
                Tardis.ChanegExplosion(ExImage);
                //throttle -= 5;
                //Tardis.changeTop(throttle);

            }
            Invalidate();
           
            doesCollide();
         }

        public void fillCoords()
        {
            if (XGenerate <= 300)
            {
                int Xone = newrandom.Next(XGenerate + 15, XGenerate + 25);

                int Xtwo = newrandom.Next(Xone +1, Xone + 20);

                int Yone = newrandom.Next(3, 49);
                int Ytwo = newrandom.Next(Yone + 1, Yone + 15);
               // int count = 0;
                if (Xtwo > 4999)
                {
                    //count++;
                    //string c = count.ToString();

                    //this.Text = c;


                    Xtwo = 4999;

                }
                while (Ytwo > 90)
                {
                    Ytwo = newrandom.Next(Yone + 1, Yone + 15);
                }
                XGenerate = Xtwo;
                MakeSection(Xone, Yone, Xtwo, Ytwo);
            }
            MakeSection(0, 0,4999, 2);
            MakeSection(0, 48, 4999, 51);
            //MakeSection(60, 4, 62, 8);
            //MakeSection(60, 4, 62, 8);
            //MakeSection(360, 4, 362, 8);
        }

        public void MakeSection(int x1, int y1, int x2, int y2)
        {
            for (int i = x1; i <= x2; i++)
            {
                for (int j = y1; j <= y2; j++)
                {
                    coordsArray[i, j] = 1;
                }
            }
        }

        public void DrawOrNot()
        {
            //Determine which terrain should be drawn 
            //This code looks at the coordinates on screen (X * y)
            for (int j = 0; j < 51; j++)
            {
                for (int i = 0; i < 90; i++)
                {
                    //If there is a 1 at spot (i,j) then find the corresponding terrain and set it to show
                    if (coordsArray[i, j] == 1)
                    {
                        terrainList[(j * 90) + i].SetShow(true);

                    }
                    else 
                    {
                        terrainList[(j * 90) + i].SetShow(false);

                    }
                }
            } 
        }

        public void ShiftTerrain()
        {
            //This method takes every coordinate in the level and sets
            // it equal to the coordinate one spot to the right 
            // this is how the level appears to scrool. 
            for (int i = 0; i < 4999; i++)
            {
                for (int j = 0; j < 90; j++)
                {
                    coordsArray[i, j] = coordsArray[i + 1, j];
                }
            }
            XGenerate--;
            recBack.X -= 1;
            recBack2.X -= 1;
            gameScore.Add();
           
            lblScore.Text = "Score: " + (gameScore.GetScore().ToString());
            if (recBack.Right < 0)
            {
                recBack.X = recBack2.Right;
            }
            if (recBack2.Right < 0)
            {
                recBack2.X = recBack.Right;

            }
        }

    }

    public class Copter

         {
             Rectangle recCopter = new Rectangle();
             Bitmap bmpCopter = new Bitmap(CopterGame.Properties.Resources.TardisRotation28);
             Bitmap bmpBlack = new Bitmap(CopterGame.Properties.Resources.black);
             ImageAttributes attr = new ImageAttributes();
             Rectangle recLight = new Rectangle();
             Rectangle recBody = new Rectangle();

               // Explosion graphics
             Rectangle recExplode = new Rectangle();
             Bitmap bmpExplosion = new Bitmap(CopterGame.Properties.Resources.explosion1);
             ImageAttributes attrexplosion = new ImageAttributes();
              
          //   Rectangle 
             public Copter()
             {
                 
                 recCopter.Height = bmpCopter.Height;
                 recCopter.Width = bmpCopter.Width;
                 recCopter.X = 250;
                 recCopter.Y = 50;

                 recLight.X = recCopter.X + 23; recLight.Y = recCopter.Y; recLight.Width = 7; recLight.Height = 8;

                 recBody.X = recCopter.X; recBody.Y = recCopter.Y + recLight.Height;

                 recBody.Width = bmpCopter.Width; recBody.Height = bmpCopter.Height - 8;

                 recExplode.Width = bmpExplosion.Width;
                 recExplode.Height = bmpExplosion.Height;


                 attr.SetColorKey(Color.FromArgb(0, 255, 128), Color.FromArgb(0, 255, 128));
                 attrexplosion.SetColorKey(Color.FromArgb(0, 0, 0), Color.FromArgb(60,99,176));

             }

             public void changeTop (int throttle)
             {
                 recCopter.Y = recCopter.Top -  throttle;
                 recBody.Y = recBody.Top - throttle;
                 recLight.Y = recLight.Top - throttle;

                 recExplode.X = 230; recExplode.Y = recCopter.Y ;
             }

             public void DrawTardis(Graphics g)
             {
                 g.DrawImage(bmpCopter, recCopter, 0, 0, bmpCopter.Width, bmpCopter.Height, GraphicsUnit.Pixel, attr);
              //   g.DrawImage(bmpBlack, recBody, 0, 0, recBody.Width, recBody.Height, GraphicsUnit.Pixel, attr);
                // g.DrawImage(bmpBlack, recLight, 0, 0, recBody.Width, recBody.Height, GraphicsUnit.Pixel, attr);
             }

             public void ChanegExplosion(double explosionImage)
             {
                 if (explosionImage == 1)
                 {
                     bmpExplosion = CopterGame.Properties.Resources.explosion1;
                 }
                 else if (explosionImage == 2)
                 {
                     bmpExplosion = CopterGame.Properties.Resources.explosion2;
                 }
                 else if (explosionImage == 3)
                 {
                     bmpExplosion = CopterGame.Properties.Resources.explosion3;
                 }
                 else if (explosionImage == 4)
                 {
                     bmpExplosion = CopterGame.Properties.Resources.explosion4;
                 }
                 else if (explosionImage == 5)
                 {
                     bmpExplosion = CopterGame.Properties.Resources.explosion5;
                 }
                 else if (explosionImage == 6)
                 {
                     bmpExplosion = CopterGame.Properties.Resources.explosion6;
                 }

                 if (explosionImage > 6 && (explosionImage / 2 == Math.Floor(explosionImage/2)))
                 {
                     bmpExplosion = CopterGame.Properties.Resources.explosion6;
                 }
                 else if (explosionImage > 6 && (explosionImage / 2 != Math.Floor(explosionImage/2)))
                 {
                     bmpExplosion = CopterGame.Properties.Resources.explosion7;
                 }
                 //recExplode.X = 250; recExplode.Y = recCopter.Y + 25;

             }

             public void DrawExplosion(Graphics g)
             {
                 g.DrawImage(bmpExplosion, recExplode, 0, 0, bmpExplosion.Width, bmpExplosion.Height, GraphicsUnit.Pixel, attrexplosion);
             }

             public void changePic(double nextimage)
             {
                 if (nextimage == 1)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation1;
                 }
                 else if (nextimage == 2)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation2;
                 }
                 else if (nextimage == 3)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation3;
                 }
                 else if (nextimage == 4)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation4;
                 }
                 else if (nextimage == 5)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation5;
                 }
                 else if (nextimage == 6)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation6;
                 }
                 else if (nextimage == 7)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation7;
                 }
                 else if (nextimage == 8)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation8;
                 }
                 else if (nextimage == 9)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation9;
                 }
                 else if (nextimage == 10)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation10;
                 }
                 else if (nextimage == 11)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation11;
                 }
                 else if (nextimage == 12)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation12;
                 }
                 else if (nextimage == 13)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation13;
                 }
                 else if (nextimage == 14)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation14;
                 }
                 else if (nextimage == 15)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation15;
                 }
                 else if (nextimage == 16)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation16;
                 }
                 else if (nextimage == 17)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation17;
                 }
                 else if (nextimage == 18)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation18;
                 }
                 else if (nextimage == 19)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation19;
                 }
                 else if (nextimage == 20)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation20;
                 }
                 else if (nextimage == 21)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation21;
                 }
                 else if (nextimage == 22)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation22;
                 }
                 else if (nextimage == 23)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation23;
                 }
                 else if (nextimage == 24)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation24;
                 }
                 else if (nextimage == 25)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation25;
                 }
                 else if (nextimage == 26)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation26;
                 }
                 else if (nextimage == 27)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation27;
                 }
                 else if (nextimage == 28)
                 {
                     bmpCopter = CopterGame.Properties.Resources.TardisRotation28;
                 }

                 //Change light collision code 
                 if (nextimage == 1 || nextimage == 8 || nextimage == 15 || nextimage == 22)
                 {
                     recLight.X = recBody.X + 23;
                 }
                 else if (nextimage == 2 || nextimage == 3 || nextimage == 9 || nextimage == 10 || nextimage == 16 || nextimage == 17 || nextimage == 23 || nextimage == 24)
                 {
                     recLight.X = recBody.X + 27;
                 }
                 else
                 {
                     recLight.X = recBody.X + 26;
                 }
                 
             }

             public Rectangle GetBody()
             {
                 return recBody;
             }

             public Rectangle GetLight()
             {
                 return recLight; 
             }

             public void Reset()
             {
                 bmpCopter = new Bitmap(CopterGame.Properties.Resources.TardisRotation28);
                 recCopter.Height = bmpCopter.Height;
                 recCopter.Width = bmpCopter.Width;
                 recCopter.X = 250; recCopter.Y = 50;
                 recLight.X = recCopter.X + 23; recLight.Y = recCopter.Y; recLight.Width = 7; recLight.Height = 8;

                 recBody.X = recCopter.X; recBody.Y = recCopter.Y + recLight.Height;
             }
         }

    public class Terrain
    {
        Rectangle recTerrain = new Rectangle();
        Bitmap bmpTerrain = new Bitmap(CopterGame.Properties.Resources.terrain);
        ImageAttributes attr = new ImageAttributes();
        bool Drawable = true;
        bool showing =false   ;

        public void Draw(Graphics g)
        {
            if (showing == true)
            {
                g.DrawImage(bmpTerrain, recTerrain, 0, 0, bmpTerrain.Width, bmpTerrain.Height, GraphicsUnit.Pixel, attr);
            }

        }

        public bool GetDrawable()
        {
            return Drawable; 
        }

        public bool GetShow()
        {
            return showing;
        }

        public void SetShow(bool show)
        {
            showing = show ;
        }

        public void SetDrawable(bool Drawable2)
        {
            Drawable = Drawable2;
        }

        public Terrain(int x, int y )
        {
            recTerrain.Height = bmpTerrain.Height;
            recTerrain.Width = bmpTerrain.Width;
            recTerrain.X = x;
            recTerrain.Y = y;
        }

        public Rectangle GetRec()
        {
            return recTerrain; 
        }
    }

    public class Score
    {
        double myScore = 0;

        public void Reset()
        {
           myScore = 0;
        }

        public void Add()
        {
            myScore++;

        }

        public double GetScore()
        {
            return myScore;
        }


    }


}



