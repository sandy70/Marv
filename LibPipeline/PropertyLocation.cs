﻿using System.ComponentModel;

namespace LibPipeline
{
    public class PropertyLocation : Location
    {
        private int id;

        public int Id
        {
            get
            {
                return this.id;
            }

            set
            {
                if (value != this.id)
                {
                    this.id = value;

                    this.OnPropertyChanged("Id");
                }
            }
        }
    }
}