using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerViewWPF.Model
{
    public class Harddisk
    {
        string driveLetter;
        long mbSize;
        public string DriveLetter
        {
            get { return driveLetter; }
            set { driveLetter = value; }
        }
        public long MbSize
        {
            get { return mbSize; }
            set { mbSize = value; }
        }
    }
}
