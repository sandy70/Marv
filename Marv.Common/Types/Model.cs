﻿using System.ComponentModel;

namespace Marv.Common
{
    public interface IModel : INotifyPropertyChanged
    {
        bool IsEnabled { get; set; }

        bool IsSelected { get; set; }

        string Key { get; set; }

        string Name { get; set; }
    }

    public class Model : IModel
    {
        private bool isEnabled;
        private bool isSelected;
        private string key = "";
        private string name = "";

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }

            set
            {
                if (value != this.isEnabled)
                {
                    this.isEnabled = value;
                    this.RaisePropertyChanged("IsEnabled");
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                if (value != this.isSelected)
                {
                    this.isSelected = value;
                    this.RaisePropertyChanged("IsSelected");
                }
            }
        }

        public string Key
        {
            get
            {
                return this.key;
            }

            set
            {
                if (value != this.key)
                {
                    this.key = value;
                    this.RaisePropertyChanged("Key");
                }
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (value != this.name)
                {
                    this.name = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}