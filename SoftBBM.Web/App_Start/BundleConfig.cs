using System.Web;
using System.Web.Optimization;

namespace SoftBBM.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));
            bundles.Add(new ScriptBundle("~/bundles/shared").Include(
                      "~/app/shared/modules/softbbm.common.js",
                      "~/app/shared/services/apiService.js",
                      "~/app/shared/services/notificationService.js",
                      "~/app/shared/directives/customPager.directive.js",
                      "~/app/shared/directives/customPagerSmall.directive.js",
                      "~/app/shared/directives/format.directive.js",
                      "~/app/shared/services/authData.js",
                      "~/app/shared/services/authenticationService.js",
                      "~/app/shared/services/loginService.js",
                      "~/app/shared/filters/statusVNPayFilter.js",
                      "~/app/shared/directives/barcodeGenerator.js",
                      "~/app/shared/directives/fileUpload.directive.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/module").Include(
                      "~/app/app.js",
                      "~/app/components/home/rootController.js",
                      "~/app/components/suppliers/suppliers.module.js",
                      "~/app/components/branches/branches.module.js",
                      "~/app/components/channels/channels.module.js",
                      "~/app/components/stocks/stocks.module.js",
                      "~/app/components/books/books.module.js",
                      "~/app/components/stockins/stockins.module.js",
                      "~/app/components/stockouts/stockouts.module.js",
                      "~/app/components/orders/orders.module.js",
                      "~/app/components/application_roles/applicationRoles.module.js",
                      "~/app/components/application_groups/applicationGroups.module.js",
                      "~/app/components/application_users/applicationUsers.module.js",
                      "~/app/components/stamps/stamp.module.js",
                      "~/app/components/customers/customers.module.js",
                      "~/app/components/product_categories/productCategories.module.js",
                      "~/app/components/product_logs/productLogs.module.js",
                      "~/app/components/return_suppliers/returnSupplier.module.js",
                      "~/app/components/imports/imports.module.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/controller").Include(
                      "~/app/shared/views/baseController.js",
                      "~/app/components/home/homeController.js",
                      "~/app/components/login/loginController.js",
                      "~/app/components/suppliers/supplierListController.js",
                      "~/app/components/suppliers/supplierAddController.js",
                      "~/app/components/branches/branchListController.js",
                      "~/app/components/branches/branchAddController.js",
                      "~/app/components/channels/channelListController.js",
                      "~/app/components/channels/channelAddController.js",
                      "~/app/components/stocks/stockListController.js",
                      "~/app/components/stocks/stockChannelPricesController.js",
                      "~/app/components/books/bookListController.js",
                      "~/app/components/books/bookDetailController.js",
                      "~/app/components/books/bookEditController.js",
                      "~/app/components/books/bookAddController.js",
                      "~/app/components/books/branchbookListController.js",
                      "~/app/components/books/branchbookDetailController.js",
                      "~/app/components/books/branchbookEditController.js",
                      "~/app/components/books/branchbookAddController.js",
                      "~/app/components/stockins/stockinListController.js",
                      "~/app/components/stockins/stockinDetailController.js",
                      "~/app/components/stockins/stockinEditController.js",
                      "~/app/components/stockins/stockinAddController.js",
                      "~/app/components/orders/orderListController.js",
                      //"~/app/components/orders/orderAddController.js",
                      "~/app/components/orders/orderDetailController.js",
                      "~/app/components/application_roles/applicationRoleListController.js",
                      "~/app/components/application_roles/applicationRoleAddController.js",
                      "~/app/components/application_roles/applicationRoleEditController.js",
                      "~/app/components/application_groups/applicationGroupAddController.js",
                      "~/app/components/application_groups/applicationGroupListController.js",
                      "~/app/components/application_groups/applicationGroupEditController.js",
                      "~/app/components/application_users/applicationUserListController.js",
                      "~/app/components/application_users/applicationUserAddController.js",
                      "~/app/components/application_users/applicationUserEditController.js",
                      "~/app/components/stocks/stockTotalAllController.js",
                      "~/app/components/stockouts/stockoutListController.js",
                      "~/app/components/stockouts/stockoutDetailController.js",
                      "~/app/components/stockouts/stockoutAddController.js",
                      "~/app/components/stockouts/stockoutEditController.js",
                      "~/app/components/stocks/adjustmentStockListController.js",
                      "~/app/components/stocks/adjustmentStockAddController.js",
                      "~/app/components/stocks/adjustmentStockDetailController.js",
                      "~/app/components/stamps/stampAddController.js",
                      "~/app/components/stocks/processModalController.js",
                      "~/app/components/stocks/processImportModalController.js",
                      "~/app/components/stockouts/soldProductsByDateController.js",
                      "~/app/components/application_users/userEditController.js",
                      "~/app/components/stocks/productAddController.js",
                      "~/app/components/customers/customerListController.js",
                      "~/app/components/customers/customerAddController.js",
                      "~/app/components/product_categories/productCategoryListController.js",
                      "~/app/components/product_categories/productCategoryAddController.js",
                      "~/app/components/books/processBookModalController.js",
                      "~/app/components/product_logs/productLogListController.js",
                      "~/app/components/product_logs/deleteLogController.js",
                      "~/app/components/stockins/stockinThenOutController.js",
                      "~/app/components/return_suppliers/returnSupplierAddController.js",
                      "~/app/components/return_suppliers/returnSupplierListController.js",
                      "~/app/components/return_suppliers/returnSupplierDetailController.js",
                      "~/app/components/stockins/stockinThenUpdatePriceController.js",
                      "~/app/components/stocks/stockEditProductController.js",
                      "~/app/components/stockins/stockinPaymentController.js",
                      "~/app/components/product_categories/productCategoryEditController.js",
                      "~/app/components/imports/importController.js",
                      "~/app/components/imports/productProcessImportController.js",
                      "~/app/components/imports/channelPriceProcessImportController.js"
                      ));

            BundleTable.EnableOptimizations = true;
        }
    }
}
