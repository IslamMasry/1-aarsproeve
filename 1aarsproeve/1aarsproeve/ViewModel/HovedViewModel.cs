﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Eventmaker.Common;
using _1aarsproeve.Common;
using _1aarsproeve.Handler;
using _1aarsproeve.Model;
using _1aarsproeve.Persistens;
using _1aarsproeve.View;

namespace _1aarsproeve.ViewModel
{
    /// <summary>
    /// DataContext klasse til Views: Hovedmenu, SkrivBesked
    /// </summary>
    public class HovedViewModel
    {
        private ICommand _skrivBeskedCommand;
        private ICommand _logUdCommand;
        private ICommand _accepterAnmodningCommand;
        private ICommand _annullerAnmodningCommand;
        private ICommand _selectedAnmodningerCommand;
        private GeneriskSingleton<BeskedModel> _beskedCollection;

        #region Get Set properties
        /// <summary>
        /// SelectedVagter static property
        /// </summary>
        public static AnmodningModel SelectedAnmodning { get; set; }
        public ObservableCollection<AnmodningModel> AnmodningCollection { get; set; }
        /// <summary>
        /// Singleton vagtcollection
        /// </summary>
        public ObservableCollection<BeskedModel> BeskedCollection
        {
            get { return _beskedCollection.Collection; }
            set { _beskedCollection.Collection = value; }
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
        /// Hovedhandler property
        /// </summary>
        public HovedHandler HovedHandler { get; set; }
        /// <summary>
        /// SkjulKnap property
        /// </summary>
        public Visibility SkjulKnap { get; set; }
        /// <summary>
        /// IngenAnmodninger property
        /// </summary>
        public string IngenAnmodninger { get; set; }
        #endregion

        /// <summary>
        /// Constructor for HovedViewModel
        /// </summary>
        public HovedViewModel()
        {
            _beskedCollection = GeneriskSingleton<BeskedModel>.Instance();
            Setting = ApplicationData.Current.LocalSettings;

            Brugernavn = (string)Setting.Values["Brugernavn"];
            SkjulKnap = Hjaelpeklasse.Stilling((int)Setting.Values["StillingId"]);

            AnmodningCollection = new ObservableCollection<AnmodningModel>();
            InitialiserAnmodninger();

            HovedHandler = new HovedHandler(this);
        }
        /// <summary>
        /// Initialiserer anmodningerAnmodningerModel
        /// </summary>
        public void InitialiserAnmodninger()
        {
            AnmodningCollection.Clear();
            var queryAnmodning = PersistensFacade<AnmodningModel>.LoadDB("api/AnmodningModels").Result;
            var queryAnmodning1 =
                from a in queryAnmodning
                where a.Brugernavn == Brugernavn
                select a;
            if (queryAnmodning1.Any() == false)
            {
                IngenAnmodninger = "Der blev ikke fundet nogle anmodninger";
            }
            foreach (var item in queryAnmodning1)
            {
                AnmodningCollection.Add(item);
            }
        }
        #region Commands
        /// <summary>
        /// SelectedAnmodningerCommand property
        /// </summary>
        public ICommand SelectedAnmodningerCommand
        {
            get
            {
                _selectedAnmodningerCommand = new RelayArgCommand<AnmodningModel>(a => HovedHandler.SetSelectedAnmodning(a));
                return _selectedAnmodningerCommand;
            }
            set { _selectedAnmodningerCommand = value; }
        }
        public ICommand AccepterAnmodningCommand
        {
            get
            {
                if (_accepterAnmodningCommand == null)
                    _accepterAnmodningCommand = new RelayCommand(HovedHandler.AccepterAnmodning);
                return _accepterAnmodningCommand;
            }
            set { _accepterAnmodningCommand = value; }
        }
        public ICommand AnnullerAnmodningCommand
        {
            get
            {
                if (_annullerAnmodningCommand == null)
                    _annullerAnmodningCommand = new RelayCommand(HovedHandler.AnnullerAnmodning);
                return _annullerAnmodningCommand;
            }
            set { _annullerAnmodningCommand = value; }
        }
        /// <summary>
        /// SkrivBesked command
        /// </summary>
        public ICommand SkrivBeskedCommand
        {
            get
            {
                if (_skrivBeskedCommand == null)
                    _skrivBeskedCommand = new RelayCommand(HovedHandler.SkrivBesked);
                return _skrivBeskedCommand;
            }
            set { _skrivBeskedCommand = value; }
        }

        /// <summary>
        /// Logger brugeren ud
        /// </summary>
        public ICommand LogUdCommand
        {
            get
            {
                if (_logUdCommand == null)
                    _logUdCommand = new RelayCommand(Hjaelpeklasse.LogUd);
                return _logUdCommand;
            }
            set { _logUdCommand = value; }
        }
       
        #endregion
    }
}