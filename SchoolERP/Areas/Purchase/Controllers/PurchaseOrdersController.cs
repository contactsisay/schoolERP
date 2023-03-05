using BALibrary.Inventory;
using BALibrary.Purchase;
using BALibrary.Purchase.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Data;
using SchoolERP.Models;

namespace SchoolERP.Areas.Purchase.Controllers
{
    [Area("Purchase")]
    public class PurchaseOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Purchase/PurchaseOrders
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PurchaseOrders.Include(p => p.Product).Include(p => p.Supplier);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Purchase/PurchaseOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            if (id == null || _context.PurchaseOrders == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrders
                .Include(p => p.Product)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }

            return View(purchaseOrder);
        }

        // GET: Purchase/PurchaseOrders/Create
        public IActionResult Create()
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name");

            #region Preparing parameters
            var products = _context.Products.Include(p => p.Uom);
            var suppliers = _context.Suppliers;
            var pOptions = string.Empty;
            var sOptions = string.Empty;
            foreach (Product p in products)
            {
                pOptions += p.Id + "#" + (p.Name + "(" + p.Code + ")") + "#" + p.Uom.Name + "#" + p.MinimumOrderLevel + ",";//id#(name+code)#uom-name#minimum-order-level
            }
            foreach (Supplier s in suppliers)
            {
                sOptions += s.Id + "#" + (s.Name + "(" + s.ContactPersonName + ")") + "#" + s.ContactPersonMobile + "#" + s.Address + ",";//id#(name+contact-person-name)#contact-person-mobile#supplier-address
            }
            #endregion

            ViewData["pOptions"] = pOptions;
            ViewData["sOptions"] = sOptions;
            return View();
        }

        // POST: Purchase/PurchaseOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,SupplierId,RequiredAmount,ApprovedAmount,SupplierInvoiceNo")] PurchaseOrder purchaseOrder)
        {
            int currentUserId = 1;//default admin account id
            if (HttpContext.Session.GetString(SessionVariable.SessionKeyUserId) != null)
                currentUserId = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.SessionKeyUserId));

            if (ModelState.IsValid)
            {
                _context.Add(purchaseOrder);
                int pass = await _context.SaveChangesAsync();

                if (pass > 0)
                {
                    HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "success");
                    HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " Saved Successfully!");
                }
                else
                {
                    HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "error");
                    HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " NOT Saved!");
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", purchaseOrder.ProductId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Address", purchaseOrder.SupplierId);
            return View(purchaseOrder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePR(PurchaseOrderViewModel purchaseOrderViewModel, IFormCollection iformCollection)
        {
            int currentUserId = 1;//default admin account id
            if (HttpContext.Session.GetString(SessionVariable.SessionKeyUserId) != null)
                currentUserId = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.SessionKeyUserId));

            ProductBatch productBatch = new ProductBatch();
            PurchaseOrder purchaseOrder = new PurchaseOrder();

            for (int i = 0; i < iformCollection.Count; i++)
            {
                if (!string.IsNullOrEmpty(iformCollection["productid_" + i]))
                {
                    productBatch = new ProductBatch();
                    productBatch.ProductId = Convert.ToInt32(iformCollection["productid_" + i].ToString());
                    if (!string.IsNullOrEmpty(iformCollection["batch_no_" + i]))
                        productBatch.BatchNo = iformCollection["batch_no_" + i].ToString();
                    else
                        break;

                    productBatch.ManufacturedDate = Convert.ToDateTime(iformCollection["manufactured_date_" + i].ToString());
                    productBatch.BestBefore = Convert.ToDateTime(iformCollection["best_before_" + i].ToString());
                    productBatch.ExpirationDate = Convert.ToDateTime(iformCollection["expiry_date_" + i].ToString());
                    productBatch.PurchasingPrice = Convert.ToDecimal(iformCollection["purchasing_price_" + i].ToString());
                    productBatch.SellingPrice = Convert.ToDecimal(iformCollection["selling_price_" + i].ToString());
                    productBatch.IsSellable = (iformCollection["is_sellable_" + i].ToString() == "on" ? true : false);
                    productBatch.IsTaxable = (iformCollection["istaxable_" + i].ToString() == "on" ? true : false);
                    productBatch.EmployeeId = currentUserId;
                    productBatch.Status = 1;

                    purchaseOrder = new PurchaseOrder();
                    if (!string.IsNullOrEmpty(iformCollection["supplierid_" + i]))
                        purchaseOrder.SupplierId = Convert.ToInt32(iformCollection["supplierid_" + i].ToString());
                    else
                        break;

                    purchaseOrder.ProductId = productBatch.ProductId;
                    purchaseOrder.SupplierInvoiceNo = (string.IsNullOrEmpty(iformCollection["invoice_no"]) ? string.Empty : iformCollection["invoice_no"].ToString());
                    purchaseOrder.RequiredAmount = Convert.ToInt32(iformCollection["quantity_" + i].ToString());
                    purchaseOrder.ApprovedAmount = Convert.ToInt32(iformCollection["quantity_" + i].ToString());
                    purchaseOrder.RequestedAt = DateTime.Now;
                    purchaseOrder.RequestedBy = currentUserId;
                    purchaseOrder.ApprovedBy = currentUserId;
                    purchaseOrder.ApprovedAt = DateTime.Now;
                    purchaseOrder.Status = 1;

                    #region Saving the details

                    //Purchase Order
                    int poId = 0;
                    if (purchaseOrder != null)
                    {
                        purchaseOrder.RequiredAmount = purchaseOrder.ApprovedAmount;
                        purchaseOrder.RequestedBy = currentUserId;
                        purchaseOrder.RequestedAt = DateTime.Now;
                        purchaseOrder.ProductId = productBatch.ProductId;

                        _context.Add(purchaseOrder);
                        poId = await _context.SaveChangesAsync();
                    }

                    //Product Batch
                    int pbId = 0;
                    if (poId > 0 && productBatch != null)
                    {
                        productBatch.PurchaseOrderId = purchaseOrder.Id;
                        productBatch.EmployeeId = currentUserId;
                        productBatch.Status = 1;

                        _context.Add(productBatch);
                        pbId = await _context.SaveChangesAsync();
                    }

                    //Stock
                    int sId = 0;
                    if (poId > 0 && pbId > 0)
                    {
                        Stock stock = new Stock();
                        stock.ProductBatchId = productBatch.Id;
                        stock.InitialQuantity = purchaseOrder.ApprovedAmount;
                        stock.SoldQuantity = 0;
                        stock.CurrentQuantity = purchaseOrder.ApprovedAmount;
                        stock.ActionTaken = 0;//purchase
                        stock.Description = "Entered from Purchase Order";
                        stock.Status = 0;
                        stock.EmployeeId = currentUserId;
                        stock.UpdatedAt = DateTime.Now;

                        _context.Stocks.Add(stock);
                        int pass = await _context.SaveChangesAsync();

                        if (pass > 0)
                        {
                            HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "success");
                            HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " Saved Successfully!");
                        }
                        else
                        {
                            HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "error");
                            HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " NOT Saved!");
                        }
                    }
                    #endregion
                }
            }

            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", purchaseOrder.ProductId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", purchaseOrder.SupplierId);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ImportFromExcel()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            int currentUserId = 1;//default admin account id
            if (HttpContext.Session.GetString(SessionVariable.SessionKeyUserId) != null)
                currentUserId = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.SessionKeyUserId));

            //uploading file
            #region Uploading File
            string fileName = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\import_" + file.FileName);
            using (FileStream fileStream = System.IO.File.Create(fileName))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }
            #endregion

            //Reading excel and importing data
            int i = 0;
            #region Reading excel and importing data
            using (var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    do
                    {
                        while (reader.Read())
                        {
                            //passing the header
                            if (i == 0) {
                                i++;
                                continue;
                            }   

                            //checking if the read itemcode and others are NOT null
                            if (reader.GetString(0) != null && reader.GetString(1) != null && reader.GetString(2) != null && Convert.ToInt32(reader.GetDouble(11)) > 0)
                            {
                                string productName = reader.GetString(0);
                                string productCategoryName = reader.GetString(1);
                                string productCode = reader.GetString(2);
                                string uomText = reader.GetString(3);
                                string supplierInvoiceNo = reader.GetString(4);
                                DateTime manufacturedDate = reader.GetDateTime(5);
                                DateTime bbDate = reader.GetDateTime(6);
                                DateTime exprDate = reader.GetDateTime(7);
                                double purchasingPrice = reader.GetDouble(8);
                                double sellingPrice = reader.GetDouble(9);
                                double isTaxable = reader.GetDouble(10);
                                double inQuantity = reader.GetDouble(11);
                                double soldQuantity = reader.GetDouble(12);
                                string batchNo = reader.GetString(13);
                                string supplierName = reader.GetString(14);

                                decimal stockBalance = Convert.ToDecimal(inQuantity) - Convert.ToDecimal(soldQuantity);

                                //checking if objet is found or not
                                int uomId = Common.GetObjectIdFromName("uoms", "Name", uomText);
                                int productId = Common.GetObjectIdFromName("products", "Name", productName);
                                int productCategoryId = Common.GetObjectIdFromName("productcategories", "Name", productCategoryName);
                                int supplierId = Common.GetObjectIdFromName("suppliers", "Name", supplierName);

                                #region Creating UOM if NOT exists
                                //creating uom if not exists
                                if (uomId <= 0)
                                {
                                    //create uom
                                    Uom uom = new Uom();
                                    uom.Name = uomText;
                                    uom.Status = 1;
                                    _context.Uoms.Add(uom);
                                    _context.SaveChanges();

                                    uomId = uom.Id;
                                }
                                #endregion

                                #region Creating Supplier if NOT exists
                                //creating product category if NOT exists
                                if (supplierId <= 0)
                                {
                                    Supplier supplier = new Supplier()
                                    {
                                        Name = supplierName,
                                        SupplierPhone = "+2519",
                                        ContactPersonEmail = supplierName + "@supplier.com",
                                        ContactPersonName = supplierName,
                                        ContactPersonMobile = "+2519",
                                        TINNo = "123",
                                        VATRegNo = supplierName.ToCharArray()[0] + "123",
                                        Address = "aa",
                                        Status = 1
                                    };
                                    _context.Suppliers.Add(supplier);
                                    _context.SaveChanges();

                                    supplierId = supplier.Id;
                                }
                                #endregion

                                #region Creating Product Category if NOT exists
                                //creating product category if NOT exists
                                if (productCategoryId <= 0)
                                {
                                    ProductCategory productCategory = new ProductCategory()
                                    {
                                        Name = productCategoryName,
                                        Status = 1
                                    };
                                    _context.ProductCategories.Add(productCategory);
                                    _context.SaveChanges();

                                    productCategoryId = productCategory.Id;
                                }
                                #endregion

                                #region Creating Product if NOT exists
                                //creating item if not exists
                                if (productId <= 0)
                                {
                                    Product product = new Product()
                                    {
                                        ProductCategoryId = productCategoryId,
                                        Name = productName,
                                        UomId = uomId,
                                        Code = productCode,
                                        MinimumOrderLevel = 10,
                                        Status = 1
                                    };
                                    _context.Products.Add(product);
                                    _context.SaveChanges();

                                    productId = product.Id;
                                }
                                #endregion

                                #region Saving all products which are NOT registered yet!
                                //checking for item (after product and uom is found or created)
                                if (uomId != 0 && productId != 0)
                                {
                                    bool product_batch_exists = false;
                                    List<ProductBatch> productBatches = _context.ProductBatches.Where(pb => pb.ProductId == productId).Where(pb => pb.BatchNo.Equals(batchNo.ToUpper())).OrderByDescending(pb => pb.Id).ToList();
                                    if (productBatches == null || productBatches.Count == 0)
                                    {
                                        //adding if product batch is not exists (with same batch no and product id)
                                        if (!product_batch_exists)
                                        {
                                            #region Create Purchase Order
                                            //Create Purchase Order
                                            PurchaseOrder purchaseOrder = new PurchaseOrder()
                                            {
                                                ProductId = productId,
                                                ApprovedAmount = Convert.ToDecimal(inQuantity),
                                                RequiredAmount = Convert.ToDecimal(inQuantity),
                                                ApprovedAt = DateTime.Now,
                                                ApprovedBy = currentUserId,
                                                RequestedAt = DateTime.Now,
                                                RequestedBy = currentUserId,
                                                SupplierId = supplierId,
                                                SupplierInvoiceNo = supplierInvoiceNo,
                                                Status = 1,
                                            };
                                            _context.PurchaseOrders.Add(purchaseOrder);
                                            _context.SaveChanges();
                                            #endregion

                                            #region Create Product Batch
                                            //Create Product Batch
                                            ProductBatch productBatch = new ProductBatch()
                                            {
                                                ProductId = productId,
                                                BatchNo = batchNo,
                                                BestBefore = Convert.ToDateTime(bbDate),
                                                ManufacturedDate = Convert.ToDateTime(manufacturedDate),
                                                ExpirationDate = Convert.ToDateTime(exprDate),
                                                EmployeeId = currentUserId,
                                                IsSellable = true,
                                                IsTaxable = Convert.ToBoolean(isTaxable),
                                                PurchaseOrderId = purchaseOrder.Id,
                                                PurchasingPrice = Convert.ToDecimal(purchasingPrice),
                                                SellingPrice = Convert.ToDecimal(sellingPrice),
                                                Status = (int)Common.StockEntry.IMPORTED
                                            };
                                            _context.ProductBatches.Add(productBatch);
                                            _context.SaveChanges();
                                            #endregion

                                            #region Create Stock
                                            //Create Stock
                                            Stock stock = new Stock()
                                            {
                                                ProductBatchId = productBatch.Id,
                                                InitialQuantity = Convert.ToDecimal(inQuantity),
                                                SoldQuantity = Convert.ToDecimal(soldQuantity),
                                                CurrentQuantity = (stockBalance < 0 ? 0 : stockBalance),
                                                UpdatedAt = DateTime.Now,
                                                ActionTaken = (int)Common.StockEntry.IMPORTED,
                                                EmployeeId = currentUserId,
                                                Description = (stockBalance < 0 ? "Stock Imported BUT there is negative Balance (changed to ZERO)" : "Stock Imported!"),
                                                Status = (int)Common.StockEntry.IMPORTED,
                                            };
                                            _context.Stocks.Add(stock);
                                            _context.SaveChanges();
                                            #endregion
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    } while (reader.NextResult());
                }
            }
            #endregion


            return View();
        }

        // GET: Purchase/PurchaseOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);

            if (id == null || _context.PurchaseOrders == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", purchaseOrder.ProductId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Address", purchaseOrder.SupplierId);

            var productBatch = _context.ProductBatches.Where(pb => pb.PurchaseOrderId == purchaseOrder.Id).FirstOrDefault();
            PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel();
            purchaseOrderViewModel.PurchaseOrder = purchaseOrder;
            purchaseOrderViewModel.ProductBatch = productBatch;

            return View(purchaseOrderViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPR(PurchaseOrderViewModel purchaseOrderViewModel)
        {
            int currentUserId = 1;//default admin account id
            if (HttpContext.Session.GetString(SessionVariable.SessionKeyUserId) != null)
                currentUserId = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.SessionKeyUserId));

            ProductBatch productBatch = purchaseOrderViewModel.ProductBatch;
            PurchaseOrder purchaseOrder = purchaseOrderViewModel.PurchaseOrder;

            //Purchase Order
            int poId = 0;
            if (purchaseOrder != null)
            {
                purchaseOrder.RequiredAmount = purchaseOrder.ApprovedAmount;
                purchaseOrder.RequestedBy = currentUserId;
                purchaseOrder.RequestedAt = DateTime.Now;
                purchaseOrder.ProductId = productBatch.ProductId;

                _context.Update(purchaseOrder);
                poId = await _context.SaveChangesAsync();
            }

            //Product Batch
            int pbId = 0;
            if (poId > 0 && productBatch != null)
            {
                productBatch.PurchaseOrderId = purchaseOrder.Id;
                productBatch.EmployeeId = currentUserId;

                _context.Update(productBatch);
                pbId = await _context.SaveChangesAsync();
            }

            //Stock
            int sId = 0;
            if (poId > 0 && pbId > 0)
            {
                Stock stock = new Stock();
                stock.ProductBatchId = productBatch.Id;
                stock.InitialQuantity = purchaseOrder.ApprovedAmount;
                stock.SoldQuantity = purchaseOrder.ApprovedAmount;
                stock.CurrentQuantity = purchaseOrder.ApprovedAmount;
                stock.ProductBatchId = pbId;
                stock.ActionTaken = 0;//purchase
                stock.Description = "Entered from Purchase Order";
                stock.Status = 0;
                stock.EmployeeId = currentUserId;
                stock.UpdatedAt = DateTime.Now;

                _context.Stocks.Update(stock);
                int pass = await _context.SaveChangesAsync();

                if (pass > 0)
                {
                    HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "success");
                    HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " Updated Successfully!");
                }
                else
                {
                    HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "error");
                    HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " NOT Updated!");
                }
            }

            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", purchaseOrder.ProductId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Address", purchaseOrder.SupplierId);
            return RedirectToAction(nameof(Index));
        }

        // POST: Purchase/PurchaseOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,SupplierId,RequiredAmount,ApprovedAmount,SupplierInvoiceNo")] PurchaseOrder purchaseOrder)
        {
            if (id != purchaseOrder.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(purchaseOrder);
                    int pass = await _context.SaveChangesAsync();

                    if (pass > 0)
                    {
                        HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "success");
                        HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " Updated Successfully!");
                    }
                    else
                    {
                        HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "error");
                        HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " NOT Updated!");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseOrderExists(purchaseOrder.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", purchaseOrder.ProductId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Address", purchaseOrder.SupplierId);
            return View(purchaseOrder);
        }

        // GET: Purchase/PurchaseOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);

            if (id == null || _context.PurchaseOrders == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrders
                .Include(p => p.Product)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }

            return View(purchaseOrder);
        }

        // POST: Purchase/PurchaseOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PurchaseOrders == null)
            {
                return Problem("Entity set 'ApplicationDbContext.PurchaseOrders'  is null.");
            }
            var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            if (purchaseOrder != null)
            {
                _context.PurchaseOrders.Remove(purchaseOrder);
            }

            int pass = await _context.SaveChangesAsync();

            if (pass > 0)
            {
                HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "success");
                HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " Deleted Successfully!");
            }
            else
            {
                HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "error");
                HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " NOT Deleted!");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseOrderExists(int id)
        {
            return _context.PurchaseOrders.Any(e => e.Id == id);
        }
    }
}
