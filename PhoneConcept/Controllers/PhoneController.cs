using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PhoneNumbers;
using PhoneConcept.Models;

namespace PhoneConcept.Controllers
{
    public class PhoneController : Controller
    {
        private static PhoneNumberUtil _phoneUtil;
        private Countries _countries;

        public PhoneController(IHostingEnvironment env)
        {
            _phoneUtil = PhoneNumberUtil.GetInstance();
            string hostingPath = Path.GetDirectoryName(env.ContentRootPath);
            string dataPath = "PhoneConcept\\Data\\iso3166-slim-2.json";
            _countries = new Countries(Path.Combine(hostingPath, dataPath));
        }
        public IActionResult Check()
        {
            var model = new PhoneNumberCheckViewModel()
            {
                CountryCodeSelected = "US",
                Countries = _countries.CountrySelectList
            };
            return View();
        }

        [HttpPost]
        public IActionResult Check(PhoneNumberCheckViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentException(nameof(model));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    PhoneNumber phoneNumber = _phoneUtil.Parse(model.PhoneNumberRaw, model.CountryCodeSelected);
                    ModelState.FirstOrDefault(x => x.Key == nameof(model.Valid)).Value.RawValue =
                        _phoneUtil.IsValidNumberForRegion(phoneNumber, model.CountryCodeSelected);
                    ModelState.FirstOrDefault(x => x.Key == nameof(model.PhoneNumberType)).Value.RawValue =
                        _phoneUtil.GetNumberType(phoneNumber);
                    ModelState.FirstOrDefault(x => x.Key == nameof(model.CountryCode)).Value.RawValue =
                        _phoneUtil.GetRegionCodeForNumber(phoneNumber);
                    ModelState.FirstOrDefault(x => x.Key == nameof(model.PhoneNumberFormatted)).Value.RawValue =
                        _phoneUtil.FormatOutOfCountryCallingNumber(phoneNumber, "US");
                    ModelState.FirstOrDefault(x => x.Key == nameof(model.PhoneNumberMobileDialing)).Value.RawValue =
                        _phoneUtil.FormatNumberForMobileDialing(phoneNumber, model.CountryCodeSelected, true);

                    ModelState.FirstOrDefault(x => x.Key == nameof(model.HasExtension)).Value.RawValue =
                        phoneNumber.HasExtension;

                    ModelState.FirstOrDefault(x => x.Key == nameof(model.CountryCodeSelected)).Value.RawValue =
                        model.CountryCodeSelected;
                    ModelState.FirstOrDefault(x => x.Key == nameof(model.PhoneNumberRaw)).Value.RawValue =
                        model.PhoneNumberRaw;

                    model.Countries = _countries.CountrySelectList;

                    return View(model);
                }
                catch (NumberParseException npex)
                {
                    ModelState.AddModelError(npex.ErrorType.ToString(), npex.Message);
                }
            }

            model.Countries = _countries.CountrySelectList;

            ModelState.SetModelValue(nameof(model.CountryCodeSelected), model.CountryCodeSelected, model.CountryCodeSelected);
            ModelState.SetModelValue(nameof(model.PhoneNumberRaw), model.PhoneNumberRaw, model.PhoneNumberRaw);

            ModelState.SetModelValue(nameof(model.Valid), false, null);
            model.Valid = false;
            ModelState.SetModelValue(nameof(model.HasExtension), false, null);
            model.HasExtension = false;
            ModelState.SetModelValue(nameof(model.PhoneNumberType), null, null);
            model.PhoneNumberType = null;
            ModelState.SetModelValue(nameof(model.CountryCode), null, null);
            model.CountryCode = null;
            ModelState.SetModelValue(nameof(model.PhoneNumberFormatted), null, null);
            model.PhoneNumberFormatted = null;
            ModelState.SetModelValue(nameof(model.PhoneNumberMobileDialing), null, null);
            model.PhoneNumberMobileDialing = null;

            return View(model);
        }
    }
}