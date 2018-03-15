using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Photo_tools
{
    public partial class Form1 : Form
    {
        //TODO:
        /*
         * 1.選擇資料夾
         * 2.抓取內部檔案
         * 3.過濾只抓照片
         * 4.照片切割
         * 5.存取
        */
        String _SavePath = "";
        String _Path = "";  //User給的路徑
        String[] _File;     //User給的路徑下所有檔案
        List<String> _target;//User給的路徑下"符合條件"的檔案
        List<String> _targetName;//User給的路徑下"符合條件"的檔案
        int width = 640;
        int height = 400;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            StringBuilder sb3 = new StringBuilder();
            

            try
            {
                //選擇資料夾
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    _Path = folderBrowserDialog1.SelectedPath;
                    _target = new List<string>();
                    _File = Directory.GetFiles(_Path);
                }



                //SHOW出所有檔案
                foreach (var x in _File)
                {
                    sb.Append(x).Append("\r\n");
                }

                //過濾
                //Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*
                List<String> Regex_pattern = new List<string>();
                _targetName = new List<string>();
                Regex_pattern.Add(".*jpg");
                Regex_pattern.Add(".*JPG");
                Regex_pattern.Add(".*bmp");
                Regex_pattern.Add(".*BMP");
                Regex_pattern.Add(".*gif");
                Regex_pattern.Add(".*GIF");
                Regex_pattern.Add(".*png");
                Regex_pattern.Add(".*PNG");
                foreach (var s in Regex_pattern)
                {
                    Regex regex = new Regex(s);
                    MatchCollection mc = regex.Matches(sb.ToString());
                    foreach (var x in mc)
                    {
                        _target.Add(x.ToString());
                        String ss = x.ToString().Substring(_Path.Length + 1);
                        sb2.Append(ss).Append("\r\n");
                        
                    }
                }

                //符合的檔案
                textBox1.Text = sb2.ToString();
                label1.Text = "訊息: 共" + _target.Count() + "個符合的檔案";


                //存檔名稱
                Regex reg = new Regex(@".*\.");
                MatchCollection mc2 = reg.Matches(sb2.ToString());
                foreach (var x in mc2)
                {
                    _targetName.Add(x.ToString());
                }
                


            }
            catch (NullReferenceException nrex) { MessageBox.Show("未選擇檔案\r\n"+nrex.ToString()); }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
            {
                _SavePath = folderBrowserDialog2.SelectedPath;
                textBox4.Text = _SavePath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            //基礎設定
            if (!String.IsNullOrEmpty(textBox2.Text) && !String.IsNullOrEmpty(textBox3.Text))
            {
                width = int.Parse(textBox2.Text);
                height = int.Parse(textBox3.Text);
            }
            String Extension = "";
            System.Drawing.Imaging.ImageFormat format;
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    format = System.Drawing.Imaging.ImageFormat.Jpeg;
                    Extension ="jpg";
                    break;
                case 1:
                    format = System.Drawing.Imaging.ImageFormat.Png;
                    Extension = "png";
                    break;
                case 2:
                    format = System.Drawing.Imaging.ImageFormat.Gif;
                    Extension = "gif";
                    break;
                case 3:
                    format = System.Drawing.Imaging.ImageFormat.Bmp;
                    Extension = "bmp";
                    break;
                default:
                    format = System.Drawing.Imaging.ImageFormat.Jpeg;
                    Extension = "jpg";
                    break;
            }




            //如果沒有裁切
            if (!checkBox1.Checked)
            {
                try
                {
                    for (int i = 0; i < _targetName.Count; i++)
                    {
                        Image ii = Image.FromFile(_target[i]);
                        Bitmap bp = new Bitmap(ii, width, height);
                        //如果有浮水印
                        

                        bp.Save(_SavePath + @"\" + "(處理)" + _targetName[i] + Extension, format);
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.ToString()); }
                finally { MessageBox.Show("處理完成"); }
            }
            else
            {
                try
                {
                    for (int i = 0; i < _targetName.Count; i++)
                    {
                        Image ii = Image.FromFile(_target[i]);
                        Statistics.Screen sc = new Statistics.Screen(ii, width, height, int.Parse(textBox6.Text), int.Parse(textBox5.Text));
                        for (int j = 0; j < sc.Img.Count; j++)
                        {
                           
                            sc.Img[j].Save(_SavePath + @"\" + "(處理)" + _targetName[i] + j + "." + Extension, format);
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.ToString()); }
                finally { MessageBox.Show("處理完成"); }
            }


        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox5.Enabled = true;
                textBox6.Enabled = true;
            }
            else
            {
                textBox5.Enabled = false;
                textBox6.Enabled = false;
            }
        }

        

        private void Watermark(Image img,String text)
        {
            Bitmap bitmap = new Bitmap(img);
            Graphics gg = Graphics.FromImage(bitmap);
            Font ff = new Font("標楷體", 20, FontStyle.Bold);
            Brush bb = new SolidBrush(Color.Red);
            gg.DrawString(text, ff, bb, new Point(20, 20));

        }

       
    }
}
