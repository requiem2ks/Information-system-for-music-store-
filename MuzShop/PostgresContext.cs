using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;

namespace MuzShop;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ProductFDto> ProductsByCategoryResults { get; set; }
    public virtual DbSet<OrderHistoryDto> OrderHistoryResults { get; set; }
    public virtual DbSet<SalesReportDto> SalesReportResults { get; set; }


    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Courier> Couriers { get; set; }

    public virtual DbSet<Delivery> Deliveries { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<Instrumentsetup> Instrumentsetups { get; set; }

    public virtual DbSet<Jobtitle> Jobtitles { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Orderline> Orderlines { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Paymentmethod> Paymentmethods { get; set; }

    public virtual DbSet<Personaldiscount> Personaldiscounts { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Productcategory> Productcategories { get; set; }

    public virtual DbSet<Productreservation> Productreservations { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Promotionproduct> Promotionproducts { get; set; }

    public virtual DbSet<Provider> Providers { get; set; }

    public virtual DbSet<Purchase> Purchases { get; set; }

    public virtual DbSet<Receiptsatthestorage> Receiptsatthestorages { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<Shoppingcart> Shoppingcarts { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Storage> Storages { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5777;Database=postgres;Username=postgres;Password=123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgres_fdw");

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Rolename)
                .HasMaxLength(50)
                .HasColumnName("rolename");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Clientid).HasColumnName("clientid");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Employeeid).HasColumnName("employeeid");
            entity.Property(e => e.Hashpassword)
                .HasMaxLength(255)
                .HasColumnName("hashpassword");
            entity.Property(e => e.Roleid).HasColumnName("roleid");

            entity.HasOne(d => d.Client).WithMany(p => p.Users)
                .HasForeignKey(d => d.Clientid)
                .HasConstraintName("users_clientid_fkey");

            entity.HasOne(d => d.Employee).WithMany(p => p.Users)
                .HasForeignKey(d => d.Employeeid)
                .HasConstraintName("users_employeeid_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.Roleid)
                .HasConstraintName("users_roleid_fkey");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Clientid).HasName("clients_pkey");

            entity.ToTable("clients");

            entity.Property(e => e.Clientid).HasColumnName("clientid");
            entity.Property(e => e.Address)
                .HasMaxLength(50)
                .HasColumnName("address");
            entity.Property(e => e.Fio)
                .HasMaxLength(50)
                .HasColumnName("fio");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<Courier>(entity =>
        {
            entity.HasKey(e => e.Courierid).HasName("couriers_pkey");

            entity.ToTable("couriers");

            entity.Property(e => e.Courierid).HasColumnName("courierid");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Fio)
                .HasMaxLength(50)
                .HasColumnName("fio");
            entity.Property(e => e.Liftingcapacity)
                .HasMaxLength(20)
                .HasColumnName("liftingcapacity");
            entity.Property(e => e.Maximumsize)
                .HasMaxLength(30)
                .HasColumnName("maximumsize");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
            entity.Property(e => e.Typeoftransport)
                .HasMaxLength(50)
                .HasColumnName("typeoftransport");
        });

        modelBuilder.Entity<Delivery>(entity =>
        {
            entity.HasKey(e => e.Deliveryid).HasName("deliveries_pkey");

            entity.ToTable("deliveries");

            entity.Property(e => e.Deliveryid).HasColumnName("deliveryid");
            entity.Property(e => e.Courierid).HasColumnName("courierid");
            entity.Property(e => e.Dateofdelivery).HasColumnName("dateofdelivery");
            entity.Property(e => e.Deliveryaddress)
                .HasMaxLength(50)
                .HasColumnName("deliveryaddress");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Statusofdelivery).HasColumnName("statusofdelivery");

            entity.HasOne(d => d.Courier).WithMany(p => p.Deliveries)
                .HasForeignKey(d => d.Courierid)
                .HasConstraintName("deliveries_courierid_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.Deliveries)
                .HasForeignKey(d => d.Orderid)
                .HasConstraintName("deliveries_orderid_fkey");

            entity.HasOne(d => d.StatusofdeliveryNavigation).WithMany(p => p.Deliveries)
                .HasForeignKey(d => d.Statusofdelivery)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("deliveries_statusofdelivery_fkey");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Employeeid).HasName("employees_pkey");

            entity.ToTable("employees");

            entity.Property(e => e.Employeeid).HasColumnName("employeeid");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Fio)
                .HasMaxLength(50)
                .HasColumnName("fio");
            entity.Property(e => e.Jobtitle).HasColumnName("jobtitle");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");

            entity.HasOne(d => d.JobtitleNavigation).WithMany(p => p.Employees)
                .HasForeignKey(d => d.Jobtitle)
                .HasConstraintName("employees_jobtitle_fkey");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.Gradeid).HasName("grades_pkey");

            entity.ToTable("grades");

            entity.Property(e => e.Gradeid).HasColumnName("gradeid");
            entity.Property(e => e.Valuegrade).HasColumnName("valuegrade");
        });

        modelBuilder.Entity<Instrumentsetup>(entity =>
        {
            entity.HasKey(e => e.Settingid).HasName("instrumentsetup_pkey");

            entity.ToTable("instrumentsetup");

            entity.Property(e => e.Settingid).HasColumnName("settingid");
            entity.Property(e => e.Descriptionsetting)
                .HasMaxLength(300)
                .HasColumnName("descriptionsetting");
            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Settingtype)
                .HasMaxLength(50)
                .HasColumnName("settingtype");
            entity.Property(e => e.Setupcost)
                .HasPrecision(15, 2)
                .HasColumnName("setupcost");

            entity.HasOne(d => d.Product).WithMany(p => p.Instrumentsetups)
                .HasForeignKey(d => d.Productid)
                .HasConstraintName("instrumentsetup_productid_fkey");
        });

        modelBuilder.Entity<Jobtitle>(entity =>
        {
            entity.HasKey(e => e.Jobtitleid).HasName("jobtitles_pkey");

            entity.ToTable("jobtitles");

            entity.Property(e => e.Jobtitleid).HasColumnName("jobtitleid");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Orderid).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Clientid).HasColumnName("clientid");
            entity.Property(e => e.Employeeid).HasColumnName("employeeid");
            entity.Property(e => e.Orderdate).HasColumnName("orderdate");
            entity.Property(e => e.Orderenddate).HasColumnName("orderenddate");
            entity.Property(e => e.Paymentid).HasColumnName("paymentid");
            entity.Property(e => e.Statusoforder).HasColumnName("statusoforder");

            entity.HasOne(d => d.Client).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Clientid)
                .HasConstraintName("orders_clientid_fkey");

            entity.HasOne(d => d.Employee).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Employeeid)
                .HasConstraintName("orders_employeeid_fkey");

            entity.HasOne(d => d.Payment).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Paymentid)
                .HasConstraintName("orders_paymentid_fkey");

            entity.HasOne(d => d.StatusoforderNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Statusoforder)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("orders_statusoforder_fkey");
        });

        modelBuilder.Entity<Orderline>(entity =>
        {
            entity.HasKey(e => e.Orderlineid).HasName("orderlines_pkey");

            entity.ToTable("orderlines");

            entity.Property(e => e.Orderlineid).HasColumnName("orderlineid");
            entity.Property(e => e.Discount).HasColumnName("discount");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Settingid).HasColumnName("settingid");
            entity.Property(e => e.Unitprice)
                .HasPrecision(15, 2)
                .HasColumnName("unitprice");

            entity.HasOne(d => d.DiscountNavigation).WithMany(p => p.Orderlines)
                .HasForeignKey(d => d.Discount)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("orderlines_discount_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.Orderlines)
                .HasForeignKey(d => d.Orderid)
                .HasConstraintName("orderlines_orderid_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.Orderlines)
                .HasForeignKey(d => d.Productid)
                .HasConstraintName("orderlines_productid_fkey");

            entity.HasOne(d => d.Setting).WithMany(p => p.Orderlines)
                .HasForeignKey(d => d.Settingid)
                .HasConstraintName("orderlines_settingid_fkey");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Paymentid).HasName("payments_pkey");

            entity.ToTable("payments");

            entity.Property(e => e.Paymentid).HasColumnName("paymentid");
            entity.Property(e => e.Dateofpayment).HasColumnName("dateofpayment");
            entity.Property(e => e.Paymentmethod).HasColumnName("paymentmethod");
            entity.Property(e => e.Paymentsum)
                .HasPrecision(15, 2)
                .HasColumnName("paymentsum");

            entity.HasOne(d => d.PaymentmethodNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.Paymentmethod)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("payments_paymentmethod_fkey");
        });

        modelBuilder.Entity<Paymentmethod>(entity =>
        {
            entity.HasKey(e => e.Paymentmethodid).HasName("paymentmethods_pkey");

            entity.ToTable("paymentmethods");

            entity.Property(e => e.Paymentmethodid).HasColumnName("paymentmethodid");
            entity.Property(e => e.Paymentmethodname)
                .HasMaxLength(30)
                .HasColumnName("paymentmethodname");
        });

        modelBuilder.Entity<Personaldiscount>(entity =>
        {
            entity.HasKey(e => e.Personaldiscountid).HasName("personaldiscounts_pkey");

            entity.ToTable("personaldiscounts");

            entity.Property(e => e.Personaldiscountid).HasColumnName("personaldiscountid");
            entity.Property(e => e.Clientid).HasColumnName("clientid");
            entity.Property(e => e.Discountvalue)
                .HasPrecision(15, 2)
                .HasColumnName("discountvalue");
            entity.Property(e => e.Enddate).HasColumnName("enddate");
            entity.Property(e => e.Startdate).HasColumnName("startdate");

            entity.HasOne(d => d.Client).WithMany(p => p.Personaldiscounts)
                .HasForeignKey(d => d.Clientid)
                .HasConstraintName("personaldiscounts_clientid_fkey");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Productid).HasName("products_pkey");

            entity.ToTable("products");

            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .HasColumnName("description");
            entity.Property(e => e.Price)
                .HasPrecision(15, 2)
                .HasColumnName("price");
            entity.Property(e => e.Productcategoryid).HasColumnName("productcategoryid");
            entity.Property(e => e.Productname)
                .HasMaxLength(50)
                .HasColumnName("productname");

            entity.HasOne(d => d.Productcategory).WithMany(p => p.Products)
                .HasForeignKey(d => d.Productcategoryid)
                .HasConstraintName("products_productcategoryid_fkey");
        });

        modelBuilder.Entity<Productcategory>(entity =>
        {
            entity.HasKey(e => e.Productcategoryid).HasName("productcategories_pkey");

            entity.ToTable("productcategories");

            entity.Property(e => e.Productcategoryid).HasColumnName("productcategoryid");
            entity.Property(e => e.Descriptioncategory)
                .HasMaxLength(300)
                .HasColumnName("descriptioncategory");
            entity.Property(e => e.Namecategory)
                .HasMaxLength(40)
                .HasColumnName("namecategory");
        });

        modelBuilder.Entity<Productreservation>(entity =>
        {
            entity.HasKey(e => e.Productreservationid).HasName("productreservation_pkey");

            entity.ToTable("productreservation");

            entity.Property(e => e.Productreservationid).HasColumnName("productreservationid");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Prepaymentpercentage).HasColumnName("prepaymentpercentage");
            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Quantityofreserved).HasColumnName("quantityofreserved");
            entity.Property(e => e.Reservationid).HasColumnName("reservationid");
            entity.Property(e => e.Unitpricereservation)
                .HasPrecision(15, 2)
                .HasColumnName("unitpricereservation");

            entity.HasOne(d => d.Order).WithMany(p => p.Productreservations)
                .HasForeignKey(d => d.Orderid)
                .HasConstraintName("productreservation_orderid_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.Productreservations)
                .HasForeignKey(d => d.Productid)
                .HasConstraintName("productreservation_productid_fkey");

            entity.HasOne(d => d.Reservation).WithMany(p => p.Productreservations)
                .HasForeignKey(d => d.Reservationid)
                .HasConstraintName("productreservation_reservationid_fkey");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.Stockid).HasName("promotions_pkey");

            entity.ToTable("promotions");

            entity.Property(e => e.Stockid).HasColumnName("stockid");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .HasColumnName("description");
            entity.Property(e => e.Discountvalue)
                .HasPrecision(15, 2)
                .HasColumnName("discountvalue");
            entity.Property(e => e.Enddate).HasColumnName("enddate");
            entity.Property(e => e.Namestock)
                .HasMaxLength(50)
                .HasColumnName("namestock");
            entity.Property(e => e.Startdate).HasColumnName("startdate");
            entity.Property(e => e.Typeofstock)
                .HasMaxLength(50)
                .HasColumnName("typeofstock");
        });

        modelBuilder.Entity<Promotionproduct>(entity =>
        {
            entity.HasKey(e => e.Promotionproductsid).HasName("promotionproducts_pkey");

            entity.ToTable("promotionproducts");

            entity.Property(e => e.Promotionproductsid).HasColumnName("promotionproductsid");
            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Stockid).HasColumnName("stockid");

            entity.HasOne(d => d.Product).WithMany(p => p.Promotionproducts)
                .HasForeignKey(d => d.Productid)
                .HasConstraintName("promotionproducts_productid_fkey");

            entity.HasOne(d => d.Stock).WithMany(p => p.Promotionproducts)
                .HasForeignKey(d => d.Stockid)
                .HasConstraintName("promotionproducts_stockid_fkey");
        });

        modelBuilder.Entity<Provider>(entity =>
        {
            entity.HasKey(e => e.Providerid).HasName("providers_pkey");

            entity.ToTable("providers");

            entity.Property(e => e.Providerid).HasColumnName("providerid");
            entity.Property(e => e.Address)
                .HasMaxLength(50)
                .HasColumnName("address");
            entity.Property(e => e.Nameprovider)
                .HasMaxLength(50)
                .HasColumnName("nameprovider");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(e => e.Purchaseid).HasName("purchases_pkey");

            entity.ToTable("purchases");

            entity.Property(e => e.Purchaseid).HasColumnName("purchaseid");
            entity.Property(e => e.Datepurchase).HasColumnName("datepurchase");
            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Providerid).HasColumnName("providerid");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Unitprice)
                .HasPrecision(10, 2)
                .HasColumnName("unitprice");

            entity.HasOne(d => d.Product).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.Productid)
                .HasConstraintName("purchases_productid_fkey");

            entity.HasOne(d => d.Provider).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.Providerid)
                .HasConstraintName("purchases_providerid_fkey");
        });

        modelBuilder.Entity<Receiptsatthestorage>(entity =>
        {
            entity.HasKey(e => e.Receiptsatthestoragesid).HasName("receiptsatthestorages_pkey");

            entity.ToTable("receiptsatthestorages");

            entity.Property(e => e.Receiptsatthestoragesid).HasColumnName("receiptsatthestoragesid");
            entity.Property(e => e.Dateofreceipt).HasColumnName("dateofreceipt");
            entity.Property(e => e.Purchaseid).HasColumnName("purchaseid");
            entity.Property(e => e.Storageid).HasColumnName("storageid");

            entity.HasOne(d => d.Purchase).WithMany(p => p.Receiptsatthestorages)
                .HasForeignKey(d => d.Purchaseid)
                .HasConstraintName("receiptsatthestorages_purchaseid_fkey");

            entity.HasOne(d => d.Storage).WithMany(p => p.Receiptsatthestorages)
                .HasForeignKey(d => d.Storageid)
                .HasConstraintName("receiptsatthestorages_storageid_fkey");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Reservationid).HasName("reservations_pkey");

            entity.ToTable("reservations");

            entity.Property(e => e.Reservationid).HasColumnName("reservationid");
            entity.Property(e => e.Reservationdate).HasColumnName("reservationdate");
            entity.Property(e => e.Reservationenddate).HasColumnName("reservationenddate");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Reviewid).HasName("reviews_pkey");

            entity.ToTable("reviews");

            entity.Property(e => e.Reviewid).HasColumnName("reviewid");
            entity.Property(e => e.Clientid).HasColumnName("clientid");
            entity.Property(e => e.Dateofrevocation).HasColumnName("dateofrevocation");
            entity.Property(e => e.Grade).HasColumnName("grade");
            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Rewievtext)
                .HasMaxLength(300)
                .HasColumnName("rewievtext");

            entity.HasOne(d => d.Client).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.Clientid)
                .HasConstraintName("reviews_clientid_fkey");

            entity.HasOne(d => d.GradeNavigation).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.Grade)
                .HasConstraintName("reviews_grade_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.Productid)
                .HasConstraintName("reviews_productid_fkey");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.Saleid).HasName("sales_pkey");

            entity.ToTable("sales");

            entity.Property(e => e.Saleid).HasColumnName("saleid");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Profitamount)
                .HasPrecision(15, 2)
                .HasColumnName("profitamount");
            entity.Property(e => e.Saledate).HasColumnName("saledate");
            entity.Property(e => e.Totalamount)
                .HasPrecision(15, 2)
                .HasColumnName("totalamount");

            entity.HasOne(d => d.Order).WithMany(p => p.Sales)
                .HasForeignKey(d => d.Orderid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sales_orderid_fkey");
        });

        modelBuilder.Entity<Shoppingcart>(entity =>
        {
            entity.HasKey(e => e.Basketitemid).HasName("shoppingcart_pkey");

            entity.ToTable("shoppingcart");

            entity.Property(e => e.Basketitemid).HasColumnName("basketitemid");
            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Product).WithMany(p => p.Shoppingcarts)
                .HasForeignKey(d => d.Productid)
                .HasConstraintName("shoppingcart_productid_fkey");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Statusid).HasName("statuses_pkey");

            entity.ToTable("statuses");

            entity.Property(e => e.Statusid).HasColumnName("statusid");
            entity.Property(e => e.Namestatus)
                .HasMaxLength(30)
                .HasColumnName("namestatus");
        });

        modelBuilder.Entity<Storage>(entity =>
        {
            entity.HasKey(e => e.Storageid).HasName("storages_pkey");

            entity.ToTable("storages");

            entity.Property(e => e.Storageid).HasColumnName("storageid");
            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Purchaseid).HasColumnName("purchaseid");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Product).WithMany(p => p.Storages)
                .HasForeignKey(d => d.Productid)
                .HasConstraintName("storages_productid_fkey");

            entity.HasOne(d => d.Purchase).WithMany(p => p.Storages)
                .HasForeignKey(d => d.Purchaseid)
                .HasConstraintName("storages_purchaseid_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
