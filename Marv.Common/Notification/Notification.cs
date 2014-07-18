﻿using System;

namespace Marv.Common
{
    public interface INotification
    {
        string Description { get; set; }

        bool IsIndeterminate { get; }

        string Name { get; set; }

        double Value { get; }

        void Close();

        void Open();

        event EventHandler Closed;
    }

    public abstract class Notification : Model, INotification
    {
        private double _value = 100;
        private string description;
        private bool isIndeterminate;

        public event EventHandler Closed;

        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                if (value != this.description)
                {
                    this.description = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool IsIndeterminate
        {
            get
            {
                return this.isIndeterminate;
            }

            protected set
            {
                if (value != this.isIndeterminate)
                {
                    this.isIndeterminate = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public double Value
        {
            get
            {
                return this._value;
            }

            protected set
            {
                if (value != this._value)
                {
                    this._value = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public void Close()
        {
            if (this.Closed != null)
            {
                this.Closed(this, new EventArgs());
            }
        }

        public abstract void Open();
    }
}