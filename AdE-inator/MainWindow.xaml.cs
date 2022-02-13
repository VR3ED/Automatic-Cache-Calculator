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

namespace AdE_inator
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool conByteOffsett;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CKgrandBlocco_Checked(object sender, RoutedEventArgs e)
        {
            conByteOffsett = ! CKgrandBlocco.IsChecked.Value;
            cmbBlocco.IsEnabled = true;
        }

        private void CKParoleBlocco_Checked(object sender, RoutedEventArgs e)
        {
            conByteOffsett = CKParoleBlocco.IsChecked.Value;
            if(CKParoleBlocco.IsChecked.Value)
            {
                cmbBlocco.SelectedItem = cmbiD;
                cmbBlocco.IsEnabled = false;
            }         
        }

        public static double conversione(string potenza, double valore)
        {
            double val = valore;

            if (potenza.Contains("kByte"))
            {
                val = valore * Math.Pow(2, 10);
            }
            else if (potenza.Contains("MByte"))
            {
                val = valore * Math.Pow(2, 20);
            }
            else if (potenza.Contains("GByte"))
            {
                val = valore * Math.Pow(2,30);
            }
            else
            {
                val = valore;
            }

            return val;
        }

        private void btnCalcola_Click(object sender, RoutedEventArgs e)
        {
            barraAssociativo.Children.RemoveRange(0, barraAssociativo.Children.Count);
            barraDiretto.Children.RemoveRange(0, barraDiretto.Children.Count);
            barraSetAssociativa.Children.RemoveRange(0, barraSetAssociativa.Children.Count);

            double bitRamAlByte, tag, indBloccoInCache, indiceParola, byteOffset = 0, indSetInCache, NBlocchiCache, NblocchiRam, NlineeCache;

            double ramTotale = conversione(cmbMemGrand.SelectedItem.ToString(), int.Parse(TxtMemTor.Text) );
            double cacheTotale = conversione(cmbCache.SelectedItem.ToString(), int.Parse(TxtCacheTot.Text));
            double grandezzaBlocchi;

            try
            {
                if (CKgrandBlocco.IsChecked.Value)
                {
                    grandezzaBlocchi = conversione(cmbBlocco.SelectedItem.ToString(), int.Parse(TxtBlocco.Text));
                }
                else
                {
                    grandezzaBlocchi = int.Parse(TxtBlocco.Text) * conversione(cmbGrandParola.SelectedItem.ToString(), int.Parse(TxtGrandParola.Text)); //numero parole * Grandezza singola parola
                    byteOffset = Math.Log(int.Parse(TxtGrandParola.Text), 2);
                }

                //cache indirizzamento diretto
                bitRamAlByte = Math.Log(ramTotale, 2);
                indiceParola = Math.Log(grandezzaBlocchi, 2) - byteOffset;
                indBloccoInCache = Math.Log(cacheTotale / grandezzaBlocchi, 2);
                tag = bitRamAlByte - indBloccoInCache - indiceParola - byteOffset;

                NblocchiRam = ramTotale / grandezzaBlocchi;
                NBlocchiCache = cacheTotale / grandezzaBlocchi;

                txbDiretto.Text = "TAG: " + tag + "; indice del blocco in cache: " + indBloccoInCache + "; indice parola nel blocco: " + indiceParola + "; byte offsett:" + byteOffset;
                txbNdiretto.Text = "N°blocchi in ram:2^" + Math.Log(NblocchiRam,2) + "; N°blocchi in cache:2^" + Math.Log( NBlocchiCache,2);

                //riempimento barra indDiretto
                for (double i=bitRamAlByte; i>0; i--)
                {
                    TextBox x = new TextBox();
                    x.IsEnabled = false;
                    x.BorderBrush = System.Windows.Media.Brushes.Black;
                    x.BorderThickness = new Thickness(0, 1, 1, 1);
                    x.Width = 1145 / bitRamAlByte;
                    x.TextAlignment = TextAlignment.Right;

                    if(i<=byteOffset)
                    {
                        x.Background = Brushes.Orange;
                    }
                    else if(i<= byteOffset + indiceParola)
                    {
                        x.Background = Brushes.Blue;
                    }
                    else if (i <= byteOffset + indiceParola + indBloccoInCache)
                    {
                        x.Background = Brushes.Green;
                    }
                    else
                    {
                        x.Background = Brushes.Yellow;
                    }
                    x.Text = i+"";
                    barraDiretto.Children.Add(x);
                }

                //cache associativa
                bitRamAlByte = Math.Log(ramTotale, 2);
                indiceParola = Math.Log(grandezzaBlocchi, 2) - byteOffset;
                tag = bitRamAlByte - indiceParola - byteOffset;

                NblocchiRam = ramTotale / grandezzaBlocchi;
                NBlocchiCache = cacheTotale / grandezzaBlocchi;

                txbAssociativa.Text = "TAG: " + tag + "; indice parola nel blocco: " + indiceParola + "; byte offsett:" + byteOffset;
                txbNAssociativa.Text = "N°blocchi in ram:2^" + Math.Log( NblocchiRam,2) + "; N°blocchi in cache:2^" + Math.Log( NBlocchiCache,2);

                //riempimento barra ind associativo
                for (double i = bitRamAlByte; i > 0; i--)
                {
                    TextBox x = new TextBox();
                    x.IsEnabled = false;
                    x.BorderBrush = System.Windows.Media.Brushes.Black;
                    x.BorderThickness = new Thickness(0, 1, 1, 1);
                    x.Width = 1145 / bitRamAlByte;
                    x.TextAlignment = TextAlignment.Right;

                    if (i <= byteOffset)
                    {
                        x.Background = Brushes.Orange;
                    }
                    else if (i <= byteOffset + indiceParola)
                    {
                        x.Background = Brushes.Blue;
                    }
                    else
                    {
                        x.Background = Brushes.Yellow;
                    }
                    x.Text = i + "";
                    barraAssociativo.Children.Add(x);
                }

                //cache set-associatva
                bitRamAlByte = Math.Log(ramTotale, 2);
                indiceParola = Math.Log(grandezzaBlocchi, 2) - byteOffset;
                double dimensioneRigaCache = int.Parse(txtVie.Text) * grandezzaBlocchi;
                indSetInCache = Math.Log(cacheTotale / dimensioneRigaCache, 2);
                tag = bitRamAlByte - indSetInCache - indiceParola - byteOffset;

                NblocchiRam = ramTotale / grandezzaBlocchi;
                NBlocchiCache = cacheTotale / grandezzaBlocchi;
                NlineeCache = cacheTotale / dimensioneRigaCache;

                txbSetAssociativa.Text = "TAG: " + tag + "; indice del set in cache: " + indSetInCache + "; indice parola nel blocco: " + indiceParola + "; byte offsett:" + byteOffset;
                txbNSetAssociativa.Text = "N°blocchi in ram:2^" + Math.Log( NblocchiRam,2) + "; N°blocchi in cache:2^" + Math.Log( NBlocchiCache,2) + "; N°linee(set) in cache:2^" + Math.Log( NlineeCache,2);

                //riempimento barra ind SetAsssociativo
                for (double i = bitRamAlByte; i > 0; i--)
                {
                    TextBox x = new TextBox();
                    x.IsEnabled = false;
                    x.BorderBrush = System.Windows.Media.Brushes.Black;
                    x.BorderThickness = new Thickness(0, 1, 1, 1);
                    x.Width = 1145 / bitRamAlByte;
                    x.TextAlignment = TextAlignment.Right;

                    if (i <= byteOffset)
                    {
                        x.Background = Brushes.Orange;
                    }
                    else if (i <= byteOffset + indiceParola)
                    {
                        x.Background = Brushes.Blue;
                    }
                    else if (i <= byteOffset + indiceParola + indSetInCache)
                    {
                        x.Background = Brushes.Green;
                    }
                    else
                    {
                        x.Background = Brushes.Yellow;
                    }
                    x.Text = i + "";
                    barraSetAssociativa.Children.Add(x);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("checka il box per stabilire se è con o senza grandezza delle parole");              
            }
            
        }
    }
}
