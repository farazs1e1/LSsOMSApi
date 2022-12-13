using Microsoft.AspNetCore.Http;
using OMSServices.Enum;
using OMSServices.Models;
using OMSServices.Services;
using OMSServices.Utils;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OMSApi.Models
{
    public class Trader : IValidatableObject
    {
        public string ID { get; set; }
        [DisplayName("Username")]
        [Required(ErrorMessage = "Username not provided"), RegularExpression("^[a-zA-Z0-9]{4,100}$", ErrorMessage = "Invalid Username. Username should contain alpha numeric in any order and has a length between 4 to 100.")]
        public string UserName { get; set; }
        [DisplayName("TIF")]
        [Required]
        public List<string> TifList { get; set; }
        [DisplayName("Destination")]
        [Required]
        public List<string> DestinationList { get; set; }
        [DisplayName("commType")]
        [Required]
        public List<string> CommTypeList { get; set; }
        [DisplayName("OrdType")]
        [Required]
        public List<string> OrdTypeList { get; set; }
        [DisplayName("Account")]
        [Required]
        public List<string> AccountList { get; set; }
        [DisplayName("BoothId")]
        public string BoothId { get; set; }
        [DisplayName("FirmId")]
        public string FirmId { get; set; }
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password not provided"), RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{8,15}$", ErrorMessage = "Invalid Password. Password should contain at least one capital letter, one small letter, one special character & length between 8 to 15.")]
        public string Password { get; set; }
        [DisplayName("Profile Servers")]
        public string ProfileServers { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IStaticDataService staticDataService = (IStaticDataService)validationContext.GetService(typeof(IStaticDataService));
            var httpContextAccessor = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));

            var clientId = httpContextAccessor.HttpContext.User.ClientId();
            var userIdentifier = httpContextAccessor.HttpContext.User.UserIdentifier();

            // validate accounts
            foreach (var item in Validate(staticDataService, QueryType.Account, clientId, userIdentifier, AccountList, nameof(AccountList)))
            {
                yield return item;
            }

            // validate tifs
            foreach (var item in Validate(staticDataService, QueryType.TIF, clientId, userIdentifier, TifList, nameof(TifList)))
            {
                yield return item;
            }

            // validate destinations
            foreach (var item in Validate(staticDataService, QueryType.Destination, clientId, userIdentifier, DestinationList, nameof(DestinationList)))
            {
                yield return item;
            }

            // validate commission types
            foreach (var item in Validate(staticDataService, QueryType.CommType, clientId, userIdentifier, CommTypeList, nameof(CommTypeList)))
            {
                yield return item;
            }

            // validate order types
            foreach (var item in Validate(staticDataService, QueryType.OrdType, clientId, userIdentifier, OrdTypeList, nameof(OrdTypeList)))
            {
                yield return item;
            }
        }

        private IEnumerable<ValidationResult> Validate(IStaticDataService staticDataService, QueryType queryType, string clientId, string userIdentifier, List<string> actualValues, string propertyName)
        {
            if (actualValues.Count < 1)
            {
                yield return new ValidationResult("List cannot be empty", new[] { propertyName });
            }
            else
            {
                var expectedValues = staticDataService.GetStaticDataAsync<StaticDataValues>(queryType, null, clientId, userIdentifier).Result;
                foreach (string actualValue in actualValues)
                {
                    bool found = false;
                    foreach (var item in expectedValues.EventData)
                    {
                        if (actualValue.Equals(item.Value))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        yield return new ValidationResult(string.Format("Incorrect value: {0}", actualValue), new[] { propertyName });
                    }
                }
            }
        }
    }
}
