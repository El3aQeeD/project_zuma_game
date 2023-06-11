using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.SqlServer.Server;

namespace project_zuma
{
    public class Circle
    {
        public int Rad;
        public int XC;
        public int YC;
        public float Xe;
        public float Ye;
        public int start;
        public int end;
        public bool draw = false;
        public List<float> points = new List<float>();
        public void Drawcircle(Graphics g, int start, int end, int Rad, float cx, float cy , bool draw)
        {
            float thRadian = (float)(292 * Math.PI / 180);
            float x = (float)(Rad * Math.Cos(thRadian));
            float y = (float)(Rad * Math.Sin(thRadian));
            x += XC;
            y += YC;
            for (float i = start; i <= end; i += 1f)
            {
                thRadian = (float)(i * Math.PI / 180);
                x = (float)(Rad * Math.Cos(thRadian));
                y = (float)(Rad * Math.Sin(thRadian));

                x += XC;
                y += YC;

                g.FillEllipse(Brushes.Black, x, y, 10, 10);
            }
        }
        public int getAngle(int xm, int ym)
        {
            for (float i = 0; i < 360; i += 1)
            {
                float thRadian = (float)(i * Math.PI / 180);
                float x = (float)(Rad * Math.Cos(thRadian));
                float y = (float)(Rad * Math.Sin(thRadian));

                x += XC;
                y += YC;
                //return (int)x;
                if (ym <= (int)y + 15 && ym >= (int)y - 15 && xm <= (int)x + 15 && xm >= (int)x - 15)
                {
                    return (int)i;
                }

            }
            //double angle2 = Math.Atan2(ym, xm);
            return (int)0;
        }
        public PointF Getnextpoint(int theta)
        {

            PointF p = new PointF();
            float thRadian = (float)(theta * Math.PI / 180);
            p.X = (float)(Rad * Math.Cos(thRadian)) + XC;
            p.Y = (float)(Rad * Math.Sin(thRadian)) + YC;
            return p;
        }
    }
}
