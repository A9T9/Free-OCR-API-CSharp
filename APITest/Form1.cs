using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;


namespace OCRAPITest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            cmbLanguage.SelectedIndex = 5;//English
        }

        private string getSelectedLanguage()
        {

            //https://ocr.a9t9.com/OCRAPI#PostParameters

            //Czech = cze; Danish = dan; Dutch = dut; English = eng; Finnish = fin; French = fre; 
            //German = ger; Hungarian = hun; Italian = ita; Norwegian = nor; Polish = pol; Portuguese = por;
            //Spanish = spa; Swedish = swe; ChineseSimplified = chs; Greek = gre; Japanese = jpn; Russian = rus;
            //Turkish = tur; ChineseTraditional = cht; Korean = kor

            string strLang = "";
            switch (cmbLanguage.SelectedIndex)
            {
                case 0:
                    strLang = "chs";
                    break;

                case 1:
                    strLang = "cht";
                    break;
                case 2:
                    strLang = "cze";
                    break;
                case 3:
                    strLang = "dan";
                    break;
                case 4:
                    strLang = "dut";
                    break;
                case 5:
                    strLang = "eng";
                    break;
                case 6:
                    strLang = "fin";
                    break;
                case 7:
                    strLang = "fre";
                    break;
                case 8:
                    strLang = "ger";
                    break;
                case 9:
                    strLang = "gre";
                    break;
                case 10:
                    strLang = "hun";
                    break;
                case 11:
                    strLang = "jap";
                    break;
                case 12:
                    strLang = "kor";
                    break;
                case 13:
                    strLang = "nor";
                    break;
                case 14:
                    strLang = "pol";
                    break;
                case 15:
                    strLang = "por";
                    break;
                case 16:
                    strLang = "spa";
                    break;
                case 17:
                    strLang = "swe";
                    break;
                case 18:
                    strLang = "tur";
                    break;

            }
            return strLang;

        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Filter = "jpeg files|*.jpg;*.JPG";
            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                FileInfo fileInfo = new FileInfo(fileDlg.FileName);
                if (fileInfo.Length > 1024 * 1024)
                {
                    MessageBox.Show("jpeg file's size can not be larger than 1Mb");
                    return;
                }
                pictureBox.BackgroundImage = Image.FromFile(fileDlg.FileName);
            }
        }

        private byte[] ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                return imageBytes;
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox.BackgroundImage == null)
                return;

            txtResult.Text = "";

            button1.Enabled = false;
            button2.Enabled = false;

            try
            {
                HttpClient httpClient = new HttpClient();

                //Removed the api key from headers
                //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("apikey", "5a64d478-9c89-43d8-88e3-c65de9999580");

                MultipartFormDataContent form = new MultipartFormDataContent();
                //5a64d478-9c89-43d8-88e3-c65de9999580
                form.Add(new StringContent("helloworld"), "apikey"); //Added api key in form data
                form.Add(new StringContent(getSelectedLanguage()), "language");


                byte[] imageData = ImageToBase64(pictureBox.BackgroundImage, System.Drawing.Imaging.ImageFormat.Jpeg);
                form.Add(new ByteArrayContent(imageData, 0, imageData.Length), "image", "image.jpg");

                HttpResponseMessage response = await httpClient.PostAsync("https://ocr.a9t9.com/api/Parse/Image", form);

                string strContent = await response.Content.ReadAsStringAsync();



                Rootobject ocrResult = JsonConvert.DeserializeObject<Rootobject>(strContent);

  
                if (ocrResult.OCRExitCode == 1)
                  {
                         for (int i = 0; i < ocrResult.ParsedResults.Count() ; i++)
                         {
                             txtResult.Text = txtResult.Text + ocrResult.ParsedResults[i].ParsedText ;
                         }
                     }
                     else
                     {
                         MessageBox.Show("ERROR: " + strContent);
                     }
                    


            }
            catch (Exception exception)
            {
                MessageBox.Show("Ooops");
            }

            button1.Enabled = true;
            button2.Enabled = true;
        }



    }
}



