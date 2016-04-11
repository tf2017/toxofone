namespace Toxofone.UI
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Windows.Forms;
    using Svg;

    public static class UIUtils
    {
        public static void DrawWithSvg(PictureBox pb, string svgLocator)
        {
            if (pb == null)
            {
                return;
            }

            if (pb.Width <= 0 || pb.Height <= 0)
            {
                return;
            }

            if (string.IsNullOrEmpty(svgLocator))
            {
                if (pb.InvokeRequired)
                {
                    pb.BeginInvoke(new Action(() =>
                    {
                        Clear(pb, Color.Transparent);
                    }));

                }
                else
                {
                    Clear(pb, Color.Transparent);
                }
                return;
            }

            try
            {
                SvgDocument svgDoc = null;
                if (svgLocator.StartsWith("res://", StringComparison.OrdinalIgnoreCase))
                {
                    String resName = svgLocator.Substring(6);
                    using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(resName))
                    {
                        svgDoc = SvgDocument.Open<SvgDocument>(s, null);
                    }
                }
                else if (svgLocator.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                {
                    String filePath = svgLocator.Substring(7);
                    using (Stream s = File.Open(filePath, FileMode.Open, FileAccess.Read))
                    {
                        svgDoc = SvgDocument.Open<SvgDocument>(s, null);
                    }
                }
                else if (svgLocator.IndexOf("<svg") >= 0 && svgLocator.IndexOf("</svg>") >= 0)
                {
                    using (Stream s = new MemoryStream(Encoding.UTF8.GetBytes(svgLocator)))
                    {
                        svgDoc = SvgDocument.Open<SvgDocument>(s, null);
                    }
                }
                else
                {
                    throw new ArgumentException("Unsupported SVG locator supplied: " + svgLocator);
                }

                if (pb.InvokeRequired)
                {
                    pb.BeginInvoke(new Action<SvgDocument>((svgDocToDraw) =>
                    {
                        DrawSvgDoc(pb, svgDocToDraw);
                    }), svgDoc);

                }
                else
                {
                    DrawSvgDoc(pb, svgDoc);
                }
            }
            catch (Exception)
            {
                if (pb.InvokeRequired)
                {
                    pb.BeginInvoke(new Action(() =>
                    {
                        DrawErrorMark(pb, Color.Red);
                    }));

                }
                else
                {
                    DrawErrorMark(pb, Color.Red);
                }
            }
        }

        private static void DrawSvgDoc(PictureBox pb, SvgDocument svgDoc)
        {
            pb.Image = svgDoc.Draw(pb.Width, pb.Height);
        }

        private static void Clear(PictureBox pb, Color color)
        {
            Bitmap bmp = new Bitmap(pb.Width, pb.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(color);
            }
            pb.Image = bmp;
        }

        private static void DrawErrorMark(PictureBox pb, Color color)
        {
            Bitmap bmp = new Bitmap(pb.Width, pb.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                using (Pen pen = new Pen(color, -1))
                {
                    g.DrawRectangle(pen, 0, 0, pb.Width - 1, pb.Height - 1);
                    g.DrawLine(pen, 0, 0, pb.Width - 1, pb.Height - 1);
                    g.DrawLine(pen, pb.Width - 1, 0, 0, pb.Height - 1);
                }
            }
            pb.Image = bmp;
        }
    }
}
