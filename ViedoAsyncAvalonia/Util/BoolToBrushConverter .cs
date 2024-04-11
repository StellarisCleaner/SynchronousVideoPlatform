﻿using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViedoAsyncAvalonia.Util
{
    internal class BoolToBrushConverter:IValueConverter
    {
        public IBrush TrueBrush { get; set; }
        public IBrush FalseBrush { get; set; }

        public BoolToBrushConverter()
        {
            // 默认颜色
            TrueBrush = Brushes.Green;
            FalseBrush = Brushes.Gray;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value)
            {
                return TrueBrush;
            }
            return FalseBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
