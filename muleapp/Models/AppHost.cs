using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;

namespace Mule
{
    /// <summary>
    /// Example data representation of application servers
    /// </summary>
    public class AppHost
    {
        [Key]//Unique id for object
        public string URL { get; set; } = "";

        string version = "";
        [Required] //Property cannot be null
        public string Version { get { return version; } set { version = value ?? ""; } }

        [DataType(DataType.MultilineText)] //Input style/formatting hint
        public string Description { get; set; } = "";

        [HiddenInput(DisplayValue = false)] //Property won't show in edit form
        public DateTime Updated { get; private set; } = DateTime.Now;

        public CacheValue<AppHost, long> RTT = new CacheValue<AppHost, long>
        {
            Update = self => new Ping().SendPingAsync(self.URL).Result.RoundtripTime,
        };

        /// Equality Comparer (determines if DB row will be overwritten or created)
        public override bool Equals(object obj) => URL == (obj as AppHost)?.URL;

        /// Unique key definition (for looking up CacheValues)
        public override int GetHashCode() => URL.GetHashCode();
    }
}