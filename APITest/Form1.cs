using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;


namespace OCRAPITest
{
    public partial class Form1 : Form
    {

        public string ImagePath { get; set; }
        public string PdfPath { get; set; }

        public Form1()
        {
            InitializeComponent();
            cmbLanguage.SelectedIndex = 6;//English
        }

        private string getSelectedLanguage()
        {

            //https://ocr.space/OCRAPI#PostParameters

            //Czech = cze; Danish = dan; Dutch = dut; English = eng; Finnish = fin; French = fre; 
            //German = ger; Hungarian = hun; Italian = ita; Norwegian = nor; Polish = pol; Portuguese = por;
            //Spanish = spa; Swedish = swe; ChineseSimplified = chs; Greek = gre; Japanese = jpn; Russian = rus;
            //Turkish = tur; ChineseTraditional = cht; Korean = kor

            string strLang = "";
            switch (cmbLanguage.SelectedIndex)
            {
                case 0:
                    strLang = "ara";
                    break;

                case 1:
                    strLang = "chs";
                    break;

                case 2:
                    strLang = "cht";
                    break;
                case 3:
                    strLang = "cze";
                    break;
                case 4:
                    strLang = "dan";
                    break;
                case 5:
                    strLang = "dut";
                    break;
                case 6:
                    strLang = "eng";
                    break;
                case 7:
                    strLang = "fin";
                    break;
                case 8:
                    strLang = "fre";
                    break;
                case 9:
                    strLang = "ger";
                    break;
                case 10:
                    strLang = "gre";
                    break;
                case 11:
                    strLang = "hun";
                    break;
                case 12:
                    strLang = "jap";
                    break;
                case 13:
                    strLang = "kor";
                    break;
                case 14:
                    strLang = "nor";
                    break;
                case 15:
                    strLang = "pol";
                    break;
                case 16:
                    strLang = "por";
                    break;
                case 17:
                    strLang = "spa";
                    break;
                case 18:
                    strLang = "swe";
                    break;
                case 19:
                    strLang = "tur";
                    break;

            }
            return strLang;

        }

       private void button1_Click(object sender, EventArgs e)
        {
            PdfPath = ImagePath = ""; pictureBox.BackgroundImage = null;
            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Filter = "jpeg and png files|*.png;*.jpg;*.JPG";
            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                FileInfo fileInfo = new FileInfo(fileDlg.FileName);
                if (fileInfo.Length > 5* 1024 * 1024)
                {
                    //Size limit depends: Free API 1 MB, PRO API 5 MB and more
                    MessageBox.Show("Image file size limit reached (1MB free API)");
                    return;
                }
                pictureBox.BackgroundImage = Image.FromFile(fileDlg.FileName);
                ImagePath = fileDlg.FileName;
                lblInfo.Text = "Image loaded: "+ fileInfo.Name;
                lblInfo.BackColor = Color.LightGreen;
            }
        }

        private void btnPDF_Click(object sender, EventArgs e)
        {
            PdfPath = ImagePath = "";
            pictureBox.BackgroundImage = null;
            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Filter = "pdf files|*.pdf;";
            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                FileInfo fileInfo = new FileInfo(fileDlg.FileName);
                if (fileInfo.Length > 5* 1024 * 1024 )
                {
                    //Size limit depends: Free API 1 MB, PRO API 5 MB and more
                    MessageBox.Show("PDF file size should not be larger than 5Mb");
                    return;
                }
                PdfPath = fileDlg.FileName;
                //PDF files are loaded, but can not be displayed in the image control. That does not affect the OCR.
                lblInfo.Text = "PDF loaded [but not displayed]: " + fileInfo.Name;
                lblInfo.BackColor = Color.LightSalmon;
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


            if (string.IsNullOrEmpty(ImagePath) && string.IsNullOrEmpty(PdfPath))
                return;

            txtResult.Text = "";

            button1.Enabled = false;
            button2.Enabled = false;
            btnPDF.Enabled = false;

            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.Timeout = new TimeSpan(1, 1, 1);


                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new StringContent("helloworld"), "apikey"); //Added api key in form data
                form.Add(new StringContent(getSelectedLanguage()), "language");

                form.Add(new StringContent("2"), "ocrengine"); 
                form.Add(new StringContent("true"), "scale");
                form.Add(new StringContent("true"), "istable");

                if (string.IsNullOrEmpty(ImagePath) == false)
                {
                    byte[] imageData = File.ReadAllBytes(ImagePath);
                    form.Add(new ByteArrayContent(imageData, 0, imageData.Length), "image", "image.jpg");
                }
                else if (string.IsNullOrEmpty(PdfPath) == false)
                {
                    byte[] imageData = File.ReadAllBytes(PdfPath);
                    form.Add(new ByteArrayContent(imageData, 0, imageData.Length), "PDF", "pdf.pdf");
                }

                HttpResponseMessage response = await httpClient.PostAsync("https://api.ocr.space/Parse/Image", form);

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
            btnPDF.Enabled = true;
        }


    }
}



