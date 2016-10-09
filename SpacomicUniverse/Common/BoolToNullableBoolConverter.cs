using System;
using Windows.UI.Xaml.Data;

namespace SpacomicUniverse {

	public sealed class BoolToNullableBoolConverter : IValueConverter {
		public object Convert( object value, Type targetType, object parameter, string language ) =>
			value is bool && ( bool )value;

		public object ConvertBack( object value, Type targetType, object parameter, string language ) =>
			value is bool? ? ( bool? )value ?? false : false;
	}

	public sealed class NullableBoolToBoolConverter : IValueConverter {
		public object Convert( object value, Type targetType, object parameter, string language ) =>
			value is bool? ? ( bool? )value ?? false : false;

		public object ConvertBack( object value, Type targetType, object parameter, string language ) =>
			value is bool && ( bool )value;
	}
}
