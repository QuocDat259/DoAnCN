using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace NhaKhoa.Models
{
    public partial class NhaKhoaModel : DbContext
    {
        public NhaKhoaModel()
            : base("name=NhaKhoa")
        {
        }

        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<ChiTietThuoc> ChiTietThuoc { get; set; }
        public virtual DbSet<DanhGia> DanhGia { get; set; }
        public virtual DbSet<DanhGiaNhaSi> DanhGiaNhaSi { get; set; }
        public virtual DbSet<DichVu> DichVu { get; set; }
        public virtual DbSet<DonThuoc> DonThuoc { get; set; }
        public virtual DbSet<HinhThucThanhToan> HinhThucThanhToan { get; set; }
        public virtual DbSet<KhungGio> KhungGio { get; set; }
        public virtual DbSet<PhieuDatLich> PhieuDatLich { get; set; }
        public virtual DbSet<Phong> Phong { get; set; }
        public virtual DbSet<TinTuc> TinTuc { get; set; }
        public virtual DbSet<ThoiKhoaBieu> ThoiKhoaBieu { get; set; }
        public virtual DbSet<Thu> Thu { get; set; }
        public virtual DbSet<Thuoc> Thuoc { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRoles>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.DanhGiaNhaSi)
                .WithOptional(e => e.AspNetUsers)
                .HasForeignKey(e => e.Id_Nhasi);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.PhieuDatLich)
                .WithOptional(e => e.AspNetUsers)
                .HasForeignKey(e => e.IdBenhNhan);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.TinTuc)
                .WithOptional(e => e.AspNetUsers)
                .HasForeignKey(e => e.Id_admin);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.ThoiKhoaBieu)
                .WithOptional(e => e.AspNetUsers)
                .HasForeignKey(e => e.Id_Nhasi);

            modelBuilder.Entity<DonThuoc>()
                .HasMany(e => e.ChiTietThuoc)
                .WithRequired(e => e.DonThuoc)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Thuoc>()
                .HasMany(e => e.ChiTietThuoc)
                .WithRequired(e => e.Thuoc)
                .WillCascadeOnDelete(false);
        }
    }
}
