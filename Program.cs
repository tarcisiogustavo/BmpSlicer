using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BmpSlicer
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Bitmap Files (*.bmp)|*.bmp";
            openFileDialog.Title = "Selecione o arquivo BMP";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Obtenha o caminho do arquivo selecionado
                string inputFile = openFileDialog.FileName;

                // Determine o nome e o caminho do diretório de saída
                string outputDirectory = Path.GetDirectoryName(inputFile);
                string outputPrefix = Path.GetFileNameWithoutExtension(inputFile);

                // Solicita a altura da imagem
                int sliceHeight = -1;
                while (sliceHeight <= 0)
                {
                    string input = ShowInputDialog("BmpSlicer", "Digite a altura da imagem:");
                    if (string.IsNullOrEmpty(input))
                    {
                        return;
                    }

                    if (int.TryParse(input, out sliceHeight) && sliceHeight > 0)
                    {
                        break;
                    }

                    MessageBox.Show("Altura inválida. Digite um número inteiro positivo.", "Altura da imagem", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // Abre o arquivo BMP de entrada
                Bitmap inputBitmap = new Bitmap(inputFile);

                // Crie os arquivos de saída com a altura especificada
                int numSlices = (int)Math.Ceiling((double)inputBitmap.Height / sliceHeight);
                for (int i = 0; i < numSlices; i++)
                {
                    int sliceActualHeight = sliceHeight;
                    if (i == numSlices - 1)
                    {
                        sliceActualHeight = inputBitmap.Height - i * sliceHeight;
                    }

                    Rectangle sourceRectangle = new Rectangle(0, i * sliceHeight, inputBitmap.Width, sliceActualHeight);
                    Bitmap sliceBitmap = inputBitmap.Clone(sourceRectangle, inputBitmap.PixelFormat);

                    string outputFile = Path.Combine(outputDirectory, outputPrefix + i.ToString("D4") + ".bmp");
                    sliceBitmap.Save(outputFile, inputBitmap.RawFormat);

                    sliceBitmap.Dispose();
                }

                inputBitmap.Dispose();

                MessageBox.Show("Processamento concluído.", "Conclusão", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private static string ShowInputDialog(string title, string promptText)
        {
            Form prompt = new Form();
            prompt.Width = 350;
            prompt.Height = 150;
            prompt.Text = title;
            Label textLabel = new Label() { Left = 50, Top = 20, Text = promptText };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 200 };
            Button confirmation = new Button() { Text = "Ok", Left = 100, Width = 100, Top = 70 };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.ShowDialog();
            return textBox.Text;
        }
    }
}
