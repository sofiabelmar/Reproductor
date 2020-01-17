using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;



namespace Reproductor
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AudioFileReader reader; //leer el archivo
        WaveOut output; // reproducir el archivo, exclusivo de salida

        public MainWindow()
        {
            InitializeComponent();
            ListaDispositivosSalida();

            btnReproducir.IsEnabled = false; //deshabilitar botines
            btnDetener.IsEnabled = false;
            btnPausa.IsEnabled = false;
        }

        void ListaDispositivosSalida()  //lo que hace esta funcion es tomar el nombre de cada dispositivo de salida y con un for los recore e imprime en el cb
        {
            cbDispositivoSalida.Items.Clear();
            for(int i=0; i< WaveOut.DeviceCount; i++)
            {
                WaveOutCapabilities capacidades = WaveOut.GetCapabilities(i);
                cbDispositivoSalida.Items.Add(capacidades.ProductName);
            }
            cbDispositivoSalida.SelectedIndex = 0;
        }

        private void BtnExaminar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                txtRutaArchivo1.Text = openFileDialog.FileName;
                btnReproducir.IsEnabled = true;
            }
        }

        private void BtnReproducir_Click(object sender, RoutedEventArgs e)
        {

            if (output != null && output.PlaybackState == PlaybackState.Paused)
            {
                //retomo la reproducción
                output.Play();
                btnReproducir.IsEnabled = false;
                btnPausa.IsEnabled = true;
                btnDetener.IsEnabled = false;
            }
            else
            {
                if (txtRutaArchivo1.Text != null && txtRutaArchivo1.Text != string.Empty)
                {
                    reader = new AudioFileReader(txtRutaArchivo1.Text);

                    output = new WaveOut();
                    output.DeviceNumber = cbDispositivoSalida.SelectedIndex; //Se le dice en que dispositivo se va a reproducir

                    output.PlaybackStopped += Output_PlaybackStopped;

                    output.Init(reader); //inicializar el output (reproduccion), necesita un wave provider en este caso "reader"
                    output.Play(); // Reproducir el archivo

                    btnReproducir.IsEnabled = false; //habilitar botones
                    btnDetener.IsEnabled = true;
                    btnPausa.IsEnabled = true;

                    lblTiempoTotal.Text = reader.TotalTime.ToString().Substring(0, 8);
                }
            }

            
                
        }

        private void Output_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            reader.Dispose();
            output.Dispose();
        }

        private void BtnDetener_Click(object sender, RoutedEventArgs e)
        {
            if(output != null)
            {
                output.Stop();
                btnReproducir.IsEnabled = true;
                btnPausa.IsEnabled = false;
                btnDetener.IsEnabled = false;
            }
        }

        private void BtnPausa_Click(object sender, RoutedEventArgs e)
        {
            if (output != null)
            {
                output.Pause();
                btnReproducir.IsEnabled = true;
                btnPausa.IsEnabled = false;
                btnDetener.IsEnabled = true;
            }
        }
    }
}
