﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Generator_pociągów
{
    public partial class mainForm : Form
    {
        string[] names;
        List<string> aWag = new List<string>();
        List<string> bWag = new List<string>();
        List<string> wWag = new List<string>();
        List<string> loco = new List<string>();

        int count = 0;
        int szerZest = 0;
        List<MiniImage> zestawienie = new List<MiniImage>();

        Bitmap sklad;
        Graphics grph;


        public mainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
             sklad = new Bitmap(pictureBox3.Width, 58);
             names = Directory.GetFiles(Application.StartupPath + @"\res", "*.gif",
                                                     SearchOption.TopDirectoryOnly);
            foreach (string name in names)
            {
                string nameB = Path.GetFileName(name);
                nameB = nameB.Substring(0, nameB.Length - 6); // potem dodac -a.gif
                
                if (nameB.StartsWith("A"))
                {
                    aWag.Add(nameB);
                }
                else if (nameB.StartsWith("B"))
                {
                    bWag.Add(nameB);
                }
                else if (nameB.StartsWith("W"))
                {
                    wWag.Add(nameB);
                }
                else
                {
                    loco.Add(nameB);
                }
            }

            comboBox1.SelectedIndex = 0;
            listBox1.DataSource = aWag;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(addPosrTxt.Text != "")
            {
                posrList.Items.Add(addPosrTxt.Text);
                Globals.posrednie.Add(addPosrTxt.Text);
                addPosrTxt.Text = "";
                addPosrTxt.Focus();
            }
            else
            {

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int idx = posrList.SelectedIndex;
                posrList.Items.RemoveAt(idx);
                Globals.posrednie.RemoveAt(idx);
            }
            catch (Exception ex)
            {

            }
        }

        private void numberTxt_TextChanged(object sender, EventArgs e)
        {
            Globals.number = numberTxt.Text;
        }

        private void nameTxt_TextChanged(object sender, EventArgs e)
        {
            Globals.name = nameTxt.Text;
        }

        private void entTxt_TextChanged(object sender, EventArgs e)
        {
            Globals.finish = entTxt.Text.ToUpper();
        }

        private void startTxt_TextChanged(object sender, EventArgs e)
        {
            Globals.start = startTxt.Text.ToUpper();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(pictureBox1.BackgroundImage != null)
            {
                DialogResult dr = saveFileDialog1.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    pictureBox1.BackgroundImage.Save(saveFileDialog1.FileName);
                }
            }
            else
            {
                MessageBox.Show("Proszę wygenerowac najpierw podgląd", "Ostrzeżenie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        // Generator
        private void button4_Click(object sender, EventArgs e)
        {
            if (startTxt.Text != "" && entTxt.Text != ""
                && nameTxt.Text != "" && numberTxt.Text != "")
            {
                try
                {
                    Bitmap bm = new Bitmap(1080, 795);
                    Graphics graphics = Graphics.FromImage(bm);
                    Pen pen = new Pen(Color.Black, 5f);
                    SolidBrush bg = new SolidBrush(Color.White);
                    Rectangle rect = new Rectangle(0 + 50, 60, 980, 100);


                    // Background
                    PointF[] pf = new PointF[]
                    {
                new PointF(0, 0),
                new PointF(0, 795),
                new PointF(1080, 795),
                new PointF(1080, 0)
                    };
                    graphics.FillPolygon(bg, pf);

                    // Separator
                    graphics.DrawLine(pen, new PointF(0 + 50, 150), new PointF(1080 - 50, 150));

                    // Number
                    graphics.DrawString(Globals.number, new Font("Arial", 50), Brushes.Red, rect);

                    // Name
                    StringFormat formatB = new StringFormat(StringFormatFlags.DirectionRightToLeft);

                    graphics.DrawString(Globals.name.ToUpper(), new Font("Arial", 50, FontStyle.Italic), Brushes.Red, rect, formatB);

                    // Start station

                    Rectangle rectStart = new Rectangle(0 + 50, 200, 1080 - 50, 150);
                    graphics.DrawString(Globals.start, new Font("Arial", 50), Brushes.Black, rectStart);


                    // Final station
                    Rectangle rectEnd = new Rectangle(0 + 50, 695, 980, 150);
                    graphics.DrawString(Globals.finish, new Font("Arial", 50), Brushes.Black, rectEnd, formatB);


                    // Stacje posrendnie

                    StringFormat format = new StringFormat();
                    format.LineAlignment = StringAlignment.Center;
                    format.Alignment = StringAlignment.Center;

                    Rectangle posr = new Rectangle(50, 285, 980, 400);
                    string posrednie = "";
                    for (int i = 0; i < Globals.posrednie.Count; i++)
                    {
                        posrednie += Globals.posrednie[i] + " - ";
                    }

                    if (posrednie.Length > 2)
                    {
                        posrednie = posrednie.Substring(0, posrednie.Length - 2);
                    }

                    if (posrednie.Length > 180)
                    {
                        graphics.DrawString(posrednie, new Font("Arial", 30), Brushes.Black, posr, format);
                    }
                    else if (posrednie.Length < 100)
                    {
                        graphics.DrawString(posrednie, new Font("Arial", 40), Brushes.Black, posr, format);

                    }
                    else
                    {
                        graphics.DrawString(posrednie, new Font("Arial", 35), Brushes.Black, posr, format);
                    }


                    // Show graphics
                    pictureBox1.BackgroundImage = null;
                    pictureBox1.BackgroundImage = bm;
                }
                catch (Exception ex)
                {
                    DialogResult dr = MessageBox.Show("Podczas generowania tabliczki wystąpił błąd. Sprawdź czy pola" +
                        " początku relacji, końca relacji, nazwy pociągu i numeru są wypełnione. W przeciwnym wypadku" +
                        " prześlij poniższą treść w zgłoszeniu.\n\n" + ex.ToString() +
                        "\n\nCzy chcesz zgłosić błąd na githubie?", "Błąd", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                    if (dr == DialogResult.Yes)
                    {
                        Process.Start("https://github.com/panmrherobrine/train-generator/issues");
                    }
                }
            }
            if (startTxt.Text == "")
            {
                startTxt.BackColor = Color.Red;
            }
            else
            {
                startTxt.BackColor = Color.White;
            }
            if (entTxt.Text == "")
            {
                entTxt.BackColor = Color.Red;
            }
            else
            {
                entTxt.BackColor = Color.White;
            }
            if (nameTxt.Text == "")
            {
                nameTxt.BackColor = Color.Red;
            }
            else
            {
                nameTxt.BackColor = Color.White;
            }
            if (numberTxt.Text == "")
            {
                numberTxt.BackColor = Color.Red;
            }
            else
            {
                numberTxt.BackColor = Color.White;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Process.Start("https://skrj.plk-sa.pl/kalkulacja/");
        }


        // Generator zestawien
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.Text)
            {
                case "Wagony A":
                    listBox1.DataSource = aWag;
                    break;
                case "Wagony B":
                    listBox1.DataSource = bWag;
                    break;
                case "Wagony W":
                    listBox1.DataSource = wWag;
                    break;
                default:
                    listBox1.DataSource = loco;
                    break;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                pictureBox2.BackgroundImage = new Bitmap(Application.StartupPath + @"\res\" + listBox1.SelectedItem + "-a.gif");
            }
            catch (Exception ex)
            {

            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Graphics g = Graphics.FromImage(sklad);
            if (pictureBox2.BackgroundImage.Height >= 40 && pictureBox2.BackgroundImage.Height <= 50)
            {
                g.DrawImage(pictureBox2.BackgroundImage,
                    new Rectangle(0 + szerZest, 58 - pictureBox2.BackgroundImage.Height, pictureBox2.BackgroundImage.Width, 41),
                    new Rectangle(0, 0, pictureBox2.BackgroundImage.Width, pictureBox2.BackgroundImage.Height),
                    GraphicsUnit.Pixel);
            }
            else if(pictureBox2.BackgroundImage.Height >= 52)
            {
                g.DrawImage(pictureBox2.BackgroundImage,
                    new Rectangle(0 + szerZest, 58 - pictureBox2.BackgroundImage.Height, pictureBox2.BackgroundImage.Width, 58),
                    new Rectangle(0, 0, pictureBox2.BackgroundImage.Width, pictureBox2.BackgroundImage.Height),
                    GraphicsUnit.Pixel);
            }
            else
            {
                MessageBox.Show(pictureBox2.BackgroundImage.Height.ToString());
            }
            szerZest += pictureBox2.BackgroundImage.Width;

            pictureBox3.Image = sklad;

            hScrollBar1.Maximum = szerZest + 7;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            szerZest = 0;
            sklad = new Bitmap(pictureBox3.Width, 58);
            pictureBox3.Image = null;
            hScrollBar1.Maximum = szerZest + 7;
            hScrollBar1.Value = 0;
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            pictureBox3.Location = new Point(-hScrollBar1.Value,
                pictureBox3.Location.Y);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if(pictureBox3.Image != null)
            {
                Bitmap map = new Bitmap(szerZest, 58);
                Rectangle rect = new Rectangle(0, 0, szerZest, 58);
                Graphics gr = Graphics.FromImage(map);
                gr.DrawImage(pictureBox3.Image, rect, rect, GraphicsUnit.Pixel);

                DialogResult dr = saveFileDialog1.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    map.Save(saveFileDialog1.FileName);
                }
            }
            else
            {
                MessageBox.Show("Nie wprowadzono żadnego zestawienia", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button9_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/panmrherobrine/train-generator/issues");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (pictureBox1.BackgroundImage != null)
            {
                Clipboard.SetImage(pictureBox1.BackgroundImage);
            }
            else
            {
                MessageBox.Show("Proszę wygenerowac najpierw podgląd", "Ostrzeżenie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (pictureBox3.Image != null)
            {
                Bitmap map = new Bitmap(szerZest, 58);
                Rectangle rect = new Rectangle(0, 0, szerZest, 58);
                Graphics gr = Graphics.FromImage(map);
                gr.DrawImage(pictureBox3.Image, rect, rect, GraphicsUnit.Pixel);

                Clipboard.SetImage(map);
            }
            else
            {
                MessageBox.Show("Nie wprowadzono żadnego zestawienia", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            posrList.Items.Clear();
            Globals.posrednie = new List<string>();
        }
    }

    class MiniImage
    {
        int width;
        Image image;
        public MiniImage(Image image, int width)
        {
            this.image = image;
            this.width = width;
        }
    }
}
