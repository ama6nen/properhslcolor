using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace properhslcolor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
           
            InitializeComponent();
            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty
             | BindingFlags.Instance | BindingFlags.NonPublic, null,
             panel1, new object[] { true });
        }

        const float speed = 0.005f;
        const float speed_other = speed*20f;
        bool mode1 = false, mode2 = false;
        Vector3 hsl = new Vector3(0f, 0.5f, 0.5f); //hue saturation lightness

        void change_color()
        {
            hsl.x += speed;
            if (hsl.x > 1f)
            {
                hsl.x = 0f;
                hsl.y += mode1 ? -speed_other : speed_other;
            }

            if (hsl.y > 1f || hsl.y < 0.4f)
            {
                if (hsl.y > 1f)
                {
                    mode1 = true;
                    hsl.y -= speed_other;
                }
                else
                {
                    mode1 = false;
                    hsl.y += speed_other;
                }
                hsl.z += mode2 ? -speed_other : speed_other;

            }

            if (hsl.z > 0.7f || hsl.z < 0.3f)
            {
                if (hsl.z > 0.7f)
                {
                    mode2 = true;
                    hsl.z -= speed_other;
                }
                else
                {
                    mode2 = false;
                    hsl.z += speed_other;
                }
            }

            var cl = hsl.to_rgb();
            exampletxt.ForeColor = cl;
            label1.Text = $"[stats]{Environment.NewLine}rgb({cl.R}, {cl.G}, {cl.B}){Environment.NewLine}hsl({hsl.ToString()})";

        }
      
        private async void Panel1_Paint(object sender, PaintEventArgs e)
        {
            change_color();
            var then = DateTime.Now;

            var br = new LinearGradientBrush(panel1.ClientRectangle, Color.Black, Color.Black, 0, false);
            var cb = new ColorBlend();
            cb.Positions = new[] { 0f, 0.33f, 0.66f, 1f };
            cb.Colors = new[] { hsl.to_rgb(), new Vector3(hsl.x - 0.075f, hsl.y, hsl.z).to_rgb(), new Vector3(hsl.x - 0.15f, hsl.y, hsl.z).to_rgb(), new Vector3(hsl.x - 0.225f, hsl.y, hsl.z).to_rgb() };
            br.InterpolationColors = cb;
            e.Graphics.FillRectangle(br, panel1.ClientRectangle);
            await Task.Delay(Math.Max(1000 / 60 - (int)(DateTime.Now - then).TotalMilliseconds, 0));
            panel1.Invalidate();
        }
    }
 
    public class Vector3
    {
        public float x, y, z;

        public Vector3(float x2, float y2, float z2)
        {
            this.x = x2;
            this.y = y2;
            this.z = z2;
        }

        public override string ToString() => $"{Math.Round(x, 2)}, {Math.Round(y, 2)}, {Math.Round(z, 2)}";
        public Color to_rgb()
        {
            double h = this.x;
            double sl = this.y;
            double l = this.z;

            if (h < 0f)
               h = 1f - -h;
            else if (h > 1f)
                h = -(-h + 1f);

            double v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl), r = l, g = l , b = l;
            
            if (v > 0)
            {
                int sextant;
                double m, sv, fract, vsf, mid1, mid2;

                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0;
                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }

            return Color.FromArgb(Convert.ToByte(r * 255.0f), Convert.ToByte(g * 255.0f), Convert.ToByte(b * 255.0f));
        }

    }
}
