using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.ViewModels
{
    public class EditChildRegionModel
    {
        public long Id_Edit { get; set; }
        public string Code_Edit { get; set; }
        public string Name_Edit { get; set; }
        public string NameCN_Edit { get; set; }
        public long ParantId_Edit { get; set; }
        public string ParentName { get; set; }
        public int Sort_Edit { get; set; }
    }
}