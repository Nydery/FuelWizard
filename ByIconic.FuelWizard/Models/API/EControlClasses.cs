using System;
using System.Collections.Generic;
using System.Text;

namespace ByIconic.FuelWizard.Models.API
{
    class EControlClasses
    {
        public class GasStationPublic
        {
            public int id { get; set; }
            public string name { get; set; }
            public Location location { get; set; }
            public Contact contact { get; set; }
            public Openinghour[] openingHours { get; set; }
            public Offerinformation offerInformation { get; set; }
            public Paymentmethods paymentMethods { get; set; }
            public Paymentarrangements paymentArrangements { get; set; }
            public int position { get; set; }
            public bool open { get; set; }
            public float distance { get; set; }
            public Price[] prices { get; set; }
        }

        public class Location
        {
            public string address { get; set; }
            public string postalCode { get; set; }
            public string city { get; set; }
            public float latitude { get; set; }
            public float longitude { get; set; }
        }

        public class Contact
        {
            public string telephone { get; set; }
            public string fax { get; set; }
            public string mail { get; set; }
        }

        public class Offerinformation
        {
            public bool service { get; set; }
            public bool selfService { get; set; }
            public bool unattended { get; set; }
        }

        public class Paymentmethods
        {
            public bool cash { get; set; }
            public bool debitCard { get; set; }
            public bool creditCard { get; set; }
            public string others { get; set; }
        }

        public class Paymentarrangements
        {
            public bool cooperative { get; set; }
            public bool clubCard { get; set; }
            public string accessMod { get; set; }
        }

        public class Openinghour
        {
            public string day { get; set; }
            public string label { get; set; }
            public int order { get; set; }
            public string from { get; set; }
            public string to { get; set; }
        }

        public class Price
        {
            public string fuelType { get; set; }
            public float amount { get; set; }
            public string label { get; set; }
        }

    }
}
