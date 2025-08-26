using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Business.Interfaces;

namespace YastCleaner.Business.Services
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime DateTimeActual() => DateTime.Now;
    }
}
