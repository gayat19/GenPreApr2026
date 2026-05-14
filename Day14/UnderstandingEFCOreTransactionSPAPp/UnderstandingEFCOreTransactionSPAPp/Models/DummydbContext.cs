using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingEFCOreTransactionSPAPp.Models;

public partial class DummydbContext : DbContext
{
    public DummydbContext()
    {
    }

    public DummydbContext(DbContextOptions<DummydbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AlphabeticalListOfProduct> AlphabeticalListOfProducts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CategorySalesFor1997> CategorySalesFor1997s { get; set; }

    public virtual DbSet<CurrentProductList> CurrentProductLists { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerAndSuppliersByCity> CustomerAndSuppliersByCities { get; set; }

    public virtual DbSet<Customerdemographic> Customerdemographics { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderDetailsExtended> OrderDetailsExtendeds { get; set; }

    public virtual DbSet<OrderSubtotal> OrderSubtotals { get; set; }

    public virtual DbSet<OrdersQry> OrdersQries { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductSalesFor1997> ProductSalesFor1997s { get; set; }

    public virtual DbSet<ProductsAboveAveragePrice> ProductsAboveAveragePrices { get; set; }

    public virtual DbSet<ProductsByCategory> ProductsByCategories { get; set; }

    public virtual DbSet<QuarterlyOrder> QuarterlyOrders { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<SalesByCategory> SalesByCategories { get; set; }

    public virtual DbSet<SalesTotalsByAmount> SalesTotalsByAmounts { get; set; }

    public virtual DbSet<Shipper> Shippers { get; set; }

    public virtual DbSet<SummaryOfSalesByQuarter> SummaryOfSalesByQuarters { get; set; }

    public virtual DbSet<SummaryOfSalesByYear> SummaryOfSalesByYears { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Territory> Territories { get; set; }

    public virtual DbSet<Tran> Trans { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=dummydb;Username=postgres;Password=password");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("account");

            entity.Property(e => e.Aacno).HasColumnName("aacno");
            entity.Property(e => e.Balance).HasColumnName("balance");
        });

        modelBuilder.Entity<AlphabeticalListOfProduct>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("alphabetical_list_of_products");

            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.Categoryname)
                .HasMaxLength(15)
                .HasColumnName("categoryname");
            entity.Property(e => e.Discontinued).HasColumnName("discontinued");
            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Productname)
                .HasMaxLength(40)
                .HasColumnName("productname");
            entity.Property(e => e.Quantityperunit)
                .HasMaxLength(20)
                .HasColumnName("quantityperunit");
            entity.Property(e => e.Reorderlevel).HasColumnName("reorderlevel");
            entity.Property(e => e.Supplierid).HasColumnName("supplierid");
            entity.Property(e => e.Unitprice)
                .HasPrecision(19, 4)
                .HasColumnName("unitprice");
            entity.Property(e => e.Unitsinstock).HasColumnName("unitsinstock");
            entity.Property(e => e.Unitsonorder).HasColumnName("unitsonorder");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Categoryid).HasName("pk_categories");

            entity.ToTable("categories");

            entity.HasIndex(e => e.Categoryname, "idx_categories_categoryname");

            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.Categoryname)
                .HasMaxLength(15)
                .HasColumnName("categoryname");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Picture).HasColumnName("picture");
        });

        modelBuilder.Entity<CategorySalesFor1997>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("category_sales_for_1997");

            entity.Property(e => e.Categoryname)
                .HasMaxLength(15)
                .HasColumnName("categoryname");
            entity.Property(e => e.Categorysales).HasColumnName("categorysales");
        });

        modelBuilder.Entity<CurrentProductList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("current_product_list");

            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Productname)
                .HasMaxLength(40)
                .HasColumnName("productname");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Customerid).HasName("pk_customers");

            entity.ToTable("customers");

            entity.HasIndex(e => e.City, "idx_customers_city");

            entity.HasIndex(e => e.Companyname, "idx_customers_companyname");

            entity.HasIndex(e => e.Postalcode, "idx_customers_postalcode");

            entity.HasIndex(e => e.Region, "idx_customers_region");

            entity.Property(e => e.Customerid)
                .HasMaxLength(5)
                .IsFixedLength()
                .HasColumnName("customerid");
            entity.Property(e => e.Address)
                .HasMaxLength(60)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(15)
                .HasColumnName("city");
            entity.Property(e => e.Companyname)
                .HasMaxLength(40)
                .HasColumnName("companyname");
            entity.Property(e => e.Contactname)
                .HasMaxLength(30)
                .HasColumnName("contactname");
            entity.Property(e => e.Contacttitle)
                .HasMaxLength(30)
                .HasColumnName("contacttitle");
            entity.Property(e => e.Country)
                .HasMaxLength(15)
                .HasColumnName("country");
            entity.Property(e => e.Fax)
                .HasMaxLength(24)
                .HasColumnName("fax");
            entity.Property(e => e.Phone)
                .HasMaxLength(24)
                .HasColumnName("phone");
            entity.Property(e => e.Postalcode)
                .HasMaxLength(10)
                .HasColumnName("postalcode");
            entity.Property(e => e.Region)
                .HasMaxLength(15)
                .HasColumnName("region");

            entity.HasMany(d => d.Customertypes).WithMany(p => p.Customers)
                .UsingEntity<Dictionary<string, object>>(
                    "Customercustomerdemo",
                    r => r.HasOne<Customerdemographic>().WithMany()
                        .HasForeignKey("Customertypeid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_customercustomerdemo"),
                    l => l.HasOne<Customer>().WithMany()
                        .HasForeignKey("Customerid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_customercustomerdemo_customers"),
                    j =>
                    {
                        j.HasKey("Customerid", "Customertypeid").HasName("pk_customercustomerdemo");
                        j.ToTable("customercustomerdemo");
                        j.IndexerProperty<string>("Customerid")
                            .HasMaxLength(5)
                            .IsFixedLength()
                            .HasColumnName("customerid");
                        j.IndexerProperty<string>("Customertypeid")
                            .HasMaxLength(10)
                            .IsFixedLength()
                            .HasColumnName("customertypeid");
                    });
        });

        modelBuilder.Entity<CustomerAndSuppliersByCity>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("customer_and_suppliers_by_city");

            entity.Property(e => e.City)
                .HasMaxLength(15)
                .HasColumnName("city");
            entity.Property(e => e.Companyname)
                .HasMaxLength(40)
                .HasColumnName("companyname");
            entity.Property(e => e.Contactname)
                .HasMaxLength(30)
                .HasColumnName("contactname");
            entity.Property(e => e.Relationship).HasColumnName("relationship");
        });

        modelBuilder.Entity<Customerdemographic>(entity =>
        {
            entity.HasKey(e => e.Customertypeid).HasName("pk_customerdemographics");

            entity.ToTable("customerdemographics");

            entity.Property(e => e.Customertypeid)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("customertypeid");
            entity.Property(e => e.Customerdesc).HasColumnName("customerdesc");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Employeeid).HasName("pk_employees");

            entity.ToTable("employees");

            entity.HasIndex(e => e.Lastname, "idx_employees_lastname");

            entity.HasIndex(e => e.Postalcode, "idx_employees_postalcode");

            entity.Property(e => e.Employeeid).HasColumnName("employeeid");
            entity.Property(e => e.Address)
                .HasMaxLength(60)
                .HasColumnName("address");
            entity.Property(e => e.Birthdate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("birthdate");
            entity.Property(e => e.City)
                .HasMaxLength(15)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(15)
                .HasColumnName("country");
            entity.Property(e => e.Extension)
                .HasMaxLength(4)
                .HasColumnName("extension");
            entity.Property(e => e.Firstname)
                .HasMaxLength(10)
                .HasColumnName("firstname");
            entity.Property(e => e.Hiredate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("hiredate");
            entity.Property(e => e.Homephone)
                .HasMaxLength(24)
                .HasColumnName("homephone");
            entity.Property(e => e.Lastname)
                .HasMaxLength(20)
                .HasColumnName("lastname");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.Photo).HasColumnName("photo");
            entity.Property(e => e.Photopath)
                .HasMaxLength(255)
                .HasColumnName("photopath");
            entity.Property(e => e.Postalcode)
                .HasMaxLength(10)
                .HasColumnName("postalcode");
            entity.Property(e => e.Region)
                .HasMaxLength(15)
                .HasColumnName("region");
            entity.Property(e => e.Reportsto).HasColumnName("reportsto");
            entity.Property(e => e.Title)
                .HasMaxLength(30)
                .HasColumnName("title");
            entity.Property(e => e.Titleofcourtesy)
                .HasMaxLength(25)
                .HasColumnName("titleofcourtesy");

            entity.HasOne(d => d.ReportstoNavigation).WithMany(p => p.InverseReportstoNavigation)
                .HasForeignKey(d => d.Reportsto)
                .HasConstraintName("fk_employees_employees");

            entity.HasMany(d => d.Territories).WithMany(p => p.Employees)
                .UsingEntity<Dictionary<string, object>>(
                    "Employeeterritory",
                    r => r.HasOne<Territory>().WithMany()
                        .HasForeignKey("Territoryid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_employeeterritories_territories"),
                    l => l.HasOne<Employee>().WithMany()
                        .HasForeignKey("Employeeid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_employeeterritories_employees"),
                    j =>
                    {
                        j.HasKey("Employeeid", "Territoryid").HasName("pk_employeeterritories");
                        j.ToTable("employeeterritories");
                        j.IndexerProperty<int>("Employeeid").HasColumnName("employeeid");
                        j.IndexerProperty<string>("Territoryid")
                            .HasMaxLength(20)
                            .HasColumnName("territoryid");
                    });
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("invoices");

            entity.Property(e => e.Address)
                .HasMaxLength(60)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(15)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(15)
                .HasColumnName("country");
            entity.Property(e => e.Customerid)
                .HasMaxLength(5)
                .IsFixedLength()
                .HasColumnName("customerid");
            entity.Property(e => e.Customername)
                .HasMaxLength(40)
                .HasColumnName("customername");
            entity.Property(e => e.Discount).HasColumnName("discount");
            entity.Property(e => e.Extendedprice)
                .HasPrecision(19, 4)
                .HasColumnName("extendedprice");
            entity.Property(e => e.Freight)
                .HasPrecision(19, 4)
                .HasColumnName("freight");
            entity.Property(e => e.Orderdate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("orderdate");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Postalcode)
                .HasMaxLength(10)
                .HasColumnName("postalcode");
            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Productname)
                .HasMaxLength(40)
                .HasColumnName("productname");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Region)
                .HasMaxLength(15)
                .HasColumnName("region");
            entity.Property(e => e.Requireddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("requireddate");
            entity.Property(e => e.Salesperson).HasColumnName("salesperson");
            entity.Property(e => e.Shipaddress)
                .HasMaxLength(60)
                .HasColumnName("shipaddress");
            entity.Property(e => e.Shipcity)
                .HasMaxLength(15)
                .HasColumnName("shipcity");
            entity.Property(e => e.Shipcountry)
                .HasMaxLength(15)
                .HasColumnName("shipcountry");
            entity.Property(e => e.Shipname)
                .HasMaxLength(40)
                .HasColumnName("shipname");
            entity.Property(e => e.Shippeddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("shippeddate");
            entity.Property(e => e.Shippername)
                .HasMaxLength(40)
                .HasColumnName("shippername");
            entity.Property(e => e.Shippostalcode)
                .HasMaxLength(10)
                .HasColumnName("shippostalcode");
            entity.Property(e => e.Shipregion)
                .HasMaxLength(15)
                .HasColumnName("shipregion");
            entity.Property(e => e.Unitprice)
                .HasPrecision(19, 4)
                .HasColumnName("unitprice");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Orderid).HasName("pk_orders");

            entity.ToTable("orders");

            entity.HasIndex(e => e.Customerid, "idx_orders_customerid");

            entity.HasIndex(e => e.Customerid, "idx_orders_customersorders");

            entity.HasIndex(e => e.Employeeid, "idx_orders_employeeid");

            entity.HasIndex(e => e.Employeeid, "idx_orders_employeesorders");

            entity.HasIndex(e => e.Orderdate, "idx_orders_orderdate");

            entity.HasIndex(e => e.Shippeddate, "idx_orders_shippeddate");

            entity.HasIndex(e => e.Shipvia, "idx_orders_shippersorders");

            entity.HasIndex(e => e.Shippostalcode, "idx_orders_shippostalcode");

            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Customerid)
                .HasMaxLength(5)
                .IsFixedLength()
                .HasColumnName("customerid");
            entity.Property(e => e.Employeeid).HasColumnName("employeeid");
            entity.Property(e => e.Freight)
                .HasPrecision(19, 4)
                .HasDefaultValueSql("0")
                .HasColumnName("freight");
            entity.Property(e => e.Orderdate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("orderdate");
            entity.Property(e => e.Requireddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("requireddate");
            entity.Property(e => e.Shipaddress)
                .HasMaxLength(60)
                .HasColumnName("shipaddress");
            entity.Property(e => e.Shipcity)
                .HasMaxLength(15)
                .HasColumnName("shipcity");
            entity.Property(e => e.Shipcountry)
                .HasMaxLength(15)
                .HasColumnName("shipcountry");
            entity.Property(e => e.Shipname)
                .HasMaxLength(40)
                .HasColumnName("shipname");
            entity.Property(e => e.Shippeddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("shippeddate");
            entity.Property(e => e.Shippostalcode)
                .HasMaxLength(10)
                .HasColumnName("shippostalcode");
            entity.Property(e => e.Shipregion)
                .HasMaxLength(15)
                .HasColumnName("shipregion");
            entity.Property(e => e.Shipvia).HasColumnName("shipvia");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Customerid)
                .HasConstraintName("fk_orders_customers");

            entity.HasOne(d => d.Employee).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Employeeid)
                .HasConstraintName("fk_orders_employees");

            entity.HasOne(d => d.ShipviaNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Shipvia)
                .HasConstraintName("fk_orders_shippers");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => new { e.Orderid, e.Productid }).HasName("pk_order_details");

            entity.ToTable("order_details");

            entity.HasIndex(e => e.Orderid, "idx_order_details_orderid");

            entity.HasIndex(e => e.Orderid, "idx_order_details_ordersorder_details");

            entity.HasIndex(e => e.Productid, "idx_order_details_productid");

            entity.HasIndex(e => e.Productid, "idx_order_details_productsorder_details");

            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Discount).HasColumnName("discount");
            entity.Property(e => e.Quantity)
                .HasDefaultValue((short)1)
                .HasColumnName("quantity");
            entity.Property(e => e.Unitprice)
                .HasPrecision(19, 4)
                .HasColumnName("unitprice");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.Orderid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_order_details_orders");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.Productid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_order_details_products");
        });

        modelBuilder.Entity<OrderDetailsExtended>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("order_details_extended");

            entity.Property(e => e.Discount).HasColumnName("discount");
            entity.Property(e => e.Extendedprice)
                .HasPrecision(19, 4)
                .HasColumnName("extendedprice");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Productname)
                .HasMaxLength(40)
                .HasColumnName("productname");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Unitprice)
                .HasPrecision(19, 4)
                .HasColumnName("unitprice");
        });

        modelBuilder.Entity<OrderSubtotal>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("order_subtotals");

            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Subtotal).HasColumnName("subtotal");
        });

        modelBuilder.Entity<OrdersQry>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("orders_qry");

            entity.Property(e => e.Address)
                .HasMaxLength(60)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(15)
                .HasColumnName("city");
            entity.Property(e => e.Companyname)
                .HasMaxLength(40)
                .HasColumnName("companyname");
            entity.Property(e => e.Country)
                .HasMaxLength(15)
                .HasColumnName("country");
            entity.Property(e => e.Customerid)
                .HasMaxLength(5)
                .IsFixedLength()
                .HasColumnName("customerid");
            entity.Property(e => e.Employeeid).HasColumnName("employeeid");
            entity.Property(e => e.Freight)
                .HasPrecision(19, 4)
                .HasColumnName("freight");
            entity.Property(e => e.Orderdate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("orderdate");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Postalcode)
                .HasMaxLength(10)
                .HasColumnName("postalcode");
            entity.Property(e => e.Region)
                .HasMaxLength(15)
                .HasColumnName("region");
            entity.Property(e => e.Requireddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("requireddate");
            entity.Property(e => e.Shipaddress)
                .HasMaxLength(60)
                .HasColumnName("shipaddress");
            entity.Property(e => e.Shipcity)
                .HasMaxLength(15)
                .HasColumnName("shipcity");
            entity.Property(e => e.Shipcountry)
                .HasMaxLength(15)
                .HasColumnName("shipcountry");
            entity.Property(e => e.Shipname)
                .HasMaxLength(40)
                .HasColumnName("shipname");
            entity.Property(e => e.Shippeddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("shippeddate");
            entity.Property(e => e.Shippostalcode)
                .HasMaxLength(10)
                .HasColumnName("shippostalcode");
            entity.Property(e => e.Shipregion)
                .HasMaxLength(15)
                .HasColumnName("shipregion");
            entity.Property(e => e.Shipvia).HasColumnName("shipvia");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Productid).HasName("pk_products");

            entity.ToTable("products");

            entity.HasIndex(e => e.Categoryid, "idx_products_categoriesproducts");

            entity.HasIndex(e => e.Categoryid, "idx_products_categoryid");

            entity.HasIndex(e => e.Productname, "idx_products_productname");

            entity.HasIndex(e => e.Supplierid, "idx_products_supplierid");

            entity.HasIndex(e => e.Supplierid, "idx_products_suppliersproducts");

            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.Discontinued)
                .HasDefaultValue((short)0)
                .HasColumnName("discontinued");
            entity.Property(e => e.Productname)
                .HasMaxLength(40)
                .HasColumnName("productname");
            entity.Property(e => e.Quantityperunit)
                .HasMaxLength(20)
                .HasColumnName("quantityperunit");
            entity.Property(e => e.Reorderlevel)
                .HasDefaultValue((short)0)
                .HasColumnName("reorderlevel");
            entity.Property(e => e.Supplierid).HasColumnName("supplierid");
            entity.Property(e => e.Unitprice)
                .HasPrecision(19, 4)
                .HasDefaultValueSql("0")
                .HasColumnName("unitprice");
            entity.Property(e => e.Unitsinstock)
                .HasDefaultValue((short)0)
                .HasColumnName("unitsinstock");
            entity.Property(e => e.Unitsonorder)
                .HasDefaultValue((short)0)
                .HasColumnName("unitsonorder");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.Categoryid)
                .HasConstraintName("fk_products_categories");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Products)
                .HasForeignKey(d => d.Supplierid)
                .HasConstraintName("fk_products_suppliers");
        });

        modelBuilder.Entity<ProductSalesFor1997>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("product_sales_for_1997");

            entity.Property(e => e.Categoryname)
                .HasMaxLength(15)
                .HasColumnName("categoryname");
            entity.Property(e => e.Productname)
                .HasMaxLength(40)
                .HasColumnName("productname");
            entity.Property(e => e.Productsales).HasColumnName("productsales");
        });

        modelBuilder.Entity<ProductsAboveAveragePrice>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("products_above_average_price");

            entity.Property(e => e.Productname)
                .HasMaxLength(40)
                .HasColumnName("productname");
            entity.Property(e => e.Unitprice)
                .HasPrecision(19, 4)
                .HasColumnName("unitprice");
        });

        modelBuilder.Entity<ProductsByCategory>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("products_by_category");

            entity.Property(e => e.Categoryname)
                .HasMaxLength(15)
                .HasColumnName("categoryname");
            entity.Property(e => e.Discontinued).HasColumnName("discontinued");
            entity.Property(e => e.Productname)
                .HasMaxLength(40)
                .HasColumnName("productname");
            entity.Property(e => e.Quantityperunit)
                .HasMaxLength(20)
                .HasColumnName("quantityperunit");
            entity.Property(e => e.Unitsinstock).HasColumnName("unitsinstock");
        });

        modelBuilder.Entity<QuarterlyOrder>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("quarterly_orders");

            entity.Property(e => e.City)
                .HasMaxLength(15)
                .HasColumnName("city");
            entity.Property(e => e.Companyname)
                .HasMaxLength(40)
                .HasColumnName("companyname");
            entity.Property(e => e.Country)
                .HasMaxLength(15)
                .HasColumnName("country");
            entity.Property(e => e.Customerid)
                .HasMaxLength(5)
                .IsFixedLength()
                .HasColumnName("customerid");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.Regionid).HasName("pk_region");

            entity.ToTable("region");

            entity.Property(e => e.Regionid)
                .ValueGeneratedNever()
                .HasColumnName("regionid");
            entity.Property(e => e.Regiondescription)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("regiondescription");
        });

        modelBuilder.Entity<SalesByCategory>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("sales_by_category");

            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.Categoryname)
                .HasMaxLength(15)
                .HasColumnName("categoryname");
            entity.Property(e => e.Productname)
                .HasMaxLength(40)
                .HasColumnName("productname");
            entity.Property(e => e.Productsales).HasColumnName("productsales");
        });

        modelBuilder.Entity<SalesTotalsByAmount>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("sales_totals_by_amount");

            entity.Property(e => e.Companyname)
                .HasMaxLength(40)
                .HasColumnName("companyname");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Saleamount).HasColumnName("saleamount");
            entity.Property(e => e.Shippeddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("shippeddate");
        });

        modelBuilder.Entity<Shipper>(entity =>
        {
            entity.HasKey(e => e.Shipperid).HasName("pk_shippers");

            entity.ToTable("shippers");

            entity.Property(e => e.Shipperid).HasColumnName("shipperid");
            entity.Property(e => e.Companyname)
                .HasMaxLength(40)
                .HasColumnName("companyname");
            entity.Property(e => e.Phone)
                .HasMaxLength(24)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<SummaryOfSalesByQuarter>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("summary_of_sales_by_quarter");

            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Shippeddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("shippeddate");
            entity.Property(e => e.Subtotal).HasColumnName("subtotal");
        });

        modelBuilder.Entity<SummaryOfSalesByYear>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("summary_of_sales_by_year");

            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Shippeddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("shippeddate");
            entity.Property(e => e.Subtotal).HasColumnName("subtotal");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Supplierid).HasName("pk_suppliers");

            entity.ToTable("suppliers");

            entity.HasIndex(e => e.Companyname, "idx_suppliers_companyname");

            entity.HasIndex(e => e.Postalcode, "idx_suppliers_postalcode");

            entity.Property(e => e.Supplierid).HasColumnName("supplierid");
            entity.Property(e => e.Address)
                .HasMaxLength(60)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(15)
                .HasColumnName("city");
            entity.Property(e => e.Companyname)
                .HasMaxLength(40)
                .HasColumnName("companyname");
            entity.Property(e => e.Contactname)
                .HasMaxLength(30)
                .HasColumnName("contactname");
            entity.Property(e => e.Contacttitle)
                .HasMaxLength(30)
                .HasColumnName("contacttitle");
            entity.Property(e => e.Country)
                .HasMaxLength(15)
                .HasColumnName("country");
            entity.Property(e => e.Fax)
                .HasMaxLength(24)
                .HasColumnName("fax");
            entity.Property(e => e.Homepage).HasColumnName("homepage");
            entity.Property(e => e.Phone)
                .HasMaxLength(24)
                .HasColumnName("phone");
            entity.Property(e => e.Postalcode)
                .HasMaxLength(10)
                .HasColumnName("postalcode");
            entity.Property(e => e.Region)
                .HasMaxLength(15)
                .HasColumnName("region");
        });

        modelBuilder.Entity<Territory>(entity =>
        {
            entity.HasKey(e => e.Territoryid).HasName("pk_territories");

            entity.ToTable("territories");

            entity.Property(e => e.Territoryid)
                .HasMaxLength(20)
                .HasColumnName("territoryid");
            entity.Property(e => e.Regionid).HasColumnName("regionid");
            entity.Property(e => e.Territorydescription)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("territorydescription");

            entity.HasOne(d => d.Region).WithMany(p => p.Territories)
                .HasForeignKey(d => d.Regionid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_territories_region");
        });

        modelBuilder.Entity<Tran>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("trans_pkey");

            entity.ToTable("trans");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Fromacc).HasColumnName("fromacc");
            entity.Property(e => e.Toacc).HasColumnName("toacc");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .HasColumnName("user_id");
            entity.Property(e => e.UserPassword)
                .HasMaxLength(50)
                .HasColumnName("user_password");
            entity.Property(e => e.UserRole)
                .HasMaxLength(10)
                .HasColumnName("user_role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
