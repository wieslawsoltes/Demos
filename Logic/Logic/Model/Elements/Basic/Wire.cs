// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Logic.Model.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Model
{
    [DataContract]
    public class Wire : Element
    {
        private double _invertedThickness;
        private double _thickness;
        private Pin _start;
        private Pin _end;
        private bool _invertStart = false;
        private bool _invertEnd = false;
        private bool _disableShortenWire = false;
        private Location _startPoint;
        private Location _endPoint = new Location();
        private Location _startCenter = new Location();
        private Location _endCenter = new Location();

        [DataMember]
        public Pin Start
        {
            get { return _start; }
            set
            {
                if (value != _start)
                {
                    // remove listener for start pin change notifications
                    if (_start != null)
                        _start.PropertyChanged -= PinPropertyChanged;

                    if (_start != null)
                        Children.Remove(_start);

                    _start = value;

                    // add listener for start pin change notifications
                    if (_start != null)
                        _start.PropertyChanged += PinPropertyChanged;

                    if (_start != null)
                        Children.Add(_start);

                    // update inveted start/end location
                    this.CalculateLocation();

                    Notify("Start");
                }
            }
        }

        [DataMember]
        public Pin End
        {
            get { return _end; }
            set
            {
                if (value != _end)
                {
                    // remove listener for end pin change notifications
                    if (_end != null)
                        _end.PropertyChanged -= PinPropertyChanged;

                    if (_end != null)
                        Children.Remove(_end);

                    _end = value;

                    // add listener for end pin change notifications
                    if (_end != null)
                        _end.PropertyChanged += PinPropertyChanged;

                    if (_end != null)
                        Children.Add(_end);

                    // update inveted start/end location
                    this.CalculateLocation();

                    Notify("End");
                }
            }
        }

        [DataMember]
        public bool InvertStart
        {
            get { return _invertStart; }
            set
            {
                if (value != _invertStart)
                {
                    _invertStart = value;

                    // update inveted start/end location
                    this.CalculateLocation();

                    Notify("InvertStart");
                }
            }
        }

        [DataMember]
        public bool DisableShortenWire
        {
            get { return _disableShortenWire; }
            set
            {
                if (value != _disableShortenWire)
                {
                    _disableShortenWire = value;

                    // update inveted start/end location
                    this.CalculateLocation();

                    Notify("DisableShortenWire");
                }
            }
        }

        [DataMember]
        public bool InvertEnd
        {
            get { return _invertEnd; }
            set
            {
                if (value != _invertEnd)
                {
                    _invertEnd = value;

                    // update inveted start/end location
                    this.CalculateLocation();

                    Notify("InvertEnd");
                }
            }
        }

        [IgnoreDataMember]
        public Location StartPoint
        {
            get { return _startPoint; }
            set
            {
                if (value != _startPoint)
                {
                    _startPoint = value;
                    Notify("StartPoint");
                }
            }
        }

        [IgnoreDataMember]
        public Location EndPoint
        {
            get { return _endPoint; }
            set
            {
                if (value != _endPoint)
                {
                    _endPoint = value;
                    Notify("EndPoint");
                }
            }
        }

        [IgnoreDataMember]
        public Location StartCenter
        {
            get { return _startCenter; }
            set
            {
                if (value != _startCenter)
                {
                    _startCenter = value;
                    Notify("StartCenter");
                }
            }
        }

        [IgnoreDataMember]
        public Location EndCenter
        {
            get { return _endCenter; }
            set
            {
                if (value != _endCenter)
                {
                    _endCenter = value;
                    Notify("EndCenter");
                }
            }
        }

        public Wire()
            : base()
        {
            this.Initialize();
        }

        public void CalculateLocation()
        {
            // check for valid stan/end pin
            if (this._start == null || this._end == null)
                return;

            // check if object was initialized
            if (this._startPoint == null || this._endPoint == null || this._startCenter == null || this._endCenter == null)
                return;

            Options options = Defaults.Options;

            if (options != null)
                this._thickness = options.Thickness / 2.0;
            else
                this._thickness = 1.0 / 2.0;

            double startX = this._start.X;
            double startY = this._start.Y;
            double endX = this._end.X;
            double endY = this._end.Y;

            // calculate new inverted start/end position
            double alpha = Math.Atan2(startY - endY, endX - startX);
            double theta = Math.PI - alpha;
            double zet = theta - Math.PI / 2;
            double sizeX = Math.Sin(zet) * (_invertedThickness - _thickness);
            double sizeY = Math.Cos(zet) * (_invertedThickness - _thickness);

            // TODO: shorten wire
            if (options != null && this.DisableShortenWire == false /* && options.CaptureMouse == false */)
            {
                bool isStartSignal = _start.Parent is Signal;
                bool isEndSignal = _end.Parent is Signal;

                // shorten start
                if (isStartSignal == true && isEndSignal == false && options.ShortenLineStarts == true)
                {
                    if (Math.Round(startY, 1) == Math.Round(endY, 1))
                    {
                        startX = _end.X - 15;
                    }
                }

                // shorten end
                if (isStartSignal == false && isEndSignal == true && options.ShortenLineEnds == true)
                {
                    if (Math.Round(startY, 1) == Math.Round(endY, 1))
                    {
                        endX = _start.X + 15;
                    }
                }
            }

            // set wire start location
            if (this._invertStart)
            {
                _startCenter.X = startX + sizeX - _invertedThickness;
                _startCenter.Y = startY - sizeY - _invertedThickness;

                _startPoint.X = startX + (2 * sizeX);
                _startPoint.Y = startY - (2 * sizeY);
            }
            else
            {
                _startCenter.X = startX;
                _startCenter.Y = startY;

                _startPoint.X = startX;
                _startPoint.Y = startY;
            }

            // set line end location
            if (this._invertEnd)
            {
                _endCenter.X = endX - sizeX - _invertedThickness;
                _endCenter.Y = endY + sizeY - _invertedThickness;

                _endPoint.X = endX - (2 * sizeX);
                _endPoint.Y = endY + (2 * sizeY);
            }
            else
            {
                _endCenter.X = endX;
                _endCenter.Y = endY;

                _endPoint.X = endX;
                _endPoint.Y = endY;
            }
        }

        public void Initialize()
        {
            this._startPoint = new Location();
            this._endPoint = new Location();
            this._startCenter = new Location();
            this._endCenter = new Location();

            this._invertedThickness = 10.0 / 2.0;

            Options options = Defaults.Options;

            if (options != null)
            {
                this._thickness = options.Thickness / 2.0;

                options.PropertyChanged += Options_PropertyChanged;
            }
            else
            {
                this._thickness = 1.0 / 2.0;
            }

            this.CalculateLocation();
        }

        private void Options_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // TODO: 
            if (e.PropertyName == "Thickness" || e.PropertyName == "ShortenLineStarts" || e.PropertyName == "ShortenLineEnds" || e.PropertyName == "CaptureMouse")
            {
                this.CalculateLocation();
            }
        }

        [OnDeserialized]
        private void Initialize(StreamingContext s)
        {
            this.Initialize();
        }

        private void PinPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "X" || e.PropertyName == "Y")
            {
                this.CalculateLocation();
            }
        }

        public override object Clone()
        {
            Wire element = (Wire)this.MemberwiseClone();
            element.Children = Element.CopyObservableCollection<Element>(this.Children);
            element.Id = Guid.NewGuid().ToString();
            return element;
        }
    }
}
