namespace NhaKhoa.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DonThuoc")]
    public partial class DonThuoc
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DonThuoc()
        {
            ChiTietThuoc = new HashSet<ChiTietThuoc>();
        }

        [Key]
        public int Id_donthuoc { get; set; }

        public int? Id_phieudat { get; set; }

        public string Chandoan { get; set; }

        public DateTime? NgayGio { get; set; }

        public int? Soluong { get; set; }

        public double? TongTien { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiTietThuoc> ChiTietThuoc { get; set; }

        public virtual PhieuDatLich PhieuDatLich { get; set; }
    }
}
