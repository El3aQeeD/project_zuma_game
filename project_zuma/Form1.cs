using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace project_zuma
{
    class Ground
    {
        public int width;
        public int height;
        public Bitmap img;

    }

    class Turtle
    {
        public int x, y;
        public int width;
        public int height;
        public Bitmap img;

    }

    class Ball
    {
        public PointF currentPoint;
        public int ballType = -1;
        public float ballCurrentTime = 0.00f;
        public int currentCurve = 0;
        public int width;
        public int height;
        public int xe, ye;
        public Bitmap img;
        public bool isMoving = true;
        public bool draw = true;

    }

    class Line { 
        public int xs, ys;
        public int xe, ye;
    
    }

    public partial class Form1 : Form
    {

        // lists
        List<Ball> Balls = new List<Ball>();
        List<Ball> ZumaBalls = new List<Ball>();
        List<Circle> Circles = new List<Circle>();
        List<Turtle> Turtles = new List<Turtle>();
        List<Line> Lines = new List<Line>();

        //
        Ground ground;
        List<BezCurve> curves = new List<BezCurve>();
        int whichCurve = -1;


        PointF carPoint;
        Bitmap off;


        /// <summary>
        /// 
        /// </summary>
        /// 
        Timer tt = new Timer();

        // variables 
        bool zumaballMove = false;
        int kk = 0;
        int ct = 0;
        int cttick = 0;
        int ctMatchedType = 0;
        int startPos = -1;
        int endPos = -1;
        PointF a;
        bool reshapeBalls = false;

         int angle = 40;
        public Form1()
        {
            InitializeComponent();
            this.Paint += new PaintEventHandler(Form1_Paint);
            this.WindowState = FormWindowState.Maximized;
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            this.Load += new EventHandler(Form1_Load);
            this.MouseDown += new MouseEventHandler(Form1_MouseDown);
            this.MouseMove += Form1_MouseMove;
            tt.Tick += Tt_Tick;
            tt.Start();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {


            int Rad = (int)Euclidean(Turtles[0].x + Turtles[0].width / 2, Turtles[0].y + Turtles[0].height / 2, e.X, e.Y);

            Circles[0].Rad = Rad;


            angle = Circles[0].getAngle(e.X, e.Y);
            this.Text = "t " + angle;

        }

        private void Tt_Tick(object sender, EventArgs e)
        {

            
            if (Balls.Count > 0) {

                //this.Text = " " + Balls[0].ballCurrentTime;
                if (reshapeBalls)
                {
                    if (startPos < Balls.Count)
                    {
                        if (Math.Abs(Balls[startPos].ballCurrentTime - Balls[startPos - 1].ballCurrentTime) >= 0.01f && Math.Abs(Balls[startPos].ballCurrentTime - Balls[startPos - 1].ballCurrentTime) <= 0.02f && Balls[startPos].isMoving == true && Balls[startPos - 1].isMoving == false)
                        {
                            
                            //this.Text = "a " + Balls[startPos].ballCurrentTime + "b " + Balls[startPos - 1].ballCurrentTime + "tot " + (Balls[startPos].ballCurrentTime - Balls[startPos - 1].ballCurrentTime);

                            // after tail reaches the head move all balls again
                            for (int k = 0; k < startPos; k++)
                            {
                                Balls[k].isMoving = true;
                                reshapeBalls = false;

                            }
                            checkMatecd(startPos);
                            //tt.Stop();
                        }
                    }
                    else //case
                    {
                        for (int k = 0; k < Balls.Count; k++)
                        {
                            Balls[k].isMoving = true;
                            reshapeBalls = false;

                        }
                        
                    }
                   
                }
                
                // moving the balls 
                for (int i = 0; i < Balls.Count; i++) 
                {
                    if (Balls[i].isMoving == true)
                    {
                        Balls[i].ballCurrentTime += 0.01f;

                        // if ball time exceed 1 means that we need to move to the next curve object
                        if (Balls[i].ballCurrentTime >= 1.0f)
                        {
                            Balls[i].currentCurve++;
                            
                            // means if ball reach black hole
                            if (Balls[i].currentCurve == 4)
                            {
                                Balls[i].draw = false;
                                tt.Stop();
                            }
                            Balls[i].ballCurrentTime = 0.01f;
                        }

                    }
                    

                }
            
            }

            // if there is a one ball at least 
            if (Balls.Count >= 1)
            {
                // create ball every 0.02 
                if (Balls[Balls.Count - 1].ballCurrentTime >= 0.02f)
                {
                    ct++;
                    if (ct <= 10)
                       createBall();
                    

                }
            }
            else
            {
                if(cttick <= 50)
                createBall();
            }


            if (zumaballMove)
            {


                int speed = 60;  // zuma ball speed 
                moveZumaBall(ZumaBalls[0].xe, ZumaBalls[0].ye, ZumaBalls[0],speed);
                Lines[0].xe = (int)ZumaBalls[0].currentPoint.X;
                Lines[0].ye = (int)ZumaBalls[0].currentPoint.Y;
                for (int i = 0; i < Balls.Count; i++)
                {


                    if (Balls[i].currentPoint.X < ZumaBalls[0].currentPoint.X && Balls[i].currentPoint.X + Balls[i].width > ZumaBalls[0].currentPoint.X  && ZumaBalls[0].currentPoint.Y  > Balls[i].currentPoint.Y && ZumaBalls[0].currentPoint.Y < Balls[i].currentPoint.Y + Balls[i].height)
                    {
                        startPos = -1;
                        endPos = -1;

                        ZumaBalls[0].ballCurrentTime = Balls[i].ballCurrentTime;
                        ZumaBalls[0].currentCurve = Balls[i].currentCurve;
                        // move the ball after the hitted ball to reshape the balls
                        for (int k = i; k >= 0; k--)
                        {
                            Balls[k].ballCurrentTime += 0.01111f;
                        }

                        // case 1
                        if (Balls[i].ballType == ZumaBalls[0].ballType) 
                        {


                            // check ballType lower than hitted ball if they have the same type
                            for (int k = i - 1; k >= 0; k--)
                            {
                                if (Balls[k].ballType == ZumaBalls[0].ballType)
                                {
                                    startPos = k;
                                    ctMatchedType++;

                                }
                                else
                                {
                                    break;
                                }
                            }


                            // check ballType upper and equal the hitted ball if they have the same type
                            for (int k = i; k < Balls.Count; k++)
                            {
                                if (Balls[k].ballType == ZumaBalls[0].ballType)
                                {
                                    endPos = k;
                                    ctMatchedType++;

                                }
                                else
                                {
                                    break;
                                }
                            }

                            // special case
                            if (startPos == -1 && endPos != -1)
                            {
                                startPos = i;
                            }

                        }
                        //case 2
                        if (Balls[i].ballType != ZumaBalls[0].ballType)
                        {

                            // check ballType upper and equal the hitted ball if they have the same type
                            for (int k = i+1; k < Balls.Count; k++)
                            {
                                if (Balls[k].ballType == ZumaBalls[0].ballType)
                                {
                                    endPos = k;
                                    ctMatchedType++;

                                }
                                else
                                {
                                    break;
                                }
                            }
                            
                            // special case
                            if (endPos != -1)
                            {
                                startPos = i+1;
                            }
                        }



                        if (ctMatchedType >= 2)
                        {
                            // Deleting Balls within the range
                            if (startPos != -1 && endPos != -1)
                            {
                                //this.Text = "hhh " + startPos + " as " + endPos; 
                                Balls.RemoveRange(startPos, Math.Abs(endPos - startPos + 1));
                                //tt.Stop();
                            }
                           

                            // after removing I want to stop moving the head balls
                            for (int k = 0; k < startPos; k++)
                            {
                                Balls[k].isMoving = false;
                                reshapeBalls = true;
                                
                            }

                            

                        }
                        else // means insert the ball there is no matched balls enough
                        {
                            
                            ZumaBalls[0].width = Balls[i].width;
                            ZumaBalls[0].height = Balls[i].height;
                            Balls.Insert(i + 1, ZumaBalls[0]);
                            ZumaBalls.RemoveAt(0);

                        }

                        // reset this variables and exit loop we dont need to loop over all balls already one ball hitted 
                        ctMatchedType = 0;
                        Lines.Clear();
                        ZumaBalls.RemoveAt(0);
                        zumaballMove = false;
                        break;
                        
                    }
                }

                // if zuma ball move out of screen clear the line and zuma ball 
                if (ZumaBalls.Count > 1) {

                    if (ZumaBalls[0].currentPoint.X > this.Width || ZumaBalls[0].currentPoint.X - 40 < 0 || ZumaBalls[0].currentPoint.Y > this.Height || ZumaBalls[0].currentPoint.Y < 0)
                    {
                        zumaballMove = false;
                        ZumaBalls.RemoveAt(0);
                        Lines.Clear();
                    }
                }
                

            }
            if (cttick > 10)
            {
                if (Balls.Count == 0)
                {
                    tt.Stop();
                }
            }
            //this.Text = "type 0 (blue) 1(yellow) 2(green) " + ZumaBalls[0].ballType;
            DrawDubb(this.CreateGraphics());
        }
        
        void checkMatecd(int startpos)
        {
            int ct1 = 0;
            int ct2 = 0;
            int endPos = startpos;
            int type = Balls[startpos].ballType ;
            for (int i = startpos + 1; i < Balls.Count; i++)
            {
                
                if (Balls[i].ballType == type)
                {
                    endPos = i;
                    ct1++;

                }
                else
                {
                    break;
                }

            }

            for (int i = startpos - 1; i >= 0; i--)
            {
                if (Balls[i].ballType == type)
                {
                    startpos = i;
                    ct2++;

                }
                else
                {
                    break;
                }

            }

            if (ct1 >= 1 && ct2 >= 1)
            {
                this.Text = "hhh1 " + startPos + " as " + endPos; 
                Balls.RemoveRange(startpos, Math.Abs(endPos - startpos + 1));
                //tt.Stop();  
                startPos =startpos;
                reshapeBalls = false;

                // after removing I want to stop moving the head balls
                for (int k = 0; k < startPos; k++)
                {
                    Balls[k].isMoving = false;
                    reshapeBalls = true;

                }

                //tt.Stop();
            }

            if (ct1 >= 2 && ct2 == 0)
            {
                this.Text = "hhh2 " + startpos + " as " + endPos; 
                Balls.RemoveRange(startpos, Math.Abs(endPos - startpos + 1));
                //tt.Stop();  
                startPos = startpos;
                reshapeBalls = false;

                // after removing I want to stop moving the head balls
                for (int k = 0; k < startPos; k++)
                {
                    Balls[k].isMoving = false;
                    reshapeBalls = true;

                }

                tt.Stop();
            }

            if (ct1 == 0 && ct2 >= 2)
            {
                this.Text = "hhh3 " + startpos + " as " + endPos; 
                Balls.RemoveRange(startpos, Math.Abs(endPos - startpos + 1));

                reshapeBalls = false;
                startPos = startpos;
                reshapeBalls = false;

                // after removing I want to stop moving the head balls
                for (int k = 0; k < startPos; k++)
                {
                    Balls[k].isMoving = false;
                    reshapeBalls = true;

                }
                 //tt.Stop();  
            }


        }
         
        void Form1_Load(object sender, EventArgs e)
        {
            createGround();
            placeTheCurvePoints();           
            //createBall();
            createTurtle();
            CreateCircles(0,360,2000,644,532);
            createZumaBall();

            if (off == null)
            {
                off = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            }
            DrawDubb(this.CreateGraphics());
        }

        void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            DrawDubb(this.CreateGraphics());
        }

        void DrawDubb(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }



        private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //this.Text = "x : " + e.X + " y : " + e.Y;
            // Calculate the angle between the circle's center and the mouse position
            
           // moveTurtle(kk);

            int x = e.X;
            int y = e.Y;
            Lines.Clear();
            
            createLine(x, y);
            if (!zumaballMove)
            {
                
                zumaballMove = true;
                moveZumaBall(x, y, ZumaBalls[0],100);
            }
            

            DrawDubb(this.CreateGraphics());
        }

        void createGround()
        {
            ground = new Ground();
            ground.img = new Bitmap("bgMod-2.jpg");
            ground.height = this.Height;
            ground.width = this.Width;
        }

        void createBall()
        {
            Random rand = new Random();

            int r = rand.Next(0, 3);

            Ball pnn = new Ball();

            if (r == 0)
            {
                pnn.img = new Bitmap("Blue.png");
                pnn.ballType = 0;
            }

            if (r == 1)
            {
                pnn.img = new Bitmap("Yellow.png");
                pnn.ballType = 1;

            }

            if (r == 2)
            {
                pnn.img = new Bitmap("Green.png");
                pnn.ballType = 2;

            }
            pnn.width = 60;
            pnn.height = 60;
            Balls.Add(pnn);

        }

        Ball moveZumaBall(int xe , int ye, Ball ball,int speed)
        {
            float xs = Turtles[0].x + Turtles[0].img.Width / 2;
            float ys = Turtles[0].y + Turtles[0].img.Height / 2;
            float dx = xe - xs;
            float dy = ye - ys;
            float cx = ball.currentPoint.X;
            float cy = ball.currentPoint.Y;
            float m = dy / dx;
            ball.xe = xe;
            ball.ye = ye;
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                if (xs < xe)
                {
                    cx += speed;
                    cy += m * speed;
                    if (cx >= xe)
                    {
                        //flagStop = true;
                    }
                }
                else
                {
                    cx -= speed;
                    cy -= m * speed;
                    if (cx <= xe)
                    {
                        //flagStop = true;
                    }
                }

            }
            if (Math.Abs(dx) < Math.Abs(dy))
            {
                if (ys < ye)
                {
                    cy += speed;
                    cx += (1 / m) * speed;
                    if (cy >= ye)
                    {
                        //flagStop = true;
                    }
                }
                else
                {
                    cy -= speed;
                    cx -= (1 / m) * speed;
                    if (cy <= ye)
                    {
                        //flagStop = true;
                    }
                }
            }
            ball.currentPoint.X = cx;
            ball.currentPoint.Y = cy;
            return ball;
        }

        void createLine(int xe, int ye)
        {
            Line pnn = new Line();
            pnn.xs = (int)Turtles[0].x + Turtles[0].img.Width / 2;
            pnn.ys = (int)Turtles[0].y + Turtles[0].img.Height / 2; ;
            pnn.xe = xe;
            pnn.ye = ye;

            Lines.Add(pnn);
        
        }

        void createZumaBall()
        {
            Random rand = new Random();
            for (int i = 0; i < 50; i++) {
                int r = rand.Next(0, 3);

                Ball pnn = new Ball();

                if (r == 0)
                {
                    pnn.img = new Bitmap("Blue.png");
                    pnn.ballType = 0;
                }

                if (r == 1)
                {
                    pnn.img = new Bitmap("Yellow.png");
                    pnn.ballType = 1;

                }

                if (r == 2)
                {
                    pnn.img = new Bitmap("Green.png");
                    pnn.ballType = 2;

                }

                pnn.width = 40;
                pnn.height = 40;
                pnn.currentPoint.X = Turtles[0].x + Turtles[0].img.Width / 2;
                pnn.currentPoint.Y = Turtles[0].y + Turtles[0].img.Height / 2;
                ZumaBalls.Add(pnn);
            }
            

        }

        void CreateCircles(int start, int end, int rad , int XC , int YC)
        {
            Circle c = new Circle();

            c.XC = XC;
            c.YC = YC;
            c.Rad = rad;
            c.start = start;
            c.end = end;
            Circles.Add(c);
        }

        void createTurtle()
        {

            Turtle pnn = new Turtle();

            pnn.img = new Bitmap("zuma.png");
            pnn.x = 500;
            pnn.y = 400;
            pnn.width = pnn.img.Width;
            pnn.height = pnn.img.Height;

            Turtles.Add(pnn);

        }

        public double Euclidean(int x1, int y1, int x2, int y2)
        {
            int x = Math.Abs(x2 - x1) * Math.Abs(x2 - x1);
            int y = Math.Abs(y2 - y1) * Math.Abs(y2 - y1);

            double distance = Math.Sqrt(x + y);

            return distance;
        }

        void moveTurtle(int v, Graphics g)
        {
            
        }

        private void DrawScene(Graphics g)
        {
            g.Clear(Color.White);
            g.DrawImage(ground.img, 0, 0, ground.width, ground.height);
            /*
            for (int i = 0; i < curves.Count; i++)
            {

                curves[i].drawYourSelf(g, Brushes.Red);
                curves[0].DrawControlPoints(g);
            }*/
            
            for (int i = 0; i < Balls.Count; i++)
            {
                if (Balls[i].draw)
                {
                    carPoint = curves[Balls[i].currentCurve].calcCurvePointAtTime(Balls[i].ballCurrentTime);
                    Balls[i].currentPoint.X = carPoint.X;
                    Balls[i].currentPoint.Y = carPoint.Y;
                    g.DrawImage(Balls[i].img, carPoint.X, carPoint.Y, Balls[i].width, Balls[i].height);
                }

                
            }

            

            for (int i = 0; i < Circles.Count; i++)
            {
                Circles[i].Drawcircle(g, Circles[i].start, Circles[i].end, Circles[i].Rad, Circles[i].XC, Circles[i].YC,true);

            }
            
            

            for (int i = 0; i < Lines.Count; i++)
            {

                // Create a Pen object with a width of 3 pixels and the color red
                Pen pen = new Pen(Color.Green, 3);

                // Draw a line from (x1, y1) to (x2, y2) using the Pen
                g.DrawLine(pen, Lines[i].xs, Lines[i].ys, Lines[i].xe, Lines[i].ye);
            }


            

            Matrix originalMatrix = g.Transform;
            // Set the rotation point to the center of the image
            float centerX = Turtles[0].x + Turtles[0].width / 2f;
            float centerY = Turtles[0].y + Turtles[0].height / 2f;

            // Rotate the image around the center point
            g.TranslateTransform(centerX, centerY);
            g.RotateTransform(angle);
            g.TranslateTransform(-centerX, -centerY);



            // Draw the rotated image
            g.DrawImage(Turtles[0].img, Turtles[0].x, Turtles[0].y, Turtles[0].width, Turtles[0].height);
            //g.DrawImage(ZumaBalls[0].img, ZumaBalls[0].currentPoint.X, ZumaBalls[0].currentPoint.Y, ZumaBalls[0].width, ZumaBalls[0].height);
            g.Transform = originalMatrix;

            for (int i = 0; i < 1; i++)
            {
                g.DrawImage(ZumaBalls[i].img, ZumaBalls[i].currentPoint.X, ZumaBalls[i].currentPoint.Y, ZumaBalls[i].width, ZumaBalls[i].height);
            }
            // g.DrawString("Press Space to toggel (drag) ", new Font("System", 20), Brushes.Black, 10, 10);
        }

        private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            DrawDubb(e.Graphics);
        }

        public void placeTheCurvePoints()
        {
            curves.Add(new BezCurve());
            whichCurve++;
            //Curve 1:
            curves[whichCurve].LCtrPts.Add(new PointF(885, -1));
            curves[whichCurve].LCtrPts.Add(new PointF(1089, 83));
            curves[whichCurve].LCtrPts.Add(new PointF(1231, 151));
            curves[whichCurve].LCtrPts.Add(new PointF(1465, 292));
            curves[whichCurve].LCtrPts.Add(new PointF(1670, 437));
            curves[whichCurve].LCtrPts.Add(new PointF(1700, 508));
            curves[whichCurve].LCtrPts.Add(new PointF(1680, 806));
            curves[whichCurve].LCtrPts.Add(new PointF(1552, 800));
            curves[whichCurve].LCtrPts.Add(new PointF(1399, 900));
            curves[whichCurve].LCtrPts.Add(new PointF(1223, 948));

            curves.Add(new BezCurve());
            whichCurve++;
            // Curve 2:
            curves[whichCurve].LCtrPts.Add(new PointF(1224, 947));
            curves[whichCurve].LCtrPts.Add(new PointF(1012, 988));
            curves[whichCurve].LCtrPts.Add(new PointF(740, 1011));
            curves[whichCurve].LCtrPts.Add(new PointF(554, 977));
            curves[whichCurve].LCtrPts.Add(new PointF(346, 888));
            curves[whichCurve].LCtrPts.Add(new PointF(68, 840));
            curves[whichCurve].LCtrPts.Add(new PointF(0, 690));
            curves[whichCurve].LCtrPts.Add(new PointF(35, 429));
            curves[whichCurve].LCtrPts.Add(new PointF(165, 276));
            curves[whichCurve].LCtrPts.Add(new PointF(408, 182));

            curves.Add(new BezCurve());
            whichCurve++;
            // Curve 3:
            curves[whichCurve].LCtrPts.Add(new PointF(408, 182));
            curves[whichCurve].LCtrPts.Add(new PointF(629, 141));
            curves[whichCurve].LCtrPts.Add(new PointF(817, 188));
            curves[whichCurve].LCtrPts.Add(new PointF(1025, 233));
            curves[whichCurve].LCtrPts.Add(new PointF(1239, 317));
            curves[whichCurve].LCtrPts.Add(new PointF(1534, 536));
            curves[whichCurve].LCtrPts.Add(new PointF(1641, 585));
            curves[whichCurve].LCtrPts.Add(new PointF(1545, 890));
            curves[whichCurve].LCtrPts.Add(new PointF(1125, 851));
            curves[whichCurve].LCtrPts.Add(new PointF(1104, 874));
            curves[whichCurve].LCtrPts.Add(new PointF(902, 872));

            curves.Add(new BezCurve());
            whichCurve++;
            // Curve 4:
            curves[whichCurve].LCtrPts.Add(new PointF(904, 873));
            curves[whichCurve].LCtrPts.Add(new PointF(754, 873));
            curves[whichCurve].LCtrPts.Add(new PointF(663, 843));
            curves[whichCurve].LCtrPts.Add(new PointF(335, 810));
            curves[whichCurve].LCtrPts.Add(new PointF(21, 757));
            curves[whichCurve].LCtrPts.Add(new PointF(14, 375));
            curves[whichCurve].LCtrPts.Add(new PointF(184, 307));
            curves[whichCurve].LCtrPts.Add(new PointF(541, 151));
            curves[whichCurve].LCtrPts.Add(new PointF(688, 252));
            curves[whichCurve].LCtrPts.Add(new PointF(866, 340));
            curves[whichCurve].LCtrPts.Add(new PointF(1247, 407));
            curves[whichCurve].LCtrPts.Add(new PointF(1200, 639));
        }

    }
}


