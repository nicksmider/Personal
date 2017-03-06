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

namespace TrafficGame4
{
    public partial class Form1 : Form
    {
        Bitmap bmpBackground = TrafficGame4.Properties.Resources.Background;
        Rectangle recBackground = new Rectangle();
        ImageAttributes attrBack = new ImageAttributes();
        Graphics g;

        Random gameRandom = new Random();

        List<Vehicle> genericList = new List<Vehicle>();

        Rectangle mouseRectangle = new Rectangle();

        Score GameScore = new Score();

        Timer gameTimer = new Timer();

        Button startGame = new Button();


        Label currentScoreLbl = new Label();
        Label bestScoreLbl = new Label();
        Font myFont = new Font("Arial", 40, FontStyle.Italic, GraphicsUnit.Pixel);
        Font buttonFont = new Font("Arial", 25, FontStyle.Bold, GraphicsUnit.Pixel);

        int ClickDownPositionX;
        int ClickDownPositionY;
        bool difCheck = false; 
        int dificultyNum = 0;

        public Form1()
        {
            InitializeComponent();

            startGame.Width = 100;
            startGame.Height = 125;
            startGame.Text = "Start New Game";
            

            this.Controls.Add(startGame);
            
            currentScoreLbl.Top = 10;
            currentScoreLbl.Height = 40;
            currentScoreLbl.Width = 150;
            currentScoreLbl.Font = myFont;
            currentScoreLbl.ForeColor = Color.WhiteSmoke;
            currentScoreLbl.BackColor = Color.Transparent;

            startGame.ForeColor = Color.Black;
            startGame.BackColor = Color.WhiteSmoke;
            startGame.Font = buttonFont; 


            bestScoreLbl.Top = 8;
            bestScoreLbl.Height = 40;
            bestScoreLbl.Width = 150;
            bestScoreLbl.Font = myFont;
            bestScoreLbl.ForeColor = Color.WhiteSmoke;
            bestScoreLbl.BackColor = Color.Transparent;
            bestScoreLbl.Left = 150;
          
            this.Controls.Add(currentScoreLbl);
            this.Controls.Add(bestScoreLbl);

            //Vehicle testVehicle = new Vehicle(7);
            //BlueCar testBlue = new BlueCar(5);
            //Truck testTruck = new Truck(2);
            //genericList.Add(testBlue);
            //genericList.Add(testVehicle);
            //genericList.Add(testTruck);

            recBackground.Height = bmpBackground.Height;
            recBackground.Width = bmpBackground.Width;
            recBackground.X = 0;
            recBackground.Y = 0;

            mouseRectangle.Height = 10; mouseRectangle.Width = 10;

            gameTimer.Interval = 50;
            gameTimer.Enabled = true; gameTimer.Tick += gameTimer_Tick;

            this.Height = recBackground.Height;
            this.Width = recBackground.Width;
            currentScoreLbl.Left = this.Width - currentScoreLbl.Width - 100;

            startGame.Left = this.Width - startGame.Width - 50;
            startGame.Top = this.Height - startGame.Height - 40;
            this.Paint += Form1_Paint;
            this.MouseDown += Form1_MouseDown;
            this.MouseUp += Form1_MouseUp;
            this.DoubleBuffered = true;
            this.MouseMove += Form1_MouseMove;
            startGame.Click += startGame_Click;
        }

        public int diffNum()
        {
            double curScore = GameScore.GetScore();
            int interveal = 8;
            if ((Math.Floor(curScore / interveal) == curScore / interveal) && difCheck == false)
            {
                difCheck = true;
                dificultyNum++;
            }
            else if ((Math.Floor(curScore / interveal) != curScore / interveal) && difCheck == true)
            {
                difCheck = false;
            }
            //if (curScore == 5)
            //{
            //    dificultyNum = 1;
            //}
            //else if (curScore == 10)
            //{
            //    dificultyNum = 2;
            //}
            //else if (curScore == 20)
            //{
            //    dificultyNum = 3;

            //}
            //else if (curScore == 30)
            //{
            //    dificultyNum = 4;
            //}
            //else if (curScore == 40)
            //{
            //    dificultyNum = 5;

            //}
            //else if (curScore == 50)
            //{
            //    dificultyNum = 6;
            //}
            //else if (curScore == 60)
            //{
            //    dificultyNum = 7;
            //}
            return dificultyNum;
        }

        void startGame_Click(object sender, EventArgs e)
        {
            resetGame();
            gameTimer.Enabled = true;
        }

        public void resetGame()
        {
            GameScore.resetScore();
            dificultyNum = 0;
            difCheck = false;  
            for (int i = genericList.Count - 1; i >= 0; i--)
            {
                genericList.RemoveAt(i);
            }

        }

        void Form1_MouseMove(object sender, MouseEventArgs e)
        {

            foreach (Vehicle v in genericList)
            {
                if (v.getCollided())
                {
                    v.MeasureSpeed(ClickDownPositionX, ClickDownPositionY, e.X, e.Y);
                }
            }


        }

        void gameTimer_Tick(object sender, EventArgs e)
        {

            if (genericList.Count < 2 + diffNum())
            {
                int UpTo = 0;

                UpTo = gameRandom.Next(0, 85);


                int WillMake = gameRandom.Next(0, UpTo);
                if (WillMake == 4)
                {
                    bool madeCar = false;
                    while (!madeCar)
                    {
                        int CarType = gameRandom.Next(1, 8);
                        int CarRow = gameRandom.Next(1, 9);
                        if (CarType == 1)
                        {
                            genericList.Add(new BlueCar(CarRow));

                        }
                        else if (CarType == 2)
                        {
                            genericList.Add(new Truck(CarRow));
                        }
                        else if (CarType == 3)
                        {
                            genericList.Add(new Vehicle(CarRow));
                        }
                        else if (CarType == 4)
                        {
                            genericList.Add(new YellowCar(CarRow));
                        }
                        else if (CarType == 5)
                        {
                            genericList.Add(new BlueTruck(CarRow));
                        }
                        else if (CarType == 6)
                        {
                            genericList.Add(new Moto(CarRow));
                        }
                        else if (CarType == 7)
                        {
                            genericList.Add(new GMoto(CarRow));
                        }

                        int numIn = genericList.Count;
                        foreach (Vehicle v2 in genericList)
                        {
                            
                            if (genericList[genericList.Count -1] != v2 && genericList[genericList.Count - 1].getPlacingRect().IntersectsWith(v2.getPlacingRect()))
                            {
                                genericList.Remove(genericList[genericList.Count - 1]);
                                break;
                                
                            }
                            
                        }
                        if (numIn == genericList.Count)
                        {
                            madeCar = true;
                        }
                    }
                }
            }

            foreach (Vehicle b in genericList)
            {
                if (b.isBroke())
                {
                    b.Breaked();
                }
            }

            for (int i = genericList.Count - 1; i >= 0; i--)
            {
                if (genericList[i].GetMoveable())
                {
                    genericList[i].Move();

                }
                else if (!genericList[i].GetMoveable())
                {
                    genericList.RemoveAt(i);
                    GameScore.Add();
                }
            }
            bool exitLoop = false; 
            foreach (Vehicle v1 in genericList)
            {
                foreach (Vehicle v2 in genericList)
                {
                    if (
                        (
                        v1.getCol1().IntersectsWith(v2.getCol1()) 
                        || v1.getCol2().IntersectsWith(v2.getCol1())
                        || v1.getCol2().IntersectsWith(v2.getCol2()) 
                        || v1.getCol1().IntersectsWith(v2.getCol2())
                        )

                        && (v1 != v2)
                        
                        )
                        
                    {
                        v1.Collision(v2);
                        if (v1.getEnd())
                        {
                            //resetGame();
                            gameTimer.Enabled = false;
                            exitLoop = true;
                            break; 
                            
                        }
                    }
                }
                if (exitLoop == true)
                {
                    break;
                }
            }
            this.Text = "Traffic:" + dificultyNum.ToString() + "   "  + genericList.Count().ToString();
            currentScoreLbl.Text = GameScore.GetScore().ToString();
            bestScoreLbl.Text = GameScore.getHighScore().ToString();
            Invalidate();

        }

        void Form1_MouseUp(object sender, MouseEventArgs e)
        {

            foreach (Vehicle v in genericList)
            {
                if (v.getCollided())
                {
                    v.setCollided(false);
                }
                mouseRectangle.X = e.X - 5; mouseRectangle.Y = e.Y - 5;
                if (v.getVehicleRectangle().IntersectsWith(mouseRectangle))
                {
                    if (v.getSpeed() != 0)
                    {
                        v.Break();
                    }
                    else if (v.getSpeed() == 0)
                    {
                        v.resetSpeed();
                    }
                }
            }


        }

        void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            //gameTimer.Enabled = true; 
            mouseRectangle.X = e.X - 5; mouseRectangle.Y = e.Y - 5;

            foreach (Vehicle v in genericList)
            {
                if (v.getVehicleRectangle().IntersectsWith(mouseRectangle))
                {
                    ClickDownPositionX = e.X;
                    ClickDownPositionY = e.Y;
                    v.setCollided(true);
                }
            }


        }

        void Form1_Paint(object sender, PaintEventArgs e)
        {
            g = e.Graphics;

            DrawBackground(g);
            foreach (Vehicle v in genericList)
            {
                v.DrawVehicle(g);
            }


        }

        public void DrawBackground(Graphics g)
        {
            g.DrawImage(bmpBackground, recBackground, 0, 0, bmpBackground.Width, bmpBackground.Height, GraphicsUnit.Pixel, attrBack);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Top = 0;
            this.Left = 0;
        }


    }


    public class Vehicle
    {
        protected Rectangle vehicleRectangle = new Rectangle();
        protected Bitmap bmpRectangle = TrafficGame4.Properties.Resources.grayCar;
        protected ImageAttributes vehicleAttr = new ImageAttributes();
        protected Rectangle placingRec = new Rectangle();
        bool gameEnd = false; 

        protected int path;
        int speed = 4;
        int speedOrigional = 4;
        protected int newspeed;
        protected bool Collide = false;

        protected int realLeft;
        protected int realWidth;
        protected int realRight;
        protected bool Braked;
        

        protected int bmpWidth;

        protected bool vehicleMoveable = true;


        protected int LeftPositionX = 0;
        protected int LeftPositionY = 0;
        protected bool OutofTunnel = false;

        //Collision rectangles; 
        protected Rectangle rect1 = new Rectangle();
        protected Rectangle rect2 = new Rectangle();
         
        int breakCounter = 0;

        public Vehicle(int paths)
        {

            path = paths;
            if (path == 1)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.grayCar;
                vehicleRectangle.X = 470;
                vehicleRectangle.Y = -69;
            }
            else if (path == 2)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.grayCar;
                vehicleRectangle.X = 523;
                vehicleRectangle.Y = -69;
            }
            else if (path == 3)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.grayCarUp;
                vehicleRectangle.X = 575;
                vehicleRectangle.Y = 690;
            }
            else if (path == 4)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.grayCarUp;
                vehicleRectangle.X = 628;
                vehicleRectangle.Y = 690;
            }
            else if (path == 5)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.grayCarRight;
                vehicleRectangle.X = 0;
                vehicleRectangle.Y = 328;
            }
            else if (path == 6)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.grayCarRight;
                vehicleRectangle.X = 0;
                vehicleRectangle.Y = 381;
            }
            else if (path == 7)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.grayCarLeft;
                vehicleRectangle.X = 1136;
                vehicleRectangle.Y = 223;
            }
            else if (path == 8)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.grayCarLeft;
                vehicleRectangle.X = 1136;
                vehicleRectangle.Y = 276;
            }
            vehicleRectangle.Height = bmpRectangle.Height;
            vehicleRectangle.Width = bmpRectangle.Width;

            realLeft = vehicleRectangle.Left;
            realWidth = vehicleRectangle.Width;
            realRight = vehicleRectangle.Right;
            bmpWidth = bmpRectangle.Width;
            vehicleRectangle.Height = bmpRectangle.Height;
            vehicleRectangle.Width = bmpRectangle.Width;

            placingRec = vehicleRectangle;

            if (path == 7 || path == 8)
            {
                vehicleRectangle.Width = 0;
                bmpWidth = 0;

            }


            //Setting rectangles

            rect1 = vehicleRectangle;
            rect2 = vehicleRectangle;

            if (path == 1 || path == 2)
            {
                rect1.Width = 25;
                rect1.Height = 64;

                rect2.Width = 38;
                rect2.Height = 42;

                rect1.X += 6;
                rect2.Y += 8;
            }
            else if (path == 3 || path == 4)
            {
                rect1.Width = 25;
                rect1.Height = 64;

                rect2.Width = 38;
                rect2.Height = 42;

                rect1.X += 7;
                rect2.Y += 14;
            }
            else if (path == 5 || path == 6)
            {
                rect1.Height = 25;
                rect1.Width = 64;

                rect2.Height = 38;
                rect2.Width = 42;

                rect1.Y += 6;
                rect2.X += 8;
            }
            else  if (path == 7 || path == 8)
            {
                rect1.Height = 25;
                rect1.Width = 64;

                rect2.Height = 38;
                rect2.Width = 42;

                rect1.Y += 7;
                rect2.X += 14;
            }

            vehicleAttr.SetColorKey(Color.FromArgb(92, 98, 112), Color.FromArgb(92, 98, 112));



        }

        public Vehicle()
        {
        }
         
        public void DrawVehicle(Graphics g)
        {
            Bitmap rectBmp = new Bitmap(TrafficGame4.Properties.Resources.black);
            g.DrawImage(bmpRectangle, vehicleRectangle, LeftPositionX, LeftPositionY, bmpWidth, bmpRectangle.Height, GraphicsUnit.Pixel, vehicleAttr);
            //g.DrawImage(rectBmp, rect1, 0, 0, rect1.Width, rect1.Height, GraphicsUnit.Pixel, vehicleAttr);
            //g.DrawImage(rectBmp, rect2, 0, 0, rect2.Width, rect2.Height, GraphicsUnit.Pixel, vehicleAttr);
        }

        public void Move()
        {
            if (path == 1 || path == 2)
            {
                vehicleRectangle.Y += speed;
                placingRec.Y += speed;
                rect1.Y += speed;
                rect2.Y += speed;
                CheckVertical();
            }
            else if (path == 3 || path == 4)
            {
                vehicleRectangle.Y -= speed;
                placingRec.Y -= speed;
                rect1.Y -= speed;
                rect2.Y -= speed;
                CheckVertical2();
            }
            else if (path == 5 || path == 6)
            {
                if (OutofTunnel == true)
                {
                    vehicleRectangle.X += speed;
                    placingRec.X += speed;
                    
                }
                rect1.X += speed;
                rect2.X += speed;
                realLeft += speed;
                realRight += speed;
                ChangeSizeHorizontal();
            }
            else if (path == 7 || path == 8)
            {

                vehicleRectangle.X -= speed;
                placingRec.X -= speed;
                rect1.X -= speed;
                rect2.X -= speed;
                realLeft -= speed;
                realRight -= speed;
                ChangeSizeHorizontal2();
            }
        }

        public Rectangle getPlacingRect()
        {
            return placingRec;
        }

        public bool isBroke()
        {
            return Braked;

        }

        public void Break()
        {
            speed = 0;
            breakCounter = 0;
            Braked = true; 
        }  

        public void Breaked()
        {
            breakCounter++;
            if (breakCounter > 130)
            {
                Braked = false;
                speed = speedOrigional;
                breakCounter = 0;
            }
        }

        public void MeasureSpeed(int startX, int startY, int finishX, int finishY)
        {
            if (path == 1 || path == 2)
            {
                newspeed = ((finishY - startY) / 12);
                //double doubleSpeed = (double)(newspeed / 12);
                //speed = (int)doubleSpeed; 
            }
            else if (path == 3 || path == 4)
            {
                newspeed = ((startY - finishY) / 12);
                //double doubleSpeed = (double)(newspeed / 12);
                //speed = (int)doubleSpeed; 
            }
            else if (path == 5 || path == 6)
            {
                newspeed = ((finishX - startX) / 12);
                //double doubleSpeed = (double)(newspeed / 12);
                //speed = (int)doubleSpeed; 
            }
            else if (path == 7 || path == 8)
            {
                newspeed = ((startX - finishX) / 12);
                //double doubleSpeed = (double)(newspeed / 12);
                //speed = (int)doubleSpeed; 
            }
            if (newspeed > 40)
            {
                newspeed = 40;
            }
            if (newspeed > 0)
            {
                speed = newspeed;
            }

        }

        public Rectangle getVehicleRectangle()
        {
            return vehicleRectangle;
        }

        public void setCollided(bool doesCollide)
        {
            Collide = doesCollide;
        }

        public bool getCollided()
        {
            return Collide;
        }

        public int getSpeed()
        {
            return speed;
        }

        public void resetSpeed()
        {
            speed = 4;
        }

        public void CheckVertical()
        {
            if (vehicleRectangle.Top > 640)
            {
                vehicleMoveable = false;
            }

        }

        public void CheckVertical2()
        {
            if (vehicleRectangle.Bottom < 0)
            {
                vehicleMoveable = false;
            }

        }

        public void ChangeSizeHorizontal()
        {

            if (realLeft <= 87 && realRight > 87)
            {
                vehicleRectangle.X = 87;


                LeftPositionX = vehicleRectangle.Width - (realRight - 87);

            }
            else if (vehicleRectangle.Right <= 87)
            {
                LeftPositionX = 2000;
                OutofTunnel = false;
            }
            else if (realLeft > 87)
            {
                OutofTunnel = true;

            }

            if (realRight >= 1049)
            {
                vehicleRectangle.X = 1049 - vehicleRectangle.Width;
                OutofTunnel = false;
                bmpWidth -= speed;
                vehicleRectangle.Width -= speed;
                vehicleRectangle.X += speed;


            }
            if (vehicleRectangle.Left > 1049)
            {
                bmpWidth = 0;
                vehicleMoveable = false;
            }

        }

        public void ChangeSizeHorizontal2()
        {

            if (realLeft <= 87 && realRight > 87)
            {
                OutofTunnel = false;
                vehicleRectangle.X = 87;


                LeftPositionX = vehicleRectangle.Width - (realRight - 87);

            }
            else if (realRight <= 87)
            {
                LeftPositionX = 2000;
                bmpWidth = 0;
                OutofTunnel = true;
                vehicleMoveable = false;
            }


            if (realLeft <= 1046 && realRight > 1046)
            {
                //  OutofTunnel = false; 
                bmpWidth += speed;
                vehicleRectangle.Width += speed;
                if (bmpWidth > bmpRectangle.Width)
                {
                    bmpWidth = bmpRectangle.Width;
                    vehicleRectangle.Width = bmpWidth;
                }
                //if (vehicleRectangle.Left > 1049)
                //{
                //    LeftPositionX = 2000;
                //}

            }

        }

        public void SetMoveable(bool MoveableSet)
        {
            vehicleMoveable = MoveableSet;
        }

        public bool GetMoveable()
        {
            return vehicleMoveable;
        }

        public void SetRecNew(int x, int y)
        {
            vehicleRectangle.X = x;
            vehicleRectangle.Y = y;
            rect1.X = x;
            rect2.X = x;
            rect1.Y = y;
            rect2.Y = y;
        }

        public bool getEnd()
        {
            return gameEnd;
        }

        public Rectangle getCol1()
        {
            return rect1;
        }

        public Rectangle getCol2()
        {
            return rect2;

        }

        public void Collision(Vehicle v)  
        {
            if (rect1.IntersectsWith(v.getCol1()) || rect2.IntersectsWith(v.getCol1())
                || rect1.IntersectsWith(v.getCol2()) || rect2.IntersectsWith(v.getCol2()))
                {
                    if (path == v.path && Braked && v.getSpeed() == speedOrigional)
                    {
                        v.Break();
                        int amount = speed+1;
                        Rectangle rectv = v.getVehicleRectangle();
                        if ((path == 1 || path == 2) && (v.getVehicleRectangle().Y + v.getVehicleRectangle().Height >= vehicleRectangle.Y - amount))
                        {
                            v.SetRecNew(rectv.X, vehicleRectangle.Y - amount - v.getVehicleRectangle().Height);
                        }
                        else if ((path == 3 || path == 4) && (v.getVehicleRectangle().Y <= vehicleRectangle.Y + vehicleRectangle.Height + amount))
                        {
                            v.SetRecNew(rectv.X, vehicleRectangle.Y + vehicleRectangle.Height + amount);
                        }
                        else if ((path == 5 || path == 6) && (v.getVehicleRectangle().X + v.getVehicleRectangle().Width >= vehicleRectangle.X - amount))
                        {
                            v.SetRecNew(vehicleRectangle.X - amount - v.getVehicleRectangle().Width, rectv.Y);
                        }
                        else if ((path == 7 || path == 8) && (v.getVehicleRectangle().X <= vehicleRectangle.X + vehicleRectangle.Width + amount))
                        {
                            v.SetRecNew(vehicleRectangle.X + vehicleRectangle.Width + amount, rectv.Y); 
                        }
                    }
                    else
                    {
                        gameEnd = true;
                    }
                }
                
        }

    }


    public class Truck : Vehicle
    {
        public Truck(int paths)
        {

            path = paths;
            if (path == 1)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.truckDown;
                vehicleRectangle.X = 466;
                vehicleRectangle.Y = -107;
            }
            else if (path == 2)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.truckDown;
                vehicleRectangle.X = 519;
                vehicleRectangle.Y = -107;
            }
            else if (path == 3)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.truckUp;
                vehicleRectangle.X = 571;
                vehicleRectangle.Y = 650 + 107;
            }
            else if (path == 4)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.truckUp;
                vehicleRectangle.X = 624;
                vehicleRectangle.Y = 650 +107;
            }
            else if (path == 5)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.truckRight;
                vehicleRectangle.X = 0;
                vehicleRectangle.Y = 323;
            }
            else if (path == 6)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.truckRight;
                vehicleRectangle.X = 0;
                vehicleRectangle.Y = 376;
            }
            else if (path == 7)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.truckLeft;
                vehicleRectangle.X = 1136;
                vehicleRectangle.Y = 218;
            }
            else if (path == 8)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.truckLeft;
                vehicleRectangle.X = 1136;
                vehicleRectangle.Y = 272;
            }
            vehicleRectangle.Height = bmpRectangle.Height;
            vehicleRectangle.Width = bmpRectangle.Width;

            realLeft = vehicleRectangle.Left;
            realWidth = vehicleRectangle.Width;
            realRight = vehicleRectangle.Right;
            bmpWidth = bmpRectangle.Width;
            vehicleRectangle.Height = bmpRectangle.Height;
            vehicleRectangle.Width = bmpRectangle.Width;

            placingRec = vehicleRectangle;rect1 = vehicleRectangle;
            rect2 = vehicleRectangle;
            if (path == 7 || path == 8)
            {
                vehicleRectangle.Width = 0;
                bmpWidth = 0;

            }
            
            //if (path == 1 || path == 2)
            //{
            //    rect1.Width = 46;
            //    rect1.Height = 107;

            //    rect2.Width = 38;
            //    rect2.Height = 33;

            //    rect1.X += 0;
            //    rect2.X += 4;
            //    rect2.Y += 39;

            //}
            //else if (path == 3 || path == 4)
            //{
            //    rect1.Width = 46;
            //    rect1.Height = 107;

            //    rect2.Width = 38;
            //    rect2.Height = 33;

            //    rect1.X += 0;
            //    rect1.Y += 35;

            //    rect2.X += 4;
            //    rect2.Y += 33;
            //}
            //else if (path == 5 || path == 6)
            //{
            //    rect1.Height = 46;
            //    rect1.Width = 107;

            //    rect2.Height = 38;
            //    rect2.Width = 33;

            //    rect1.Y += 0;
            //    rect2.Y += 4;
            //    rect2.X += 39;
            //}
            //else if (path == 7 || path == 8)
            //{
            //    rect1.Height = 46;
            //    rect1.Width = 107;

            //    rect2.Height = 38;
            //    rect2.Width = 33;

            //    rect1.Y += 0;
            //    rect1.X += 35;

            //    rect2.Y += 4;
            //    rect2.X += 33;
            //}


            vehicleAttr.SetColorKey(Color.FromArgb(92, 98, 112), Color.FromArgb(92, 98, 112));



        }
    }

    public class BlueCar : Vehicle
    {
        public BlueCar(int paths)
        {

            path = paths;
            if (path == 1)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.blueCarDown;
                vehicleRectangle.X = 470;
                vehicleRectangle.Y = -64;
            }
            else if (path == 2)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.blueCarDown;
                vehicleRectangle.X = 523;
                vehicleRectangle.Y = -64;
            }
            else if (path == 3)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.blueCarUp;
                vehicleRectangle.X = 575;
                vehicleRectangle.Y = 705;
            }
            else if (path == 4)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.blueCarUp;
                vehicleRectangle.X = 628;
                vehicleRectangle.Y = 705;
            }
            else if (path == 5)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.blueCarRight;
                vehicleRectangle.X = 0;
                vehicleRectangle.Y = 328;
            }
            else if (path == 6)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.blueCarRight;
                vehicleRectangle.X = 0;
                vehicleRectangle.Y = 381;
            }
            else if (path == 7)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.blueCarLeft;
                vehicleRectangle.X = 1136;
                vehicleRectangle.Y = 223;
            }
            else if (path == 8)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.blueCarLeft;
                vehicleRectangle.X = 1136;
                vehicleRectangle.Y = 276;
            }
            vehicleRectangle.Height = bmpRectangle.Height;
            vehicleRectangle.Width = bmpRectangle.Width;

            realLeft = vehicleRectangle.Left;
            realWidth = vehicleRectangle.Width;
            realRight = vehicleRectangle.Right;
            bmpWidth = bmpRectangle.Width;
            vehicleRectangle.Height = bmpRectangle.Height;
            vehicleRectangle.Width = bmpRectangle.Width;

            placingRec = vehicleRectangle;
            if (path == 7 || path == 8)
            {
                vehicleRectangle.Width = 0;
                bmpWidth = 0;

            }
            rect1 = vehicleRectangle;
            rect2 = vehicleRectangle;
            if (path == 1 || path == 2)
            {
                rect1.Width = 25;
                rect1.Height = 64;

                rect2.Width = 38;
                rect2.Height = 42;

                rect1.X += 6;
                rect2.Y += 8;
            }
            else if (path == 3 || path == 4)
            {
                rect1.Width = 25;
                rect1.Height = 64;

                rect2.Width = 38;
                rect2.Height = 42;

                rect1.X += 7;
                rect2.Y += 14;
            }
            else if (path == 5 || path == 6)
            {
                rect1.Height = 25;
                rect1.Width = 64;

                rect2.Height = 38;
                rect2.Width = 42;

                rect1.Y += 6;
                rect2.X += 8;
            }
            else if (path == 7 || path == 8)
            {
                rect1.Height = 25;
                rect1.Width = 64;

                rect2.Height = 38;
                rect2.Width = 42;

                rect1.Y += 7;
                rect2.X += 14;
            }


            vehicleAttr.SetColorKey(Color.FromArgb(92, 98, 112), Color.FromArgb(92, 98, 112));



        }
    }

    public class YellowCar : Vehicle
    {
        public YellowCar(int paths)
        {

            path = paths;
            if (path == 1)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.blueCarDown;
                vehicleRectangle.X = 470;
                vehicleRectangle.Y = -64;
            }
            else if (path == 2)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.blueCarDown;
                vehicleRectangle.X = 523;
                vehicleRectangle.Y = -64;
            }
            else if (path == 3)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.blueCarUp;
                vehicleRectangle.X = 575;
                vehicleRectangle.Y = 705;
            }
            else if (path == 4)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.blueCarUp;
                vehicleRectangle.X = 628;
                vehicleRectangle.Y = 705;
            }
            else if (path == 5)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.blueCarRight;
                vehicleRectangle.X = 0;
                vehicleRectangle.Y = 328;
            }
            else if (path == 6)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.blueCarRight;
                vehicleRectangle.X = 0;
                vehicleRectangle.Y = 381;
            }
            else if (path == 7)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.blueCarLeft;
                vehicleRectangle.X = 1136;
                vehicleRectangle.Y = 223;
            }
            else if (path == 8)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.blueCarLeft;
                vehicleRectangle.X = 1136;
                vehicleRectangle.Y = 276;
            }
            vehicleRectangle.Height = bmpRectangle.Height;
            vehicleRectangle.Width = bmpRectangle.Width;

            realLeft = vehicleRectangle.Left;
            realWidth = vehicleRectangle.Width;
            realRight = vehicleRectangle.Right;
            bmpWidth = bmpRectangle.Width;
            vehicleRectangle.Height = bmpRectangle.Height;
            vehicleRectangle.Width = bmpRectangle.Width;

            placingRec = vehicleRectangle;
            if (path == 7 || path == 8)
            {
                vehicleRectangle.Width = 0;
                bmpWidth = 0;

            }
            rect1 = vehicleRectangle;
            rect2 = vehicleRectangle;
            if (path == 1 || path == 2)
            {
                rect1.Width = 25;
                rect1.Height = 64;

                rect2.Width = 38;
                rect2.Height = 42;

                rect1.X += 6;
                rect2.Y += 8;
            }
            else if (path == 3 || path == 4)
            {
                rect1.Width = 25;
                rect1.Height = 64;

                rect2.Width = 38;
                rect2.Height = 42;

                rect1.X += 7;
                rect2.Y += 14;
            }
            else if (path == 5 || path == 6)
            {
                rect1.Height = 25;
                rect1.Width = 64;

                rect2.Height = 38;
                rect2.Width = 42;

                rect1.Y += 6;
                rect2.X += 8;
            }
            else if (path == 7 || path == 8)
            {
                rect1.Height = 25;
                rect1.Width = 64;

                rect2.Height = 38;
                rect2.Width = 42;

                rect1.Y += 7;
                rect2.X += 14;
            }


            vehicleAttr.SetColorKey(Color.FromArgb(92, 98, 112), Color.FromArgb(92, 98, 112));



        }
    }

    public class Moto : Vehicle
    {
        public Moto(int paths)
        {

            path = paths;
            if (path == 1)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.motoDown;
                vehicleRectangle.X = 470;
                vehicleRectangle.Y = -64;
            }
            else if (path == 2)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.motoDown;
                vehicleRectangle.X = 523;
                vehicleRectangle.Y = -64;
            }
            else if (path == 3)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.motoUp;
                vehicleRectangle.X = 575;
                vehicleRectangle.Y = 705;
            }
            else if (path == 4)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.motoUp;
                vehicleRectangle.X = 628;
                vehicleRectangle.Y = 705;
            }
            else if (path == 5)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.motoRight;
                vehicleRectangle.X = 0;
                vehicleRectangle.Y = 328;
            }
            else if (path == 6)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.motoRight;
                vehicleRectangle.X = 0;
                vehicleRectangle.Y = 381;
            }
            else if (path == 7)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.motoLeft;
                vehicleRectangle.X = 1136;
                vehicleRectangle.Y = 223;
            }
            else if (path == 8)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.motoLeft;
                vehicleRectangle.X = 1136;
                vehicleRectangle.Y = 276;
            }
            vehicleRectangle.Height = bmpRectangle.Height;
            vehicleRectangle.Width = bmpRectangle.Width;

            realLeft = vehicleRectangle.Left;
            realWidth = vehicleRectangle.Width;
            realRight = vehicleRectangle.Right;
            bmpWidth = bmpRectangle.Width;
            vehicleRectangle.Height = bmpRectangle.Height;
            vehicleRectangle.Width = bmpRectangle.Width;
            placingRec = vehicleRectangle;

            if (path == 7 || path == 8)
            {
                vehicleRectangle.Width = 0;
                bmpWidth = 0;

            }

            rect1 = vehicleRectangle;
            rect2 = vehicleRectangle;
            if (path == 1 || path == 2)
            {
                rect1.Width = 21;
                rect1.Height = 48;


                rect1.X += 4;

            }
            else if (path == 3 || path == 4)
            {
                rect1.Width = 21;
                rect1.Height = 48;


                rect1.X += 4;

            }
            else if (path == 5 || path == 6)
            {
                rect1.Height = 21;
                rect1.Width = 48;



                rect1.Y += 4;

            }
            else if (path == 7 || path == 8)
            {
                rect1.Height = 21;
                rect1.Width = 48;



                rect1.Y += 4;

            }
            rect2 = rect1;
            vehicleAttr.SetColorKey(Color.FromArgb(92, 98, 112), Color.FromArgb(92, 98, 112));



        }
    }

    public class GMoto : Vehicle
    {
        public GMoto(int paths)
        {

            path = paths;
            if (path == 1)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.motoDown2;
                vehicleRectangle.X = 470;
                vehicleRectangle.Y = -64;
            }
            else if (path == 2)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.motoDown2;
                vehicleRectangle.X = 523;
                vehicleRectangle.Y = -64;
            }
            else if (path == 3)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.motoUp2;
                vehicleRectangle.X = 575;
                vehicleRectangle.Y = 705;
            }
            else if (path == 4)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.motoUp2;
                vehicleRectangle.X = 628;
                vehicleRectangle.Y = 705;
            }
            else if (path == 5)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.motoRight2;
                vehicleRectangle.X = 0;
                vehicleRectangle.Y = 328;
            }
            else if (path == 6)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.motoRight2;
                vehicleRectangle.X = 0;
                vehicleRectangle.Y = 381;
            }
            else if (path == 7)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.motoLeft2;
                vehicleRectangle.X = 1136;
                vehicleRectangle.Y = 223;
            }
            else if (path == 8)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.motoLeft2;
                vehicleRectangle.X = 1136;
                vehicleRectangle.Y = 276;
            }
            vehicleRectangle.Height = bmpRectangle.Height;
            vehicleRectangle.Width = bmpRectangle.Width;

            realLeft = vehicleRectangle.Left;
            realWidth = vehicleRectangle.Width;
            realRight = vehicleRectangle.Right;
            bmpWidth = bmpRectangle.Width;
            vehicleRectangle.Height = bmpRectangle.Height;
            vehicleRectangle.Width = bmpRectangle.Width;
            placingRec = vehicleRectangle;

            if (path == 7 || path == 8)
            {
                vehicleRectangle.Width = 0;
                bmpWidth = 0;

            }
            rect1 = vehicleRectangle;
            rect2 = vehicleRectangle;
            if (path == 1 || path == 2)
            {
                rect1.Width = 21;
                rect1.Height = 48;


                rect1.X += 4;

            }
            else if (path == 3 || path == 4)
            {
                rect1.Width = 21;
                rect1.Height = 48;


                rect1.X += 4;

            }
            else if (path == 5 || path == 6)
            {
                rect1.Height = 21;
                rect1.Width = 48;



                rect1.Y += 4;

            }
            else if (path == 7 || path == 8)
            {
                rect1.Height = 21;
                rect1.Width = 48;



                rect1.Y += 4;

            }
            rect2 = rect1;
            vehicleAttr.SetColorKey(Color.FromArgb(92, 98, 112), Color.FromArgb(92, 98, 112));



        }
    }

    public class BlueTruck : Vehicle
    {
        public BlueTruck(int paths)
        {

            path = paths;
            if (path == 1)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.BlueTruckDown;
                vehicleRectangle.X = 466;
                vehicleRectangle.Y = -107;
            }
            else if (path == 2)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.BlueTruckDown;
                vehicleRectangle.X = 519;
                vehicleRectangle.Y = -107;
            }
            else if (path == 3)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.BlueTruckUp;
                vehicleRectangle.X = 571;
                vehicleRectangle.Y = 650 + 107;
            }
            else if (path == 4)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.BlueTruckUp;
                vehicleRectangle.X = 624;
                vehicleRectangle.Y = 650 + 107;
            }
            else if (path == 5)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.BlueTruckRight;
                vehicleRectangle.X = 0;
                vehicleRectangle.Y = 323;
            }
            else if (path == 6)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.BlueTruckRight;
                vehicleRectangle.X = 0;
                vehicleRectangle.Y = 376;
            }
            else if (path == 7)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.BlueTruckLeft;
                vehicleRectangle.X = 1136;
                vehicleRectangle.Y = 218;
            }
            else if (path == 8)
            {
                bmpRectangle = TrafficGame4.Properties.Resources.BlueTruckLeft;
                vehicleRectangle.X = 1136;
                vehicleRectangle.Y = 272;
            }
            vehicleRectangle.Height = bmpRectangle.Height;
            vehicleRectangle.Width = bmpRectangle.Width;

            realLeft = vehicleRectangle.Left;
            realWidth = vehicleRectangle.Width;
            realRight = vehicleRectangle.Right;
            bmpWidth = bmpRectangle.Width;
            vehicleRectangle.Height = bmpRectangle.Height;
            vehicleRectangle.Width = bmpRectangle.Width;

            placingRec = vehicleRectangle;rect1 = vehicleRectangle;
            rect2 = vehicleRectangle;
            if (path == 7 || path == 8)
            {
                vehicleRectangle.Width = 0;
                bmpWidth = 0;

            }
            
            //if (path == 1 || path == 2)
            //{
            //    rect1.Width = 46;
            //    rect1.Height = 107;

            //    rect2.Width = 38;
            //    rect2.Height = 33;

            //    rect1.X += 0;
            //    rect2.X += 4;
            //    rect2.Y += 39;

            //}
            //else if (path == 3 || path == 4)
            //{
            //    rect1.Width = 46;
            //    rect1.Height = 107;

            //    rect2.Width = 38;
            //    rect2.Height = 33;

            //    rect1.X += 0;
            //    rect1.Y += 35;

            //    rect2.X += 4;
            //    rect2.Y += 33;
            //}
            //else if (path == 5 || path == 6)
            //{
            //    rect1.Height = 46;
            //    rect1.Width = 107;

            //    rect2.Height = 38;
            //    rect2.Width = 33;

            //    rect1.Y += 0;
            //    rect2.Y += 4;
            //    rect2.X += 39;
            //}
            //else if (path == 7 || path == 8)
            //{
            //    rect1.Height = 46;
            //    rect1.Width = 107;

            //    rect2.Height = 38;
            //    rect2.Width = 33;

            //    rect1.Y += 0;
            //    rect1.X += 35;

            //    rect2.Y += 4;
            //    rect2.X += 33;
            //}

            vehicleAttr.SetColorKey(Color.FromArgb(92, 98, 112), Color.FromArgb(92, 98, 112));



        }
    }


    public class Score
    {
        int score = 0;
        int frequencySpeed=0;
        int highScore = 0;


        public void Add()
        {
            score++;
            if (highScore < score)
            {
                highScore = score; 
            }
            frequencySpeed += 2;
        }

        public int FrequencyOfCars()
        {
            return frequencySpeed;

        }

        public int GetScore()
        {
            return score;
        }

        public int getHighScore()
        {
            return highScore;

        }

        public void resetScore()
        {
            score = 0; frequencySpeed = 0;

        }

    }

}

