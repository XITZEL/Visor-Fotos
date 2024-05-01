using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace VisorFotos
{
    public partial class UserControl1 : UserControl
    {
        public delegate void ImagenSeleccionadaDelegate(object sender, ImagenSeleccionadaArgs e);
        public event ImagenSeleccionadaDelegate ImagenSeleccionada;

        private string directorio = "";
        private int dimension;
        private int separacion;

        private int borde = 7;
        private List<ImagenConNombre> imagenes = new List<ImagenConNombre>();

        public string Directorio
        {
            get { return directorio; }
            set
            {
                directorio = value;
                getImagenes();
                UpdateControl();
            }
        }

        public int Dimension
        {
            get { return dimension; }
            set
            {
                dimension = value;
                UpdateControl();
            }
        }

        public int Separacion
        {
            get { return separacion; }
            set
            {
                separacion = value;
                UpdateControl();
            }
        }




        public UserControl1()
        {
            InitializeComponent();
        }

        private void getImagenes()
        {
            if (directorio.Length != 0)
            {
                imagenes.Clear();
                DirectoryInfo dir = new DirectoryInfo(Directorio);
                foreach (FileInfo file in dir.GetFiles("*.jpg"))
                {
                    imagenes.Add(new ImagenConNombre(Bitmap.FromFile(file.FullName), file.FullName));
                }
            }
        }

        private void UpdateControl()
        {
            panel1.SuspendLayout();

            foreach (Control ctrl in panel1.Controls)
            {
                ctrl.Dispose();
            }

            panel1.Controls.Clear();

            int col = borde, fila = borde;

            foreach (ImagenConNombre img in imagenes)
            {
                PictureBox pic = new PictureBox();
                pic.Image = img.Imagen;
                pic.Tag = img.FileName;
                pic.Size = new Size(dimension, dimension);
                pic.Location = new Point(col, fila);

                panel1.Controls.Add(pic);

                pic.Click += new EventHandler(this.pic_Click);

                col += dimension + separacion;
                if ((col + dimension + separacion + borde) > this.Width)
                {
                    col = borde;
                    fila += dimension + separacion;
                }
            }

            panel1.ResumeLayout();
        }

        public void Refresh()
        {
            getImagenes();
            UpdateControl();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            UpdateControl();
            base.OnSizeChanged(e);
        }

        private void pic_Click(object sender, System.EventArgs e)
        {
            PictureBox picSeleccionado = (PictureBox)sender;
            ImagenSeleccionadaArgs args = new ImagenSeleccionadaArgs(picSeleccionado.Image, (string)picSeleccionado.Tag);
            ImagenSeleccionada(this, args);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }

    internal class ImagenConNombre
    {
        public Image Imagen { get; set; }
        public string FileName { get; set; }
        public ImagenConNombre(Image img, string fileName)
        {
            Imagen = img;
            FileName = fileName;
        }
    }

    public class ImagenSeleccionadaArgs : EventArgs
    {
        public Image Imagen { get; set; }
        public string FileName { get; set; }
        public ImagenSeleccionadaArgs(Image img, string fileName)
        {
            Imagen = img;
            FileName = fileName;
        }
    }
}