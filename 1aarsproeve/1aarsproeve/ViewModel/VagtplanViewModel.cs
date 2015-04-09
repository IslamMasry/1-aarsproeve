﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using _1aarsproeve.Annotations;
using _1aarsproeve.Common;
using _1aarsproeve.View;

namespace _1aarsproeve.ViewModel
{
    /// <summary>
    /// DataContext klasse til Views: OpretVagt, RedigerVagt, Vagtplan
    /// </summary>
    class VagtplanViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Gør det muligt at gemme værdier i local storage
        /// </summary>
        public ApplicationDataContainer Setting { get; set; }
        /// <summary>
        /// Brugernavn property
        /// </summary>
        public string Brugernavn { get; set; }
        /// <summary>
        /// Sætter mandag til bestemt farve
        /// </summary>
        public Brush MandagFarve { get; set; }
        /// <summary>
        /// Sætter tirsdag til bestemt farve
        /// </summary>
        public Brush TirsdagFarve { get; set; }
        /// <summary>
        /// Sætter onsdag til bestemt farve
        /// </summary>
        public Brush OnsdagFarve { get; set; }
        /// <summary>
        /// Sætter torsdag til bestemt farve
        /// </summary>
        public Brush TorsdagFarve { get; set; }
        /// <summary>
        /// Sætter fredag til bestemt farve
        /// </summary>
        public Brush FredagFarve { get; set; }
        /// <summary>
        /// Sætter lørdag til bestemt farve
        /// </summary>
        public Brush LoerdagFarve { get; set; }
        /// <summary>
        /// Sætter søndag til bestemt farve
        /// </summary>
        public Brush SoendagFarve { get; set; }

        /// <summary>
        /// Ugenummer property
        /// </summary>
        private int _ugenummer;

        /// <summary>
        /// Mandag property
        /// </summary>
        private string _mandag;

        /// <summary>
        /// Tirsdag property
        /// </summary>
        private string _tirsdag;

        /// <summary>
        /// Onsdag property
        /// </summary>
        private string _onsdag;

        /// <summary>
        /// Torsdag property
        /// </summary>
        private string _torsdag;

        /// <summary>
        /// Fredag property
        /// </summary>
        private string _fredag;

        /// <summary>
        /// Lørdag property
        /// </summary>
        private string _loerdag;

        /// <summary>
        /// Søndag property
        /// </summary>
        private string _soendag;
        /// <summary>
        /// Collection af ugedage
        /// </summary>
        public ObservableCollection<Ugedage> UgedageCollection { get; set; }
        /// <summary>
        /// AlleVagterCommand property
        /// </summary>
        public ICommand AlleVagterCommand { get; set; }
        /// <summary>
        /// FrieVagterCommand property
        /// </summary>
        public ICommand FrieVagterCommand { get; set; }
        /// <summary>
        /// MineVagterCommand property
        /// </summary>
        public ICommand MineVagterCommand { get; set; }
        /// <summary>
        /// LogUdCommand property
        /// </summary>
        public ICommand LogUdCommand { get; set; }
        /// <summary>
        /// Constructor for VagtplanViewModel
        /// </summary>

        public ICommand ForrigeUgeCommand { get; set; }
        public ICommand NaesteUgeCommand { get; set; }
        public VagtplanViewModel()
        {
            Setting = ApplicationData.Current.LocalSettings;
            Setting.Values["Brugernavn"] = "Daniel Winther";

            Brugernavn = (string) Setting.Values["Brugernavn"];

            NuvaerendeUgedag(new SolidColorBrush(Color.FromArgb(255, 169, 169, 169)), new SolidColorBrush(Color.FromArgb(255, 184, 19, 35)));

            FindUgenummer("da-DK");
            Ugedage();

            InitialiserUgedage();
            InitialiserAnsatte();

            AlleVagterCommand = new RelayCommand(AlleVagter);
            FrieVagterCommand = new RelayCommand(FrieVagter);
            MineVagterCommand = new RelayCommand(MineVagter);
            LogUdCommand = new RelayCommand(LogUd);

            ForrigeUgeCommand = new RelayCommand(ForrigeUge);
            NaesteUgeCommand = new RelayCommand(NaesteUge);
        }

        private void ForrigeUge()
        {

            Ugenummer = Ugenummer - 1;

            if (Ugenummer <= 0)
            {
                Ugenummer = 52;
            }
            Ugedage();

            InitialiserAnsatte();
        }

        private void NaesteUge()
        {

            Ugenummer = Ugenummer + 1;

            if (Ugenummer >= 52)
            {
                Ugenummer = 1;
            }

            Ugedage();
            InitialiserAnsatte();
        }
        /// <summary>
        /// Sætter datoerne for hver ugedag
        /// </summary>
        public void Ugedage()
        {
            Mandag = FoersteDagPaaUge(Ugenummer).ToString("dd. MMMM", new CultureInfo("da-DK"));
            Tirsdag = FoersteDagPaaUge(Ugenummer).AddDays(1).ToString("dd. MMMM", new CultureInfo("da-DK"));
            Onsdag = FoersteDagPaaUge(Ugenummer).AddDays(2).ToString("dd. MMMM", new CultureInfo("da-DK"));
            Torsdag = FoersteDagPaaUge(Ugenummer).AddDays(3).ToString("dd. MMMM", new CultureInfo("da-DK"));
            Fredag = FoersteDagPaaUge(Ugenummer).AddDays(4).ToString("dd. MMMM", new CultureInfo("da-DK"));
            Loerdag = FoersteDagPaaUge(Ugenummer).AddDays(5).ToString("dd. MMMM", new CultureInfo("da-DK"));
            Soendag = FoersteDagPaaUge(Ugenummer).AddDays(6).ToString("dd. MMMM", new CultureInfo("da-DK"));
        }

        /// <summary>
        /// Angiver farve på nuværende ugedag
        /// </summary>
        /// <param name="brush">Angiver en farve, som er i dag</param>
        /// <param name="brushOriginal">Angiver en farve, som ikke er i dag</param>
        public void NuvaerendeUgedag(SolidColorBrush brush, SolidColorBrush brushOriginal)
        {
            if (MandagFarve == null || TirsdagFarve == null || OnsdagFarve == null || TorsdagFarve == null || FredagFarve == null || LoerdagFarve == null || SoendagFarve == null)
            {
                MandagFarve = brushOriginal;
                TirsdagFarve = brushOriginal;
                OnsdagFarve = brushOriginal;
                TorsdagFarve = brushOriginal;
                FredagFarve = brushOriginal;
                LoerdagFarve = brushOriginal;
                SoendagFarve = brushOriginal;
            }
            switch (DateTime.Now.ToString("dddd"))
            {
                case "Monday":
                    MandagFarve = brush;
                    break;

                case "Tuesday":
                    TirsdagFarve = brush;
                    break;

                case "Wednesday":
                    OnsdagFarve = brush;
                    break;

                case "Thursday":
                    TorsdagFarve = brush;
                    break;

                case "Friday":
                    FredagFarve = brush;
                    break;

                case "Saturday":
                    LoerdagFarve = brush;
                    break;

                case "Sunday":
                    SoendagFarve = brush;
                    break;
            }
        }
        /// <summary>
        /// Finder ugenummer
        /// </summary>
        /// <param name="kulturInfo">Angiver hvilket land man er i</param>
        public void FindUgenummer(string kulturInfo)
        {
            var kultur = CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(kulturInfo);
            Ugenummer = kultur.Calendar.GetWeekOfYear(DateTime.Today, DateTimeFormatInfo.GetInstance(kultur).CalendarWeekRule, DateTimeFormatInfo.GetInstance(kultur).FirstDayOfWeek);
        }
        /// <summary>
        /// Finder første dag på ugen
        /// </summary>
        /// <param name="ugePaaAaret">Angiver ugenummer</param>
        /// <returns></returns>
        public DateTime FoersteDagPaaUge(int ugePaaAaret)
        {
            DateTime jan1 = new DateTime(DateTime.Today.Year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime foersteTorsdag = jan1.AddDays(daysOffset);
            int firstWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(foersteTorsdag, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var ugenummer = ugePaaAaret;
            if (firstWeek <= 1)
            {
                ugenummer -= 1;
            }
            var resultat = foersteTorsdag.AddDays(ugenummer * 7);
            return resultat.AddDays(-3);
        }

        #region InitialiserUgedage
        /// <summary>
        /// Initialisere ugedage
        /// </summary>
        public void InitialiserUgedage()
        {
            UgedageCollection = new ObservableCollection<Ugedage>()
            {
                new Ugedage {Ugedag = "Mandag", AnsatteListe = new ObservableCollection<Ansatte>()},
                new Ugedage {Ugedag = "Tirsdag", AnsatteListe = new ObservableCollection<Ansatte>()},
                new Ugedage {Ugedag = "Onsdag", AnsatteListe = new ObservableCollection<Ansatte>()},
                new Ugedage {Ugedag = "Torsdag", AnsatteListe = new ObservableCollection<Ansatte>()},
                new Ugedage {Ugedag = "Fredag", AnsatteListe = new ObservableCollection<Ansatte>()},
                new Ugedage {Ugedag = "Lørdag", AnsatteListe = new ObservableCollection<Ansatte>()},
                new Ugedage {Ugedag = "Søndag", AnsatteListe = new ObservableCollection<Ansatte>()},
            };
        }
        #endregion

        #region InitialiserAnsatte
        /// <summary>
        /// Initialisere ansatte
        /// </summary>
        public void InitialiserAnsatte()
        {
            /*for (int i = 0; i < UgedageCollection.Count; i++)
            {
                UgedageCollection[i].AnsatteListe.Clear();

                UgedageCollection[i].AnsatteListe.Add(new Ansatte
                {
                    Navn = "Daniel Winther",
                    Tidspunkt = "16:30 - 20:30",
                });
                UgedageCollection[i].AnsatteListe.Add(new Ansatte
                {
                    Navn = "Benjamin Jensen",
                    Tidspunkt = "07:00 - 16:50",
                });
                UgedageCollection[i].AnsatteListe.Add(new Ansatte
                {
                    Navn = "Jari Larsen",
                    Tidspunkt = "16:00 - 19:50",
                });
                UgedageCollection[i].AnsatteListe.Add(new Ansatte
                {
                    Navn = "Jacob Balling",
                    Tidspunkt = "06:00 - 14:20",
                });
                UgedageCollection[i].AnsatteListe.Add(new Ansatte
                {
                    Navn = "Ubemandet",
                    Tidspunkt = "08:00 - 12:50",
                });
            }*/
            UgedageCollection[0].AnsatteListe.Add(new Ansatte
            {
                Navn = "Daniel Winther",
                Tidspunkt = "16:00 - 19:50",
                Ugenummer = 15
            });
            UgedageCollection[4].AnsatteListe.Add(new Ansatte
            {
                Navn = "Ubemandet",
                Tidspunkt = "15:00 - 18:10",
                Ugenummer = 16
            });
            UgedageCollection[4].AnsatteListe.Add(new Ansatte
            {
                Navn = "Benjamin Jensen",
                Tidspunkt = "15:00 - 18:10",
                Ugenummer = 16
            });
            for (int i = 0; i < UgedageCollection.Count; i++)
            {
                var query =
                    from u in UgedageCollection[i].AnsatteListe.ToList()
                    orderby u.Tidspunkt, u.Navn 
                    where u.Ugenummer == Ugenummer
                    select u;
                UgedageCollection[i].AnsatteListe.Clear();
                foreach (var ansatte in query)
                {
                    UgedageCollection[i].AnsatteListe.Add(ansatte);
                }
            }
        }
        #endregion

        /// <summary>
        /// Viser allevagter
        /// </summary>
        public void AlleVagter()
        {
            InitialiserAnsatte();
        }
        /// <summary>
        /// Viser frie vagter
        /// </summary>
        public void FrieVagter()
        {
            InitialiserAnsatte();
            for (int i = 0; i < UgedageCollection.Count; i++)
            {
                var query =
                    from u in UgedageCollection[i].AnsatteListe.ToList()
                    where u.Navn == "Ubemandet"
                    orderby u.Tidspunkt, u.Navn ascending 
                    select u;
                UgedageCollection[i].AnsatteListe.Clear();
                foreach (var ansatte in query)
                {
                    UgedageCollection[i].AnsatteListe.Add(ansatte);
                }
            }
        }
        /// <summary>
        /// Viser mine vagter
        /// </summary>
        public void MineVagter()
        {
            InitialiserAnsatte();
            for (int i = 0; i < UgedageCollection.Count; i++)
            {
                var query =
                    from u in UgedageCollection[i].AnsatteListe.ToList()
                    where u.Navn == Brugernavn
                    orderby u.Tidspunkt, u.Navn ascending 
                    select u;
                UgedageCollection[i].AnsatteListe.Clear();

                foreach (var ansatte in query)
                {
                    UgedageCollection[i].AnsatteListe.Add(ansatte);
                }
            }
        }
        /// <summary>
        /// Logger brugeren ud
        /// </summary>
        public void LogUd()
        {
            Setting.Values.Remove("Brugernavn");

            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(Login));
        }

        public string Mandag
        {
            get { return _mandag; }
            set
            {
                _mandag = value;
                OnPropertyChanged("Mandag");
            }
        }

        public string Tirsdag
        {
            get { return _tirsdag; }
            set
            {
                _tirsdag = value;
                OnPropertyChanged("Tirsdag");
            }
        }

        public string Onsdag
        {
            get { return _onsdag; }
            set
            {
                _onsdag = value;
                OnPropertyChanged("Onsdag");
            }
        }

        public string Torsdag
        {
            get { return _torsdag; }
            set
            {
                _torsdag = value;
                OnPropertyChanged("Torsdag");
            }
        }

        public string Fredag
        {
            get { return _fredag; }
            set
            {
                _fredag = value;
                OnPropertyChanged("Fredag");
            }
        }

        public string Loerdag
        {
            get { return _loerdag; }
            set
            {
                _loerdag = value;
                OnPropertyChanged("Loerdag");
            }
        }

        public string Soendag
        {
            get { return _soendag; }
            set
            {
                _soendag = value;
                OnPropertyChanged("Soendag");
            }
        }

        public int Ugenummer
        {
            get { return _ugenummer; }
            set
            {
                _ugenummer = value;
                OnPropertyChanged("Ugenummer");
            }
        }
        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
    #region Forsøgsklasser
    internal class Ugedage
    {
        public string Ugedag { get; set; }
        public ObservableCollection<Ansatte> AnsatteListe { get; set; }
    }

    internal class Ansatte
    {
        public string Navn { get; set; }
        public string Tidspunkt { get; set; }
        public int Ugenummer { get; set; }
    }
    #endregion
}