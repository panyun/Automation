﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfInspect.Core;

namespace WpfInspect.ViewModels
{
    public interface IDetailViewModel
    {
        string Key { get; set; }
        string Value { get; set; }
        bool Important { get; set; }
    }

    public class DetailViewModel : ObservableObject, IDetailViewModel
    {
        public DetailViewModel(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get { return GetProperty<string>(); } set { SetProperty(value); } }
        public string Value { get { return GetProperty<string>(); } set { SetProperty(value); } }
        public bool Important { get { return GetProperty<bool>(); } set { SetProperty(value); } }
    }
}
