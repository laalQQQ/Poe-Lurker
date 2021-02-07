﻿//-----------------------------------------------------------------------
// <copyright file="StringConverter.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Converters
{
    using System;
    using System.Windows.Data;
    using System.Windows.Markup;

    /// <summary>
    /// CurrencyType converter.
    /// </summary>
    /// <seealso cref="System.Windows.Markup.MarkupExtension" />
    /// <seealso cref="System.Windows.Data.IValueConverter" />
    public class StringConverter : MarkupExtension, IValueConverter
    {
        #region Declarations

        private static StringConverter _instance;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StringConverter"/> class.
        /// </summary>
        public StringConverter()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        /// <returns>
        /// The object value to set on the property where the extension is applied.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new StringConverter());
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var myType = (string)parameter;
            var type = (string)value;
            return myType == type ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <exception cref="System.NotImplementedException">Not implemented.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}