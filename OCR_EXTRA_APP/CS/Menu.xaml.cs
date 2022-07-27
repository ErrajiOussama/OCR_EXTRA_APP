using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace OCR_EXTRA_APP
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        
        Etat_Civil etat_civil;
        Correction correction;
        OSRisation ocrisation;
        Parametre paramatrie;
        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;
        }
        public Menu()
        {   
            InitializeComponent();
        }
            private void initColor()
            {
                menuEtat_civil.Foreground = Brushes.White;
                menuEtat_civil.Background = Brushes.Orange;
                menuCorrection.Foreground = Brushes.White;
                menuCorrection.Background = Brushes.Orange;
            }

            private void menuEtat_civil_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            {
                if (etat_civil != null)
                {
                    MainFrame.Navigate(etat_civil);
                }
                else
                {
                    etat_civil = new Etat_Civil();
                    MainFrame.Navigate(etat_civil);
                }
            }

            private void menuCorrection_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            {
                if (correction != null)
                {
                    MainFrame.Navigate(correction);
                }
                else
                {
                    correction = new Correction();
                    MainFrame.Navigate(correction);
                }
            }
            private void menuOSResation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            {
                if (ocrisation != null)
                {
                    MainFrame.Navigate(ocrisation);
                }
                else
                {
                    ocrisation = new OSRisation();
                    MainFrame.Navigate(ocrisation);
                }
            }
            private void menuPrametrie_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            {
                if (paramatrie != null)
                {
                    MainFrame.Navigate(paramatrie);
                }
                else
                {
                    paramatrie = new Parametre();
                    MainFrame.Navigate(paramatrie);
                }
            }
       
    }
    }