using System;
using System.ComponentModel;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;
using Newtonsoft.Json;

#region UPDATE : USING

using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;

#endregion

#region LICENZA : USING

using NM_CTG_Licenza;
using Button = cAlgo.API.Button;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

#endregion

namespace cAlgo
{
    /// <summary>
    /// Estensioni che rendono il codice più scorrevole con metodi non previsti dalla libreria cAlgo
    /// </summary>
    public static class Extensions
    {

        #region Enum

        /// <summary>
        /// Enumeratore per esporre il nome del colore nelle opzioni
        /// </summary>
        public enum ColorNameEnum
        {

            AliceBlue,
            AntiqueWhite,
            Aqua,
            Aquamarine,
            Azure,
            Beige,
            Bisque,
            Black,
            BlanchedAlmond,
            Blue,
            BlueViolet,
            Brown,
            BurlyWood,
            CadetBlue,
            Chartreuse,
            Chocolate,
            Coral,
            CornflowerBlue,
            Cornsilk,
            Crimson,
            Cyan,
            DarkBlue,
            DarkCyan,
            DarkGoldenrod,
            DarkGray,
            DarkGreen,
            DarkKhaki,
            DarkMagenta,
            DarkOliveGreen,
            DarkOrange,
            DarkOrchid,
            DarkRed,
            DarkSalmon,
            DarkSeaGreen,
            DarkSlateBlue,
            DarkSlateGray,
            DarkTurquoise,
            DarkViolet,
            DeepPink,
            DeepSkyBlue,
            DimGray,
            DodgerBlue,
            Firebrick,
            FloralWhite,
            ForestGreen,
            Fuchsia,
            Gainsboro,
            GhostWhite,
            Gold,
            Goldenrod,
            Gray,
            Green,
            GreenYellow,
            Honeydew,
            HotPink,
            IndianRed,
            Indigo,
            Ivory,
            Khaki,
            Lavender,
            LavenderBlush,
            LawnGreen,
            LemonChiffon,
            LightBlue,
            LightCoral,
            LightCyan,
            LightGoldenrodYellow,
            LightGray,
            LightGreen,
            LightPink,
            LightSalmon,
            LightSeaGreen,
            LightSkyBlue,
            LightSlateGray,
            LightSteelBlue,
            LightYellow,
            Lime,
            LimeGreen,
            Linen,
            Magenta,
            Maroon,
            MediumAquamarine,
            MediumBlue,
            MediumOrchid,
            MediumPurple,
            MediumSeaGreen,
            MediumSlateBlue,
            MediumSpringGreen,
            MediumTurquoise,
            MediumVioletRed,
            MidnightBlue,
            MintCream,
            MistyRose,
            Moccasin,
            NavajoWhite,
            Navy,
            OldLace,
            Olive,
            OliveDrab,
            Orange,
            OrangeRed,
            Orchid,
            PaleGoldenrod,
            PaleGreen,
            PaleTurquoise,
            PaleVioletRed,
            PapayaWhip,
            PeachPuff,
            Peru,
            Pink,
            Plum,
            PowderBlue,
            Purple,
            Red,
            RosyBrown,
            RoyalBlue,
            SaddleBrown,
            Salmon,
            SandyBrown,
            SeaGreen,
            SeaShell,
            Sienna,
            Silver,
            SkyBlue,
            SlateBlue,
            SlateGray,
            Snow,
            SpringGreen,
            SteelBlue,
            Tan,
            Teal,
            Thistle,
            Tomato,
            Transparent,
            Turquoise,
            Violet,
            Wheat,
            White,
            WhiteSmoke,
            Yellow,
            YellowGreen

        }

        /// <summary>
        /// Enumeratore per esporre nei parametri una scelta con menu a tendina
        /// </summary>
        public enum CapitalTo
        {

            Balance,
            Equity

        }

        public enum OpenTradeType
        {

            All,
            Buy,
            Sell,
            Auto

        }

        public enum StrategyType
        {

            Moderate,
            Aggressive,
            Auto

        }

        #endregion

        #region Class

        /// <summary>
        /// Classe per monitorare le posizioni di una specifica strategia
        /// </summary>
        public class Monitor
        {

            private Positions _allPositions = null;

            /// <summary>
            /// Standard per la raccolta di informazioni nel Monitor
            /// </summary>
            public class Information
            {

                public double TotalNetProfit = 0;
                public double MinVolumeInUnits = 0;
                public double MaxVolumeInUnits = 0;
                public double MidVolumeInUnits = 0;
                public int BuyPositions = 0;
                public int SellPositions = 0;
                public Position FirstPosition = null;
                public Position LastPosition = null;
                public double HighestHighAfterFirstOpen = 0;
                public double LowestLowAfterFirstOpen = 0;

            }

            /// <summary>
            /// Standard per l'interpretazione dell'orario in double
            /// </summary>
            public class PauseTimes
            {

                public double Over = 0;
                public double Under = 0;

            }

            /// <summary>
            /// Standard per la gestione del break even
            /// </summary>
            public class BreakEvenData
            {

                // --> In caso di operazioni multiple sarebbe bene evitare la gestione di tutte
                public bool OnlyFirst = false;
                public bool Negative = false;
                public double Activation = 0;
                public int LimitBar = 0;
                public double Distance = 0;

            }

            /// <summary>
            /// Standard per la gestione del trailing
            /// </summary>
            public class TrailingData
            {

                // --> In caso di operazioni multiple sarebbe bene evitare la gestione di tutte
                public bool OnlyFirst = false;
                public bool ProActive = false;
                public double Activation = 0;
                public double Distance = 0;

            }

            /// <summary>
            /// Memorizza lo stato di apertura di una operazione nella Bar corrente
            /// </summary>
            public bool OpenedInThisBar = false;

            /// <summary>
            /// Memorizza lo stato di apertura di una operazione con il trigger corrente
            /// </summary>
            public bool OpenedInThisTrigger = false;

            /// <summary>
            /// Valore univoco che identifica la strategia
            /// </summary>
            public readonly string Label;

            /// <summary>
            /// Il Simbolo da monitorare in relazione alla Label
            /// </summary>
            public readonly Symbol Symbol;

            /// <summary>
            /// Le Bars con il quale la strategia si muove ed elabora le sue condizioni
            /// </summary>
            public readonly Bars Bars;

            /// <summary>
            /// Il riferimento temporale della pausa
            /// </summary>
            public readonly PauseTimes Pause;

            /// <summary>
            /// Le informazioni raccolte dopo la chiamata .Update()
            /// </summary>
            public Information Info { get; private set; }

            /// <summary>
            /// Le posizioni filtrate in base al simbolo e alla label
            /// </summary>
            public Position[] Positions { get; private set; }

            /// <summary>
            /// Monitor per la raccolta d'informazioni inerenti la strategia in corso
            /// </summary>
            public Monitor(string NewLabel, Symbol NewSymbol, Bars NewBars, Positions AllPositions, PauseTimes NewPause)
            {

                Label = NewLabel;
                Symbol = NewSymbol;
                Bars = NewBars;
                Pause = NewPause;

                Info = new Information();

                _allPositions = AllPositions;

                // --> Rendiamo sin da subito disponibili le informazioni
                Update(false, null, null, 0);

            }

            /// <summary>
            /// Filtra e rende disponibili le informazioni per la strategia monitorata. Eventualmente Chiude e gestisce le operazioni
            /// </summary>
            public Information Update(bool closeall, BreakEvenData breakevendata, TrailingData trailingdata, double SafeLoss, TradeType? filtertype = null)
            {

                // --> Raccolgo le informazioni che mi servono per avere il polso della strategia
                Positions = _allPositions.FindAll(Label, Symbol.Name);

                // --> Devo trascinarmi i vecchi dati prima di aggiornarli come massimali
                double highestHighAfterFirstOpen = (Positions.Length > 0) ? Info.HighestHighAfterFirstOpen : 0;
                double lowestLowAfterFirstOpen = (Positions.Length > 0) ? Info.LowestLowAfterFirstOpen : 0;

                // --> Resetto le informazioni
                Info = new Information
                {

                    // --> Inizializzo con i vecchi dati
                    HighestHighAfterFirstOpen = highestHighAfterFirstOpen,
                    LowestLowAfterFirstOpen = lowestLowAfterFirstOpen

                };

                double tmpVolume = 0;

                foreach (Position position in Positions)
                {

                    // --> Per il trailing proactive e altre feature devo conoscere lo stato attuale
                    if (Info.HighestHighAfterFirstOpen == 0 || Symbol.Ask > Info.HighestHighAfterFirstOpen)
                        Info.HighestHighAfterFirstOpen = Symbol.Ask;
                    if (Info.LowestLowAfterFirstOpen == 0 || Symbol.Bid < Info.LowestLowAfterFirstOpen)
                        Info.LowestLowAfterFirstOpen = Symbol.Bid;

                    // --> Per prima cosa devo controllare se chiudere la posizione
                    if (closeall && (filtertype == null || position.TradeType == filtertype))
                    {

                        position.Close();
                        continue;

                    }

                    // --> Il broker potrebbe non accettare lo stoploss e quindi non settarlo, intervengo ?
                    if (SafeLoss > 0 && position.StopLoss == null)
                    {

                        TradeResult result = position.ModifyStopLossPips(SafeLoss);

                        // --> Troppa voaltilità potrebbe portare a proporzioni e valori errati, comunque non andiamo oltre 
                        if (result.Error == ErrorCode.InvalidRequest || result.Error == ErrorCode.InvalidStopLossTakeProfit)
                        {

                            position.Close();

                        }

                        continue;

                    }

                    // --> Poi tocca al break even
                    if ((breakevendata != null && !breakevendata.OnlyFirst) || Positions.Length == 1)
                        _checkBreakEven(position, breakevendata);

                    // --> Poi tocca al trailing
                    if ((trailingdata != null && !trailingdata.OnlyFirst) || Positions.Length == 1)
                        _checkTrailing(position, trailingdata);

                    Info.TotalNetProfit += position.NetProfit;
                    tmpVolume += position.VolumeInUnits;

                    switch (position.TradeType)
                    {
                        case TradeType.Buy:

                            Info.BuyPositions++;
                            break;

                        case TradeType.Sell:

                            Info.SellPositions++;
                            break;

                    }

                    if (Info.FirstPosition == null || position.EntryTime < Info.FirstPosition.EntryTime)
                        Info.FirstPosition = position;

                    if (Info.LastPosition == null || position.EntryTime > Info.LastPosition.EntryTime)
                        Info.LastPosition = position;

                    if (Info.MinVolumeInUnits == 0 || position.VolumeInUnits < Info.MinVolumeInUnits)
                        Info.MinVolumeInUnits = position.VolumeInUnits;

                    if (Info.MaxVolumeInUnits == 0 || position.VolumeInUnits > Info.MaxVolumeInUnits)
                        Info.MaxVolumeInUnits = position.VolumeInUnits;

                }

                // --> Restituisce una Exception Overflow di una operazione aritmetica, da approfondire
                //     Info.MidVolumeInUnits = Symbol.NormalizeVolumeInUnits(tmpVolume / Positions.Length,RoundingMode.ToNearest);
                Info.MidVolumeInUnits = Math.Round(tmpVolume / Positions.Length, 0);

                return Info;

            }

            /// <summary>
            /// Chiude tutte le posizioni del monitor
            /// </summary>
            public void CloseAllPositions(TradeType? filtertype = null)
            {

                Update(true, null, null, 0, filtertype);

            }

            /// <summary>
            /// Stabilisce se si è in GAP passando una certa distanza da misurare
            /// </summary>
            public bool InGAP(double distance)
            {

                return Symbol.DigitsToPips(Bars.LastGAP()) >= distance;

            }

            /// <summary>
            /// Controlla la fascia oraria per determinare se rientra in quella di pausa, utilizza dati double 
            /// perchè la ctrader non permette di esporre dati time, da aggiornare non appena la ctrader lo permette
            /// </summary>
            /// <returns>Conferma la presenza di una fascia oraria in pausa</returns>
            public bool InPause(DateTime timeserver)
            {

                // -->> Poichè si utilizzano dati double per esporre i parametri dobbiamo utilizzare meccanismi per tradurre l'orario
                string nowHour = (timeserver.Hour < 10) ? string.Format("0{0}", timeserver.Hour) : string.Format("{0}", timeserver.Hour);
                string nowMinute = (timeserver.Minute < 10) ? string.Format("0{0}", timeserver.Minute) : string.Format("{0}", timeserver.Minute);

                // --> Stabilisco il momento di controllo in formato double
                double adesso = Convert.ToDouble(string.Format("{0},{1}", nowHour, nowMinute));

                // --> Confronto elementare per rendere comprensibile la logica
                if (Pause.Over < Pause.Under && adesso >= Pause.Over && adesso <= Pause.Under)
                {

                    return true;

                }
                else if (Pause.Over > Pause.Under && ((adesso >= Pause.Over && adesso <= 23.59) || adesso <= Pause.Under))
                {

                    return true;

                }

                return false;

            }

            /// <summary>
            /// Controlla ed effettua la modifica in break-even se le condizioni le permettono
            /// </summary>
            private void _checkBreakEven(Position position, BreakEvenData breakevendata)
            {

                if (breakevendata == null || breakevendata.Activation == 0)
                    return;

                double activation = Symbol.PipsToDigits(breakevendata.Activation);

                int currentMinutes = Bars.TimeFrame.ToMinutes();
                DateTime limitTime = position.EntryTime.AddMinutes(currentMinutes * breakevendata.LimitBar);
                bool limitActivation = (breakevendata.LimitBar > 0 && Bars.Last(0).OpenTime >= limitTime);

                double distance = Symbol.PipsToDigits(breakevendata.Distance);

                switch (position.TradeType)
                {

                    case TradeType.Buy:

                        double breakevenpointbuy = Math.Round(position.EntryPrice + distance, Symbol.Digits);

                        if ((Symbol.Bid >= (position.EntryPrice + activation) || limitActivation) && (position.StopLoss == null || position.StopLoss < breakevenpointbuy))
                        {

                            position.ModifyStopLossPrice(breakevenpointbuy);

                        }
                        else if (breakevendata.Negative && (Symbol.Bid <= (position.EntryPrice - activation) || limitActivation) && (position.TakeProfit == null || position.TakeProfit > breakevenpointbuy))
                        {

                            position.ModifyTakeProfitPrice(breakevenpointbuy);

                        }

                        break;

                    case TradeType.Sell:

                        double breakevenpointsell = Math.Round(position.EntryPrice - distance, Symbol.Digits);

                        if ((Symbol.Ask <= (position.EntryPrice - activation)) && (position.StopLoss == null || position.StopLoss > breakevenpointsell))
                        {

                            position.ModifyStopLossPrice(breakevenpointsell);

                        }
                        else if (breakevendata.Negative && (Symbol.Ask >= (position.EntryPrice + activation)) && (position.TakeProfit == null || position.TakeProfit < breakevenpointsell))
                        {

                            position.ModifyTakeProfitPrice(breakevenpointsell);

                        }

                        break;

                }

            }

            /// <summary>
            /// Controlla ed effettua la modifica in trailing se le condizioni le permettono
            /// </summary>
            private void _checkTrailing(Position position, TrailingData trailingdata)
            {

                if (trailingdata == null || trailingdata.Activation == 0 || trailingdata.Distance == 0)
                    return;

                double trailing = 0;
                double distance = Symbol.PipsToDigits(trailingdata.Distance);
                double activation = Symbol.PipsToDigits(trailingdata.Activation);

                switch (position.TradeType)
                {

                    case TradeType.Buy:

                        trailing = Math.Round(Symbol.Bid - distance, Symbol.Digits);

                        if ((Symbol.Bid >= (position.EntryPrice + activation)) && (position.StopLoss == null || position.StopLoss < trailing))
                        {

                            position.ModifyStopLossPrice(trailing);

                        }
                        else if (trailingdata.ProActive && Info.HighestHighAfterFirstOpen > 0 && position.StopLoss != null && position.StopLoss > 0)
                        {

                            // --> Devo determinare se è partita l'attivazione
                            double activationprice = position.EntryPrice + activation;
                            double firsttrailing = Math.Round(activationprice - distance, Symbol.Digits);

                            // --> Partito il trailing? Sono in retrocessione ?
                            if (position.StopLoss >= firsttrailing)
                            {

                                double limitpriceup = Info.HighestHighAfterFirstOpen;
                                double limitpricedw = Math.Round(Info.HighestHighAfterFirstOpen - distance, Symbol.Digits);

                                double k = Math.Round(limitpriceup - Symbol.Ask, Symbol.Digits);

                                double newtrailing = Math.Round(limitpricedw + k, Symbol.Digits);

                                if (position.StopLoss < newtrailing)
                                    position.ModifyStopLossPrice(newtrailing);

                            }

                        }

                        break;

                    case TradeType.Sell:

                        trailing = Math.Round(Symbol.Ask + Symbol.PipsToDigits(trailingdata.Distance), Symbol.Digits);

                        if ((Symbol.Ask <= (position.EntryPrice - Symbol.PipsToDigits(trailingdata.Activation))) && (position.StopLoss == null || position.StopLoss > trailing))
                        {

                            position.ModifyStopLossPrice(trailing);

                        }
                        else if (trailingdata.ProActive && Info.LowestLowAfterFirstOpen > 0 && position.StopLoss != null && position.StopLoss > 0)
                        {

                            // --> Devo determinare se è partita l'attivazione
                            double activationprice = position.EntryPrice - activation;
                            double firsttrailing = Math.Round(activationprice + distance, Symbol.Digits);

                            // --> Partito il trailing? Sono in retrocessione ?
                            if (position.StopLoss <= firsttrailing)
                            {

                                double limitpriceup = Math.Round(Info.LowestLowAfterFirstOpen + distance, Symbol.Digits);
                                double limitpricedw = Info.LowestLowAfterFirstOpen;

                                double k = Math.Round(Symbol.Bid - limitpricedw, Symbol.Digits);

                                double newtrailing = Math.Round(limitpriceup - k, Symbol.Digits);

                                if (position.StopLoss > newtrailing)
                                    position.ModifyStopLossPrice(newtrailing);

                            }

                        }

                        break;

                }

            }

        }

        /// <summary>
        /// Classe per gestire il dimensionamento delle size
        /// </summary>
        public class MonenyManagement
        {

            private readonly double _minSize = 0.01;
            private double _percentage = 0;
            private double _fixedSize = 0;
            private double _pipToCalc = 30;

            // --> Riferimenti agli oggetti esterni utili per il calcolo
            private IAccount _account = null;
            public readonly Symbol Symbol;

            /// <summary>
            /// Il capitale da utilizzare per il calcolo
            /// </summary>
            public CapitalTo CapitalType = CapitalTo.Balance;

            /// <summary>
            /// La percentuale di rischio che si vuole investire
            /// </summary>
            public double Percentage
            {

                get { return _percentage; }


                set { _percentage = (value > 0 && value <= 100) ? value : 0; }
            }

            /// <summary>
            /// La size fissa da utilizzare, bypassa tutti i parametri di calcolo
            /// </summary>
            public double FixedSize
            {

                get { return _fixedSize; }



                set { _fixedSize = (value >= _minSize) ? value : 0; }
            }


            /// <summary>
            /// La distanza massima dall'ingresso con il quale calcolare le size
            /// </summary>
            public double PipToCalc
            {

                get { return _pipToCalc; }

                set { _pipToCalc = (value > 0) ? value : 100; }
            }


            /// <summary>
            /// Il capitale effettivo sul quale calcolare il rischio
            /// </summary>
            public double Capital
            {

                get
                {

                    switch (CapitalType)
                    {

                        case CapitalTo.Equity:

                            return _account.Equity;
                        default:


                            return _account.Balance;

                    }

                }
            }



            // --> Costruttore
            public MonenyManagement(IAccount NewAccount, CapitalTo NewCapitalTo, double NewPercentage, double NewFixedSize, double NewPipToCalc, Symbol NewSymbol)
            {

                _account = NewAccount;

                Symbol = NewSymbol;

                CapitalType = NewCapitalTo;
                Percentage = NewPercentage;
                FixedSize = NewFixedSize;
                PipToCalc = NewPipToCalc;

            }

            /// <summary>
            /// Restituisce il numero di lotti in formato 0.01
            /// </summary>
            public double GetLotSize()
            {

                // --> Hodeciso di usare una size fissa
                if (FixedSize > 0)
                    return FixedSize;

                // --> La percentuale di rischio in denaro
                double moneyrisk = Capital / 100 * Percentage;

                // --> Traduco lo stoploss o il suo riferimento in double
                double sl_double = PipToCalc * Symbol.PipSize;

                // --> In formato 0.01 = microlotto double lots = Math.Round(Symbol.VolumeInUnitsToQuantity(moneyrisk / ((sl_double * Symbol.TickValue) / Symbol.TickSize)), 2);
                // --> In formato volume 1K = 1000 Math.Round((moneyrisk / ((sl_double * Symbol.TickValue) / Symbol.TickSize)), 2);
                double lots = Math.Round(Symbol.VolumeInUnitsToQuantity(moneyrisk / ((sl_double * Symbol.TickValue) / Symbol.TickSize)), 2);

                if (lots < _minSize)
                    return _minSize;

                return lots;

            }

        }

        #endregion

        #region Helper

        /// <summary>
        /// Restituisce il colore corrispondente a partire dal nome
        /// </summary>
        /// <returns>Il colore corrispondente</returns>
        public static API.Color ColorFromEnum(ColorNameEnum colorName)
        {

            return API.Color.FromName(colorName.ToString("G"));

        }

        #endregion

        #region Bars

        /// <summary>
        /// Si ottiene l'indice della candela partendo dal suo orario di apertura
        /// </summary>
        /// <param name="MyTime">La data e l'ora di apertura della candela</param>
        /// <returns></returns>
        public static int GetIndexByDate(this Bars thisBars, DateTime thisTime)
        {

            for (int i = thisBars.ClosePrices.Count - 1; i >= 0; i--)
            {

                if (thisTime == thisBars.OpenTimes[i])
                    return i;

            }

            return -1;

        }

        public static double LastGAP(this Bars thisBars)
        {

            double K = 0;

            if (thisBars.ClosePrices.Last(1) > thisBars.OpenPrices.LastValue)
            {

                K = Math.Round(thisBars.ClosePrices.Last(1) - thisBars.OpenPrices.LastValue, 5);

            }
            else if (thisBars.ClosePrices.Last(1) < thisBars.OpenPrices.LastValue)
            {

                K = Math.Round(thisBars.OpenPrices.LastValue - thisBars.ClosePrices.Last(1), 5);

            }

            return K;

        }

        #endregion

        #region Bar

        /// <summary>
        /// Misura la grandezza di una candela, tenendo conto della sua direzione
        /// </summary>
        /// <returns>Il corpo della candela, valore uguale o superiore a zero</returns>
        public static double Body(this Bar thisBar)
        {

            return thisBar.IsBullish() ? thisBar.Close - thisBar.Open : thisBar.Open - thisBar.Close;


        }

        /// <summary>
        /// Verifica la direzione rialzista di una candela
        /// </summary>
        /// <returns>True se la candela è rialzista</returns>        
        public static bool IsBullish(this Bar thisBar)
        {

            return thisBar.Close > thisBar.Open;

        }

        /// <summary>
        /// Verifica la direzione ribassista di una candela
        /// </summary>
        /// <returns>True se la candela è ribassista</returns>        
        public static bool IsBearish(this Bar thisBar)
        {

            return thisBar.Close < thisBar.Open;

        }

        /// <summary>
        /// Verifica se una candela ha un open uguale al close
        /// </summary>
        /// <returns>True se la candela è una doji con Open e Close uguali</returns>        
        public static bool IsDoji(this Bar thisBar)
        {

            return thisBar.Close == thisBar.Open;

        }

        #endregion

        #region Symbol

        /// <summary>
        /// Converte il numero di pips corrente da digits a double
        /// </summary>
        /// <param name="Pips">Il numero di pips nel formato Digits</param>
        /// <returns></returns>
        public static double DigitsToPips(this Symbol thisSymbol, double Pips)
        {

            return Math.Round(Pips / thisSymbol.PipSize, 2);

        }

        /// <summary>
        /// Converte il numero di pips corrente da double a digits
        /// </summary>
        /// <param name="Pips">Il numero di pips nel formato Double (2)</param>
        /// <returns></returns>
        public static double PipsToDigits(this Symbol thisSymbol, double Pips)
        {

            return Math.Round(Pips * thisSymbol.PipSize, thisSymbol.Digits);

        }

        public static double RealSpread(this Symbol thisSymbol)
        {

            return Math.Round(thisSymbol.Spread / thisSymbol.PipSize, 2);

        }

        #endregion

        #region TimeFrame

        /// <summary>
        /// Restituisce in minuti il timeframe corrente
        /// </summary>
        public static int ToMinutes(this TimeFrame thisTimeFrame)
        {

            if (thisTimeFrame == TimeFrame.Daily)
                return 60 * 24;
            if (thisTimeFrame == TimeFrame.Day2)
                return 60 * 24 * 2;
            if (thisTimeFrame == TimeFrame.Day3)
                return 60 * 24 * 3;
            if (thisTimeFrame == TimeFrame.Hour)
                return 60;
            if (thisTimeFrame == TimeFrame.Hour12)
                return 60 * 12;
            if (thisTimeFrame == TimeFrame.Hour2)
                return 60 * 2;
            if (thisTimeFrame == TimeFrame.Hour3)
                return 60 * 3;
            if (thisTimeFrame == TimeFrame.Hour4)
                return 60 * 4;
            if (thisTimeFrame == TimeFrame.Hour6)
                return 60 * 6;
            if (thisTimeFrame == TimeFrame.Hour8)
                return 60 * 8;
            if (thisTimeFrame == TimeFrame.Minute)
                return 1;
            if (thisTimeFrame == TimeFrame.Minute10)
                return 10;
            if (thisTimeFrame == TimeFrame.Minute15)
                return 15;
            if (thisTimeFrame == TimeFrame.Minute2)
                return 2;
            if (thisTimeFrame == TimeFrame.Minute20)
                return 20;
            if (thisTimeFrame == TimeFrame.Minute3)
                return 3;
            if (thisTimeFrame == TimeFrame.Minute30)
                return 30;
            if (thisTimeFrame == TimeFrame.Minute4)
                return 4;
            if (thisTimeFrame == TimeFrame.Minute45)
                return 45;
            if (thisTimeFrame == TimeFrame.Minute5)
                return 5;
            if (thisTimeFrame == TimeFrame.Minute6)
                return 6;
            if (thisTimeFrame == TimeFrame.Minute7)
                return 7;
            if (thisTimeFrame == TimeFrame.Minute8)
                return 8;
            if (thisTimeFrame == TimeFrame.Minute9)
                return 9;
            if (thisTimeFrame == TimeFrame.Monthly)
                return 60 * 24 * 30;
            if (thisTimeFrame == TimeFrame.Weekly)
                return 60 * 24 * 7;

            return 0;

        }

        #endregion

    }

}

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.FullAccess)]
    public class Adrenaline : Robot
    {

        #region Identity

        public const int ID = 189113;

        public const string NAME = "Adrenaline";

        public const string VERSION = "1.1.0";

        #endregion

        #region UPDATE : VARIABILI

        private const string PRODUCTPAGE = "https://ctrader.guru/product/adrenaline/";
        private const string LICENSEPAGE = "https://ctrader.guru/license/";

        #endregion

        #region LICENZA : VARIABILI

        string productName = NAME;
        readonly string endpoint = "https://ctrader.guru/_checkpoint_/";

        DateTime licenzaExpire;
        CL_CTG_Licenza licenza = null;
        CL_CTG_Licenza.LicenzaInfo licenzaInfo = null;
        bool exitoncalculate = false;
        private ControlBase DrawingDialog = null;
        public Extensions.ColorNameEnum TextColor = Extensions.ColorNameEnum.Coral;

        #endregion

        #region Params

        [Parameter(NAME + " " + VERSION, Group = "Identity", DefaultValue = PRODUCTPAGE)]
        public string ProductInfo { get; set; }

        [Parameter("Label ( Magic Name )", Group = "Identity", DefaultValue = NAME)]
        public string MyLabel { get; set; }

        [Parameter("Preset Information", Group = "Identity", DefaultValue = "(v.1.0.6) EURUSD 30m - Balance €1000 - BackTested 06.05.2020 To 06.05.2021 - R:R 1:3")]
        public string PresetInfo { get; set; }

        [Parameter("Max Cross Coworking (zero = unlimited)", Group = "Strategy", DefaultValue = 1, MinValue = 0)]
        public int MaxCross { get; set; }

        [Parameter("Mode", Group = "Strategy", DefaultValue = Extensions.StrategyType.Moderate)]
        public Extensions.StrategyType StrategyType { get; set; }

        [Parameter("Stop Loss", Group = "Strategy", DefaultValue = 30)]
        public int StopLoss { get; set; }

        [Parameter("Take Profit", Group = "Strategy", DefaultValue = 30)]
        public int TakeProfit { get; set; }

        [Parameter("Lots", Group = "Strategy", DefaultValue = 0.1, MinValue = 0.01, Step = 0.01)]
        public double Lots { get; set; }

        [Parameter("Balance Multiplier (zero = disabled; es. 0.01 each € 1000 )", Group = "Strategy", DefaultValue = 0, MinValue = 0, Step = 0.5)]
        public double BalanceMultiplier { get; set; }

        [Parameter("Max Spread allowed", Group = "Filters", DefaultValue = 1.5, MinValue = 0.1, Step = 0.1)]
        public double SpreadToTrigger { get; set; }

        [Parameter("Max GAP Allowed (pips)", Group = "Filters", DefaultValue = 1, MinValue = 0, Step = 0.01)]
        public double GAP { get; set; }

        [Parameter("Pause over this time", Group = "Time Zone", DefaultValue = 0, MinValue = 0, MaxValue = 23.59)]
        public double PauseOver { get; set; }

        [Parameter("Pause under this time", Group = "Time Zone", DefaultValue = 0, MinValue = 0, MaxValue = 23.59)]
        public double PauseUnder { get; set; }

        [Parameter("Multiplier", Group = "Adaptive Martingale", DefaultValue = 2, MinValue = 1)]
        public double Multiplier { get; set; }

        [Parameter("Max Loss Before Multiplier (zero = always)", Group = "Adaptive Martingale", DefaultValue = 0, MinValue = 0)]
        public int MaxLossBeforeMultiplier { get; set; }

        [Parameter("Max Consecutive Loss (zero = unlimited)", Group = "Adaptive Martingale", DefaultValue = 5, MinValue = 0)]
        public int MaxLoss { get; set; }

        [Parameter("Period", Group = "RSI", DefaultValue = 14, MinValue = 1)]
        public int RsiPeriod { get; set; }

        [Parameter("Buy Under this Value", Group = "RSI", DefaultValue = 50, MinValue = 1)]
        public double RsiUnder { get; set; }

        [Parameter("Sell Over this Value", Group = "RSI", DefaultValue = 50, MinValue = 2)]
        public double RsiOver { get; set; }

        #endregion

        #region Property

        int ConsecutiveLoss = 0;

        ParabolicSAR SAR;
        private ExponentialMovingAverage EMA200;
        private ExponentialMovingAverage EMA500;
        private RelativeStrengthIndex RSI;

        Extensions.Monitor.PauseTimes Pause1;
        Extensions.Monitor Monitor1;

        #endregion

        #region cBot Events

        protected override void OnStart()
        {

            #region LICENZA : INIT CHECK

            CL_CTG_Licenza.LicenzaConfig licConfig = new CL_CTG_Licenza.LicenzaConfig
            {
                AccountBroker = Account.BrokerName,
                AcconuntNumber = Account.Number.ToString()
            };

            licenza = new CL_CTG_Licenza(endpoint, licConfig, productName);

            _checkLicense();

            if (exitoncalculate)
                return;

            #endregion

            #region UPDATE : INIT CHECK

            _checkProductUpdate();

            #endregion

            if (StrategyType == Extensions.StrategyType.Auto)
                StrategyType = (StopLoss > TakeProfit) ? Extensions.StrategyType.Aggressive : Extensions.StrategyType.Moderate;

            Pause1 = new Extensions.Monitor.PauseTimes
            {

                Over = PauseOver,
                Under = PauseUnder

            };
            Monitor1 = new Extensions.Monitor(MyLabel, Symbol, Bars, Positions, Pause1);

            Positions.Closed += _onPositionsClosed;
            Positions.Opened += _onPositionsOpened;

            SAR = Indicators.ParabolicSAR(0.02, 0.2);
            EMA200 = Indicators.ExponentialMovingAverage(Bars.ClosePrices, 200);
            EMA500 = Indicators.ExponentialMovingAverage(Bars.ClosePrices, 500);
            RSI = Indicators.RelativeStrengthIndex(Bars.ClosePrices, RsiPeriod);

        }

        protected override void OnBar()
        {

            Monitor1.OpenedInThisBar = false;

        }

        protected override void OnTick()
        {


            #region LICENZA : LOOP CHECK                       

            if (RunningMode == RunningMode.RealTime)
            {

                if (exitoncalculate)
                {

                    if (DrawingDialog != null && !DrawingDialog.IsVisible)
                    {

                        Stop();

                    }
                    else
                    {

                        _createButtonLogin();

                    }

                    return;

                }
                else if (licenzaExpire != null && licenzaInfo.Expire.CompareTo("*") != 0 && Monitor1.Positions.Length == 0)
                {

                    if (DateTime.Compare(licenzaExpire, Server.Time) > 0)
                    {

                        // --> TODO

                    }
                    else
                    {

                        exitoncalculate = true;

                        Print("Expired (" + licenzaExpire + ")" + " (server : " + Server.Time.ToString() + ")");

                        return;

                    }

                }

            }

            #endregion

            Monitor1.Update(false, null, null, 0, null);

            if (Monitor1.OpenedInThisBar || Monitor1.Positions.Length > 0 || Monitor1.InPause(Server.Time) || !_canCowork(Monitor1))
                return;

            bool sharedFilter = (Monitor1.Symbol.RealSpread() <= SpreadToTrigger && !Monitor1.InGAP(GAP));

            bool filter1long = EMA200.Result.LastValue > EMA500.Result.LastValue;
            bool filter2long = Ask > EMA200.Result.LastValue;
            bool filter3long = RSI.Result.LastValue < RsiUnder;

            bool filter1short = EMA200.Result.LastValue < EMA500.Result.LastValue;
            bool filter2short = Bid < EMA200.Result.LastValue;
            bool filter3short = RSI.Result.LastValue > RsiOver;

            double RealMultiplier = (BalanceMultiplier > 0) ? (int)(Account.Balance / BalanceMultiplier) : 1;
            double RealLots = (RealMultiplier > 1) ? Lots * RealMultiplier : Lots;

            if (_SARTriggerLong() && sharedFilter && filter1long && filter2long && filter3long)
            {

                ExecuteOrder(Symbol.QuantityToVolumeInUnits(RealLots), TradeType.Buy);

            }
            else if (_SARTriggerShort() && sharedFilter && filter1short && filter2short && filter3short)
            {

                ExecuteOrder(Symbol.QuantityToVolumeInUnits(RealLots), TradeType.Sell);

            }

        }

        #endregion

        #region Private Methods

        private void ExecuteOrder(double volume, TradeType tradeType, double? tmpSL = 0, double? tmpTP = 0)
        {

            if (tmpSL == 0) tmpSL = StopLoss;
            if (tmpTP == 0) tmpTP = TakeProfit;

            var result = ExecuteMarketOrder(tradeType, SymbolName, volume, MyLabel, tmpSL, tmpTP);

            if (result.Error == ErrorCode.NoMoney)
            {

                Print("No Money, close cBot");
                Stop();

            }

        }

        private void _onPositionsClosed(PositionClosedEventArgs args)
        {

            var position = args.Position;

            if (position.Label != MyLabel || position.SymbolName != SymbolName)
                return;

            if (position.NetProfit < 0)
            {

                ConsecutiveLoss++;

                if (MaxLoss == 0 || ConsecutiveLoss < MaxLoss)
                {

                    //MaxLossBeforeMultiplier

                    double RealMultiplier = (MaxLossBeforeMultiplier == 0 || ConsecutiveLoss >= MaxLossBeforeMultiplier) ? Multiplier : 1;

                    TradeType reversed = (position.TradeType == TradeType.Sell) ? TradeType.Buy : TradeType.Sell;

                    ExecuteOrder(Symbol.QuantityToVolumeInUnits(Math.Round(position.Quantity * RealMultiplier, 2)), reversed);

                }
                else if (MaxLoss > 0 && ConsecutiveLoss >= MaxLoss && StrategyType == Extensions.StrategyType.Aggressive)
                {

                    ConsecutiveLoss = 0;

                }

            }
            else
            {

                ConsecutiveLoss = 0;

            }

        }

        private void _onPositionsOpened(PositionOpenedEventArgs eventArgs)
        {

            if (eventArgs.Position.SymbolName == Monitor1.Symbol.Name && eventArgs.Position.Label == Monitor1.Label)
            {

                Monitor1.OpenedInThisBar = true;
                Monitor1.OpenedInThisTrigger = true;

            }

        }

        private bool _SARTriggerShort()
        {

            return (SAR.Result.Last(1) <= Bars.OpenPrices.Last(1) && SAR.Result.LastValue >= Bars.OpenPrices.LastValue);

        }

        private bool _SARTriggerLong()
        {

            return (SAR.Result.Last(1) >= Bars.OpenPrices.Last(1) && SAR.Result.LastValue <= Bars.OpenPrices.LastValue);

        }

        private bool _canCowork(Extensions.Monitor monitor)
        {

            return (MaxCross == 0 || monitor.Positions.Length > 0) ? true : _getOtherCross().Count < MaxCross;

        }

        private List<string> _getOtherCross()
        {

            List<string> OtherCross = new List<string>();

            foreach (Position position in Positions)
            {

                if (position.SymbolName != SymbolName && !OtherCross.Contains(position.SymbolName))
                    OtherCross.Add(position.SymbolName);

            }

            return OtherCross;

        }

        #endregion

        #region LICENZA & UPDATE : PRIVATE METHOD

        // --> Versione modifica, originale in ScalFibo
        private void _checkProductUpdate()
        {

            // --> Controllo solo se sono in realtime, evito le chiamate in backtest
            if (RunningMode != RunningMode.RealTime)
                return;

            // --> Organizzo i dati per la richiesta degli aggiornamenti
            Guru.API.RequestProductInfo Request = new Guru.API.RequestProductInfo
            {

                MyProduct = new Guru.Product
                {

                    ID = ID,
                    Name = NAME,
                    Version = VERSION

                },
                AccountBroker = Account.BrokerName,
                AccountNumber = Account.Number

            };

            // --> Effettuo la richiesta
            Guru.API Response = new Guru.API(Request);

            // --> Controllo per prima cosa la presenza di errori di comunicazioni
            if (Response.ProductInfo.Exception != "")
            {

                Print("{0} Exception : {1}", NAME, Response.ProductInfo.Exception);

            }
            // --> Chiedo conferma della presenza di nuovi aggiornamenti
            else if (Response.HaveNewUpdate())
            {

                string updatemex = string.Format("{0} : Updates available {1} ( {2} )", NAME, Response.ProductInfo.LastProduct.Version, Response.ProductInfo.LastProduct.Updated);

                // --> Informo l'utente con un messaggio sul grafico e nei log del cbot
                Chart.DrawStaticText(NAME + "Updates", updatemex, VerticalAlignment.Top, cAlgo.API.HorizontalAlignment.Left, Extensions.ColorFromEnum(TextColor));
                Print(updatemex);

            }

        }

        private void _checkLicense(bool bypassread = false)
        {

            if (RunningMode != RunningMode.RealTime)
                return;

            try
            {

                // --> Controllo la licenza solo dal file
                if (!bypassread)
                    licenzaInfo = licenza.GetLicenzaFromFile();

                // --> Se non ho il login chiedo di generarlo
                if (!licenzaInfo.Login)
                {

                    _createButtonLogin();
                    exitoncalculate = true;
                    return;

                }
                else
                {

                    if (licenzaInfo.Product.CompareTo(productName.ToUpper()) != 0)
                    {

                        if (MessageBox.Show("Not for this product, remove cookie session?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                            _removeCookieAndLicense(licenza);

                        exitoncalculate = true;
                        return;

                    }
                    else
                    {

                        if ((licenzaInfo.AccountBroker.CompareTo("*") != 0 && licenzaInfo.AccountBroker.CompareTo(Account.BrokerName) != 0) || (licenzaInfo.AccountNumber.CompareTo("*") != 0 && licenzaInfo.AccountNumber.CompareTo(Account.Number.ToString()) != 0))
                        {

                            if (MessageBox.Show("Not for this account, remove cookie session?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                                _removeCookieAndLicense(licenza);

                            exitoncalculate = true;
                            return;

                        }
                        else
                        {

                            if (licenzaInfo.Expire == null || licenzaInfo.Expire.Length < 1)
                            {

                                if (MessageBox.Show("Expired, remove cookie session?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                                    _removeCookieAndLicense(licenza);

                                exitoncalculate = true;
                                return;


                            }
                            else if (licenzaInfo.Expire.CompareTo("*") != 0)
                            {

                                try
                                {

                                    String[] substringsExpire = licenzaInfo.Expire.Split(',');

                                    licenzaExpire = new DateTime(Int32.Parse(substringsExpire[0].Trim()), Int32.Parse(substringsExpire[1].Trim()), Int32.Parse(substringsExpire[2].Trim()), Int32.Parse(substringsExpire[3].Trim()), Int32.Parse(substringsExpire[4].Trim()), Int32.Parse(substringsExpire[5].Trim()));


                                    if (DateTime.Compare(licenzaExpire, Server.Time) > 0)
                                    {

                                        Print("Expire : " + licenzaExpire.ToString() + " (server : " + Server.Time.ToString() + ")");
                                        exitoncalculate = false;

                                    }
                                    else
                                    {

                                        if (MessageBox.Show("Expired (" + licenzaExpire + "), remove cookie session?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                                            _removeCookieAndLicense(licenza);

                                        exitoncalculate = true;
                                        return;

                                    }

                                }
                                catch
                                {

                                    if (MessageBox.Show("Expired, remove cookie session?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                                        _removeCookieAndLicense(licenza);

                                    exitoncalculate = true;
                                    return;

                                }

                            }
                            else
                            {

                                Print("Lifetime");
                                exitoncalculate = false;

                            }

                        }

                    }

                }

            }
            catch (Exception exp)
            {

                MessageBox.Show("Encryption issue, contact support@ctrader.guru", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                licenza.RemoveLicense();
                exitoncalculate = true;

                Print("Debug : " + exp.Message);

                return;

            }

        }

        private void _createButtonLogin()
        {

            if (RunningMode != RunningMode.RealTime)
                return;

            if (DrawingDialog != null)
            {

                DrawingDialog.IsVisible = true;
                return;

            }

            StackPanel stackPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = API.HorizontalAlignment.Center,
                Orientation = API.Orientation.Vertical,
                IsVisible = false,
                Width = 200,
                BackgroundColor = Color.Red,
                Margin = new Thickness(10, 10, 10, 10)
            };

            Button btnLogin = new Button
            {
                Text = "CTRADER GURU - LOGIN",
                BackgroundColor = Color.Red,
                ForegroundColor = Color.White,
                Top = 10,
                CornerRadius = 0,
                HorizontalContentAlignment = API.HorizontalAlignment.Center

            };
            btnLogin.Click += delegate
            {

                if (!exitoncalculate)
                    return;

                DrawingDialog.IsVisible = false;
                System.Windows.Forms.Application.DoEvents();

                try
                {


                    _createLicense();

                    OnStart();
                    OnTick();

                    DrawingDialog.IsVisible = false;
                    System.Windows.Forms.Application.DoEvents();

                }
                catch
                {
                }


            };

            stackPanel.AddChild(btnLogin);

            DrawingDialog = stackPanel;
            Chart.AddControl(DrawingDialog);

            DrawingDialog.IsVisible = true;

        }

        private void _createLicense()
        {

            if (RunningMode != RunningMode.RealTime)
                return;

            // --> Chiedo al server con i cookie, ma prima tento il recupero dal file
            licenzaInfo = licenza.GetLicenzaFromFile();

            if (licenzaInfo.ErrorProc == -2000)
            {

                MessageBox.Show("Waiting");
                return;
            }

            if (!licenzaInfo.Login || licenzaInfo.ErrorProc != 1000)
                licenzaInfo = licenza.GetLicenzaFromServer();

            // --> Ci sono problemi con i cookie
            if (licenzaInfo.ErrorProc == 2 || licenzaInfo.ErrorProc == 3 || licenzaInfo.Login == false)
            {
                // --> Rimuovo i cookie comunque
                licenza.RemoveCookie();
                licenza.RemoveLicense();


                // --> Li rigenero chiedendo il login, faccio attenzione ad altri processi
                Process[] processlist = Process.GetProcesses();
                bool finded = false;

                foreach (Process process in processlist)
                {

                    if (!String.IsNullOrEmpty(process.MainWindowTitle) && process.MainWindowTitle.ToUpper().CompareTo("CTRADER GURU - LOGIN") == 0)
                    {

                        finded = true;
                        break;

                    }

                }

                if (!finded)
                {


                    frmLogin LoginForm = new frmLogin(Account.BrokerName, Account.Number.ToString());
                    LoginForm.FormClosed += delegate
                    {

                        licenzaInfo = licenza.GetLicenzaFromServer();
                        _checkLicense();

                    };

                    LoginForm.ShowDialog();

                }
                else
                {

                    _alertChart("Others are logging in, waiting...");
                }

                exitoncalculate = true;

            }
            else
            {

                _checkLicense(true);

            }

        }

        private void _removeCookieAndLicense(CL_CTG_Licenza licenza)
        {

            licenza.RemoveCookie();
            licenza.RemoveLicense();

        }

        private void _alertChart(string mymex, bool withPrint = true)
        {

            if (RunningMode != RunningMode.RealTime)
                return;

            string mex = string.Format("{0} : {1}", NAME.ToUpper(), mymex);

            Chart.DrawStaticText("alert", mex, VerticalAlignment.Center, API.HorizontalAlignment.Center, Color.Red);
            if (withPrint)
                Print(mex);

        }

        #endregion

    }

}

/// <summary>
/// NameSpace che racchiude tutte le feature ctrader.guru
/// </summary>
namespace Guru
{
    /// <summary>
    /// Classe che definisce lo standard identificativo del prodotto nel marketplace ctrader.guru
    /// </summary>
    public class Product
    {

        public int ID = 0;
        public string Name = "";
        public string Version = "";
        public string Updated = "";
        public string LastCheck = "";

    }

    /// <summary>
    /// Offre la possibilità di utilizzare le API messe a disposizione da ctrader.guru per verificare gli aggiornamenti del prodotto.
    /// Permessi utente "AccessRights = AccessRights.FullAccess" per accedere a internet ed utilizzare JSON
    /// </summary>
    public class API
    {
        /// <summary>
        /// Costante da non modificare, corrisponde alla pagina dei servizi API
        /// </summary>
        private const string Service = "https://ctrader.guru/api/product_info/";

        private static string MainPath = string.Format("{0}\\cAlgo\\cTraderGuru\\", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        private readonly string InfoFile = string.Format("{0}update", MainPath);

        /// <summary>
        /// Costante da non modificare, utilizzata per filtrare le richieste
        /// </summary>
        private const string UserAgent = "cTrader Guru";

        /// <summary>
        /// Variabile dove verranno inserite le direttive per la richiesta
        /// </summary>
        private RequestProductInfo RequestProduct = new RequestProductInfo();

        /// <summary>
        /// Variabile dove verranno inserite le informazioni identificative dal server dopo l'inizializzazione della classe API
        /// </summary>
        public ResponseProductInfo ProductInfo = new ResponseProductInfo();

        /// <summary>
        /// Classe che formalizza i parametri di richiesta, vengono inviate le informazioni del prodotto e di profilazione a fini statistici
        /// </summary>
        public class RequestProductInfo
        {

            /// <summary>
            /// Il prodotto corrente per il quale richiediamo le informazioni
            /// </summary>
            public Product MyProduct = new Product();

            /// <summary>
            /// Broker con il quale effettiamo la richiesta
            /// </summary>
            public string AccountBroker = "";

            /// <summary>
            /// Il numero di conto con il quale chiediamo le informazioni
            /// </summary>
            public int AccountNumber = 0;

        }

        /// <summary>
        /// Classe che formalizza lo standard per identificare le informazioni del prodotto
        /// </summary>
        public class ResponseProductInfo
        {

            /// <summary>
            /// Il prodotto corrente per il quale vengono fornite le informazioni
            /// </summary>
            public Product LastProduct = new Product();

            /// <summary>
            /// Eccezioni in fase di richiesta al server, da utilizzare per controllare l'esito della comunicazione
            /// </summary>
            public string Exception = "";

            /// <summary>
            /// La risposta del server
            /// </summary>
            public string Source = "";

        }

        /// <summary>
        /// Richiede le informazioni del prodotto richiesto
        /// </summary>
        /// <param name="Request"></param>
        public API(RequestProductInfo Request)
        {

            RequestProduct = Request;

            // --> Non controllo se non ho l'ID del prodotto
            if (Request.MyProduct.ID <= 0)
                return;

            string cleanedproduct = string.Join("-", Request.MyProduct.Name.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            string fileToCheck = InfoFile + "-" + cleanedproduct.ToUpper() + ".json";

            // --> Controllo che siano passati almeno 30minuti tra una richiesta e l'altra
            try
            {

                string infodata = File.ReadAllText(fileToCheck);

                Product infolocal = JsonConvert.DeserializeObject<Product>(infodata);

                if (infolocal.LastCheck != "" && infolocal.ID == Request.MyProduct.ID)
                {

                    DateTime timeToTrigger = DateTime.Parse(infolocal.LastCheck).AddMinutes(60);

                    // --> Controllo se ci sono le condizioni per procedere
                    if (DateTime.Compare(timeToTrigger, DateTime.Now) > 0)
                    {

                        ProductInfo.LastProduct = infolocal;
                        return;

                    }

                }

            }
            catch
            {

            }

            // --> Dobbiamo supervisionare la chiamata per registrare l'eccexione
            try
            {

                // --> Strutturo le informazioni per la richiesta POST
                NameValueCollection data = new NameValueCollection
                {
                    {
                        "account_broker",
                        Request.AccountBroker
                    },
                    {
                        "account_number",
                        Request.AccountNumber.ToString()
                    },
                    {
                        "my_version",
                        Request.MyProduct.Version
                    },
                    {
                        "productid",
                        Request.MyProduct.ID.ToString()
                    }
                };

                // --> Autorizzo tutte le pagine di questo dominio
                Uri myuri = new Uri(Service);
                string pattern = string.Format("{0}://{1}/.*", myuri.Scheme, myuri.Host);

                Regex urlRegEx = new Regex(pattern);
                WebPermission p = new WebPermission(NetworkAccess.Connect, urlRegEx);
                p.Assert();

                // --> Protocollo di sicurezza https://
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;

                // -->> Richiedo le informazioni al server
                using (var wb = new WebClient())
                {

                    wb.Headers.Add("User-Agent", UserAgent);

                    var response = wb.UploadValues(myuri, "POST", data);
                    ProductInfo.Source = Encoding.UTF8.GetString(response);

                }

                // -->>> Nel cBot necessita l'attivazione di "AccessRights = AccessRights.FullAccess"
                ProductInfo.LastProduct = JsonConvert.DeserializeObject<Product>(ProductInfo.Source);
                ProductInfo.LastProduct.LastCheck = DateTime.Now.ToString();

                // --> Aggiorno il file locale
                try
                {

                    Directory.CreateDirectory(MainPath);

                    File.WriteAllText(fileToCheck, JsonConvert.SerializeObject(ProductInfo.LastProduct));

                }
                catch
                {
                }

            }
            catch (Exception Exp)
            {

                // --> Qualcosa è andato storto, registro l'eccezione
                ProductInfo.Exception = Exp.Message;

            }

        }

        /// <summary>
        /// Esegue un confronto tra le versioni per determinare la presenza di aggiornamenti
        /// </summary>
        /// <returns></returns>
        public bool HaveNewUpdate()
        {

            // --> Voglio essere sicuro che stiamo lavorando con le informazioni giuste
            return (ProductInfo.LastProduct.ID == RequestProduct.MyProduct.ID && ProductInfo.LastProduct.Version != "" && RequestProduct.MyProduct.Version != "" && ProductInfo.LastProduct.Version != null && new Version(RequestProduct.MyProduct.Version).CompareTo(new Version(ProductInfo.LastProduct.Version)) < 0);

        }

    }

}
