using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace BlackJack_
{
    /// <summary>
    /// Interaction logic for Spel.xaml
    /// </summary>
    public partial class Spel : Window
    {
        List<string> kaarten = new List<string>();
        string[] types = { "Klaveren", "Ruiten", "Harten", "Schoppen" };
        List<string> spelerKaarten = new List<string>();
        List<string> bankKaarten = new List<string>();
        Random rnd = new Random();
        SolidColorBrush groen = new SolidColorBrush(Colors.Green);
        SolidColorBrush rood = new SolidColorBrush(Colors.Red);
        int budget = 0;
        int inzet;
        public Spel()
        {
            for (int i = 0; i < 4; i++)
            {
                kaarten.Add($"{types[i]} Aas");
                for (int j = 2; j < 11; j++)
                {
                    kaarten.Add($"{types[i]} {j}");
                }
                kaarten.Add($"{types[i]} Boer");
                kaarten.Add($"{types[i]} Koningin");
                kaarten.Add($"{types[i]} Koning");
                //Console.WriteLine( kaarten.Count );
            }
            InitializeComponent();

        }

        private int KaartenOptellen(List<string> listKaarten)
        {
            
            int som = 0;
            for (int i = 0; i < listKaarten.Count; i++)
            {
                string[] cijfer = listKaarten[i].Split(' ');
                if (cijfer[1] == "Boer" || cijfer[1] == "Koningin" || cijfer[1] == "Koning")
                {
                    som += 10;
                }
                else if (cijfer[1] == "Aas")
                {
                    som += 1;
                }
                else
                {
                    som += Convert.ToInt32(cijfer[1]);
                }
            }
            return som;
        }

        private Boolean ControleBust(int som)
        {
            if (som > 21)
            {
                HitButton.IsEnabled = false;
                StandButton.IsEnabled = false;
                return true;
            }
            return false;
        }

        private void StandaardInstellingen()
        {
            SpelerTxtBox.Text = string.Empty;
            BankTxtBox.Text = string.Empty;
            spelerKaarten.Clear();
            bankKaarten.Clear();
            HitButton.IsEnabled = false;
            StandButton.IsEnabled = false;
            UitslagLbl.Content = string.Empty;
        }

        private void GeefKaart(Boolean isSpeler)
        {
            if (isSpeler == true)
            {
                spelerKaarten.Add((string)kaarten[rnd.Next(0, 52)]);
                SpelerTxtBox.Text += spelerKaarten.Last().ToString() + "\r\n";
                SpelerLbl.Content = KaartenOptellen(spelerKaarten);
            }
            else
            {
                bankKaarten.Add((string)kaarten[rnd.Next(0, 52)]);
                BankTxtBox.Text += bankKaarten.Last().ToString() + "\r\n";
                BankLbl.Content = KaartenOptellen(bankKaarten);
            }
        }
        private void DeelButton_Click(object sender, RoutedEventArgs e)
        {
            StandaardInstellingen();
            inzet = Convert.ToInt32(Math.Round(budget * (InzetSlider.Value/10)));
            if (inzet > (budget / 10))
            {
                InzetLbl.Content= inzet.ToString();
                HitButton.IsEnabled = true;
                StandButton.IsEnabled = true;
                for (int i = 0; i < 2; i++)
                {
                    GeefKaart(true);
                }
                GeefKaart(false);
            }
            else
            {
                MessageBox.Show("Inzet moet minimaal 10% bedragen van het budget.");
                DeelButton.IsEnabled = true;
                StandButton.IsEnabled = false;
                HitButton.IsEnabled = false;
            }
            Console.WriteLine(inzet);
            
        }

        private void HitButton_Click(object sender, RoutedEventArgs e)
        {
            int som;
            GeefKaart(true);
            som = KaartenOptellen(spelerKaarten);
            if (ControleBust(som) == true)
            {
                SpelerLbl.Content = "Bust";
                UitslagLbl.Content = "Verloren";
                UitslagLbl.Foreground = rood;
                HitButton.IsEnabled = false;
                StandButton.IsEnabled = false;
                budget -= inzet;
                BudgetLbl.Content = budget;
            }
            else
            {
                SpelerLbl.Content = som;
            }
        }

        private void StandButton_Click(object sender, RoutedEventArgs e)
        {
            HitButton.IsEnabled = false;
            StandButton.IsEnabled = false;
            int som = Convert.ToInt32(BankLbl.Content);
            while (som < 16)
            {
                GeefKaart(false);
                som = KaartenOptellen(bankKaarten);
            }
            if (ControleBust(som) == true)
            {
                BankLbl.Content = "Bust";
                UitslagLbl.Content = "Gewonnen";
                UitslagLbl.Foreground = groen;
            }
            else
            {
                switch (SpelResultaat(spelerKaarten, bankKaarten, inzet))
                {
                    case 1: 
                        UitslagLbl.Content = "Gewonnen";
                        UitslagLbl.Foreground = groen;
                        break;
                    case 2:
                        UitslagLbl.Content = "Push";
                        break;
                    case 3:
                        UitslagLbl.Content = "Verloren";
                        UitslagLbl.Foreground = rood;
                        break;
                }
                BudgetLbl.Content = budget;
                BankLbl.Content = som;
            }
        }

        private int SpelResultaat(List<string> spelerKaarten, List<string> bankKaarten, int inzet) 
        {
            int somSpeler = KaartenOptellen(spelerKaarten);
            int somBank = KaartenOptellen(bankKaarten);
            if (somSpeler > somBank)
            {
                budget += (inzet * 2);
                return 1;
            }
            else if (somSpeler == somBank)
            {
                budget+= inzet;
                return 2;
            }
            budget -= inzet;
            if (budget == 0)
            {
                NieuwBtn.IsEnabled= true;
                DeelButton.IsEnabled = false;
                HitButton.IsEnabled = false;
                StandButton.IsEnabled = false;
            }
            BudgetLbl.Content = budget.ToString();
            return 3;
        }

        private void NieuwBtn_Click(object sender, RoutedEventArgs e)
        {
            DeelButton.IsEnabled = true;
            HitButton.IsEnabled = true;
            StandButton.IsEnabled = true;
            NieuwBtn.IsEnabled = false;
            budget = 100;
            BudgetLbl.Content = budget.ToString();
        }
    }
}

