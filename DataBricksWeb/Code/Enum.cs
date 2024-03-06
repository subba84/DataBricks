using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataBricksWeb
{
    public class UserRoleIds
    {
        public const int Admin = 1;
        public const int Customer = 2;
        public const int Partner = 3;
        public const int Sales = 4;
        public const int Technical = 5;
        public const int Distributor = 6;
    }
    public class FileCategory
    {
        public const string BusinessCard = "Business Card";
        public const string PO = "PO";
        public const string ZipFile = "Zip File";
        public const string Document = "Document";
    }
    public class Statusses
    {
        public const string Submitted = "Submitted";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
    }
}