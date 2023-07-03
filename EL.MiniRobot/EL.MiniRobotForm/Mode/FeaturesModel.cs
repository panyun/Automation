using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using EL;
using EL.Robot.Component;
using EL.Robot.Core;
using Mysqlx.Crud;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace MiniRobotForm.Mode
{
    [AutoMap(typeof(Feature))]
    public class FeatureModel : ObservableObject
    {
        private long _id;
        public long Id { get => _id; set => SetProperty(ref _id, value); }
        private string _name;
        public string Name { get => _name; set => SetProperty(ref _name, value); }
        private string _headImg;
        public string HeadImg { get => _headImg; set => SetProperty(ref _headImg, value); }
        private long _sort;
        public long ViewSort { get => _sort; set => SetProperty(ref _sort, value); }
        private long _createDate;
        public long CreateDate { get => _createDate; set => SetProperty(ref _createDate, value); }
        private string _note;
        public string Note { get => _note; set => SetProperty(ref _note, value); }
    }
     
    public static class ModelConvert
    {
        private static MapperConfiguration config;
        static ModelConvert()
        {
            config = new MapperConfiguration(cfg =>
            {
                // 扫描当前程序集
                cfg.AddMaps(System.AppDomain.CurrentDomain.GetAssemblies());
            });
        }
        public static T2 To<T1, T2>(T1 t) where T2 : new() where T1 : new()
        {
            var mapper = config.CreateMapper();
            return mapper.Map<T2>(t);
           
        }
        public static ObservableCollection<T2> TosUI<T1, T2>(List<T1> t1s) where T2 : new() where T1 : new()
        {
            ObservableCollection<T2> list = new();
            foreach (var item in t1s)
            {
                var m = To<T1, T2>(item);
                list.Add(m);
            }
            return list;
        }
        public static List<T2> Tos<T1, T2>(List<T1> t1s) where T2 : new() where T1 : new()
        {
            List<T2> list = new();
            foreach (var item in t1s)
            {
                var m = To<T1, T2>(item);
                list.Add(m);
            }
            return list;
        }
    }
}
