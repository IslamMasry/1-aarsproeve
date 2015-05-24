﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Eventmaker.Common;
using _1aarsproeve.Annotations;
using _1aarsproeve.Common;
using _1aarsproeve.Handler;
using _1aarsproeve.Model;
using _1aarsproeve.View;
using _1aarsproeve.Persistens;

namespace _1aarsproeve.ViewModel
{
    /// <summary>
    /// DataContext klasse til Views: OpretVagt, RedigerVagt, Vagtplan
    /// </summary>
    public class VagtplanViewModel : INotifyPropertyChanged
    {
        #region Backing fields

        private ICommand _opretVagtCommand;
        private ICommand _redigerVagtCommand;
        private ICommand _sletVagtCommand;
        private ICommand _forrigeUgeCommand;
        private ICommand _naesteUgeCommand;
        private ICommand _alleVagterCommand;
        private ICommand _frieVagterCommand;
        private ICommand _mineVagterCommand;
        private ICommand _sortCommand;
        private ICommand _eksporterAlleCommand;
        private ICommand _eksporterMineCommand;
        private ICommand _logudCommand;
        private ICommand _selectedVagterCommand;
        private ICommand _anmodVagtCommand;
        private ICommand _navigerRedigerVagtCommand;
        private Action<ObservableCollection<VagtplanView>[]> _sorting;
        private int _aar;

        private string _mandag;
        private string _tirsdag;
        private string _onsdag;
        private string _torsdag;
        private string _fredag;
        private string _loerdag;
        private string _soendag;
        private VagtplanSingleton _vagtCollection;

        #endregion

        #region Ugedage farver og collections

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
        /// Alle mandagsvagter
        /// </summary>
        public ObservableCollection<VagtplanView> MandagVagter;

        /// <summary>
        /// Alle tirsdagsvagter
        /// </summary>
        public ObservableCollection<VagtplanView> TirsdagVagter;

        /// <summary>
        /// Alle onsdagsvagter
        /// </summary>
        public ObservableCollection<VagtplanView> OnsdagVagter;

        /// <summary>
        /// Alle torsdagssvagter
        /// </summary>
        public ObservableCollection<VagtplanView> TorsdagVagter;

        /// <summary>
        /// Alle fredagsvagter
        /// </summary>
        public ObservableCollection<VagtplanView> FredagVagter;

        /// <summary>
        /// Alle lørdagsvagter
        /// </summary>
        public ObservableCollection<VagtplanView> LoerdagVagter;

        /// <summary>
        /// Alle søndagsvagter
        /// </summary>
        public ObservableCollection<VagtplanView> SoendagVagter;

        #endregion

        #region Commands

        /// <summary>
        /// OpretVagtCommand property
        /// </summary>
        public ICommand OpretVagtCommand
        {
            get
            {
                _opretVagtCommand = new RelayCommand(VagtHandler.OpretVagt);
                return _opretVagtCommand;
            }
            set { _opretVagtCommand = value; }
        }
        /// <summary>
        /// AnmodVagtCommand property
        /// </summary>
        public ICommand AnmodVagtCommand
        {
            get
            {
                _anmodVagtCommand = new RelayCommand(VagtHandler.AnmodVagt);
                return _anmodVagtCommand;
            }
            set { _anmodVagtCommand = value; }
        }
        /// <summary>
        /// NavigerRedigerVagtCommand property
        /// </summary>
        public ICommand NavigerRedigerVagtCommand
        {
            get
            {
                _navigerRedigerVagtCommand = new RelayCommand(VagtHandler.NavigerRedigerVagt);
                return _navigerRedigerVagtCommand;
            }
            set { _navigerRedigerVagtCommand = value; }
        }
        /// <summary>
        /// RedigerVagtCommand property
        /// </summary>
        public ICommand RedigerVagtCommand
        {
            get
            {
                _redigerVagtCommand = new RelayCommand(VagtHandler.RedigerVagt);
                return _redigerVagtCommand;
            }
            set { _redigerVagtCommand = value; }
        }
        /// <summary>
        /// SletVagtCommand property
        /// </summary>
        public ICommand SletVagtCommand
        {
            get
            {
                _sletVagtCommand = new RelayCommand(VagtHandler.SletVagt);
                return _sletVagtCommand;
            }
            set { _sletVagtCommand = value; }
        }

        /// <summary>
        /// ForrigeUgeCommand property
        /// </summary>
        public ICommand ForrigeUgeCommand
        {
            get
            {
                _forrigeUgeCommand = new RelayCommand(ForrigeUge);
                return _forrigeUgeCommand;
            }
            set { _forrigeUgeCommand = value; }
        }

        /// <summary>
        /// NaesteUgeCommand property
        /// </summary>
        public ICommand NaesteUgeCommand
        {
            get
            {
                _naesteUgeCommand = new RelayCommand(NaesteUge);
                return _naesteUgeCommand;
            }
            set { _naesteUgeCommand = value; }
        }

        /// <summary>
        /// AlleVagterCommand property
        /// </summary>
        public ICommand AlleVagterCommand
        {
            get
            {
                _alleVagterCommand = new RelayCommand(() => _sorting = AlleVagter);
                return _alleVagterCommand;
            }
            set { _alleVagterCommand = value; }
        }

        /// <summary>
        /// FrieVagterCommand property
        /// </summary>
        public ICommand FrieVagterCommand
        {
            get
            {
                _frieVagterCommand = new RelayCommand(() => _sorting = FrieVagter);
                return _frieVagterCommand;
            }
            set { _frieVagterCommand = value; }
        }

        /// <summary>
        /// MineVagterCommand property
        /// </summary>
        public ICommand MineVagterCommand
        {
            get
            {
                _mineVagterCommand = new RelayCommand(() => _sorting = MineVagter);
                return _mineVagterCommand;
            }
            set { _mineVagterCommand = value; }
        }

        /// <summary>
        /// SortCommand property
        /// </summary>
        public ICommand SortCommand
        {
            get
            {
                _sortCommand = new RelayCommand(() => _sorting(VagtCollection.VagtCollectionsArray));
                return _sortCommand;
            }
            set { _sortCommand = value; }
        }

        /// <summary>
        /// EksporterAlleCommand property
        /// </summary>
        public ICommand EksporterAlleCommand
        {
            get
            {
                _eksporterAlleCommand = new RelayCommand(EksporterAlleVagter);
                return _eksporterAlleCommand;
            }
            set { _eksporterAlleCommand = value; }
        }

        /// <summary>
        /// EksporterMineCommand property
        /// </summary>
        public ICommand EksporterMineCommand
        {
            get
            {
                _eksporterMineCommand = new RelayCommand(EksporterMineVagter);
                return _eksporterMineCommand;
            }
            set { _eksporterMineCommand = value; }
        }

        /// <summary>
        /// LogUdCommand property
        /// </summary>
        public ICommand LogUdCommand
        {
            get
            {
                _logudCommand = new RelayCommand(Hjaelpeklasse.LogUd);
                return _logudCommand;
            }
            set { _logudCommand = value; }
        }

        /// <summary>
        /// SelectedVagterCommand property
        /// </summary>
        public ICommand SelectedVagterCommand
        {
            get
            {
                _selectedVagterCommand = new RelayArgCommand<VagtplanView>(v => VagtHandler.SetSelectedVagt(v));
                return _selectedVagterCommand;
            }
            set { _selectedVagterCommand = value; }
        }

        #endregion

        #region Get Set properties
        /// <summary>
        /// År property
        /// </summary>
        public int Aar
        {
            get { return _aar; }
            set
            {
                _aar = value;
                OnPropertyChanged("Aar");
            }
        }
        /// <summary>
        /// Singleton vagtcollection
        /// </summary>
        public VagtplanSingleton VagtCollection
        {
            get { return _vagtCollection; }
            set { _vagtCollection = value; }
        }

        /// <summary>
        /// Mandag property
        /// </summary>
        public string Mandag
        {
            get { return _mandag; }
            set
            {
                _mandag = value;
                OnPropertyChanged("Mandag");
            }
        }

        /// <summary>
        /// Tirsdag property
        /// </summary>
        public string Tirsdag
        {
            get { return _tirsdag; }
            set
            {
                _tirsdag = value;
                OnPropertyChanged("Tirsdag");
            }
        }

        /// <summary>
        /// Onsdag property
        /// </summary>
        public string Onsdag
        {
            get { return _onsdag; }
            set
            {
                _onsdag = value;
                OnPropertyChanged("Onsdag");
            }
        }

        /// <summary>
        /// Torsdag property
        /// </summary>
        public string Torsdag
        {
            get { return _torsdag; }
            set
            {
                _torsdag = value;
                OnPropertyChanged("Torsdag");
            }
        }

        /// <summary>
        /// Fredag property
        /// </summary>
        public string Fredag
        {
            get { return _fredag; }
            set
            {
                _fredag = value;
                OnPropertyChanged("Fredag");
            }
        }

        /// <summary>
        /// Lørdag property
        /// </summary>
        public string Loerdag
        {
            get { return _loerdag; }
            set
            {
                _loerdag = value;
                OnPropertyChanged("Loerdag");
            }
        }

        /// <summary>
        /// Søndag property
        /// </summary>
        public string Soendag
        {
            get { return _soendag; }
            set
            {
                _soendag = value;
                OnPropertyChanged("Soendag");
            }
        }

        /// <summary>
        /// Gør det muligt at gemme værdier i local storage
        /// </summary>
        public ApplicationDataContainer Setting { get; set; }
        /// <summary>
        /// Brugernavn property
        /// </summary>
        public string Brugernavn { get; set; }
        /// <summary>
        /// AnsatteListe property
        /// </summary>
        public List<Ansatte> AnsatteListe { get; set; }

        /// <summary>
        /// UgenumreListe property
        /// </summary>
        public List<int> UgenumreListe { get; set; }

        /// <summary>
        /// UgedageListe property
        /// </summary>
        public List<Ugedage> UgedageListe { get; set; }

        /// <summary>
        /// VagtHandler property
        /// </summary>
        public VagtHandler VagtHandler { get; set; }

        /// <summary>
        /// SelectedVagter static property
        /// </summary>
        public static VagtplanView SelectedVagter { get; set; }
        /// <summary>
        /// SkjulKnap property
        /// </summary>
        public Visibility SkjulKnap { get; set; }
        #endregion

        /// <summary>
        /// Constructor for VagtplanViewModel
        /// </summary>
        public VagtplanViewModel()
        {
            _vagtCollection = VagtplanSingleton.Instance();
            Setting = ApplicationData.Current.LocalSettings;
            Brugernavn = (string)Setting.Values["Brugernavn"];
            SkjulKnap = Hjaelpeklasse.Stilling((int)Setting.Values["StillingId"]);

            Aar = DateTime.Today.Year;

            NuvaerendeUgedag(new SolidColorBrush(Color.FromArgb(255, 169, 169, 169)), new SolidColorBrush(Color.FromArgb(255, 184, 19, 35)));
            Ugedage();

            VagtHandler = new VagtHandler(this);

            AnsatteListe = new List<Ansatte>();
            UgedageListe = new List<Ugedage>();
            UgenumreListe = new List<int>();
            var a = PersistensFacade<Ansatte>.LoadDB("api/Ansattes").Result;
            foreach (var item in a)
            {
                AnsatteListe.Add(item);
            }
            var u = PersistensFacade<Ugedage>.LoadDB("api/Ugedages").Result;
            foreach (var item in u)
            {
                UgedageListe.Add(item);
            }
            for (int i = 1; i <= 52; i++)
            {
                UgenumreListe.Add(i);
            }
            _sorting = AlleVagter;
        }

        #region Sort vagter

        /// <summary>
        /// Henter alle vagter
        /// </summary>
        /// <param name="vagtCollection">Tager vagtcollection som parameter</param>
        public async void AlleVagter(ObservableCollection<VagtplanView>[] vagtCollection)
        {
            try
            {
                ClearVagterCollections();
                for (int i = 0; i < vagtCollection.Length; i++)
                {
                    var query =
                        from q in VagtCollection.VagterListe
                        where q.UgedagId == i + 1 && q.Ugenummer == _vagtCollection.Ugenummer
                        orderby q.Starttidspunkt ascending, q.Sluttidspunkt
                        select q;
                    foreach (var item in query)
                    {
                        vagtCollection[i].Add(item);
                    }
                }
            }
            catch (Exception)
            {
                MessageDialog m = Hjaelpeklasse.FejlMeddelelse("Der kunne ikke udtrækkes fra databasen");
                m.ShowAsync();
            }
        }

        /// <summary>
        /// Henter frie vagter
        /// </summary>
        /// <param name="vagtCollection">Tager vagtcollection som parameter</param>
        public async void FrieVagter(ObservableCollection<VagtplanView>[] vagtCollection)
        {
            try
            {
                ClearVagterCollections();
                for (int i = 0; i < vagtCollection.Length; i++)
                {
                    var query =
                        from q in VagtCollection.VagterListe
                        where q.UgedagId == i + 1 && q.Ugenummer == _vagtCollection.Ugenummer && q.Brugernavn == "Ubemandet"
                        orderby q.Starttidspunkt ascending, q.Sluttidspunkt
                        select q;
                    foreach (var item in query)
                    {
                        vagtCollection[i].Add(item);
                    }
                }
            }
            catch (Exception)
            {
                MessageDialog m = Hjaelpeklasse.FejlMeddelelse("Der kunne ikke udtrækkes fra databasen");
                m.ShowAsync();
            }
        }

        /// <summary>
        /// Henter mine vagter
        /// </summary>
        /// <param name="vagtCollection">Tager vagtcollection som parameter</param>
        public async void MineVagter(ObservableCollection<VagtplanView>[] vagtCollection)
        {
            try
            {
                ClearVagterCollections();
                for (int i = 0; i < vagtCollection.Length; i++)
                {
                    var query =
                        from q in VagtCollection.VagterListe
                        where q.UgedagId == i + 1 && q.Ugenummer == _vagtCollection.Ugenummer && q.Brugernavn == Brugernavn
                        orderby q.Starttidspunkt ascending, q.Sluttidspunkt
                        select q;
                    foreach (var item in query)
                    {
                        vagtCollection[i].Add(item);
                    }
                }
            }
            catch (Exception)
            {
                MessageDialog m = Hjaelpeklasse.FejlMeddelelse("Der kunne ikke udtrækkes fra databasen");
                m.ShowAsync();
            }
        }

        #endregion

        #region Eksporter vagter

        /// <summary>
        /// Eksporter alle vagter
        /// </summary>
        public async void EksporterAlleVagter()
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.Desktop;
            savePicker.FileTypeChoices.Add("Filformat", new List<string>() { ".ics", ".csv" });
            savePicker.SuggestedFileName = "vagtplan-alle-uge-" + _vagtCollection.Ugenummer;

            StorageFile fil = await savePicker.PickSaveFileAsync();
            if (fil != null)
            {
                CachedFileManager.DeferUpdates(fil);
                string vagter = null;
                const string emne = "Fakta - vagt";
                const string sted = "Jyderup";
                if (fil.FileType.Equals(".ics"))
                {
                    vagter +=
                        "BEGIN:VCALENDAR\n" +
                        "VERSION:2.0\n\n";
                    for (int i = 0; i < VagtCollection.VagtCollectionsArray.Length; i++)
                    {
                        var query1 =
                            from q in VagtCollection.VagtCollectionsArray[i]
                            where q.UgedagId == i + 1 && q.Ugenummer == _vagtCollection.Ugenummer
                            select new { q.Starttidspunkt, q.Sluttidspunkt, q.Navn };
                        foreach (var item in query1)
                        {
                            vagter +=
                                "BEGIN:VEVENT\n" +
                                "DTSTART:" + FoersteDagPaaUge(Aar, _vagtCollection.Ugenummer).AddDays(i).ToString("yyyyMMdd") + "T" +
                                item.Starttidspunkt.ToString("hhmmss") + "\n" +
                                "DTEND:" + FoersteDagPaaUge(Aar, _vagtCollection.Ugenummer).AddDays(i).ToString("yyyyMMdd") + "T" +
                                item.Sluttidspunkt.ToString("hhmmss") + "\n" +
                                "SUMMARY:" + emne + " | " + item.Navn + "\n" +
                                "LOCATION:" + sted + "\n" +
                                "END:VEVENT\n\n";
                        }
                    }
                    vagter += "END:VCALENDAR";
                }
                else
                {
                    vagter +=
                        "Emne, Startdato, Starttidspunkt, Slutdato, Sluttidspunkt, Placering\n";
                    for (int i = 0; i < VagtCollection.VagtCollectionsArray.Length; i++)
                    {
                        var query1 =
                            from q in VagtCollection.VagtCollectionsArray[i]
                            where q.UgedagId == i + 1 && q.Ugenummer == _vagtCollection.Ugenummer && q.Brugernavn == Brugernavn
                            select new { q.Starttidspunkt, q.Sluttidspunkt, q.Navn };
                        foreach (var item in query1)
                        {
                            vagter +=
                                emne + " | " + item.Navn +
                                ", " + FoersteDagPaaUge(Aar, _vagtCollection.Ugenummer).AddDays(i).ToString("d") +
                                ", " + item.Starttidspunkt +
                                ", " + FoersteDagPaaUge(Aar, _vagtCollection.Ugenummer).AddDays(i).ToString("d") +
                                ", " + item.Sluttidspunkt +
                                ", " + sted + "\n";
                        }
                    }
                }
                await FileIO.WriteTextAsync(fil, vagter);
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(fil);
                if (status == FileUpdateStatus.Complete)
                {
                    MessageDialog m = Hjaelpeklasse.SuccesMeddelelse("Vagtplanen blev eksporteret som " + fil.FileType + "-fil");
                    m.ShowAsync();
                }
                else
                {
                    MessageDialog m = Hjaelpeklasse.FejlMeddelelse("Der skete en fejl under eksporteringen - prøv igen");
                    m.ShowAsync();
                }
            }
        }

        /// <summary>
        /// Eksporterer alle mine vagter
        /// </summary>
        public async void EksporterMineVagter()
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.Desktop;
            savePicker.FileTypeChoices.Add("Filformat", new List<string>() { ".ics", ".csv" });
            savePicker.SuggestedFileName = "vagtplan-mine-uge-" + _vagtCollection.Ugenummer;

            StorageFile fil = await savePicker.PickSaveFileAsync();
            if (fil != null)
            {
                CachedFileManager.DeferUpdates(fil);
                string vagter = null;
                const string emne = "Fakta - vagt";
                const string sted = "Jyderup";
                if (fil.FileType.Equals(".ics"))
                {
                    vagter +=
                        "BEGIN:VCALENDAR\n" +
                        "VERSION:2.0\n\n";
                    for (int i = 0; i < VagtCollection.VagtCollectionsArray.Length; i++)
                    {
                        var query1 =
                            from q in VagtCollection.VagtCollectionsArray[i]
                            where q.UgedagId == i + 1 && q.Ugenummer == _vagtCollection.Ugenummer && q.Brugernavn == Brugernavn
                            select new { q.Starttidspunkt, q.Sluttidspunkt };
                        foreach (var item in query1)
                        {
                            vagter +=
                                "BEGIN:VEVENT\n" +
                                "DTSTART:" + FoersteDagPaaUge(Aar, _vagtCollection.Ugenummer).AddDays(i).ToString("yyyyMMdd") + "T" +
                                item.Starttidspunkt.ToString("hhmmss") + "\n" +
                                "DTEND:" + FoersteDagPaaUge(Aar, _vagtCollection.Ugenummer).AddDays(i).ToString("yyyyMMdd") + "T" +
                                item.Sluttidspunkt.ToString("hhmmss") + "\n" +
                                "SUMMARY:" + emne + "\n" +
                                "LOCATION:" + sted + "\n" +
                                "END:VEVENT\n\n";
                        }
                    }
                    vagter += "END:VCALENDAR";
                }
                else
                {
                    vagter +=
                        "Subject, Start Date, Start Time, End Date, End Time, Location\n";
                    for (int i = 0; i < VagtCollection.VagtCollectionsArray.Length; i++)
                    {
                        var query1 =
                            from q in VagtCollection.VagtCollectionsArray[i]
                            where q.UgedagId == i + 1 && q.Ugenummer == _vagtCollection.Ugenummer && q.Brugernavn == Brugernavn
                            select new { q.Starttidspunkt, q.Sluttidspunkt };
                        foreach (var item in query1)
                        {
                            vagter +=
                                emne +
                                ", " + FoersteDagPaaUge(Aar, _vagtCollection.Ugenummer).AddDays(i).ToString("d") +
                                ", " + item.Starttidspunkt +
                                ", " + FoersteDagPaaUge(Aar, _vagtCollection.Ugenummer).AddDays(i).ToString("d") +
                                ", " + item.Sluttidspunkt +
                                ", " + sted + "\n";
                        }
                    }
                }
                await FileIO.WriteTextAsync(fil, vagter);
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(fil);
                if (status == FileUpdateStatus.Complete)
                {
                    MessageDialog m = Hjaelpeklasse.SuccesMeddelelse("Vagtplanen blev eksporteret som " + fil.FileType + "-fil");
                    m.ShowAsync();
                }
                else
                {
                    MessageDialog m = Hjaelpeklasse.FejlMeddelelse("Der skete en fejl under eksporteringen - prøv igen");
                    m.ShowAsync();
                }
            }
        }
        #endregion

        /// <summary>
        /// Henter vagter for forrige uge
        /// </summary>
        public void ForrigeUge()
        {
            _vagtCollection.Ugenummer = _vagtCollection.Ugenummer - 1;
            Ugedage();
            if (Aar == 2016 || Aar == 2021 || Aar == 2027 || Aar == 2033 || Aar == 2038 || Aar == 2044 || Aar == 2049 || Aar == 2055 || Aar == 2061)
            {
                if (_vagtCollection.Ugenummer < 1)
                {
                    _vagtCollection.Ugenummer = 53;
                    Aar--;
                    Ugedage();
                }
            }
            else
            {
                if (_vagtCollection.Ugenummer < 1)
                {
                    _vagtCollection.Ugenummer = 52;
                    Aar--;
                    Ugedage();
                }
            }
            ClearVagterCollections();
            VagtCollection.LoadVagter();
        }
        /// <summary>
        /// Henter vagter for næste uge
        /// </summary
        public void NaesteUge()
        {
            _vagtCollection.Ugenummer = _vagtCollection.Ugenummer + 1;
            Ugedage();

            if (Aar == 2015 || Aar == 2020 || Aar == 2026 || Aar == 2032 || Aar == 2037 || Aar == 2043 || Aar == 2048 || Aar == 2054 || Aar == 2060)
            {
                if (_vagtCollection.Ugenummer > 53)
                {
                    _vagtCollection.Ugenummer = 1;
                    Aar++;
                    Ugedage();
                }
            }
            else
            {
                if (_vagtCollection.Ugenummer > 52)
                {
                    _vagtCollection.Ugenummer = 1;
                    Aar++;
                    Ugedage();
                }
            }
            ClearVagterCollections();
            VagtCollection.LoadVagter();
        }
        public void ClearVagterCollections()
        {
            for (int i = 0; i < _vagtCollection.VagtCollectionsArray.Length; i++)
            {
                _vagtCollection.VagtCollectionsArray[i].Clear();
            }
        }
        /// <summary>
        /// Sætter datoerne for hver ugedag
        /// </summary>
        public void Ugedage()
        {
            Mandag = FoersteDagPaaUge(Aar, _vagtCollection.Ugenummer).ToString("d. MMMM", new CultureInfo("da-DK"));
            Tirsdag = FoersteDagPaaUge(Aar, _vagtCollection.Ugenummer).AddDays(1).ToString("d. MMMM", new CultureInfo("da-DK"));
            Onsdag = FoersteDagPaaUge(Aar, _vagtCollection.Ugenummer).AddDays(2).ToString("d. MMMM", new CultureInfo("da-DK"));
            Torsdag = FoersteDagPaaUge(Aar, _vagtCollection.Ugenummer).AddDays(3).ToString("d. MMMM", new CultureInfo("da-DK"));
            Fredag = FoersteDagPaaUge(Aar, _vagtCollection.Ugenummer).AddDays(4).ToString("d. MMMM", new CultureInfo("da-DK"));
            Loerdag = FoersteDagPaaUge(Aar, _vagtCollection.Ugenummer).AddDays(5).ToString("d. MMMM", new CultureInfo("da-DK"));
            Soendag = FoersteDagPaaUge(Aar, _vagtCollection.Ugenummer).AddDays(6).ToString("d. MMMM", new CultureInfo("da-DK"));
        }
        /// <summary>
        /// Angiver farve på nuværende ugedag
        /// </summary>
        /// <param name="brush">Angiver farven som bliver vist på i dags ugedag</param>
        /// <param name="brushOriginal">Angiver farven som bliver vist på de ugedage som ikke er i dag</param>
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
        /// Finder første dag på ugen
        /// </summary>
        /// <param name="ugePaaAaret">Angiver ugenummer</param>
        /// <param name="aar">Angiver årstal</param>
        /// <returns></returns>
        public DateTime FoersteDagPaaUge(int aar, int ugePaaAaret)
        {
            DateTime jan1 = new DateTime(aar, 1, 1);
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
        #region PropertyChanged
        /// <summary>
        /// Implementerer INotifyPropertyChanged interfacet
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}