using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Mule
{
    /// <summary>
    /// Example data representation of application servers
    /// </summary>
    public class AppHost
    {
        [Key]//Unique id for object
        public string URL { get; set; } = "";
        [Required] //Property cannot be null
        public string Version { get; set; } = "";
        [Required] //Property cannot be null
        public string Owner { get; set; } = "";
        [HiddenInput(DisplayValue = false)] //Property won't show in edit form
        public DateTime Updated { get; private set; } = DateTime.Now;

        public DateTime Touch() => Updated = DateTime.Now;

        /// Equality Comparer (determines if row will be overwritten or created)
        public override bool Equals(object obj) => URL == (obj as AppHost)?.URL;

        /// Unique key definition (use primary key)
        public override int GetHashCode() => URL.GetHashCode();
    }

    /// <summary>
    /// Access to SQLite repository of AppHosts from SQLiteContext
    /// </summary>
    public class AppHostRepository : IRepository<AppHost>
    {
        readonly SQLiteContext _context;
        public AppHostRepository(SQLiteContext context)
        {
            _context = context;
            try
            {
                context.Database.EnsureCreated();
            }
            catch (InvalidOperationException ex)
                when (ex.Message.Contains("The process has no package identity"))
            {
                //Ignore this - sqlite needs signature
            }
        }

        public AppHost Create(AppHost item)
        {
            _context.Add(item);
            _context.SaveChanges();
            return item;
        }

        public IEnumerable<AppHost> Read()
        {
            return _context.AppHosts.AsEnumerable();
        }

        public AppHost Update(AppHost item)
        {
            item.Touch();
            _context.Update(item);
            _context.SaveChanges();
            return item;
        }

        public int Delete(AppHost item)
        {
            _context.Remove(item);
            return _context.SaveChanges();
        }
    }

}