using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NhaKhoa.Models
{
    public class ThoiKhoaBieuViewModel
    {
        public List<Thu> DanhSachThu { get; set; }
        public List<ThoiKhoaBieu> DanhSachThoiKhoaBieu { get; set; }
        public DateTime[] weeks { get; set; }
        public DateTime SelectedWeek { get; set; }
        //public int SelectedYear { get; set; }
        //public int[] Years { get; set; }

        public List<ThoiKhoaBieu> WorkSchedules { get; set; }

    }
}