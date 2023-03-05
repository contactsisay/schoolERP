using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SchoolERP.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public ApplicationDbContext()
        {
        }

        #region ADMIN RELATED
        public DbSet<BALibrary.Admin.Role> Roles { get; set; }
        public DbSet<BALibrary.Admin.RoleModule> RoleModules { get; set; }
        public DbSet<BALibrary.Admin.RoleModuleException> RoleModuleExceptions { get; set; }
        public DbSet<BALibrary.Admin.ModuleTable> ModuleTables { get; set; }
        #endregion

        #region HR RELATED
        public DbSet<BALibrary.HR.Department> Departments { get; set; }
        public DbSet<BALibrary.HR.DocumentType> DocumentTypes { get; set; }
        public DbSet<BALibrary.HR.Employee> Employees { get; set; }
        public DbSet<BALibrary.HR.EmployeeAttendance> EmployeeAttendances { get; set; }
        public DbSet<BALibrary.HR.EmployeeDocument> EmployeeDocuments { get; set; }
        public DbSet<BALibrary.HR.EmployeeLeave> EmployeeLeaves { get; set; }
        public DbSet<BALibrary.HR.EmployeeOffday> EmployeeOffdays { get; set; }
        public DbSet<BALibrary.HR.EmployeeSalary> EmployeeSalaries { get; set; }
        public DbSet<BALibrary.HR.EmployeeShift> EmployeeShifts { get; set; }
        public DbSet<BALibrary.HR.EmployeeSocialMediaLink> EmployeeSocialMediaLinks { get; set; }
        public DbSet<BALibrary.HR.EmployeeTransfer> EmployeeTransfers { get; set; }
        public DbSet<BALibrary.HR.EmploymentType> EmploymentTypes { get; set; }
        public DbSet<BALibrary.HR.JobPosition> JobPositions { get; set; }
        public DbSet<BALibrary.HR.LeaveType> LeaveTypes { get; set; }
        public DbSet<BALibrary.HR.ShiftType> ShiftTypes { get; set; }
        public DbSet<BALibrary.HR.SocialMedia> SocialMedias { get; set; }
        public DbSet<BALibrary.HR.UserType> UserTypes { get; set; }
        #endregion

        #region INVENTORY RELATED
        public DbSet<BALibrary.Inventory.ManualCount> ManualCounts { get; set; }
        public DbSet<BALibrary.Inventory.Product> Products { get; set; }
        public DbSet<BALibrary.Inventory.ProductBatch> ProductBatches { get; set; }
        public DbSet<BALibrary.Inventory.ProductCategory> ProductCategories { get; set; }
        public DbSet<BALibrary.Inventory.Stock> Stocks { get; set; }
        public DbSet<BALibrary.Inventory.Uom> Uoms { get; set; }
        public DbSet<BALibrary.Inventory.Package> Packages { get; set; }
        public DbSet<BALibrary.Inventory.PackageItem> PackageItems { get; set; }
        #endregion

        #region PURCHASE RELAED
        public DbSet<BALibrary.Purchase.Supplier> Suppliers { get; set; }
        public DbSet<BALibrary.Purchase.PurchaseOrder> PurchaseOrders { get; set; }
        #endregion

        #region SALES RELATED
        public DbSet<BALibrary.Sales.Customer> Customers { get; set; }
        public DbSet<BALibrary.Sales.CustomerType> CustomerTypes { get; set; }
        public DbSet<BALibrary.Sales.Invoice> Invoices { get; set; }
        public DbSet<BALibrary.Sales.InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<BALibrary.Sales.InvoiceType> InvoiceTypes { get; set; }
        #endregion

        #region REPORT RELATED
        public DbSet<BALibrary.Report.ProfitLossReport> ProfitLossReports { get; set; }
        public DbSet<BALibrary.Report.PurchaseReport> PurchaseReports { get; set; }
        public DbSet<BALibrary.Report.SalesReport> SalesReports { get; set; }
        public DbSet<BALibrary.Report.StockReport> StockReports { get; set; }
        #endregion

        #region FINANCE RELATED
        public DbSet<BALibrary.Finance.Bank> Banks { get; set; }
        #endregion

        #region ACADEMIC RELATED
        public DbSet<BALibrary.Academic.Semester> Semesters { get; set; }
        public DbSet<BALibrary.Academic.AcademicYear> AcademicYears { get; set; }
        public DbSet<BALibrary.Academic.AcademicYearSection> AcademicYearSections { get; set; }
        public DbSet<BALibrary.Academic.Room> Rooms { get; set; }
        public DbSet<BALibrary.Academic.Class> Classes { get; set; }
        public DbSet<BALibrary.Academic.Section> Sections { get; set; }
        public DbSet<BALibrary.Academic.Session> Sessions { get; set; }
        public DbSet<BALibrary.Academic.Subject> Subjects { get; set; }
        public DbSet<BALibrary.Academic.SubjectGroup> SubjectGroups { get; set; }
        public DbSet<BALibrary.Academic.SubjectGroupDetail> SubjectGroupDetails { get; set; }
        public DbSet<BALibrary.Academic.GradingRule> GradingRules { get; set; }
        public DbSet<BALibrary.Academic.GradingRuleGroup> GradingRuleGroups { get; set; }
        public DbSet<BALibrary.Academic.GradingRuleGroupDetail> GradingRuleGroupDetails { get; set; }
        #endregion

        #region EXAMINATION RELATED
        public DbSet<BALibrary.Examination.Exam> Exams { get; set; }
        public DbSet<BALibrary.Examination.ExamGroup> ExamGroups { get; set; }
        public DbSet<BALibrary.Examination.ExamGroupDetail> ExamGroupDetails { get; set; }
        public DbSet<BALibrary.Examination.ExamSchedule> ExamSchedules { get; set; }
        #endregion

        #region HOSTEL RELATED
        public DbSet<BALibrary.Hostel.Hostel> Hostels { get; set; }
        public DbSet<BALibrary.Hostel.HostelRoom> HostelRooms { get; set; }
        public DbSet<BALibrary.Hostel.RoomType> RoomTypes { get; set; }
        #endregion

        #region REGISTRAR RELATED
        public DbSet<BALibrary.Registrar.Student> Students { get; set; }
        public DbSet<BALibrary.Registrar.StudentPromotion> StudentPromotions { get; set; }
        public DbSet<BALibrary.Registrar.StudentGuardian> StudentGuardians { get; set; }
        public DbSet<BALibrary.Registrar.Relationship> Relationships { get; set; }
        public DbSet<BALibrary.Registrar.Period> Periods { get; set; }
        public DbSet<BALibrary.Registrar.MarkSheet> MarksSheets { get; set; }
        public DbSet<BALibrary.Registrar.MarkSheetDetail> MarksSheetDetails { get; set; }
        public DbSet<BALibrary.Registrar.TimeTable> TimeTables { get; set; }
        public DbSet<BALibrary.Registrar.TimeTableDetail> TimeTableDetails { get; set; }
        public DbSet<BALibrary.Registrar.SectionTeacherSubject> SectionTeacherSubjects { get; set; }
        public DbSet<BALibrary.Registrar.SMSummary> SMSummaries { get; set; }
        public DbSet<BALibrary.Registrar.SMSummaryDetail> SMSummaryDetails { get; set; }

        #endregion

        #region TEACHER RELATED
        public DbSet<BALibrary.Teacher.ActionType> ActionTypes { get; set; }
        public DbSet<BALibrary.Teacher.StudentDisciplinaryAction> StudentDisciplinaryActions { get; set; }
        public DbSet<BALibrary.Teacher.StudentAbsentee> StudentAbsentees { get; set; }
        public DbSet<BALibrary.Teacher.StudentLeave> StudentLeaves { get; set; }
        #endregion
    }
}