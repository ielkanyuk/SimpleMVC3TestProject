using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UnicloudTestProject.Models;
using PagedList;

namespace UnicloudTestProject.Controllers
{
    public class HomeController : Controller
    {
        private UniclTestDataEntities db = new UniclTestDataEntities();
             
        /// <summary>Index</summary>
        /// <param name="fieldSearch">Поле для поиска</param>
        /// <param name="currentFilter">Текущий параметр фильтрации</param>
        /// <param name="searchString">Значение из строки поиска</param>
        /// <param name="sortOrder">Параметр сортировки по столбцам</param>
        /// <param name="page">Номер страницы</param>        
        public ActionResult Index(string fieldSearch, string currentFilter, string searchString, string sortOrder, int? page)
        {
            #region Определение списка полей для формы поиска
             
            List<SelectListItem> searchFieldsList = new List<SelectListItem> { 
                                              new SelectListItem { Text="", Value="0" },
                                              new SelectListItem { Text="ФИО", Value="1" },
                                              new SelectListItem { Text="Тип документа", Value="2" },
                                              new SelectListItem { Text="Серия и номер документа", Value="3" },
                                              new SelectListItem { Text="Дата выдачи документа", Value="4" }
                                              };

            #endregion

            #region Параметры сортировки

            ViewBag.CurrentSort = sortOrder; 
            ViewBag.NameSortParm = sortOrder == "Name" ? "Name desc" : "Name";
            ViewBag.DocTypeSortParm = sortOrder == "DType" ? "DType desc" : "DType";
            ViewBag.SerialNumSortParm = sortOrder == "SNum" ? "SNum desc" : "SNum";
            ViewBag.DateSortParm = sortOrder == "Date" ? "Date desc" : "Date";

            #endregion

            #region Сохраняем параметры фильтрации при переходе на другие страницы

            if (Request.HttpMethod == "GET")
            {
                searchString = currentFilter;                
            }
            else
            {
                page = 1;
            }
            ViewBag.CurrentFilterField = fieldSearch;
            ViewBag.CurrentFilter = searchString;            

            #endregion

            ViewBag.FieldsList = searchFieldsList;

            // получаем список документов из БД
            var docList = from s in db.Document select s;
            
            #region Фильтрация таблицы

            if (!String.IsNullOrEmpty(searchString))
            {                                
                switch (fieldSearch)
                {
                    case "1":
                        docList = docList.Where(s => (s.Person.FirstName + " " + s.Person.LastName + " " + s.Person.MiddleName).ToUpper().Contains(searchString.ToUpper()) ||
                                                     (s.Person.FirstName + " " + s.Person.MiddleName + " " + s.Person.LastName).ToUpper().Contains(searchString.ToUpper()) ||
                                                     (s.Person.MiddleName + " " + s.Person.FirstName + " " + s.Person.LastName).ToUpper().Contains(searchString.ToUpper()) ||
                                                     (s.Person.MiddleName + " " + s.Person.LastName + " " + s.Person.FirstName).ToUpper().Contains(searchString.ToUpper()) ||
                                                     (s.Person.LastName + " " + s.Person.MiddleName + " " + s.Person.FirstName).ToUpper().Contains(searchString.ToUpper()) ||
                                                     (s.Person.LastName + " " + s.Person.FirstName + " " + s.Person.MiddleName).ToUpper().Contains(searchString.ToUpper())
                                                     );                        
                        break;
                    case "2":
                        docList = docList.Where(s => s.DocTypeTable.TypeName.ToUpper().Contains(searchString.ToUpper()));
                        break;
                    case "3":
                        docList = docList.Where(s => (s.Serial + s.Number).ToUpper().Contains(searchString.Replace(" ", "").ToUpper()));
                        break;
                    case "4":
                        docList = docList.ToList<Document>().Where(s => String.Format("{0:dd.MM.yyyy}", s.IssueDate).ToUpper().Contains(searchString.ToUpper())).AsQueryable<Document>();                    
                        break;
                    default:                        
                        break;
                }
            }

            #endregion

            #region Сортировка таблицы

            switch (sortOrder)
            {
                case "Name":
                    docList = docList.OrderBy(z => z.Person.MiddleName).OrderBy(y => y.Person.LastName).OrderBy(x => x.Person.FirstName);
                    break;
                case "Name desc":
                    docList = docList.OrderByDescending(z => z.Person.MiddleName).OrderByDescending(y => y.Person.LastName).OrderByDescending(x => x.Person.FirstName);
                    break;
                case "DType":
                    docList = docList.OrderBy(x => x.DocTypeTable.TypeName);
                    break;
                case "DType desc":
                    docList = docList.OrderByDescending(x => x.DocTypeTable.TypeName);
                    break;
                case "SNum":
                    docList = docList.OrderBy(x => x.Serial == null ? x.Number : (x.Serial + x.Number));
                    break;
                case "SNum desc":
                    docList = docList.OrderByDescending(x => x.Serial == null ? x.Number : (x.Serial + x.Number));
                    break;
                case "Date":
                    docList = docList.OrderBy(x => x.IssueDate);
                    break;
                case "Date desc":
                    docList = docList.OrderByDescending(x => x.IssueDate);
                    break;
                default:
                    docList = docList.OrderBy(x => x.DocumentId);
                    break;
            }

            #endregion

            // количество записей настранице
            int pageSize = 10;

            // если не задан номер, то переход на первую страницу
            int pageIndex = page ?? 1;

            return View(docList.ToPagedList(pageIndex, pageSize));
        }
    }
}
