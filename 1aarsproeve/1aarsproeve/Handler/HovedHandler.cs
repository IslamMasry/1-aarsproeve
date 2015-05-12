﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using _1aarsproeve.Model;
using _1aarsproeve.Persistens;
using _1aarsproeve.View;
using _1aarsproeve.ViewModel;

namespace _1aarsproeve.Handler
{
    /// <summary>
    /// Handler-klasser der håndterer operationer for HovedmenuView
    /// </summary>
    public class HovedHandler
    {
        /// <summary>
        /// HovedViewModel property
        /// </summary>
        public HovedViewModel HovedViewModel { get; set; }
        /// <summary>
        /// HovedViewModel konstruktør
        /// </summary>
        /// <param name="hovedViewModel">HovedViewModel objekt parameter</param>
        public HovedHandler(HovedViewModel hovedViewModel)
        {
            HovedViewModel = hovedViewModel;
        }
        /// <summary>
        /// Skriver ny besked
        /// </summary>
        public void SkrivBesked()
        {
            PersistensFacade<HovedmenuView>.GemDB("api/Beskeders", new HovedmenuView() {Overskrift = HovedViewModel.HovedmenuView.Overskrift, Dato = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day), Beskrivelse = HovedViewModel.HovedmenuView.Beskrivelse, Udloebsdato = new DateTime(DateTime.Today.Year, DateTime.Today.Month + 1, DateTime.Today.Day), Brugernavn = HovedViewModel.Brugernavn});

            MessageDialog m = new MessageDialog("Beskeden blev oprettet", "Succes!");
            m.ShowAsync();
        }
    }
}