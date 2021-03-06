﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Caliburn.Micro;
using Frontend.Core.Helpers;
using Frontend.Core.Model.PreferenceEntries.Interfaces;
using Frontend.Core.Model.Preferences.Interfaces;

namespace Frontend.Core.Model.Preferences
{
    /// <summary>
    ///     Implementation of IPreference.
    ///     <remarks>
    ///         Also implements IDataErrorInfo, which is used for validating some user input in user interface, such as
    ///         minimum and maximum values.
    ///     </remarks>
    /// </summary>
    public class Preference<T> : PropertyChangedBase, IPreference, IDataErrorInfo
    {
        /// <summary>
        /// </summary>
        private IList<IPreferenceEntry> entries;

        /// <summary>
        /// </summary>
        private IPreferenceEntry selectedEntry;

        private T value;

        /// <summary>
        ///     Gets or sets the minimum value.
        /// </summary>
        /// <value>
        ///     The minimum value.
        /// </value>
        public T MinValue { get; set; }

        /// <summary>
        ///     Gets or sets the maximum value.
        /// </summary>
        /// <value>
        ///     The maximum value.
        /// </value>
        public T MaxValue { get; set; }

        /// <summary>
        ///     Gets or sets the current value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        public T Value
        {
            get { return value; }

            set
            {
                if (NullableComparer.AreEqual(this.value, value))
                {
                    return;
                }

                this.value = value;
                NotifyOfPropertyChange(() => Value);
            }
        }

        /// <summary>
        ///     Gets or sets the preference name. Must match the name of the preference in configuration.txt
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the friendly preference name. Usually more readable to humans than the normal name, which tends to
        ///     lack - for instance - spaces.
        /// </summary>
        /// <value>
        ///     The name of the friendly.
        /// </value>
        public string FriendlyName { get; set; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>
        ///     The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this preference has a directly editable value (as opposed to pre-defined
        ///     choices, for instance)
        /// </summary>
        /// <value>
        ///     <c>true</c> if yes; otherwise, <c>false</c>.
        /// </value>
        public bool HasDirectlyEditableValue { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this preference should be output with curly brackets instead of quotes
        /// </summary>
        /// <value>
        ///     <c>true</c> if yes; otherwise, <c>false</c>.
        /// </value>
        public bool useCurlyBraces { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this preference has a set of predefined choices.
        /// </summary>
        /// <value>
        ///     <c>true</c> if yes; otherwise, <c>false</c>.
        /// </value>
        public bool HasPreDefinedChoices
        {
            get { return Entries.Count > 0; }
        }

        /// <summary>
        ///     Gets the list of pre-defined IPreferenceEntry objects. These are the pre-defined user choices, if not null.
        /// </summary>
        /// <value>
        ///     The entries.
        /// </value>
        public IList<IPreferenceEntry> Entries
        {
            get { return entries ?? (entries = new List<IPreferenceEntry>()); }
        }

        /// <summary>
        ///     Gets or sets the selected entry. Only relevant if this list has a list of Entries to choose from.
        /// </summary>
        /// <value>
        ///     The selected entry.
        /// </value>
        public IPreferenceEntry SelectedEntry
        {
            get { return selectedEntry; }

            set
            {
                if (selectedEntry == value)
                {
                    return;
                }

                selectedEntry = value;
                OnSelectedEntrySet(value); //.Name;
                NotifyOfPropertyChange(() => SelectedEntry);
            }
        }

        #region [ OnSelectedEntrySet handling ]

        /// <summary>
        ///     Handler logic for when a new selected entry is set.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private void OnSelectedEntrySet(IPreferenceEntry entry)
        {
            if (this is IDatePreference)
            {
                ((IDatePreference) this).Value = ((IDatePreferenceEntry) entry).Name;
            }
            else if (this is INumericPreference)
            {
                ((INumericPreference) this).Value = ((INumericPreferenceEntry) entry).Name;
            }
            else if (this is IStringPreference)
            {
                ((IStringPreference) this).Value = ((IStringPreferenceEntry) entry).Name;
            }

            //return (T)entry;
        }

        #endregion

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("# " + FriendlyName + ": " + Description);

            AppendEntries(sb);

            if (useCurlyBraces)
            {
                sb.AppendLine("\t" + Name + " = {" + Value + "}");
            }
            else
            {
                sb.AppendLine("\t" + Name + " = \"" + Value + "\"");
            }

            return sb.ToString();
        }

        protected void AppendEntries(StringBuilder sb)
        {
            if (HasPreDefinedChoices)
            {
                foreach (var entry in Entries)
                {
                    sb.AppendLine("\t#\t" + entry.FriendlyName + "\t - " + entry.Description);
                }
            }
        }

        #region [ Validation ]

        /// <summary>
        ///     Gets an error message indicating what is wrong with this object.
        /// </summary>
        /// <returns>An error message indicating what is wrong with this object. The default is an empty string ("").</returns>
        public string Error
        {
            get { return null; }
        }

        /// <summary>
        ///     Gets the <see cref="System.String" /> with the specified name. Used by IDataErrorInfo
        /// </summary>
        /// <value>
        ///     The <see cref="System.String" />.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public string this[string name]
        {
            get
            {
                var result = OnValidateProperty(name);

                return result;
            }
        }

        /// <summary>
        ///     Called when [validate property].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected virtual string OnValidateProperty(string propertyName)
        {
            return null;
        }

        #endregion
    }
}