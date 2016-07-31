using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Mule
{
    public class AppHost
    {
        [Key]
        public string URL { get; set; } = "";
        public string Version { get; set; } = "";
        public string Owner { get; set; } = "";
        [HiddenInput(DisplayValue = false)]
        public DateTime Updated { get; private set; } = DateTime.Now;

        private DateTime touch() => Updated = DateTime.Now;

        public override bool Equals(object obj)
        {
            if(obj is AppHost)
            {
                return URL == (obj as AppHost).URL;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return URL.GetHashCode();
        }
    }

    /// <summary>
    /// Access to repository of AppHosts from SQLite SQLiteContext
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