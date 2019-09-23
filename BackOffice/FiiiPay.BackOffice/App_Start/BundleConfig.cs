using System.Web;
using System.Web.Optimization;

namespace FiiiPay.BackOffice
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.validate.js",
                        "~/Scripts/jquery.validate.unobtrusive.js",
                        "~/Scripts/jquery.validate.messages_zh.js",
                        "~/Plugins/bootstrap/js/bootstrap.js",
                        "~/Scripts/respond.js",
                        "~/Plugins/metismenu/metisMenu.js",
                        "~/Plugins/elektron/onoffcanvas.js",
                        "~/Plugins/elektron/elektron.js",
                        "~/Scripts/bo.control.js",
                        "~/Scripts/bo.fncontrol.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/plugins").Include(
                        "~/Plugins/switch/bootstrap-switch.min.js",
                        "~/Plugins/sweetalert/sweetalert.min.js",
                        "~/Plugins/jqGrid/js/i18n/grid.locale-cn.js",
                        "~/Plugins/jqGrid/js/jquery.jqGrid.js",//不能引用min.js，原码有改动
                        "~/Plugins/datetimepicker/js/bootstrap-datetimepicker.min.js",
                        "~/Plugins/webuploader/webuploader.min.js",
                        "~/Plugins/cropper/cropper.min.js",
                        "~/Plugins/mapbox/mapbox-gl.js",
                        "~/Plugins/mapbox/mapbox-gl-language.js",
                        "~/Plugins/mapbox/mapbox-gl-geocoder.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/loginjs").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.js"));

            bundles.Add(new StyleBundle("~/Content/logincss").Include(
                      "~/Plugins/bootstrap/css/bootstrap.css",
                      "~/Content/site.css"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Plugins/bootstrap/css/bootstrap.css",
                      "~/Plugins/font-awesome/css/font-awesome.css",
                      "~/Plugins/metismenu/metisMenu.css",
                      "~/Plugins/elektron/onoffcanvas.css",
                      "~/Plugins/elektron/elektron.css",
                      "~/Plugins/twbuttons/twbuttons.min.css",
                      "~/Plugins/sweetalert/sweetalert.css",
                      "~/Plugins/switch/bootstrap-switch.min.css",
                      "~/Plugins/jqGrid/css/ui.jqgrid-bootstrap.css",
                      "~/Plugins/jsTree/themes/default/style.min.css",
                      "~/Plugins/datetimepicker/css/bootstrap-datetimepicker.css",
                      "~/Plugins/webuploader/webuploader.css",
                      "~/Plugins/umeditor/themes/default/css/umeditor.css",
                      "~/Content/site.css",
                      "~/Content/page.css",
                      "~/Content/plugin.css",
                      "~/Plugins/cropper/cropper.min.css",
                      "~/Plugins/mapbox/mapbox-gl.css",
                      "~/Plugins/mapbox/mapbox-gl-geocoder.css"
                      ));
        }
    }
}
