using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BGBC.Core
{
    public sealed class ModelDataValidation
    {
        private static ModelDataValidation instance = null;
        private static readonly object padlock = new object();
        private readonly string _alphanumeric = @"^[ A-Za-z0-9_@./#&+-]*$";
        private readonly string _email = @"^[A-Za-z0-9._-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,6}";
        private readonly string _alpha = @"^[a-zA-Z]+[ a-zA-Z-_]*$";
        private readonly string _zip = @"^\d{5}(-\d{4})?$";

        ModelDataValidation()
        {

        }

        public static ModelDataValidation Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new ModelDataValidation();
                    }
                    return instance;
                }
            }
        }

        /// <summary>
        /// To validation alpha numeric values
        /// </summary>
        /// <param name="ModelState">Model state of action method</param>
        /// <param name="ModelValue">Model property value</param>
        /// <param name="Mandatory">For mandatory values</param>
        /// <param name="Mandatory">Model property display name</param>
        /// <param name="Mandatory">Model property name</param>
        public void AlphaNumeric(ModelStateDictionary ModelState, string ModelValue, bool Mandatory, string DisplayName, string PropertyName)
        {
            if (string.IsNullOrEmpty(ModelValue))
            {
                if (Mandatory) ModelState.AddModelError(PropertyName, string.Format("The {0} field is required.", DisplayName));
            }
            else
            {
                Regex re = new Regex(_alphanumeric);
                if (!re.IsMatch(ModelValue))
                {
                    ModelState.AddModelError(PropertyName, "Use alphanumeric characters only");
                }
            }
        }

        /// <summary>
        /// To validation alpha values
        /// </summary>
        /// <param name="ModelState">Model state of action method</param>
        /// <param name="ModelValue">Model property value</param>
        /// <param name="Mandatory">For mandatory values</param>
        /// <param name="Mandatory">Model property display name</param>
        /// <param name="Mandatory">Model property name</param>
        public void Alpha(ModelStateDictionary ModelState, string ModelValue, bool Mandatory, string DisplayName, string PropertyName)
        {
            if (string.IsNullOrEmpty(ModelValue))
            {
                if (Mandatory) ModelState.AddModelError(PropertyName, string.Format("The {0} field is required.", DisplayName));
            }
            else
            {
                Regex re = new Regex(_alpha);
                if (!re.IsMatch(ModelValue))
                {
                    ModelState.AddModelError(PropertyName, "Use alpha characters only");
                }
            }
        }

        /// <summary>
        /// To validation Email values
        /// </summary>
        /// <param name="ModelState">Model state of action method</param>
        /// <param name="ModelValue">Model property value</param>
        /// <param name="Mandatory">For mandatory values</param>
        /// <param name="Mandatory">Model property display name</param>
        /// <param name="Mandatory">Model property name</param>
        public void Email(ModelStateDictionary ModelState, string ModelValue, bool Mandatory, string DisplayName, string PropertyName)
        {
            if (string.IsNullOrEmpty(ModelValue))
            {
                if (Mandatory) ModelState.AddModelError(PropertyName, string.Format("The {0} field is required.", DisplayName));
            }
            else
            {
                Regex re = new Regex(_email);
                if (!re.IsMatch(ModelValue))
                {
                    ModelState.AddModelError(PropertyName, "Please Enter Correct Email Address");
                }
            }
        }

        /// <summary>
        /// To validation Zip values
        /// </summary>
        /// <param name="ModelState">Model state of action method</param>
        /// <param name="ModelValue">Model property value</param>
        /// <param name="Mandatory">For mandatory values</param>
        /// <param name="Mandatory">Model property display name</param>
        /// <param name="Mandatory">Model property name</param>
        public void Zip(ModelStateDictionary ModelState, string ModelValue, bool Mandatory, string DisplayName, string PropertyName)
        {
            if (string.IsNullOrEmpty(ModelValue))
            {
                if (Mandatory) ModelState.AddModelError(PropertyName, string.Format("The {0} field is required.", DisplayName));
            }
            else
            {
                Regex re = new Regex(_zip);
                if (!re.IsMatch(ModelValue))
                {
                    ModelState.AddModelError(PropertyName, "Please Enter 5 digits only");
                }
            }
        }
    }
}
