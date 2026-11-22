using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Security
{
    public static class PermissionCodes
    {
        // Here, we'll add all the permissions that the UI can use to check if the user has the permission to access.
        // There will be another classes in WebApi to filter (like Exception Middleware), and they will be added in the Middleware (Program.cs) to make the check.

        public const string CategoryView = "Category.View";
        public const string CategoryCreate = "Category.Create";
        public const string CategoryUpdate = "Category.Update";
        public const string CategoryDelete = "Category.Delete";

        public const string ProductView =   "Product.View";
        public const string ProductCreate = "Product.Create";
        public const string ProductUpdate = "Product.Update";
        public const string ProductDelete = "Product.Delete";



        public const string UsersView =   "Users.View";
        public const string UsersCreate = "Users.Create";
        public const string UsersUpdate = "Users.Update";
        public const string UsersDelete = "Users.Delete";
        public const string UsersManagePermissions = "Users.ManagePermissions";


        public const string RolesView =   "Roles.View";
        public const string RolesCreate = "Roles.Create";
        public const string RolesUpdate = "Roles.Update";
        public const string RolesDelete = "Roles.Delete";
        public const string RolesManagePermissions = "Roles.ManagePermissions";

    }
}
