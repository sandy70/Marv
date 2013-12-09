﻿namespace Marv
{
    public abstract class Command<T>
    {
        public Command(T associatedObject)
        {
            this.AssociatedObject = associatedObject;
        }

        protected T AssociatedObject { get; set; }

        public abstract void Execute();
    }
}