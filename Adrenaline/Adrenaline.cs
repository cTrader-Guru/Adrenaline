using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using System.Collections.Generic;

using System.Globalization;


namespace cAlgo
{

    public static class Extensions
    {

        #region Enum

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

        public class Monitor
        {

            private readonly Positions _allPositions = null;

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

            public class PauseTimes
            {

                public double Over = 0;
                public double Under = 0;

            }

            public class BreakEvenData
            {

                public bool OnlyFirst = false;
                public bool Negative = false;
                public double Activation = 0;
                public int LimitBar = 0;
                public double Distance = 0;

            }

            public class TrailingData
            {

                public bool OnlyFirst = false;
                public bool ProActive = false;
                public double Activation = 0;
                public double Distance = 0;

            }

            public bool OpenedInThisBar = false;

            public bool OpenedInThisTrigger = false;

            public readonly string Label;

            public readonly Symbol Symbol;

            public readonly Bars Bars;

            public readonly PauseTimes Pause;

            public Information Info { get; private set; }

            public Position[] Positions { get; private set; }

            public Monitor(string NewLabel, Symbol NewSymbol, Bars NewBars, Positions AllPositions, PauseTimes NewPause)
            {

                Label = NewLabel;
                Symbol = NewSymbol;
                Bars = NewBars;
                Pause = NewPause;

                Info = new Information();

                _allPositions = AllPositions;

                Update(false, null, null, 0);

            }

            public Information Update(bool closeall, BreakEvenData breakevendata, TrailingData trailingdata, double SafeLoss, TradeType? filtertype = null)
            {

                Positions = _allPositions.FindAll(Label, Symbol.Name);

                double highestHighAfterFirstOpen = (Positions.Length > 0) ? Info.HighestHighAfterFirstOpen : 0;
                double lowestLowAfterFirstOpen = (Positions.Length > 0) ? Info.LowestLowAfterFirstOpen : 0;

                Info = new Information 
                {

                    HighestHighAfterFirstOpen = highestHighAfterFirstOpen,
                    LowestLowAfterFirstOpen = lowestLowAfterFirstOpen

                };

                double tmpVolume = 0;

                foreach (Position position in Positions)
                {

                    if (Info.HighestHighAfterFirstOpen == 0 || Symbol.Ask > Info.HighestHighAfterFirstOpen)
                        Info.HighestHighAfterFirstOpen = Symbol.Ask;
                    if (Info.LowestLowAfterFirstOpen == 0 || Symbol.Bid < Info.LowestLowAfterFirstOpen)
                        Info.LowestLowAfterFirstOpen = Symbol.Bid;

                    if (closeall && (filtertype == null || position.TradeType == filtertype))
                    {

                        position.Close();
                        continue;

                    }

                    if (SafeLoss > 0 && position.StopLoss == null)
                    {

                        TradeResult result = position.ModifyStopLossPips(SafeLoss);

                        if (result.Error == ErrorCode.InvalidRequest || result.Error == ErrorCode.InvalidStopLossTakeProfit)
                        {

                            position.Close();

                        }

                        continue;

                    }

                    if ((breakevendata != null && !breakevendata.OnlyFirst) || Positions.Length == 1)
                        CheckBreakEven(position, breakevendata);

                    if ((trailingdata != null && !trailingdata.OnlyFirst) || Positions.Length == 1)
                        CheckTrailing(position, trailingdata);

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

                //     Info.MidVolumeInUnits = Symbol.NormalizeVolumeInUnits(tmpVolume / Positions.Length,RoundingMode.ToNearest);
                Info.MidVolumeInUnits = Math.Round(tmpVolume / Positions.Length, 0);

                return Info;

            }

            public void CloseAllPositions(TradeType? filtertype = null)
            {

                Update(true, null, null, 0, filtertype);

            }

            public bool InGAP(double distance)
            {

                return Symbol.DigitsToPips(Bars.LastGAP()) >= distance;

            }

            public bool InPause(DateTime timeserver)
            {

                string nowHour = (timeserver.Hour < 10) ? string.Format("0{0}", timeserver.Hour) : string.Format("{0}", timeserver.Hour);
                string nowMinute = (timeserver.Minute < 10) ? string.Format("0{0}", timeserver.Minute) : string.Format("{0}", timeserver.Minute);

                double adesso = Convert.ToDouble(string.Format("{0},{1}", nowHour, nowMinute));

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

            private void CheckBreakEven(Position position, BreakEvenData breakevendata)
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

            private void CheckTrailing(Position position, TrailingData trailingdata)
            {

                if (trailingdata == null || trailingdata.Activation == 0 || trailingdata.Distance == 0)
                    return;

                double trailing;
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

                            double activationprice = position.EntryPrice + activation;
                            double firsttrailing = Math.Round(activationprice - distance, Symbol.Digits);

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

                            double activationprice = position.EntryPrice - activation;
                            double firsttrailing = Math.Round(activationprice + distance, Symbol.Digits);

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

        public class MonenyManagement
        {

            private readonly double _minSize = 0.01;
            private double _percentage = 0;
            private double _fixedSize = 0;
            private double _pipToCalc = 30;

            private readonly IAccount _account = null;
            public readonly Symbol Symbol;

            public CapitalTo CapitalType = CapitalTo.Balance;

            public double Percentage
            {

                get { return _percentage; }


                set { _percentage = (value > 0 && value <= 100) ? value : 0; }
            }

            public double FixedSize
            {

                get { return _fixedSize; }



                set { _fixedSize = (value >= _minSize) ? value : 0; }
            }


            public double PipToCalc
            {

                get { return _pipToCalc; }

                set { _pipToCalc = (value > 0) ? value : 100; }
            }


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


            public MonenyManagement(IAccount NewAccount, CapitalTo NewCapitalTo, double NewPercentage, double NewFixedSize, double NewPipToCalc, Symbol NewSymbol)
            {

                _account = NewAccount;

                Symbol = NewSymbol;

                CapitalType = NewCapitalTo;
                Percentage = NewPercentage;
                FixedSize = NewFixedSize;
                PipToCalc = NewPipToCalc;

            }

            public double GetLotSize()
            {

                if (FixedSize > 0)
                    return FixedSize;

                double moneyrisk = Capital / 100 * Percentage;

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


        public static API.Color ColorFromEnum(ColorNameEnum colorName)
        {

            return API.Color.FromName(colorName.ToString("G"));

        }

        #endregion

        #region Bars

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

        public static double Body(this Bar thisBar)
        {

            return thisBar.IsBullish() ? thisBar.Close - thisBar.Open : thisBar.Open - thisBar.Close;


        }

        public static bool IsBullish(this Bar thisBar)
        {

            return thisBar.Close > thisBar.Open;

        }

        public static bool IsBearish(this Bar thisBar)
        {

            return thisBar.Close < thisBar.Open;

        }

        public static bool IsDoji(this Bar thisBar)
        {

            return thisBar.Close == thisBar.Open;

        }

        #endregion

        #region Symbol

        public static double DigitsToPips(this Symbol thisSymbol, double Pips)
        {

            return Math.Round(Pips / thisSymbol.PipSize, 2);

        }

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
        
        public static double ToDouble(this string thisString, string Culture = "en-EN")
        {

            var culture = CultureInfo.GetCultureInfo(Culture);
            return double.Parse(thisString.Replace(',', '.').ToString(CultureInfo.InvariantCulture), culture);

        }
        
        public static double ToDouble(this DateTime thisDateTime, string Culture = "en-EN")
        {

            string nowHour = (thisDateTime.Hour < 10) ? string.Format("0{0}", thisDateTime.Hour) : string.Format("{0}", thisDateTime.Hour);
            string nowMinute = (thisDateTime.Minute < 10) ? string.Format("0{0}", thisDateTime.Minute) : string.Format("{0}", thisDateTime.Minute);

            return string.Format("{0}.{1}", nowHour, nowMinute).ToDouble(Culture);

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

        public const string NAME = "Adrenaline";

        public const string VERSION = "1.1.3";

        #endregion

        #region Params

        [Parameter(NAME + " " + VERSION, Group = "Identity", DefaultValue = "https://www.google.com/search?q=ctrader+guru+adrenaline+scalper")]
        public string ProductInfo { get; set; }

        [Parameter("Label ( Magic Name )", Group = "Identity", DefaultValue = NAME)]
        public string MyLabel { get; set; }

        [Parameter("Preset Information", Group = "Identity", DefaultValue = "(v.1.1.1) EURUSD 5m - Balance €1000 - BackTested 17.11.2021 To 11.04.2022 - 134%")]
        public string PresetInfo { get; set; }

        [Parameter("Max Cross Coworking (zero = unlimited)", Group = "Strategy", DefaultValue = 1, MinValue = 0)]
        public int MaxCross { get; set; }

        [Parameter("Mode", Group = "Strategy", DefaultValue = Extensions.StrategyType.Moderate)]
        public Extensions.StrategyType StrategyType { get; set; }

        [Parameter("Stop Loss", Group = "Strategy", DefaultValue = 25)]
        public int StopLoss { get; set; }

        [Parameter("Take Profit", Group = "Strategy", DefaultValue = 41)]
        public int TakeProfit { get; set; }

        [Parameter("Lots", Group = "Strategy", DefaultValue = 0.06, MinValue = 0.01, Step = 0.01)]
        public double Lots { get; set; }

        [Parameter("Balance Multiplier (zero = disabled; es. 0.01 each € 1000 )", Group = "Strategy", DefaultValue = 0, MinValue = 0, Step = 0.5)]
        public double BalanceMultiplier { get; set; }

        [Parameter("Max Spread allowed", Group = "Filters", DefaultValue = 1, MinValue = 0.1, Step = 0.1)]
        public double SpreadToTrigger { get; set; }

        [Parameter("Max GAP Allowed (pips)", Group = "Filters", DefaultValue = 3, MinValue = 0, Step = 0.01)]
        public double GAP { get; set; }

        [Parameter("Pause over this time", Group = "Time Zone", DefaultValue = 0, MinValue = 0, MaxValue = 23.59)]
        public double PauseOver { get; set; }

        [Parameter("Pause under this time", Group = "Time Zone", DefaultValue = 0, MinValue = 0, MaxValue = 23.59)]
        public double PauseUnder { get; set; }
        
        public bool IAmInPause
        {

            get
            {

                if (PauseOver == 0 && PauseUnder == 0)
                    return false;

                double now = Server.Time.ToDouble();

                bool intraday = (PauseOver < PauseUnder && now >= PauseOver && now <= PauseUnder);
                bool overnight = (PauseOver > PauseUnder && ((now >= PauseOver && now <= 23.59) || now <= PauseUnder));

                return intraday || overnight;

            }
        }

        [Parameter("Multiplier", Group = "Adaptive Martingale", DefaultValue = 2.1, MinValue = 1)]
        public double Multiplier { get; set; }

        [Parameter("Max Loss Before Multiplier (zero = always)", Group = "Adaptive Martingale", DefaultValue = 0, MinValue = 0)]
        public int MaxLossBeforeMultiplier { get; set; }

        [Parameter("Max Consecutive Loss (zero = unlimited)", Group = "Adaptive Martingale", DefaultValue = 5, MinValue = 0)]
        public int MaxLoss { get; set; }

        [Parameter("Period", Group = "Force Index", DefaultValue = 7, MinValue = 1)]
        public int FIPeriod { get; set; }

        [Parameter("Trigger value", Group = "Force Index", DefaultValue = 8.4, MinValue = 1E-05)]
        public double FITrigger { get; set; }

        #endregion

        #region Property

        int ConsecutiveLoss = 0;

        private ExponentialMovingAverage EMA200;
        private ExponentialMovingAverage EMA500;
        private ForceIndex FI;

        Extensions.Monitor.PauseTimes Pause1;
        Extensions.Monitor Monitor1;

        #endregion

        #region cBot Events

        protected override void OnStart()
        {

            if (StrategyType == Extensions.StrategyType.Auto)
                StrategyType = (StopLoss > TakeProfit) ? Extensions.StrategyType.Aggressive : Extensions.StrategyType.Moderate;

            Pause1 = new Extensions.Monitor.PauseTimes 
            {

                Over = PauseOver,
                Under = PauseUnder

            };
            Monitor1 = new Extensions.Monitor(MyLabel, Symbol, Bars, Positions, Pause1);

            Positions.Closed += OnPositionsClosed;
            Positions.Opened += OnPositionsOpened;

            EMA200 = Indicators.ExponentialMovingAverage(Bars.ClosePrices, 200);
            EMA500 = Indicators.ExponentialMovingAverage(Bars.ClosePrices, 500);
            FI = Indicators.GetIndicator<ForceIndex>("", FIPeriod);

        }

        protected override void OnBar()
        {

            Monitor1.OpenedInThisBar = false;

        }

        protected override void OnTick()
        {

            Monitor1.Update(false, null, null, 0, null);

            if (Monitor1.OpenedInThisBar || Monitor1.Positions.Length > 0 || IAmInPause || !CanCowork(Monitor1))
                return;

            bool sharedFilter = (Monitor1.Symbol.RealSpread() <= SpreadToTrigger && !Monitor1.InGAP(GAP) && !Monitor1.OpenedInThisBar && Monitor1.Positions.Length < 1);

            bool filter1long = StrategyType == Extensions.StrategyType.Aggressive || EMA200.Result.LastValue > EMA500.Result.LastValue;
            bool filter2long = StrategyType == Extensions.StrategyType.Aggressive || Ask > EMA200.Result.LastValue;
            bool filter3long = true;

            bool filter1short = StrategyType == Extensions.StrategyType.Aggressive || EMA200.Result.LastValue < EMA500.Result.LastValue;
            bool filter2short = StrategyType == Extensions.StrategyType.Aggressive || Bid < EMA200.Result.LastValue;
            bool filter3short = true;

            double RealMultiplier = (BalanceMultiplier > 0) ? (int)(Account.Balance / BalanceMultiplier) : 1;
            double RealLots = (RealMultiplier > 1) ? Lots * RealMultiplier : Lots;

            if (FITriggerLong() && sharedFilter && filter1long && filter2long && filter3long)
            {

                ExecuteOrder(Symbol.QuantityToVolumeInUnits(RealLots), TradeType.Buy);
                Monitor1.OpenedInThisBar = true;

            }
            else if (FITriggerShort() && sharedFilter && filter1short && filter2short && filter3short)
            {

                ExecuteOrder(Symbol.QuantityToVolumeInUnits(RealLots), TradeType.Sell);
                Monitor1.OpenedInThisBar = true;

            }

        }

        #endregion

        #region Private Methods

        private void ExecuteOrder(double volume, TradeType tradeType, double? tmpSL = 0, double? tmpTP = 0)
        {

            if (tmpSL == 0)
                tmpSL = StopLoss;
            if (tmpTP == 0)
                tmpTP = TakeProfit;

            var result = ExecuteMarketOrder(tradeType, SymbolName, volume, MyLabel, tmpSL, tmpTP);

            if (result.Error == ErrorCode.NoMoney)
            {

                Print("No Money, close cBot");
                Stop();

            }

        }

        private void OnPositionsClosed(PositionClosedEventArgs args)
        {

            var position = args.Position;

            if (position.Label != MyLabel || position.SymbolName != SymbolName)
                return;

            if (position.NetProfit < 0)
            {

                ConsecutiveLoss++;

                if (MaxLoss == 0 || ConsecutiveLoss < MaxLoss)
                {

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

        private void OnPositionsOpened(PositionOpenedEventArgs eventArgs)
        {

            if (eventArgs.Position.SymbolName == Monitor1.Symbol.Name && eventArgs.Position.Label == Monitor1.Label)
            {

                Monitor1.OpenedInThisBar = true;
                Monitor1.OpenedInThisTrigger = true;

            }

        }

        private bool FITriggerShort()
        {

            return FI.Result.LastValue <= (FITrigger * -1);

        }

        private bool FITriggerLong()
        {

            return FI.Result.LastValue >= FITrigger;

        }

        private bool CanCowork(Extensions.Monitor monitor)
        {

            return (MaxCross == 0 || monitor.Positions.Length > 0) || GetOtherCross().Count < MaxCross;

        }

        private List<string> GetOtherCross()
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

    }

}
