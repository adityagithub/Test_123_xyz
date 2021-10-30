using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace Community.Api.Extensions
{
    public static class CommunityModelStateExtensions
    {
        private static string SerializeModelState(ModelStateEntry modelStateEntry)
        {
            List<string> errors = new List<string>();
            for (int i = 0; i < modelStateEntry.Errors.Count; i++)
            {
                ModelError modelError = modelStateEntry.Errors[i];
                string errorText = modelError.ErrorMessage;

                if (!string.IsNullOrEmpty(errorText))
                    errors.Add(errorText);
            }

            return string.Join(',', errors);
        }

        /// <summary>
        /// Serialize errors
        /// </summary>
        /// <param name="modelStateDictionary">ModelStateDictionary</param>
        /// <returns>Result</returns>
        public static List<string> SerializeErrors(this ModelStateDictionary modelStateDictionary)
        {
            var messageDictionary = modelStateDictionary.Where(entry => entry.Value.Errors.Any())
                .ToDictionary(entry => entry.Key, entry => SerializeModelState(entry.Value));

            return messageDictionary.Values.ToList();
        }
    }
}
