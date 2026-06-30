using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DAL;
namespace BAL.Objects
{
    public class UserRole
    {

        public List<FunctionDTO> RoleFunctions(int roleId, int companyId)
        {
            UserRoleDAL dal = new UserRoleDAL();
            var data = dal.GetFunctions(roleId, companyId);

            var lst = new List<FunctionDTO>();

            lst = (from d in data.Where(o => o.ParentId == 0)
                   select new FunctionDTO
                   {
                       Route = d.Route,
                       FunctionId = d.FunctionId,
                       MenuId = d.MenuId,
                       ParentId = d.ParentId,
                       Name = d.Name,
                       Add = d.Add,
                       Edit = d.Edit,
                       Delete = d.Delete,
                       View = d.View,
                       Children =
                       data.Where(o => o.ParentId == d.FunctionId).ToList()
                   }).ToList();


            return lst;
        }
        public bool AddRoleFunction(int roleId, int companyId, FunctionDTO func)
        {
            UserRoleDAL dal = new UserRoleDAL();
            return dal.AddRoleFunction(roleId, companyId, func);
        }

        public List<MenuDTO> GetRoleMenus(int roleId, int companyId)
        {
            UserRoleDAL dal = new UserRoleDAL();
            var allMenus = dal.AllMenus(companyId);

            var roleFunctions = dal.GetFunctions(roleId, companyId);

            var menus = (from m in allMenus
                         join f in roleFunctions on m.MenuId equals f.MenuId
                         where f.RoleFunctionId > 0 || roleId == 1
                         select m
                         ).ToList();

            var lstMenus = new List<MenuDTO>();

            foreach (var m in menus)
            {
                var parentExists = lstMenus.Where(o => o.MenuId == m.ParentMenuId).FirstOrDefault();
                var exists = lstMenus.Where(o => o.MenuId == m.MenuId).FirstOrDefault();

                if (exists == null)
                {
                    lstMenus.Add(m);

                    if (parentExists == null)
                    {
                        var parentMenu = allMenus.Where(o => o.MenuId == m.ParentMenuId).FirstOrDefault();
                        if (parentMenu != null)
                        {
                            lstMenus.Add(parentMenu);
                        }
                    }
                }
            }

            var lst = new List<MenuDTO>();

            lst =  lstMenus.Where(o => o.ParentMenuId == 0).ToList();
            if (roleId == 1)
            {
                AddChildMenus(lst, allMenus);
            }
            else
            {
                AddChildMenus(lst, lstMenus);

            }
            return lst.OrderBy(o=> o.SortOrder).ToList();

        }

        void AddChildMenus(List<MenuDTO> m, List<MenuDTO> allMenus)
        {


            foreach (var c in m)
            {
                var children = allMenus.Where(o => o.ParentMenuId == c.MenuId).ToList();
                if (children.Count() > 0)
                {
                    c.ChildMenus = children;
                    AddChildMenus(c.ChildMenus, allMenus);
                }
            }
        }



    }
}
